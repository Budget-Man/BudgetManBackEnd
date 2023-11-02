using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using static MayNghien.Common.Helpers.SearchHelper;

namespace BudgetManBackEnd.Service.Implementation
{
    public class MoneyHolderService : IMoneyHolderService
    {
        private readonly IMoneyHolderRepository _moneyHolderRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MoneyHolderService(IMoneyHolderRepository moneyHolderRepository, IAccountInfoRepository accountInfoRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _moneyHolderRepository = moneyHolderRepository;
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public AppResponse<MoneyHolderDto> CreateMoneyHolder(MoneyHolderDto request)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var UserId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                //var role = ClaimHelper.GetClainByName(_httpContextAccessor, "Role");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == UserId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var moneyHolder = _mapper.Map<MoneyHolder>(request);
                moneyHolder.AccountId = accountInfo.Id;
                moneyHolder.Id = Guid.NewGuid();
                _moneyHolderRepository.Add(moneyHolder, accountInfo.Name);
                request.Id = moneyHolder.Id;
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteMoneyHolder(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var moneyHolder = _moneyHolderRepository.Get(Id);
                moneyHolder.IsDeleted = true;

                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneyHolderDto> EditMoneyHolder(MoneyHolderDto request)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var moneyHolder = _moneyHolderRepository.Get((Guid)request.Id);
                moneyHolder.Name = request.Name;
                moneyHolder.BankName = request.BankName;
                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<MoneyHolderDto>> GetAllMoneyHolder()
        {
            var result = new AppResponse<List<MoneyHolderDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _moneyHolderRepository.GetAll().Where(m => m.Account.UserId == userId);
                var list = query.Select(m => new MoneyHolderDto
                {
                    BankName = m.BankName,
                    Id = m.Id,
                    Name = m.Name,
                }).ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneyHolderDto> GetMoneyHolder(Guid Id)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var moneyHolder = _moneyHolderRepository.Get(Id);
                var data = _mapper.Map<MoneyHolderDto>(moneyHolder);
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        public AppResponse<SearchResponse<MoneyHolderDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<MoneyHolderDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
                var numOfRecords = -_moneyHolderRepository.CountRecordsByPredicate(query);
                var model = _moneyHolderRepository.FindByPredicate(query).OrderByDescending(x=>x.CreatedOn);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new MoneyHolderDto
                    {
                        Id = x.Id,
                        BankName=x.BankName,
                        Name=x.Name
                    })
                    .ToList();


                var searchUserResult = new SearchResponse<MoneyHolderDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = List,
                };
                result.BuildResult(searchUserResult);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        private ExpressionStarter<MoneyHolder> BuildFilterExpression(IList<Filter>? Filters, Guid accountId)
        {
            try
            {
                var predicate = PredicateBuilder.New<MoneyHolder>(true);
                predicate = predicate.And(m => m.AccountId == accountId);
                predicate = predicate.And(m => m.IsDeleted == false);
                if (Filters != null)
                {
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                predicate = predicate.And(m => m.Name.Contains(filter.Value));
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
