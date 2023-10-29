using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Implementation
{
    public class BudgetRepository : GenericRepository<Budget, BudgetManDbContext>, IBudgetRepository
    {
        public BudgetRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }
    }
}
