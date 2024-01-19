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
    public class LoanRepository : GenericRepository<Loan, BudgetManDbContext>, ILoanRepository
    {
        public LoanRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }

        public LoanDto? GetDto(Guid id)
        {
            if(_context.Loans.Any(m=>m.Id == id && m.IsDeleted == false))
            {
                return _context.Loans.Where(m=>m.Id==id)
                    .Select(m=> new LoanDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        InterestRate = m.InterestRate,
                        
                        MoneyHolderId = m.MoneyHolderId,
                        MoneyHolderName=m.MoneyHolder.Name,
                        RatePeriod = m.RatePeriod,
                        RemainAmount = m.RemainAmount,  
                        TotalAmount = m.TotalAmount,
                        TotalInterest=m.TotalInterest,
                    }).First();
            }
            else
            {
                return null;
            }
        }
    }
}
