using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
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
    public class DebtService : IDebtService
    {
        private IDebtRepository _debtRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMoneyHolderRepository _moneyHolderRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IAccountBalanceTrackingRepository _accountBalanceTrackingRepository;

        public DebtService(IDebtRepository debtRepository, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository,
            IBudgetRepository budgetRepository, IMoneyHolderRepository moneyHolderRepository,
            IAccountBalanceTrackingRepository accountBalanceTrackingRepository)
        {
            _debtRepository = debtRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _budgetRepository = budgetRepository;
            _moneyHolderRepository = moneyHolderRepository;
            _accountBalanceTrackingRepository = accountBalanceTrackingRepository;
        }

        public AppResponse<DebtDto> CreateDebt(DebtDto request)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();


                var debt = new Debt();
                debt.Id = Guid.NewGuid();
                debt.AccountId = accountInfo.Id;
                debt.TotalInterest = 0;
                debt.TotalAmount = request.TotalAmount.Value;
                debt.RemainAmount = debt.TotalAmount;
                debt.MoneyHolderId = request.MoneyHolderId;
                request.Id = debt.Id;
                debt.Name = request.Name;

                var moneyHolder = _moneyHolderRepository.Get(debt.MoneyHolderId.Value);
                //if (budget.Balance == null) budget.Balance = 0;
                if (moneyHolder.Balance == null) moneyHolder.Balance = 0;
                moneyHolder.Balance += debt.TotalAmount;
                _debtRepository.Add(debt, accountInfo.Name);
                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult(request);

                var accTracking = new AccountBalanceTracking
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountInfo.Id,
                    Amount = debt.TotalAmount,
                    MoneyHolderId = moneyHolder.Id,
                    ChangeType = Common.Enum.ChangeType.Debt,
                    CurrentBalance = moneyHolder.Balance - debt.TotalAmount,
                    NewBalance = moneyHolder.Balance,
                    BudgetId=null,
                    
                };
                _accountBalanceTrackingRepository.Add(accTracking,accountInfo.Name);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<string> DeleteDebt(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var debt = _debtRepository.Get(Id);
                debt.IsDeleted = true;
                _debtRepository.Edit(debt);

                result.BuildResult("đã xóa");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DebtDto> EditDebt(DebtDto request)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var debt = _debtRepository.Get((Guid)request.Id);
                debt.Name = request.Name;
                debt.RatePeriod = request.RatePeriod;
                debt.InterestRate = request.InterestRate;
                debt.PaidAmount = request.PaidAmount;
                debt.TotalAmount = (double)request.TotalAmount;
                debt.RemainAmount = request.RemainAmount;
                debt.TotalInterest = request.TotalInterest;

                _debtRepository.Edit(debt);

                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DebtDto>> GetAllDebt()
        {
            var result = new AppResponse<List<DebtDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _debtRepository.GetAll().Where(x => x.Account.UserId == userId && x.IsDeleted != true);
                var list = query.Select(x => new DebtDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    TotalAmount = x.TotalAmount,
                    TotalInterest = x.TotalInterest,
                    RemainAmount = x.RemainAmount,
                    PaidAmount = x.PaidAmount,
                    InterestRate = x.InterestRate,
                    RatePeriod = x.RatePeriod,
                }).ToList();

                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DebtDto> GetDebt(Guid Id)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var debt = _debtRepository.GetDto(Id);
                if (debt != null)
                {
                    return result.BuildResult(debt);
                }
                else
                {
                    return result.BuildError("Debt not found");
                }
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
        public AppResponse<SearchResponse<DebtDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<DebtDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
                var numOfRecords = _debtRepository.CountRecordsByPredicate(query);
                var model = _debtRepository.FindByPredicate(query).OrderByDescending(x => x.CreatedOn);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new DebtDto
                    {
                        Id = x.Id,
                        InterestRate = x.InterestRate,
                        Name = x.Name,
                        PaidAmount = x.PaidAmount,
                        RatePeriod = x.RatePeriod,
                        RemainAmount = x.RemainAmount,
                        TotalAmount = x.TotalAmount,
                        TotalInterest = x.TotalInterest,
                        MoneyHolderId = x.MoneyHolderId,
                        MoneyHolderName = x.MoneyHolder != null ? x.MoneyHolder.Name : null,
                    })
                    .ToList();


                var searchUserResult = new SearchResponse<DebtDto>
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
        private ExpressionStarter<Debt> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
        {
            try
            {
                var predicate = PredicateBuilder.New<Debt>(true);
                if (Filters != null)
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
                predicate = predicate.And(m => m.IsDeleted == false);
                predicate = predicate.And(m => m.AccountId == accountId);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
