using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IIncomeService
    {
        AppResponse<List<IncomeDto>> GetAllIncome();
        AppResponse<IncomeDto> GetIncome(Guid Id);
        AppResponse<IncomeDto> EditIncome(IncomeDto request);
        AppResponse<string> DeleteIncome(Guid Id);
        AppResponse<IncomeDto> CreateIncome(IncomeDto request);
    }
}
