using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Context;
using BudgetManBackEnd.DAL.Models.Entity;
using Maynghien.Common.Repository;

namespace BudgetManBackEnd.DAL.Implementation
{
    public class AccountInfoRepository : GenericRepository<AccountInfo, BudgetManDbContext>, IAccountInfoRepository
    {
        public AccountInfoRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }


    }
}
