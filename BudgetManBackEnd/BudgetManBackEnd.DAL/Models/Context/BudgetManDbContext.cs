using BudgetManBackEnd.DAL.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetManBackEnd.DAL.Models.Context
{
    public partial class BudgetManDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public BudgetManDbContext() { }

        public BudgetManDbContext(DbContextOptions options) : base(options)
        {
        }
        #region dbset
        public DbSet<AccountInfo> AccountInfos { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategorys { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<DebtsPay> DebtsPays { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPay> LoanPays { get; set; }
        public DbSet<LocalTransfer> LocalTransfers { get; set; }
        public DbSet<MoneyHolder> MoneyHolders { get; set; }
        public DbSet<MoneySpend> MoneySpends { get; set; }
        public DbSet<MoneySpendDetail> MoneySpendDetails { get; set; }

        #endregion
    }
}
