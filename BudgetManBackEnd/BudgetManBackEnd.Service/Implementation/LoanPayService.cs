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
        public LoanPayService(ILoanPayRepository loanPayRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, ILoanRepository loanRepository)
        {
            _loanPayRepository = loanPayRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _loanRepository = loanRepository;
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
                    InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    LoanId = x.LoanId,
                    LoanName = x.Loan.Name,
                    PaidAmount = x.PaidAmount,
                    RatePeriod = x.RatePeriod,
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
                        InterestRate = x.InterestRate,
                        IsPaid = x.IsPaid,
                        LoanId = accountInfo.Id,
                        LoanName = x.Loan.Name,
                        PaidAmount = x.PaidAmount,
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

        public AppResponse<LoanPayDto> CreateLoanPay(LoanPayDto request)
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
                if (request.LoanId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _loanRepository.FindBy(m => m.Id == request.LoanId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                var loanPay =_mapper.Map<LoanPay>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.Loan = loan.First();
                loanPay.Loan = null;
                _loanPayRepository.Add(loanPay, accountInfo.Name);

                request.Id = loanPay.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<LoanPayDto> EditLoanPay(LoanPayDto request)
        {
            var result = new AppResponse<LoanPayDto>();
            try
            {
                var loanPay = _loanPayRepository.Get((Guid)request.Id);
                loanPay.LoanId = request.LoanId;
                loanPay.PaidAmount = request.PaidAmount;
                loanPay.Interest = request.Interest;
                loanPay.InterestRate = request.InterestRate;
                loanPay.RatePeriod = request.RatePeriod;
                loanPay.IsPaid = request.IsPaid;
                _loanPayRepository.Edit(loanPay);
                result.BuildResult(request);
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
				var numOfRecords = -_loanPayRepository.CountRecordsByPredicate(query);
				var model = _loanPayRepository.FindByPredicate(query).OrderByDescending(x=>x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new LoanPayDto
					{
						Id = x.Id,
						Interest = x.Interest,
                        InterestRate = x.InterestRate,
                        IsPaid = x.IsPaid,
                        LoanId = x.LoanId,
                        LoanName = x.Loan.Name,
                        PaidAmount = x.PaidAmount,
                        RatePeriod = x.RatePeriod,
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
