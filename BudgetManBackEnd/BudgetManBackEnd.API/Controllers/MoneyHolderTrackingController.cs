using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MoneyHolderTrackingController : Controller
    {
        private IAccountBalanceTrackingService _accountBalanceTrackingService;
        public MoneyHolderTrackingController(IAccountBalanceTrackingService service)
        {
            _accountBalanceTrackingService = service;
        }
        [HttpPost]
        [Route("Search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _accountBalanceTrackingService.Search(request);
            return Ok(result);
        }
    }
}
