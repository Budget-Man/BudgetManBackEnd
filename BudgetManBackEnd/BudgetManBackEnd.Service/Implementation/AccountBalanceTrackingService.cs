using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MayNghien.Common.Helpers.SearchHelper;

namespace BudgetManBackEnd.Service.Implementation
{
    public class AccountBalanceTrackingService : IAccountBalanceTrackingService
    {

        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountBalanceTrackingRepository _accountBalanceTrackingRepository;
        public AccountBalanceTrackingService(IAccountInfoRepository accountInfoRepository, IHttpContextAccessor httpContextAccessor, IAccountBalanceTrackingRepository accountBalanceTrackingRepository)
        {
            _accountInfoRepository = accountInfoRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountBalanceTrackingRepository = accountBalanceTrackingRepository;
        }

        public Task CreateTracking(AccountBalanceTrackingDto request)
        {
            throw new NotImplementedException();
        }

        public AppResponse<SearchResponse<AccountBalanceTrackingDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<AccountBalanceTrackingDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
                var numOfRecords = _accountBalanceTrackingRepository.CountRecordsByPredicate(query);
                var model = _accountBalanceTrackingRepository.FindByPredicate(query).Include(x => x.Budget).Include(x => x.MoneyHolder)
                    .OrderByDescending(x => x.CreatedOn);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new AccountBalanceTrackingDto
                    {

                        Id = x.Id,
                        Amount = x.Amount,
                        ChangeType = (int)x.ChangeType,
                        ChangeTypeName = nameof(x.ChangeType),
                        CurrentBalance = x.CurrentBalance,
                        MoneyHolderId = x.MoneyHolderId,
                        MoneyHolderName = x.MoneyHolder.Name,
                        NewBalance = x.NewBalance
                    })
                    .ToList();


                var searchUserResult = new SearchResponse<AccountBalanceTrackingDto>
                {
                    TotalRows = 0,
                    TotalPages = CalculateNumOfPages(0, pageSize),
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

        private ExpressionStarter<AccountBalanceTracking> BuildFilterExpression(List<Filter>? filters, Guid id)
        {
            try
            {
                var predicate = PredicateBuilder.New<AccountBalanceTracking>(true);
                if (filters != null)
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {

                            case "moneyHolderId":
                                predicate = predicate.And(m => m.MoneyHolderId.ToString() == filter.Value);
                                break;
                            case "budgetId":
                                predicate = predicate.And(m => m.BudgetId.ToString() == filter.Value);
                                break;
                            default:
                                break;
                        }
                    }
                predicate = predicate.And(m => m.IsDeleted == false);
                predicate = predicate.And(m => m.AccountId == id);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
