using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;

namespace BudgetManBackEnd.Service.Implementation
{
    public class LoanPayService : ILoanPayService
    {
        private readonly ILoanPayRepository _loanPayRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        public LoanPayService(ILoanPayRepository loanPayRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository)
        {
            _loanPayRepository = loanPayRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
        }

        public AppResponse<LoanPayDto> GetLoanPay(Guid Id)
        {
            var result = new AppResponse<LoanPayDto>();
            try
            {
                var query = _loanPayRepository.FindBy(x=>x.Id == Id).Include(x=>x.Loan);
                var loanPay = query.First();
                var data = _mapper.Map<LoanPayDto>(loanPay);
                data.LoanName = loanPay.Loan.Name;
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

                var query = _loanPayRepository.GetAll().Where(x=>x.AccountId == accountInfo.Id).Include(x=>x.Loan);
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
                var loanPay =_mapper.Map<LoanPay>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;

                _loanPayRepository.Add(loanPay);

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
                var loanPay =  _mapper.Map<LoanPay>(request);
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
    }
}
