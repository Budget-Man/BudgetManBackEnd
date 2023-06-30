using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using Maynghien.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Implementation
{
    public class BudgetCategoryRepository:GenericRepository<BudgetCategory, BudgetManDbContext>,IBudgetCategoryRepository
    {
        public BudgetCategoryRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }

        public IQueryable<BudgetCategory> GetAll(string UserId)
        {
            var result = _context.BudgetCategorys.Where(m => m.IsDeleted != false && m.Account.UserId==UserId)
                
                .AsQueryable();
            return result;
        }
        public BudgetCategory? FindById(Guid Id)
        {
            return _context.BudgetCategorys.FirstOrDefault(m => m.Id==Id &&m.IsDeleted==false);
        }
    }
}
