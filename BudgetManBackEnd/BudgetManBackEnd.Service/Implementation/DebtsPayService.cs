using System.Data.Entity;
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
	public class DebtsPayService : IDebtsPayService
    {
        private readonly IDebtsPayRepository _debtsPayRepository;
        private readonly IDebtRepository _debtRepository;
        private IMapper _mapper;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMoneyHolderRepository _moneyHolderRepository;

        public DebtsPayService(IDebtsPayRepository debtsPayRepository, IMapper mapper, 
            IAccountInfoRepository accountInfoRepository, IDebtRepository debtRepository
            , IHttpContextAccessor httpContextAccessor, IBudgetRepository budgetRepository, IMoneyHolderRepository moneyHolderRepository)
        {
            _debtsPayRepository = debtsPayRepository;
            _mapper = mapper;
            _accountInfoRepository = accountInfoRepository;
            _httpContextAccessor = httpContextAccessor;
            _debtRepository = debtRepository;
            _budgetRepository = budgetRepository;   
            _moneyHolderRepository = moneyHolderRepository;
        }

        public AppResponse<List<DebtsPayDto>> GetAllDebtsPay()
        {
            var result = new AppResponse<List<DebtsPayDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _debtsPayRepository.GetAll()
                    .Where(x => x.Account.UserId == userId && x.IsDeleted != true)
                    .Include(x=>x.Debts);
                var list = query.Select(x=> new DebtsPayDto
                {
                    Id = x.Id,
                    PaidAmount = x.PaidAmount,
                    //Interest = x.Interest,
                    //InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    DebtsId = x.DebtsId,
                    DebtsName = x.Debts.Name,
                    //RatePeriod = x.RatePeriod,
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

        public AppResponse<DebtsPayDto> GetDebtsPay(Guid Id)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var query = _debtsPayRepository.FindBy(x => x.Id == Id).Include(x => x.Debts);
                var debtsPay = query.Select(x => new DebtsPayDto
                {
                    DebtsName = x.Debts.Name,
                    Id = x.Id,
                    DebtsId= x.Debts.Id,
                    //Interest = x.Interest,
                    //InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    PaidAmount  = x.PaidAmount,
                    //RatePeriod= x.RatePeriod,
                }).First();
                result.BuildResult(debtsPay);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteDebtsPay(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var debtsPay = _debtsPayRepository.Get(Id);
                debtsPay.IsDeleted = true;

                _debtsPayRepository.Edit(debtsPay);
                var debt = _debtRepository.Get(debtsPay.DebtsId);
                debt.RemainAmount += debtsPay.PaidAmount;
                _debtRepository.Edit(debt);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> CreateDebtsPay(DebtsPayDto request, Guid DebtsId)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if(request.DebtsId==null) 
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var debts = _debtRepository.FindBy(m=>m.Id == request.DebtsId && m.IsDeleted!=true);
                if (debts.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }

                var debt = debts.First();
                var debtPay = new DebtsPay();
                debtPay.Id = Guid.NewGuid();
                debtPay.AccountId = accountInfo.Id;
                debtPay.DebtsId = DebtsId;
                if (debt.RemainAmount - debtPay.PaidAmount < 0)
                {
                    return result.BuildError("The amount paid is not greater than the remaining amount");
                }

                if (request.MoneyHolderId == null)
                {
                    return result.BuildError("Money Holder cannot be null");
                }
                //if (request.BudgetId == null)
                //{
                //    return result.BuildError("Budget cannot be null");
                //}
                //var budget = _budgetRepository.Get(request.BudgetId.Value);
                //if (budget == null)
                //{
                //    return result.BuildError("Cannot find Buddget");
                //}
                var moneyHolder = _moneyHolderRepository.Get(request.MoneyHolderId.Value);
                if (moneyHolder == null)
                {
                    return result.BuildError("Cannot find Money Holder");
                }
                debtPay.MoneyHolderId = moneyHolder.Id;
                //debtPay.BudgetId = budget.Id;
                //debtPay.Interest = request.Interest;
                //debtPay.InterestRate = debt.InterestRate;
                //debtPay.RatePeriod = debt.RatePeriod;
                debtPay.PaidAmount = request.PaidAmount;
                debtPay.IsPaid = true;



                debt.RemainAmount -= debtPay.PaidAmount;
                //if (budget.Balance == null) budget.Balance = 0;
                if (moneyHolder.Balance == null) moneyHolder.Balance = 0;
                //budget.Balance += debtPay.PaidAmount.Value + debtPay.Interest.Value;
                moneyHolder.Balance += debtPay.PaidAmount.Value ;

                _debtsPayRepository.Add(debtPay, accountInfo.Name);
                _debtRepository.Edit(debt);
                //_budgetRepository.Edit(budget);
                _moneyHolderRepository.Edit(moneyHolder);
                request.Id = debtPay.Id;
                return result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
		public AppResponse<SearchResponse<DebtsPayDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<DebtsPayDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = _debtsPayRepository.CountRecordsByPredicate(query);
				var model = _debtsPayRepository.FindByPredicate(query).Include(x=>x.Debts).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new DebtsPayDto
					{
						Id = x.Id,
						//InterestRate = x.InterestRate,
						PaidAmount = x.PaidAmount,
						//RatePeriod = x.RatePeriod,
                        DebtsId = x.DebtsId,
                        DebtsName = x.Debts.Name,
                        //Interest = x.Interest,
                        IsPaid = x.IsPaid,
					})
					.ToList();


				var searchUserResult = new SearchResponse<DebtsPayDto>
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
		private ExpressionStarter<DebtsPay> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<DebtsPay>(true);
                if(Filters!=null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						case "DebtsName":
							predicate = predicate.And(m => m.Debts.Name.Contains(filter.Value) && m.AccountId == accountId);
							break;
                         case "DebtsId":
                            predicate = predicate.And(m => m.Debts.Id.Equals(Guid.Parse(filter.Value)) && m.AccountId == accountId);
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
