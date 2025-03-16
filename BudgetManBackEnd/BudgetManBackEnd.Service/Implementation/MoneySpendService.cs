using System.Data.Entity;
using System.Diagnostics;
using System.IO.Pipelines;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Request;
using BudgetManBackEnd.Service.Contract;
using Intercom.Data;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Index.HPRtree;
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
        private IAccountBalanceTrackingRepository _accountBalanceTrackingRepository;

        public MoneySpendService(IMoneySpendRepository moneySpendRepository, 
            IMoneyHolderRepository moneyHolderRepository, IBudgetRepository budgetRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            IAccountInfoRepository accountInfoRepository, IMoneySpendDetailRepository moneySpendDetailRepository,
            IAccountBalanceTrackingRepository accountBalanceTrackingRepository)
        {
            _moneySpendRepository = moneySpendRepository;
            _moneyHolderRepository = moneyHolderRepository;
            _budgetRepository = budgetRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneySpendDetailRepository = moneySpendDetailRepository;
            _accountBalanceTrackingRepository = accountBalanceTrackingRepository;
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
                        //Details = GetMoneySpendDetail(x.Id)
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
                if (request.Details !=null && request.Details.Count > 0)
                {
                    foreach (var item in request.Details)
                    {
                        var detail = new MoneySpendDetail();
                        detail.Id = Guid.NewGuid();
                        detail.Quantity = item.Quantity;
                        detail.Price = item.Price;
                        detail.Amount = item.Quantity.Value * item.Price.Value;
                        detail.CreatedBy = userId;
                        detail.CreatedOn = DateTime.UtcNow;
                        detail.MoneySpendId = moneySpend.Id;
                        detail.AccountId = accountInfo.Id;
                        detail.Reason = item.Reason;
                        listDetails.Add(detail);
                    }
                    moneySpend.Amount = listDetails.Sum(m => m.Amount);
                }
                else
                {
                    moneySpend.Amount = request.Amount;
                }
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

                var accTracking = new AccountBalanceTracking
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountInfo.Id,
                    Amount = moneySpend.Amount,
                    MoneyHolderId = moneyHolder.Id,
                    ChangeType = Common.Enum.ChangeType.Spent,
                    CurrentBalance = moneyHolder.Balance + moneySpend.Amount,
                    NewBalance = moneyHolder.Balance,
                    BudgetId = budget.Id,
                };
                _accountBalanceTrackingRepository.Add(accTracking, accountInfo.Name);
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
                if (moneySpend == null)
                {
                    result.BuildError("MoneySpend is not existed!");
                    return result;
                }
                moneySpend.BudgetId = request.BudgetId;
                
                moneySpend.MoneyHolderId = request.MoneyHolderId;
                moneySpend.Reason = request.Reason;
                moneySpend.Description = request.Description;
                moneySpend.IsPaid = request.IsPaid;

                if (request.Details == null || request.Details.Count <= 0)
                {
                    moneySpend.Amount = request.Amount;
                    _moneySpendDetailRepository.SoftDeleteRange(x => x.MoneySpendId == moneySpend.Id);
                }
                else
                {
                    EditMoneySpendDetailsAsync(request.Details, moneySpend.Id, moneySpend.AccountId);
                    moneySpend.Amount = moneySpend.Amount = request.Details.Sum(x => x.Quantity.Value*x.Price.Value);
                }

                _moneySpendRepository.Edit(moneySpend);

                if (moneySpend.Amount != request.Amount)
                {
                    var moneyHolder = _moneyHolderRepository.Get(moneySpend.MoneyHolderId);
                    moneyHolder.Balance = moneyHolder.Balance + moneySpend.Amount - request.Amount;
                    _moneyHolderRepository.Edit(moneyHolder);
                }
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        protected void EditMoneySpendDetailsAsync(List<MoneySpendDetailDto> updatedDetails, Guid moneySpendId, Guid accountId)
        {
            var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            var existingDetails = _moneySpendDetailRepository.GetAll().Where(d => d.MoneySpendId == moneySpendId).ToList();

            var newDetails = new List<MoneySpendDetail>();
            foreach (var updatedDetail in updatedDetails)
            {
                var existingDetail = existingDetails.FirstOrDefault(d => d.Id == updatedDetail.Id);
                if (existingDetail != null)
                {
                    existingDetail.Quantity = updatedDetail.Quantity;
                    existingDetail.Price = updatedDetail.Price;
                    existingDetail.Amount = updatedDetail.Quantity.HasValue && updatedDetail.Price.HasValue
                                            ? updatedDetail.Quantity.Value * updatedDetail.Price.Value
                                            : existingDetail.Amount;
                    existingDetail.Reason = updatedDetail.Reason;
                    //existingDetail.IsPaid = updatedDetail.IsPaid;
                    existingDetail.AccountId = accountId;
                    _moneySpendDetailRepository.Edit(existingDetail);
                }
                else
                {
                    var detail = new MoneySpendDetail();
                    detail.Id = Guid.NewGuid();
                    detail.Quantity = updatedDetail.Quantity;
                    detail.Price = updatedDetail.Price;
                    detail.Amount = updatedDetail.Quantity.Value * updatedDetail.Price.Value;
                    detail.CreatedBy = userId;
                    detail.CreatedOn = DateTime.UtcNow;
                    detail.MoneySpendId = moneySpendId;
                    detail.AccountId = accountId;
                    detail.Reason = updatedDetail.Reason;
                    newDetails.Add(detail);
                }
                
            }
            if (updatedDetails.Count < existingDetails.Count)
            {
                var updatedDetailIds = updatedDetails.Select(x => x.Id).ToList();
                _moneySpendDetailRepository.SoftDeleteRange(x => !updatedDetailIds.Contains(x.Id));
            }
            _moneySpendDetailRepository.AddRange(newDetails);
        }
        public AppResponse<string> DeleteMoneySpend(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var moneySpend = _moneySpendRepository.Get(Id);
                var moneyHolder = _moneyHolderRepository.Get(moneySpend.MoneyHolderId);
                moneyHolder.Balance += moneySpend.Amount;
                moneySpend.IsDeleted = true;
                _moneySpendRepository.Edit(moneySpend);
                _moneyHolderRepository.Edit(moneyHolder);
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
                List.ForEach(x =>
                {
                    x.Details = GetMoneySpendDetail(x.Id);
                });

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
                            case "Reason":
                                predicate = predicate.And(m => m.Reason.Contains(filter.Value));
                                break;
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
				predicate = predicate.And(m => m.AccountId == accountId);
				return predicate;
			}
			catch (Exception)
			{

				throw;
			}
		}
        private List<MoneySpendDetailDto> GetMoneySpendDetail(Guid? moneySpendId)
        {
            if (moneySpendId == null)
            {
                return new List<MoneySpendDetailDto>();
            }
            var result = _moneySpendDetailRepository.GetAll().Where(x => x.MoneySpendId == moneySpendId && x.IsDeleted != true)
                .Select(x=> new MoneySpendDetailDto
                {
                    Id = x.Id,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Reason = x.Reason,
                    CreateOn = x.CreatedOn
                })
                .OrderBy(x=>x.CreateOn).ToList();
            return result;
        }
	}
}
