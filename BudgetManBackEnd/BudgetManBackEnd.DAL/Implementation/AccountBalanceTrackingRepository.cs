﻿using BudgetManBackEnd.DAL.Contract;
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
    public class AccountBalanceTrackingRepository : GenericRepository<AccountBalanceTracking, BudgetManDbContext>, IAccountBalanceTrackingRepository
    {
        public AccountBalanceTrackingRepository(BudgetManDbContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }


    }
}