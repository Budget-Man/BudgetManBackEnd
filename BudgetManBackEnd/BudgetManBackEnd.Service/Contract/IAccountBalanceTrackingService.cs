using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IAccountBalanceTrackingService
    {
        Task CreateTracking(AccountBalanceTrackingDto request);
        AppResponse<SearchResponse<AccountBalanceTrackingDto>> Search(SearchRequest request);

    }
}
