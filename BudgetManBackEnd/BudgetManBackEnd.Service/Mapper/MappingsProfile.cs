using AutoMapper;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Request;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Mapper
{
    public class MappingsProfile: Profile
    {
        public MappingsProfile()
        {
            CreateMap();
        }
        protected void CreateMap()
        {
            CreateMap<BudgetCategory, BudgetCategoryDto>()
                .ReverseMap();
            CreateMap<IdentityUser, UserModel>()
                .ReverseMap();
            CreateMap<Debt, DebtDto>()
                .ReverseMap();
            CreateMap<DebtsPay, DebtsPayDto>()
                .ReverseMap();
            CreateMap<Budget, BudgetDto>()
                .ReverseMap();
            CreateMap<Loan, LoanDto>()
                .ReverseMap();
            CreateMap<LoanPay, LoanPayDto>()
                .ReverseMap();
            CreateMap<MoneyHolder, MoneyHolderDto>()
                .ReverseMap();
            CreateMap<MoneySpend, MoneySpendDto>()
                .ReverseMap();
            CreateMap<MoneySpendDetail, MoneySpendDetailDto>()
                .ReverseMap();
            CreateMap<LocalTransfer, LocalTransferDto>()
                .ReverseMap();
            CreateMap<Income, IncomeDto>()
                .ReverseMap();
            CreateMap<MoneySpend, CreateMoneySpendRequest>()
                .ReverseMap();

        }
    }
}
