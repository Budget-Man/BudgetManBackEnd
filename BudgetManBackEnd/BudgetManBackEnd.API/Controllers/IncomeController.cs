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
    public class IncomeController : Controller
    {
        private IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }
        [HttpGet]
        public IActionResult GetAllIncome()
        {
            var result = _incomeService.GetAllIncome();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var result = _incomeService.GetIncome(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult Create(IncomeDto request)
        {
            var result = _incomeService.CreateIncome(request);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult Edit(IncomeDto request)
        {
            var result = _incomeService.EditIncome(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var result = _incomeService.DeleteIncome(Id);
            return Ok(result);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _incomeService.Search(request);
			return Ok(result);
		}
	}
}
