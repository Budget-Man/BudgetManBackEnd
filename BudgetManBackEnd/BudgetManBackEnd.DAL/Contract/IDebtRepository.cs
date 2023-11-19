using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Contract
{
    public interface IDebtRepository : IGenericRepository<Debt, BudgetManDbContext>
    {
        public DebtDto GetDto(Guid id);
    }
}
