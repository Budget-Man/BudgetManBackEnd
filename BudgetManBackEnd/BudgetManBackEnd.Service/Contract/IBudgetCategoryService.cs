using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IBudgetCategoryService
    {
        AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory(string userId);
        AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto request);
        AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto request);
        AppResponse<string> DeletebudgetCategory(Guid budgetCategoryId);
    }
}
