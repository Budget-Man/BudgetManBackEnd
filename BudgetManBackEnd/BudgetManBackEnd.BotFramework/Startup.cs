using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetManBackEnd.BotFramework
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // ✅ Register dependencies
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMoneyHolderRepository, MoneyHolderRepository>();

            // ✅ Register Bot Framework Adapter (Custom Adapter for Error Handling)
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // ✅ Register MyBot as an IBot AFTER dependencies
            services.AddTransient<IBot, MyBot>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            // Custom error handler for the bot
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError($"Exception caught: {exception.Message}");
                // You can send a message to the user, or log further details here
                await turnContext.SendActivityAsync("Sorry, an error occurred while processing your request.");
            };
        }
    }
}
