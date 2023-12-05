using System.Data.Entity;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Request;
using BudgetManBackEnd.Service.Contract;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using static MayNghien.Common.Helpers.SearchHelper;

namespace BudgetManBackEnd.Service.Implementation
{
	public class MoneySpendService:IMoneySpendService
    {
        private IMoneySpendRepository _moneySpendRepository;
        private IMoneySpendDetailRepository _moneySpendDetailRepository;
        private IMoneyHolderRepository _moneyHolderRepository;
        private IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;

        public MoneySpendService(IMoneySpendRepository moneySpendRepository, 
            IMoneyHolderRepository moneyHolderRepository, IBudgetRepository budgetRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            IAccountInfoRepository accountInfoRepository, IMoneySpendDetailRepository moneySpendDetailRepository)
        {
            _moneySpendRepository = moneySpendRepository;
            _moneyHolderRepository = moneyHolderRepository;
            _budgetRepository = budgetRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneySpendDetailRepository = moneySpendDetailRepository;
        }

        public AppResponse<MoneySpendDto> GetMoneySpend(Guid Id)
        {
            var result = new AppResponse<MoneySpendDto>();
            try
            {
                var query = _moneySpendRepository.FindBy(x => x.Id == Id)
                    .Include(x => x.Budget.BudgetCategory)
                    .Include(x=>x.MoneyHolder);
                var data = query.Select(x=> new MoneySpendDto
                {
                    Amount = x.Amount,
                    BudgetId = x.BudgetId,
                    Description = x.Description,
                    Id = x.Id,
                    IsPaid = x.IsPaid,
                    MoneyHolderId = x.MoneyHolderId,
                    MoneyHolderName = x.MoneyHolder.Name,
                    Reason = x.Reason,
                    BudgetName = x.Budget.BudgetCategory.Name,
                }).First();
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<MoneySpendDto>> GetAllMoneySpend()
        {
            var result = new AppResponse<List<MoneySpendDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _moneySpendRepository.GetAll().Where(x => x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x => x.Budget.BudgetCategory);
                var list = query
                    .Select(x => new MoneySpendDto
                    {
                        Id = x.Id,
                        BudgetId = x.BudgetId,
                        BudgetName = x.Budget.BudgetCategory.Name,
                        IsPaid = x.IsPaid,
                        MoneyHolderId = x.MoneyHolderId,
                        Amount = x.Amount,
                        MoneyHolderName = x.MoneyHolder.Name,
                        Reason= x.Reason,
                        Description= x.Description,
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

        public AppResponse<CreateMoneySpendRequest> CreateMoneySpend(CreateMoneySpendRequest request)
        {
            var result = new AppResponse<CreateMoneySpendRequest>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if(request.BudgetId == null)
                {
                    return result.BuildError("Budget cannot null");
                }
                var budgets = _budgetRepository.FindBy(m => m.Id == request.BudgetId && m.IsDeleted == false);
                if (budgets.Count()== 0)
                {
                    return result.BuildError("cannot find budget");
                }
                if(request.MoneyHolderId == null)
                {
                    return result.BuildError("Money holder cannot null");
                }
                var moneyHolders = _moneyHolderRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted == false);
                if (moneyHolders.Count()== 0)
                {
                    return result.BuildError("Cannot find money holder");
                }
                var moneySpend = _mapper.Map<MoneySpend>(request);
                moneySpend.Id = Guid.NewGuid();
                moneySpend.Budget = null;
                moneySpend.MoneyHolder = null;
                moneySpend.Reason = request.Reason;
                moneySpend.AccountId = accountInfo.Id;
                var listDetails = new List<MoneySpendDetail>();

                foreach (var item in request.Details)
                {
                    var detail = new MoneySpendDetail();
                    detail.Id= Guid.NewGuid();
                    detail.Quantity = item.Quantity;
                    detail.Price= item.Price;
                    detail.Amount = item.Quantity.Value * item.Price.Value;
                    detail.CreatedBy = userId;
                    detail.CreatedOn = DateTime.UtcNow;
                    detail.MoneySpendId= moneySpend.Id;
                    detail.AccountId = accountInfo.Id;
                    detail.Reason = item.Reason;
                    listDetails.Add(detail);
                }
                moneySpend.Amount = listDetails.Sum(m => m.Amount);
                var moneyHolder = _moneyHolderRepository.Get(request.MoneyHolderId);
                moneyHolder.Balance-=moneySpend.Amount;
                var budget =_budgetRepository.Get(request.BudgetId);
                if (budget.Balance == null) budget.Balance = 0;
                budget.Balance-=moneySpend.Amount;


                _moneySpendRepository.Add(moneySpend, userId);
                _moneySpendDetailRepository.AddRange(listDetails);
                _budgetRepository.Edit(budget);
                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneySpendDto> EditMoneySpend(MoneySpendDto request)
        {
            var result = new AppResponse<MoneySpendDto>();
            try
            {
                var moneySpend = _moneySpendRepository.Get((Guid)request.Id);
                moneySpend.BudgetId = request.BudgetId;
                moneySpend.Amount = request.Amount;
                moneySpend.MoneyHolderId = request.MoneyHolderId;
                moneySpend.Reason = request.Reason;
                moneySpend.Description = request.Description;
                moneySpend.IsPaid = request.IsPaid;
                _moneySpendRepository.Edit(moneySpend);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteMoneySpend(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var moneySpend = _moneySpendRepository.Get(Id);
                moneySpend.IsDeleted = true;
                _moneySpendRepository.Edit(moneySpend);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
		public AppResponse<SearchResponse<MoneySpendDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<MoneySpendDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = -_moneySpendRepository.CountRecordsByPredicate(query);
				var model = _moneySpendRepository.FindByPredicate(query)
					.Include(x => x.MoneyHolder).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new MoneySpendDto
					{
						Id = x.Id,
						Reason = x.Reason,
                        BudgetId = x.BudgetId,
                        BudgetName=x.Budget.Name,
                        MoneyHolderId=x.MoneyHolderId,
                        MoneyHolderName=x.MoneyHolder.Name,
                        Amount=x.Amount,
                        
					})
					.ToList();


				var searchUserResult = new SearchResponse<MoneySpendDto>
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
		private ExpressionStarter<MoneySpend> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<MoneySpend>(true);
                if(Filters != null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
                            case "moneyHolderName":
                                predicate = predicate.And(m => m.MoneyHolder.Name.Contains(filter.Value) && m.AccountId == accountId);
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
