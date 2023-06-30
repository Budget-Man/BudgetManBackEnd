using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IBudgetCategoryService
    {
        AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory(string userId);
        AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto budgetCategory);
        AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto budgetCategory);
        AppResponse<string> DeletebudgetCategory(Guid budgetCategoryId);
    }
}
