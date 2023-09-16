using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LoanController : Controller
    {
        private ILoanService _loanService;
        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }
        [HttpGet]
        public IActionResult GetAllLoan()
        {
            var result = _loanService.GetAllLoan();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoan(Guid Id)
        {
            var result = _loanService.GetLoan(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoan(LoanDto request)
        {
            var result = _loanService.CreateLoan(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditLoan(LoanDto request)
        {
            var result = _loanService.EditLoan(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoan(Guid Id)
        {
            var result = _loanService.DeleteLoan(Id);
            return Ok(result);
        }
    }
}
