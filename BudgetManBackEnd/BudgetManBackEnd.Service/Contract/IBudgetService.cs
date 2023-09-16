using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IBudgetService
    {
        AppResponse<List<BudgetDto>> GetAllBudget();
        AppResponse<BudgetDto> GetBudget(Guid Id);
        AppResponse<BudgetDto> CreateBudget(BudgetDto request);
        AppResponse<BudgetDto> EditBudget(BudgetDto request);
        AppResponse<string> DeleteBudget(Guid Id);
    }
}
