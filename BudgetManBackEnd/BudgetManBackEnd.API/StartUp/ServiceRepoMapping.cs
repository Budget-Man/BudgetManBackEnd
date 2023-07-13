using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
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
            
            #endregion Service Mapping

            #region Repository Mapping
            builder.Services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
            
            #endregion Repository Mapping
        }
    }
}
