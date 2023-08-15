using BudgetManBackEnd.Common.Enum;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Response.User;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
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
        public UserService(IConfiguration config, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IAccountInfoRepository accountInfoRepository)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountInfoRepository = accountInfoRepository;
        }
        public async Task<AppResponse<string>> CreateUser(UserModel user)
        {
            var result = new AppResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    return result.BuildError(ERR_MSG_EmailIsNullOrEmpty);
                }
                var identityUser = await _userManager.FindByNameAsync(user.UserName);
                if (identityUser != null)
                {
                    return result.BuildError(ERR_MSG_UserExisted);
                }
                var newIdentityUser = new IdentityUser { Email = user.Email, UserName = user.Email };
                var createResult = await _userManager.CreateAsync(newIdentityUser);
                await _userManager.AddPasswordAsync(newIdentityUser, user.Password);

                newIdentityUser = await _userManager.FindByEmailAsync(user.Email);
                if (newIdentityUser != null)
                {
                    if(user.Role!=null && user.Role== nameof(UserRoleEnum.TenantAdmin))
                    {
                        var AccountInfo = new AccountInfo()
                        {
                            Id = Guid.NewGuid(),
                            Balance = 0,
                            Email = user.Email,
                            CreatedBy = user.Email,
                            CreatedOn = DateTime.Now,
                            Name = user.UserName,
                            IsDeleted = false,
                            UserId = newIdentityUser.Id,
                        };
                        _accountInfoRepository.Add(AccountInfo);
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
                        await _userManager.SetLockoutEnabledAsync(identityUser, true);
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
            throw new NotImplementedException();
        }

        public AppResponse<List<UserModel>> GetAllUser()
        {
            throw new NotImplementedException();
        }

        public AppResponse<UserModel> GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public AppResponse<SearchUserResponse> Search(SearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
