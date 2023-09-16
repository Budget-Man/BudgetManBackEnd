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
    public class DebtsPayController : Controller
    {
        private IDebtsPayService _debtsPayService;

        public DebtsPayController(IDebtsPayService debtsPayService)
        {
            _debtsPayService = debtsPayService;
        }

        [HttpGet]
        public IActionResult GetAllDebtsPAy()
        {
            var result = _debtsPayService.GetAllDebtsPay();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetDebtsPay(Guid Id)
        {
            var result = _debtsPayService.GetDebtsPay(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateDebtsPay(DebtsPayDto request)
        {
            var result = _debtsPayService.CreateDebtsPay(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditDebtsPay(DebtsPayDto request)
        {
            var result  = _debtsPayService.EditDebtsPay(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteDebtsPay(Guid Id)
        {
            var result = _debtsPayService.DeleteDebtsPay(Id);
            return Ok(result);
        }
    }
}
