using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using MayNghien.Models.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MoneySpendController : Controller
    {
        private IMoneySpendService _moneySpendService;

        public MoneySpendController(IMoneySpendService moneySpendService)
        {
            _moneySpendService = moneySpendService;
        }
        [HttpGet]
        public IActionResult GetAllLoanPay()
        {
            var result = _moneySpendService.GetAllMoneySpend();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoanPay(Guid Id)
        {
            var result = _moneySpendService.GetMoneySpend(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoanPay(MoneySpendDto request)
        {
            var result = _moneySpendService.CreateMoneySpend(request);
            return Ok(result);
        }
        [HttpPut]
        //[Route("{Id}")]
        public IActionResult EditLoanPay(MoneySpendDto request)
        {
            var result = _moneySpendService.EditMoneySpend(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoanPay(Guid Id)
        {
            var result = _moneySpendService.DeleteMoneySpend(Id);
            return Ok(result);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _moneySpendService.Search(request);
			return Ok(result);
		}
	}
}