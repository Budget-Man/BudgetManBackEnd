using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using BudgetManBackEnd.BotFramework;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.BotFramework;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManBackEnd.API.StartUp
{
    public class ServiceRepoMapping
    {
        public ServiceRepoMapping()
        {

        }

        [Obsolete]
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
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IMessageService, MessageService >();
          
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

            //builder.Services.AddScoped<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
            builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>((sp) =>
            {
                var logger = sp.GetRequiredService<ILogger<BotFrameworkHttpAdapter>>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var authConfig = new AuthenticationConfiguration();
                IChannelProvider channelProvider = null; // Use a specific implementation if required
                return new BotFrameworkHttpAdapter(new ConfigurationCredentialProvider(configuration), channelProvider, logger);
            });
            builder.Services.AddTransient<IBot, MyBot>();
        }
    }
}
