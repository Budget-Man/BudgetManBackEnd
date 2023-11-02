using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class BudgetService: IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;
        private IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BudgetService(IBudgetRepository budgetRepository, IAccountInfoRepository accountInfoRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBudgetCategoryRepository budgetCategoryRepository)
        {
            _budgetRepository = budgetRepository;
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _budgetCategoryRepository = budgetCategoryRepository;
        }

        public AppResponse<BudgetDto> CreateBudget(BudgetDto request)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.BudgetCategoryId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var budgetCategories = _budgetCategoryRepository.FindBy(m => m.Id == request.BudgetCategoryId && m.IsDeleted != true);
                if (budgetCategories.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }

                var budget = _mapper.Map<Budget>(request);
                budget.Id = Guid.NewGuid();
                budget.AccountId = accountInfo.Id;
                budget.BudgetCategory = budgetCategories.First();
                _budgetRepository.Add(budget, accountInfo.Name);
                request.Id = budget.Id;
                result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;

            
        }

        public AppResponse<string> DeleteBudget(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var budget = _budgetRepository.Get(Id);
                budget.IsDeleted = true;

                _budgetRepository.Edit(budget);
                result.BuildResult("Delete successfully");
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<BudgetDto> EditBudget(BudgetDto request)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var budget = _budgetRepository.Get((Guid)request.Id);
                budget.BudgetCategoryId = request.BudgetCategoryId;
                budget.Balance = request.Balance;

                _budgetRepository.Edit(budget);

                result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<BudgetDto>> GetAllBudget()
        {
            var result =  new AppResponse<List<BudgetDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
			var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
			if (accountInfoQuery.Count() == 0)
			{
				return result.BuildError("Cannot find Account Info by this user");
			}
			try
            {
                var query = _budgetRepository.GetAll()
                    .Where(x => x.Account.UserId == userId)
                    .Include(x=>x.BudgetCategory);
                var list = query
                    .Select(x=> new BudgetDto
                    {
                        BudgetCategoryName = x.BudgetCategory.Name,
                        Balance = x.Balance,
                        Id = x.Id,
                        BudgetCategoryId = x.BudgetCategory.Id,
                    } )
                    .ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<BudgetDto> GetBudget(Guid Id)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var query = _budgetRepository.FindBy(x=>x.Id == Id);
                var data = query.Select(x => new BudgetDto
                {
                    BudgetCategoryName= x.BudgetCategory.Name,
                    Balance= x.Balance,
                    BudgetCategoryId = x.BudgetCategory.Id,
                    Id = x.Id
                }).First();
                result.BuildResult(data);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message );
            }
            return result;
        }
		public AppResponse<SearchResponse<BudgetDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<BudgetDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = _budgetRepository.CountRecordsByPredicate(query);
				var model = _budgetRepository.FindByPredicate(query).Include(x=>x.BudgetCategory)OrderByDescending(x => x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new BudgetDto
					{
						Id = x.Id,
                        Balance = x.Balance,
                        BudgetCategoryId = x.BudgetCategoryId,
                        BudgetCategoryName = x.BudgetCategory.Name
					})
					.ToList();


				var searchUserResult = new SearchResponse<BudgetDto>
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
		private ExpressionStarter<Budget> BuildFilterExpression(IList<Filter> Filters , Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<Budget>(true);
                if(Filters != null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						case "BudgetCategoryName":
							predicate = predicate.And(m => m.BudgetCategory.Name.Contains(filter.Value) && m.AccountId == accountId);
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
