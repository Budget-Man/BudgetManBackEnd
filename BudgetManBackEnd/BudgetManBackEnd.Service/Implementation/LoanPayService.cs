using System.Data.Entity;
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
	public class LoanPayService : ILoanPayService
    {
        private readonly ILoanPayRepository _loanPayRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private ILoanRepository _loanRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMoneyHolderRepository _moneyHolderRepository;
        public LoanPayService(ILoanPayRepository loanPayRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            IAccountInfoRepository accountInfoRepository, ILoanRepository loanRepository ,IBudgetRepository budgetRepository, IMoneyHolderRepository moneyHolderRepository)
        {
            _loanPayRepository = loanPayRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _loanRepository = loanRepository;
            _budgetRepository = budgetRepository;
            _moneyHolderRepository = moneyHolderRepository;
        }

        public AppResponse<LoanPayDto> GetLoanPay(Guid Id)
        {
            var result = new AppResponse<LoanPayDto>();
            try
            {
                var query = _loanPayRepository.FindBy(x=>x.Id == Id).Include(x=>x.Loan);
                var data = query.Select(x=>new LoanPayDto
                {
                    Id = x.Id,
                    Interest = x.Interest,
                    PaidAmount = x.PaidAmount,
                }).First();
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<LoanPayDto>> GetAllLoanPay()
        {
            var result = new AppResponse<List<LoanPayDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _loanPayRepository.GetAll().Where(x=>x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x=>x.Loan);
                var list = query
                    .Select(x=>new LoanPayDto
                    {
                        Id = x.Id,
                        Interest = x.Interest,
                        PaidAmount = x.PaidAmount,
                        RatePeriodName=nameof(x.RatePeriod),
                    })
                    .ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<LoanPayDto> CreateLoanPay(LoanPayDto request, Guid loanId)
        {
            var result = new AppResponse<LoanPayDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (loanId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _loanRepository.FindBy(m => m.Id == loanId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find Loan");
                }
                var Loan = loan.First();
                var loanPay = new LoanPay();
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.LoanId = loanId;
                if (Loan.RemainAmount - loanPay.PaidAmount < 0)
                {
                    return result.BuildError("The amount paid is not greater than the remaining amount");
                }

                if (request.MoneyHolderId == null)
                {
                    return result.BuildError("Money Holder cannot be null");
                }
                if (request.BudgetId == null)
                {
                    return result.BuildError("Budget cannot be null");
                }
                var budget = _budgetRepository.Get(request.BudgetId.Value);
                if (budget == null)
                {
                    return result.BuildError("Cannot find Buddget");
                }
                var moneyHolder = _moneyHolderRepository.Get(request.MoneyHolderId.Value);
                if (moneyHolder == null)
                {
                    return result.BuildError("Cannot find Money Holder");
                }
                loanPay.MoneyHolderId = moneyHolder.Id;
                loanPay.BudgetId = budget.Id;
                loanPay.Interest=request.Interest;
                loanPay.InterestRate = Loan.InterestRate;
                loanPay.RatePeriod = Loan.RatePeriod;
                loanPay.PaidAmount = request.PaidAmount;
                loanPay.IsPaid = true;
                

                
                Loan.RemainAmount -= loanPay.PaidAmount;
                
                budget.Balance += loanPay.PaidAmount.Value + loanPay.Interest.Value;
                moneyHolder.Balance += loanPay.PaidAmount.Value + loanPay.Interest.Value;

                _loanPayRepository.Add(loanPay, accountInfo.Name);
                _loanRepository.Edit(Loan);
                _budgetRepository.Edit(budget);
                _moneyHolderRepository.Edit(moneyHolder);
                request.Id = loanPay.Id;
                return result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteLoanPay(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var loanPay = _loanPayRepository.Get(Id);
                loanPay.IsDeleted = true;
                _loanPayRepository.Edit(loanPay);
                var loan = _loanRepository.Get(loanPay.LoanId);
                loan.RemainAmount += loanPay.PaidAmount;
                _loanRepository.Edit(loan);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
		public AppResponse<SearchResponse<LoanPayDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<LoanPayDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = _loanPayRepository.CountRecordsByPredicate(query);
				var model = _loanPayRepository.FindByPredicate(query).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new LoanPayDto
					{
						Id = x.Id,
						Interest = x.Interest,
                        PaidAmount = x.PaidAmount,
                        RatePeriodName=nameof(x.RatePeriod),
					})
					.ToList();


				var searchUserResult = new SearchResponse<LoanPayDto>
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
		private ExpressionStarter<LoanPay> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<LoanPay>(true);
                if(Filters!= null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						case "LoanName":
							    predicate = predicate.And(m => m.Loan.Name.Contains(filter.Value) && m.AccountId == accountId);
							break;
                        case "LoanId":
                                predicate = predicate.And(m=>m.Loan.Id.Equals(Guid.Parse(filter.Value)));
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
