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
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;

namespace BudgetManBackEnd.Service.Implementation
{
    public class MoneySpendService:IMoneySpendService
    {
        private IMoneySpendRepository _moneySpendRepository;
        private IMoneyHolderRepository _moneyHolderRepository;
        private IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;

        public MoneySpendService(IMoneySpendRepository moneySpendRepository, IMoneyHolderRepository moneyHolderRepository, IBudgetRepository budgetRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository)
        {
            _moneySpendRepository = moneySpendRepository;
            _moneyHolderRepository = moneyHolderRepository;
            _budgetRepository = budgetRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
        }

        public AppResponse<MoneySpendDto> GetMoneySpend(Guid Id)
        {
            var result = new AppResponse<MoneySpendDto>();
            try
            {
                var query = _moneySpendRepository.FindBy(x => x.Id == Id).Include(x => x.Budget.BudgetCategory).Include(x=>x.MoneyHolder);
                var loanPay = query.First();
                var data = _mapper.Map<MoneySpendDto>(loanPay);
                data.BudgetName = loanPay.Budget.BudgetCategory.Name;
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<MoneySpendDto>> GetAllMoneySpend()
        {
            var result = new AppResponse<List<MoneySpendDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _moneySpendRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.Budget.BudgetCategory);
                var list = query
                    .Select(x => new MoneySpendDto
                    {
                        Id = x.Id,
                        BudgetId = x.BudgetId,
                        BudgetName = x.Budget.BudgetCategory.Name,
                        IsPaid = x.IsPaid,
                        MoneyHolderId = x.MoneyHolderId,
                        Amount = x.Amount,
                        MoneyHolderName = x.MoneyHolder.Name,
                        Reason= x.Reason,
                        Description= x.Description,
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

        public AppResponse<MoneySpendDto> CreateMoneySpend(MoneySpendDto request)
        {
            var result = new AppResponse<MoneySpendDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.BudgetId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _moneyHolderRepository.FindBy(m => m.Id == request.BudgetId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                if (request.MoneyHolderId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan2 = _budgetRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted != true);
                if (loan2.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                var loanPay = _mapper.Map<MoneySpend>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.MoneyHolder= loan.First();
                loanPay.Budget = loan2.First();
                _moneySpendRepository.Add(loanPay, accountInfo.Name);

                request.Id = loanPay.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneySpendDto> EditMoneySpend(MoneySpendDto request)
        {
            var result = new AppResponse<MoneySpendDto>();
            try
            {
                var loanPay = _mapper.Map<MoneySpend>(request);
                _moneySpendRepository.Edit(loanPay);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteMoneySpend(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var loanPay = _moneySpendRepository.Get(Id);
                loanPay.IsDeleted = true;
                _moneySpendRepository.Edit(loanPay);
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
