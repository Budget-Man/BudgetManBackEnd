using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
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

        public MoneySpendService(IMoneySpendRepository moneySpendRepository, IMoneyHolderRepository moneyHolderRepository, IBudgetRepository budgetRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _moneySpendRepository = moneySpendRepository;
            _moneyHolderRepository = moneyHolderRepository;
            _budgetRepository = budgetRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public AppResponse<MoneySpendDto> GetMonaySpend(Guid Id)
        {
            throw new NotImplementedException();
        }

        public AppResponse<List<MoneySpendDto>> GetAllMonaySpends()
        {
            throw new NotImplementedException();
        }

        public AppResponse<MoneySpendDto> CreateMoneySpend(MoneySpendDto request)
        {
            throw new NotImplementedException();
        }

        public AppResponse<MoneySpendDto> EditMoneySpend(MoneySpendDto request)
        {
            throw new NotImplementedException();
        }

        public AppResponse<string> DeleteMoneySpend(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}
