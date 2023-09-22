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
                    IsPaid = x.IsPaid,
                    MoneySpendId = x.MoneySpendId,
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

                var query = _moneySpendDetailRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.MoneySpend);
                var list = query
                    .Select(x => new MoneySpendDetailDto
                    {
                        Id = x.Id,
                        Quantity = x.Quantity,
                        Amount = x.Amount,
                        IsPaid = x.IsPaid,
                        MoneySpendId = x.MoneySpendId,
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
                if (request.MoneySpendId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _moneySpendRepository.FindBy(m => m.Id == request.MoneySpendId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
              
                var loanPay = _mapper.Map<MoneySpendDetail>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.MoneySpend = loan.First();

                _moneySpendDetailRepository.Add(loanPay, accountInfo.Name);

                request.Id = loanPay.Id;
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
                var loanPay = _mapper.Map<MoneySpendDetail>(request);
                _moneySpendDetailRepository.Edit(loanPay);
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
    }
}
