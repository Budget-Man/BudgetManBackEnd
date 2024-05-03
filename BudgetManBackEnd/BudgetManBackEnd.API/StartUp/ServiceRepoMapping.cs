using System.Security.Cryptography.X509Certificates;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;

namespace BudgetManBackEnd.API.StartUp
{
    public class ServiceRepoMapping
    {
        public ServiceRepoMapping()
        {

        }

        public void Mapping(WebApplicationBuilder builder)
        {
            #region Service Mapping
            builder.Services.AddScoped<IBudgetCategoryService, BudgetCategoryService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDebtService, DebtService>();
            builder.Services.AddScoped<IDebtsPayService, DebtsPayService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<ILoanPayService, LoanPayService>();
            builder.Services.AddScoped<IMoneyHolderService, MoneyHolderService>();
            builder.Services.AddScoped<IMoneySpendDetailService, MoneySpendDetailService>();
            builder.Services.AddScoped<IIncomeService, IncomeService>();
            builder.Services.AddScoped<ILocalTransferService,  LocalTransferService>();
            builder.Services.AddScoped<IMoneySpendService, MoneySpendService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountBalanceTrackingService, AccountBalanceTrackingService>();
            
            #endregion Service Mapping

            #region Repository Mapping
            builder.Services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
            builder.Services.AddScoped<IAccountInfoRepository, AccountInfoRepository>();
            builder.Services.AddScoped<IDebtRepository, DebtRepository>();
            builder.Services.AddScoped<IDebtsPayRepository,DebtsPayRepository>();
            builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
            builder.Services.AddScoped<ILoanRepository, LoanRepository>();
            builder.Services.AddScoped<ILoanPayRepository, LoanPayRepository>();
            builder.Services.AddScoped<IMoneyHolderRepository, MoneyHolderRepository>();
            builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
            builder.Services.AddScoped<IMoneySpendDetailRepository, MoneySpendDetailRepository>();
            builder.Services.AddScoped<IMoneySpendRepository, MoneySpendRepository>();
            builder.Services.AddScoped<ILocalTransferRepository, LocalTransferRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAccountBalanceTrackingRepository, AccountBalanceTrackingRepository>();
            
            #endregion Repository Mapping
        }
    }
}
