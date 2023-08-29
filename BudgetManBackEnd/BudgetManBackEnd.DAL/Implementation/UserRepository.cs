using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly BudgetManDbContext _context;
        public UserRepository(BudgetManDbContext context)
        {
            _context = context;
        }

        public int CountRecordsByPredicate(Expression<Func<IdentityUser, bool>> predicate)
        {
            return _context.Users.Where(predicate).Count();
        }

        public IdentityUser? FindById(string id)
        {
            return _context.Users.Where(m => m.Id == id).FirstOrDefault();
        }

        public IQueryable<IdentityUser> FindByPredicate(Expression<Func<IdentityUser, bool>> predicate)
        {
            return _context.Users.AsNoTrackingWithIdentityResolution().Where(predicate).AsQueryable();
        }

        
    }
}
