using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BudgetController : Controller
    {
        private IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet]
        public IActionResult GetAllBudGet()
        {
            var result = _budgetService.GetAllBudget();
            return Ok(result);
        }

        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetBudget(Guid Id)
        {
            var result = _budgetService.GetBudget(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateBudget(BudgetDto request)
        {
            var result = _budgetService.CreateBudget(request);
            return Ok(result);
        }
        [HttpPut]
        //[Route("{Id}")]
        public IActionResult EditBudget(BudgetDto request)
        {
            var result = _budgetService.EditBudget(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteBudget(Guid Id)
        {
            var result =_budgetService.DeleteBudget(Id);
            return Ok(result);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _budgetService.Search(request);
			return Ok(result);
		}
	}
}
