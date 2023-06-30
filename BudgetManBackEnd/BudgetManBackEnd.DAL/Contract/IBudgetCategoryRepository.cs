using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Contract
{
    public interface IBudgetCategoryRepository : IGenericRepository<BudgetCategory, BudgetManDbContext>
    {
        IQueryable<BudgetCategory> GetAll(string UserId);
        BudgetCategory? FindById(Guid Id);
    }
}
