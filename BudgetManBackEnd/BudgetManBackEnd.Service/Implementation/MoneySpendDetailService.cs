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
	public class MoneySpendDetailService : IMoneySpendDetailService
    {
        private IMoneySpendRepository _moneySpendRepository;
        private IMoneySpendDetailRepository _moneySpendDetailRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;

        public MoneySpendDetailService(IMoneySpendRepository moneySpendRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneySpendDetailRepository moneySpendDetailRepository)
        {
            _moneySpendRepository = moneySpendRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneySpendDetailRepository = moneySpendDetailRepository;
        }

        public AppResponse<MoneySpendDetailDto> GetMoneySpendDetail(Guid Id)
        {
            var result = new AppResponse<MoneySpendDetailDto>();
            try
            {
                var query = _moneySpendDetailRepository.FindBy(x => x.Id == Id).Include(x => x.MoneySpend);
                var loanPay = query.Select(x=> new MoneySpendDetailDto
                {
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Id = x.Id,
                    Price = x.Price,
                    Reason = x.Reason,
                }).First();
                var data = _mapper.Map<MoneySpendDetailDto>(loanPay);
             
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<MoneySpendDetailDto>> GetAllMoneySpendDetail()
        {
            var result = new AppResponse<List<MoneySpendDetailDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _moneySpendDetailRepository.GetAll().Where(x => x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x => x.MoneySpend);
                var list = query
                    .Select(x => new MoneySpendDetailDto
                    {
                        Id = x.Id,
                        Quantity = x.Quantity,
                        Amount = x.Amount,
                        Price = x.Price,
                        Reason = x.Reason,
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

        public AppResponse<MoneySpendDetailDto> CreateMoneySpendDetail(MoneySpendDetailDto request)
        {
            var result = new AppResponse<MoneySpendDetailDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                
              
                var moneySpendDetail = _mapper.Map<MoneySpendDetail>(request);
                moneySpendDetail.Id = Guid.NewGuid();
                moneySpendDetail.AccountId = accountInfo.Id;
                moneySpendDetail.MoneySpend = null;
                _moneySpendDetailRepository.Add(moneySpendDetail, accountInfo.Name);

                request.Id = moneySpendDetail.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneySpendDetailDto> EditMoneySpendDetail(MoneySpendDetailDto request)
        {
            var result = new AppResponse<MoneySpendDetailDto>();
            try
            {
                var moneySpendDetail = _moneySpendDetailRepository.Get((Guid)request.Id);
                
                moneySpendDetail.Price = request.Price;
                moneySpendDetail.Quantity = request.Quantity;
                moneySpendDetail.Amount = request.Amount;
                moneySpendDetail.Reason = request.Reason;
                _moneySpendDetailRepository.Edit(moneySpendDetail);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteMoneySpendDetail(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var loanPay = _moneySpendDetailRepository.Get(Id);
                loanPay.IsDeleted = true;
                _moneySpendDetailRepository.Edit(loanPay);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
		public AppResponse<SearchResponse<MoneySpendDetailDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<MoneySpendDetailDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = -_moneySpendDetailRepository.CountRecordsByPredicate(query);
				var model = _moneySpendDetailRepository.FindByPredicate(query)
                    .Include(x=>x.MoneySpend).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new MoneySpendDetailDto
					{
						Id = x.Id,
                        Quantity = x.Quantity,
                        Amount = x.Amount,
                        Price = x.Price,
                        Reason = x.Reason,
					})
					.ToList();


				var searchUserResult = new SearchResponse<MoneySpendDetailDto>
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
		private ExpressionStarter<MoneySpendDetail> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<MoneySpendDetail>(true);
                if(Filters != null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						
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
