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
	public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IMoneyHolderRepository _moneyHolderRepository;
        public IncomeService(IIncomeRepository incomeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneyHolderRepository moneyHolderRepository)
        {
            _incomeRepository = incomeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneyHolderRepository = moneyHolderRepository;
        }

        public AppResponse<IncomeDto> GetIncome(Guid Id)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var query = _incomeRepository.FindBy(x => x.Id == Id).Include(x => x.MoneyHolder);
                var data = query.Select(x=>new IncomeDto
                {
                    Id = x.Id,
                    MoneyHolderId = x.MoneyHolderId,
                    MoneyHolderName =x.MoneyHolder.Name,
                    Name = x.Name,
                }).First();
                
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<IncomeDto>> GetAllIncome()
        {
            var result = new AppResponse<List<IncomeDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _incomeRepository.GetAll().Where(x => x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x => x.MoneyHolder);
                var list = query
                    .Select(x => new IncomeDto
                    {
                        Id = x.Id,
                       Name = x.Name,
                        MoneyHolderId = accountInfo.Id,
                       MoneyHolderName = x.MoneyHolder.Name,
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

        public AppResponse<IncomeDto> CreateIncome(IncomeDto request)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.MoneyHolderId == null)
                {
                    return result.BuildError("Money holder Cannot be null");
                }
                var query = _moneyHolderRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted != true);
                if (query.Count() == 0)
                {
                    return result.BuildError("Cannot find money holder");
                }
                var income = _mapper.Map<Income>(request);
                income.Id = Guid.NewGuid();
                income.AccountId = accountInfo.Id;
                income.MoneyHolder = null;
                _incomeRepository.Add(income, accountInfo.Name);

                request.Id = income.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<IncomeDto> EditIncome(IncomeDto request)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var income = _incomeRepository.Get((Guid)request.Id);
                income.Name = request.Name;
                income.MoneyHolderId = request.MoneyHolderId;
                _incomeRepository.Edit(income);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteIncome(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var income = _incomeRepository.Get(Id);
                income.IsDeleted = true;
                _incomeRepository.Edit(income);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
		public AppResponse<SearchResponse<IncomeDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<IncomeDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = -_incomeRepository.CountRecordsByPredicate(query);
				var model = _incomeRepository.FindByPredicate(query).Include(x=>x.MoneyHolder).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new IncomeDto
					{
						Id = x.Id,
						Name = x.Name,
                        MoneyHolderId = x.MoneyHolderId,
                        MoneyHolderName = x.MoneyHolder.Name
					})
					.ToList();


				var searchUserResult = new SearchResponse<IncomeDto>
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
		private ExpressionStarter<Income> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<Income>(true);
				predicate = predicate.And(m => m.IsDeleted == false);
				predicate = predicate.And(m => m.AccountId == accountId);
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
