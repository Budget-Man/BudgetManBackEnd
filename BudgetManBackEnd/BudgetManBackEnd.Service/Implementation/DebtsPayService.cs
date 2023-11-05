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
	public class DebtsPayService : IDebtsPayService
    {
        private readonly IDebtsPayRepository _debtsPayRepository;
        private readonly IDebtRepository _debtRepository;
        private IMapper _mapper;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DebtsPayService(IDebtsPayRepository debtsPayRepository, IMapper mapper, 
            IAccountInfoRepository accountInfoRepository, IDebtRepository debtRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _debtsPayRepository = debtsPayRepository;
            _mapper = mapper;
            _accountInfoRepository = accountInfoRepository;
            _httpContextAccessor = httpContextAccessor;
            _debtRepository = debtRepository;
        }

        public AppResponse<List<DebtsPayDto>> GetAllDebtsPay()
        {
            var result = new AppResponse<List<DebtsPayDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _debtsPayRepository.GetAll()
                    .Where(x => x.Account.UserId == userId)
                    .Include(x=>x.Debts);
                var list = query.Select(x=> new DebtsPayDto
                {
                    Id = x.Id,
                    PaidAmount = x.PaidAmount,
                    Interest = x.Interest,
                    InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    DebtsId = x.DebtsId,
                    DebtsName = x.Debts.Name,
                    RatePeriod = x.RatePeriod,
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
                    Interest = x.Interest,
                    InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    PaidAmount  = x.PaidAmount,
                    RatePeriod= x.RatePeriod,
                }).First();
                result.BuildResult(debtsPay);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> EditDebtsPay(DebtsPayDto request)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var debtsPay = _debtsPayRepository.Get((Guid)request.Id);
                debtsPay.DebtsId = request.DebtsId;
                debtsPay.PaidAmount = request.PaidAmount;
                debtsPay.RatePeriod = request.RatePeriod;
                debtsPay.InterestRate = request.InterestRate;
                debtsPay.Interest = request.Interest;
                debtsPay.IsPaid = (bool)request.IsPaid;
                _debtsPayRepository.Edit(debtsPay);

                result.BuildResult(request);
            }
            catch( Exception ex )
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
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> CreateDebtsPay(DebtsPayDto request)
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
                var debtsPay = new DebtsPay();
                debtsPay.Id = Guid.NewGuid();
                debtsPay.AccountId = accountInfo.Id;
                debtsPay.DebtsId = request.DebtsId;
                debtsPay.InterestRate = request.InterestRate;
                debtsPay.Interest= request.Interest;
                debtsPay.PaidAmount = request.PaidAmount;
                debtsPay.RatePeriod = request.RatePeriod;
                debtsPay.IsPaid = request.IsPaid??true;
                debtsPay.Debts = null;
                _debtsPayRepository.Add(debtsPay, accountInfo.Name);


                result.BuildResult(request);
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
						InterestRate = x.InterestRate,
						PaidAmount = x.PaidAmount,
						RatePeriod = x.RatePeriod,
                        DebtsId = x.DebtsId,
                        DebtsName = x.Debts.Name,
                        Interest = x.Interest,
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
