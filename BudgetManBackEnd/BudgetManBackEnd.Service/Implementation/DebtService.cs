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
	public class DebtService : IDebtService
    {
        private IDebtRepository _debtRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;

        public DebtService(IDebtRepository debtRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository)
        {
            _debtRepository = debtRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
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


                var debt = _mapper.Map<Debt>(request);
                debt.Id = Guid.NewGuid();
                debt.AccountId = accountInfo.Id;

                request.Id = debt.Id;

                _debtRepository.Add(debt, accountInfo.Name);

                result.BuildResult(request);


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
                debt.TotalAmount = request.TotalAmount;
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
                var query = _debtRepository.GetAll().Where(x=>x.Account.UserId == userId);
                var list =  query.Select(x => new DebtDto
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
                var debt = _debtRepository.Get(Id);
                var data = _mapper.Map<DebtDto>(debt);
                result.BuildResult(data);
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
                var model = _debtRepository.FindByPredicate(query);
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
