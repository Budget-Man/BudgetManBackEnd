using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class IncomeController : Controller
    {
        private IIncomeService _loanPayService;

        public IncomeController(IIncomeService loanPayService)
        {
            _loanPayService = loanPayService;
        }
        [HttpGet]
        public IActionResult GetAllIncome()
        {
            var result = _loanPayService.GetAllIncome();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoanPay(Guid Id)
        {
            var result = _loanPayService.GetIncome(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoanPay(IncomeDto request)
        {
            var result = _loanPayService.CreateIncome(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditLoanPay(IncomeDto request)
        {
            var result = _loanPayService.EditIncome(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoanPay(Guid Id)
        {
            var result = _loanPayService.DeleteIncome(Id);
            return Ok(result);
        }
    }
}
