using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IIncomeService
    {
        AppResponse<IncomeDto> GetIncome(Guid Id);
        AppResponse<List<IncomeDto>> GetAllIncome();
        AppResponse<IncomeDto> CreateIncome(IncomeDto request);
        AppResponse<IncomeDto> EditIncome(IncomeDto request);
        AppResponse<string> DeleteIncome(Guid Id);
    }
}
