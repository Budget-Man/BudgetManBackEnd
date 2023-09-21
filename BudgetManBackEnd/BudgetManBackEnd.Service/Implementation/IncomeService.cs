using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Implementation
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _loanPayRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IMoneyHolderRepository _loanRepository;
        public IncomeService(IIncomeRepository loanPayRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneyHolderRepository loanRepository)
        {
            _loanPayRepository = loanPayRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _loanRepository = loanRepository;
        }

        public AppResponse<IncomeDto> GetIncome(Guid Id)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var query = _loanPayRepository.FindBy(x => x.Id == Id).Include(x => x.MoneyHolder);
                var loanPay = query.First();
                var data = _mapper.Map<IncomeDto>(loanPay);
                data.MoneyHolderName = loanPay.MoneyHolder.Name;
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

                var query = _loanPayRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.MoneyHolder);
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
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _loanRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                var loanPay = _mapper.Map<Income>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.MoneyHolder = loan.First();
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

        public AppResponse<IncomeDto> EditIncome(IncomeDto request)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var loanPay = _mapper.Map<Income>(request);
                _loanPayRepository.Edit(loanPay);
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
