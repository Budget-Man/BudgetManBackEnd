using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class DebtController : Controller
    {
        private IDebtService _debtService;
        public DebtController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        [HttpGet]
        public IActionResult GetAllDebt()
        {
            var result = _debtService.GetAllDebt();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetDebt(Guid Id)
        {
            var result = _debtService.GetDebt(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateDebt(DebtDto request)
        {
            var result = _debtService.CreateDebt(request);
            return Ok(result);
        }
        [HttpPut]
        //[Route("{Id}")]
        public IActionResult EditDebt(DebtDto request)
        {
            var result = _debtService.EditDebt(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteDebt(Guid Id)
        {
            var result = _debtService.DeleteDebt(Id);
            return Ok(result);
        }
        [HttpPost]
        [Route("Search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _debtService.Search(request);
            return Ok(result);
        }
    }
}
