using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Implementation
{
    public class BudgetCategoryService : IBudgetCategoryService
    {
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        public AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto budgetCategory)
        {
            throw new NotImplementedException();
        }

        public AppResponse<string> DeletebudgetCategory(Guid budgetCategoryId)
        {
            throw new NotImplementedException();
        }

        public AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto budgetCategory)
        {
            throw new NotImplementedException();
        }

        public AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
