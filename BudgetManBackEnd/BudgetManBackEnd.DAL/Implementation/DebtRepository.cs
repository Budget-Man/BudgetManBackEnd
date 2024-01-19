using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Implementation
{
    public class DebtRepository : GenericRepository<Debt, BudgetManDbContext>, IDebtRepository
    {
        public DebtRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }

        public DebtDto GetDto(Guid id)
        {
            if (_context.Debts.Any(m => m.Id == id && m.IsDeleted == false))
            {
                return _context.Debts.Where(m => m.Id == id)
                    .Select(m => new DebtDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        InterestRate = m.InterestRate,
                        PaidAmount = m.PaidAmount,
                        MoneyHolderId = m.MoneyHolderId,
                        MoneyHolderName = m.MoneyHolder.Name,
                        RatePeriod = m.RatePeriod,
                        RemainAmount = m.RemainAmount,
                        TotalAmount = m.TotalAmount,
                        TotalInterest = m.TotalInterest,
                        
                    }).First();
            }
            else
            {
                return null;
            }
        }
    }
}
