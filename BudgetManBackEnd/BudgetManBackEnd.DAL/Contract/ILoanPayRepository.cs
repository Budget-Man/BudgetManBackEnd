using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Contract
{
    public interface ILoanPayRepository:IGenericRepository<LoanPay, BudgetManDbContext>
    {

    }
}
