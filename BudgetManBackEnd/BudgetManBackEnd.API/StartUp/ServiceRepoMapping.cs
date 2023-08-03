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
            
            #endregion Service Mapping

            #region Repository Mapping
            builder.Services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
            builder.Services.AddScoped<IAccountInfoRepository, AccountInfoRepository>();
            
            #endregion Repository Mapping
        }
    }
}
