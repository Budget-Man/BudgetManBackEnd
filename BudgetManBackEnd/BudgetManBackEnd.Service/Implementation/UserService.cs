using AutoMapper;
using BudgetManBackEnd.Common.Enum;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Response.User;
using BudgetManBackEnd.Service.Contract;
using Intercom.Data;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static MayNghien.Common.CommonMessage.AuthResponseMessage;

namespace BudgetManBackEnd.Service.Implementation
{
    public class UserService : IUserService
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IConfiguration config, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IAccountInfoRepository accountInfoRepository,
            IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountInfoRepository = accountInfoRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AppResponse<string>> CreateUser(UserModel user)
        {
            var result = new AppResponse<string>();
            try
            {
                //if (string.IsNullOrEmpty(user.Email))
                //{
                //    return result.BuildError(ERR_MSG_EmailIsNullOrEmpty);
                //}
                var identityUser = await _userManager.FindByNameAsync(user.UserName);
                if (identityUser != null)
                {
                    return result.BuildError(ERR_MSG_UserExisted);
                }
                if (user.Role != nameof(UserRoleEnum.TenantAdmin) && user.Role != nameof(UserRoleEnum.Admin) && user.Role != nameof(UserRoleEnum.SuperAdmin))
                {
                    return result.BuildError(ERR_MSG_RoleNotFound);
                }
                var UserId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var role = ClaimHelper.GetClainByName(_httpContextAccessor, "Role");
                if (role == nameof(UserRoleEnum.TenantAdmin) || (role == nameof(UserRoleEnum.Admin) && user.Role == nameof(UserRoleEnum.SuperAdmin)))
                {
                    return result.BuildError(ERR_MSG_NotHavePermision);
                }
                var newIdentityUser = new IdentityUser { Email = user.UserName, UserName = user.UserName };
                var createResult = await _userManager.CreateAsync(newIdentityUser);
                await _userManager.AddPasswordAsync(newIdentityUser, "Abc@123");

                newIdentityUser = await _userManager.FindByNameAsync(user.UserName);
                if (newIdentityUser != null)
                {
                    if (user.Role != null && user.Role == nameof(UserRoleEnum.TenantAdmin))
                    {
                        var AccountInfo = new AccountInfo()
                        {
                            Id = Guid.NewGuid(),
                            Balance = 0,
                            Email = user.UserName,
                            CreatedBy = user.UserName,
                            CreatedOn = DateTime.Now,
                            Name = user.UserName,
                            IsDeleted = false,
                            UserId = newIdentityUser.Id,
                        };
                        _accountInfoRepository.Add(AccountInfo, AccountInfo.Name);
                    }

                    await _userManager.AddToRoleAsync(newIdentityUser, user.Role);


                }

                return result.BuildResult(INFO_MSG_UserCreated);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }

        public async Task<AppResponse<string>> DeleteUser(string id)
        {
            var result = new AppResponse<string>();
            try
            {

                IdentityUser identityUser = new IdentityUser();

                identityUser = await _userManager.FindByIdAsync(id);
                if (identityUser != null)
                {
                    if (await _userManager.IsInRoleAsync(identityUser, "tenant"))
                    {
                        var qUserInfo = _accountInfoRepository.FindBy(m => m.UserId == id && m.IsDeleted == false);
                        if (qUserInfo.Count() > 0)
                        {
                            var AccountInfo = qUserInfo.FirstOrDefault();
                            AccountInfo.IsDeleted = true;
                            _accountInfoRepository.Edit(AccountInfo);
                        }
                        await _userManager.DeleteAsync(identityUser);
                    }
                    else
                    {
                        await _userManager.DeleteAsync(identityUser);
                    }

                }
                return result.BuildResult(INFO_MSG_UserDeleted);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }

        public async Task<AppResponse<string>> EditUser(UserModel model)
        {
            var result = new AppResponse<string>();
            if (model.Id == null)
            {
                return result.BuildError(ERR_MSG_EmailIsNullOrEmpty);
            }
            if(model.Role == null)
            {
                return result.BuildError(ERR_MSG_RoleIsNullOrEmpty);
            }
            if(! await _roleManager.RoleExistsAsync(model.Role))
            {
                return result.BuildError(ERR_MSG_RoleNotFound);
            }
            try
            {
                var identityUser = await _userManager.FindByIdAsync(model.Id);
                if (identityUser != null)
                {
                  var role= (await  _userManager.GetRolesAsync(identityUser)).FirstOrDefault();
                    if (role != null)
                    {
                        await _userManager.RemoveFromRoleAsync(identityUser, role);

                    }
                    await _userManager.AddToRoleAsync(identityUser, model.Role);
                }
                return result.BuildResult("ok");
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }

        public async Task<AppResponse<List<UserModel>>> GetAllUser()
        {
            var result = new AppResponse<List<UserModel>>();
            try
            {
                List<Filter> Filters = new List<Filter>();
                var query = await BuildFilterExpression(Filters);
                var users = _userRepository.FindByPredicate(query);
                var UserList = users.ToList();
                var dtoList = _mapper.Map<List<UserModel>>(UserList);
                if (dtoList != null && dtoList.Count > 0)
                {
                    for (int i = 0; i < UserList.Count; i++)
                    {
                        var dtouser = dtoList[i];
                        var identityUser = UserList[i];
                        dtouser.Role = (await _userManager.GetRolesAsync(identityUser)).First();
                    }
                }
                return result.BuildResult(dtoList);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }

        }

        public async Task<AppResponse<UserModel>> GetUser(string id)
        {
            var result = new AppResponse<UserModel>();
            try
            {
                List<Filter> Filters = new List<Filter>();
                var query = BuildFilterExpression(Filters);

                var identityUser = _userRepository.FindById(id);

                if (identityUser == null)
                {
                    return result.BuildError("User not found");
                }
                var dtouser = _mapper.Map<UserModel>(identityUser);

                dtouser.Role = (await _userManager.GetRolesAsync(identityUser)).First();

                return result.BuildResult(dtouser);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }

        public async Task<AppResponse<SearchUserResponse>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchUserResponse>();
            try
            {
                var query = await BuildFilterExpression(request.Filters);
                var numOfRecords = _userRepository.CountRecordsByPredicate(query);

                var users = _userRepository.FindByPredicate(query);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserList = users.Skip(startIndex).Take(pageSize).ToList();
                var dtoList = _mapper.Map<List<UserModel>>(UserList);
                if (dtoList != null && dtoList.Count > 0)
                {
                    for (int i = 0; i < UserList.Count; i++)
                    {
                        var dtouser = dtoList[i];
                        var identityUser = UserList[i];
                        var listrole = await _userManager.GetRolesAsync(identityUser);
                        if (listrole.Count > 0)
                        {
                            dtouser.Role = listrole.First();
                        }
                        
                    }
                }
                var searchUserResult = new SearchUserResponse
                {
                    TotalRows = numOfRecords,
                    TotalPages = SearchHelper.CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };

                return result.BuildResult(searchUserResult);

            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }



        private async Task<ExpressionStarter<IdentityUser>> BuildFilterExpression(IList<Filter>? Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<IdentityUser>(true);

                
                //predicate = predicate.And(m=>m.)
                if (Filters != null)
                {
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "userName":
                                predicate = predicate.And(m => m.UserName.Equals(filter.Value));
                                break;
                            case "role":
                                var userIdsbyrole = (await _userManager.GetUsersInRoleAsync((filter.Value))).Select(m=>m.Id).ToList();
                                predicate = predicate.And(m => userIdsbyrole.Contains(m.Id));
                                break;
                            default:
                                break;
                        }
                    }
                }
                
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
