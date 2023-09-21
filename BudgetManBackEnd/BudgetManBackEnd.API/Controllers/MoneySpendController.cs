using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MoneySpendController : Controller
    {
        private IMoneySpendService _loanPayService;

        public MoneySpendController(IMoneySpendService loanPayService)
        {
            _loanPayService = loanPayService;
        }
        [HttpGet]
        public IActionResult GetAllLoanPay()
        {
            var result = _loanPayService.GetAllMoneySpend();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoanPay(Guid Id)
        {
            var result = _loanPayService.GetMoneySpend(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoanPay(MoneySpendDto request)
        {
            var result = _loanPayService.CreateMoneySpend(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditLoanPay(MoneySpendDto request)
        {
            var result = _loanPayService.EditMoneySpend(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoanPay(Guid Id)
        {
            var result = _loanPayService.DeleteMoneySpend(Id);
            return Ok(result);
        }
    }
}
