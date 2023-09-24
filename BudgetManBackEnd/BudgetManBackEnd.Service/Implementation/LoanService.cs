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
    public class LoanService:ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoanService(ILoanRepository loanRepository, IAccountInfoRepository accountInfoRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _loanRepository = loanRepository;
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public AppResponse<LoanDto> CreateLoan(LoanDto request)
        {
            var result = new AppResponse<LoanDto>();
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
                var loan = _mapper.Map<Loan>(request);
                loan.Id = Guid.NewGuid();
                loan.AccountId = accountInfo.Id;

                _loanRepository.Add(loan, accountInfo.Name);

                request.Id = loan.Id;
                result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteLoan(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var loan = _loanRepository.Get(Id);
                loan.IsDeleted = true;
                _loanRepository.Edit(loan);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<LoanDto> EditLoan(LoanDto request)
        {
            var result = new AppResponse<LoanDto>();
            try
            {
                var loan = _loanRepository.Get((Guid)request.Id);
                loan.Name = request.Name;
                loan.TotalAmount = request.TotalAmount;
                loan.RemainAmount = request.RemainAmount;
                loan.LoanAmount = request.LoanAmount;
                loan.TotalInterest = request.TotalInterest;
                loan.InterestRate = request.InterestRate;
                loan.RatePeriod = request.RatePeriod;
                _loanRepository.Edit(loan);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<LoanDto>> GetAllLoan()
        {
            var result = new AppResponse<List<LoanDto>>();
            try
            {
                var query = _loanRepository.GetAll();
                var list = query
                    .Select(x => new LoanDto
                    {
                        Id = x.Id,
                        InterestRate = x.InterestRate,
                        LoanAmount = x.LoanAmount,
                        Name = x.Name,
                        RatePeriod = x.RatePeriod,
                        RemainAmount = x.RemainAmount,
                        TotalAmount = x.TotalAmount,
                        TotalInterest = x.TotalInterest
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

        public AppResponse<LoanDto> GetLoan(Guid Id)
        {
            var result = new AppResponse<LoanDto>();
            try
            {
                var query = _loanRepository.Get(Id);
                var data = _mapper.Map<LoanDto>(query);

                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
