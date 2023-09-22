﻿using System;
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
                var query = _moneySpendRepository.FindBy(x => x.Id == Id)
                    .Include(x => x.Budget.BudgetCategory)
                    .Include(x=>x.MoneyHolder);
                var data = query.Select(x=> new MoneySpendDto
                {
                    Amount = x.Amount,
                    BudgetId = x.BudgetId,
                    Description = x.Description,
                    Id = x.Id,
                    IsPaid = x.IsPaid,
                    MoneyHolderId = x.MoneyHolderId,
                    MoneyHolderName = x.MoneyHolder.Name,
                    Reason = x.Reason,
                    BudgetName = x.Budget.BudgetCategory.Name,
                }).First();
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
                if(request.BudgetId == null)
                {
                    return result.BuildError("Budget cannot null");
                }
                var budget = _budgetRepository.FindBy(m => m.Id == request.BudgetId && m.IsDeleted == false);
                if (budget.Count()== 0)
                {
                    return result.BuildError("cannot find budget");
                }
                if(request.MoneyHolderId == null)
                {
                    return result.BuildError("Money holder cannot null");
                }
                var moneyHolder = _moneyHolderRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted == false);
                if (moneyHolder.Count()== 0)
                {
                    return result.BuildError("Cannot find money holder");
                }
                var moneySpend = _mapper.Map<MoneySpend>(request);
                moneySpend.Id = Guid.NewGuid();
                moneySpend.Budget = null;
                moneySpend.Modifiedby = null;

                _moneySpendRepository.Add(moneySpend, userId);

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
                var moneySpend = _mapper.Map<MoneySpend>(request);
                _moneySpendRepository.Edit(moneySpend);
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
                var moneySpend = _moneySpendRepository.Get(Id);
                moneySpend.IsDeleted = true;
                _moneySpendRepository.Edit(moneySpend);
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
