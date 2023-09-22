using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
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

                var query = _incomeRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.MoneyHolder);
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
                    return result.BuildError("Income Cannot be null");
                }
                var query = _incomeRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted != true);
                if (query.Count() == 0)
                {
                    return result.BuildError("Cannot find Income");
                }
                var income = _mapper.Map<Income>(request);
                income.Id = Guid.NewGuid();
                income.AccountId = accountInfo.Id;
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
                var income = _mapper.Map<Income>(request);
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
    }
}
