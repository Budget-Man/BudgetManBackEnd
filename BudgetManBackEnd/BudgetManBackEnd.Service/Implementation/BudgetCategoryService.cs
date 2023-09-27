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
    public class BudgetCategoryService : IBudgetCategoryService
    {
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public BudgetCategoryService(IBudgetCategoryRepository budgetCategoryRepository, IMapper mapper,IAccountInfoRepository accountInfoRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _budgetCategoryRepository = budgetCategoryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
        }

        public AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto request)
        {
            var result = new AppResponse<BudgetCategoryDto>();
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
                var budgetcat = new BudgetCategory();
                budgetcat = _mapper.Map<BudgetCategory>(request);
                budgetcat.Id = Guid.NewGuid();
                budgetcat.AccountId = accountInfo.Id;
                _budgetCategoryRepository.Add(budgetcat, accountInfo.Name);
                request.Id = budgetcat.Id;
                result.IsSuccess = true;
                result.Data = request;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message + ":" + ex.StackTrace;
                return result;

            }

        }

        public AppResponse<string> DeletebudgetCategory(Guid request)
        {
            var result = new AppResponse<string>();
            try
            {
                var budgetcat = new BudgetCategory();
                budgetcat = _budgetCategoryRepository.Get(request);
                budgetcat.IsDeleted = true;

                _budgetCategoryRepository.Edit(budgetcat);

                result.IsSuccess = true;
                result.Data = "Delete Sucessfuly";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message + ":" + ex.StackTrace;
                return result;

            }
        }

        public AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto request)
        {
            var result = new AppResponse<BudgetCategoryDto>();
            try
            {
                var budgetcat = new BudgetCategory();
                if (request.Id == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Id cannot be null";
                    return result;
                }
                budgetcat = _budgetCategoryRepository.Get(request.Id.Value);
                budgetcat.Name = request.Name;
                //budgetcat.Id = Guid.NewGuid();
                _budgetCategoryRepository.Edit(budgetcat);

                result.IsSuccess = true;
                result.Data = request;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message + ":" + ex.StackTrace;
                return result;

            }
        }

        public AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory()
        {
            var result = new AppResponse<List<BudgetCategoryDto>>();
            
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId"); ;
            try
            {
                var query = _budgetCategoryRepository.GetAll().Where(m => m.Account.UserId == userId);
                var list = query.Select(m => new BudgetCategoryDto
                {
                    Name = m.Name,
                    Id = m.Id,
                }).ToList();
                result.IsSuccess = true;
                result.Data = list;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message =ex.Message +" "+ ex.StackTrace;
                return result;
            }

        }

        public AppResponse<BudgetCategoryDto> GetBudgetCategory(Guid budgetCategoryId)
        {
            var result = new AppResponse<BudgetCategoryDto>();
            try
            {
                var budcat = _budgetCategoryRepository.Get(budgetCategoryId);
                var data = _mapper.Map<BudgetCategoryDto>(budcat);
                result.IsSuccess = true;
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.StackTrace;
                return result;

            }

            return result;
        }

		public AppResponse<SearchResponse<BudgetCategoryDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<BudgetCategoryDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = _budgetCategoryRepository.CountRecordsByPredicate(query);
				var budgetCate = _budgetCategoryRepository.FindByPredicate(query);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = budgetCate.Skip(startIndex).Take(pageSize)
					.Select(x => new BudgetCategoryDto
					{
						Id = x.Id,
						Name = x.Name,
					})
					.ToList();


				var searchUserResult = new SearchResponse<BudgetCategoryDto>
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
		private ExpressionStarter<BudgetCategory> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<BudgetCategory>(true);

				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						case "Name":
							predicate = predicate.And(m => m.Name.Contains(filter.Value) && m.AccountId == accountId);
							break;
						default:
							break;
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
