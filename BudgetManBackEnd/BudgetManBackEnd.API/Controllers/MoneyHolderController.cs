using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using MayNghien.Models.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MoneyHolderController : Controller
    {
        private IMoneyHolderService _moneyHolderService;
        public MoneyHolderController(IMoneyHolderService moneyHolderService)
        {
            _moneyHolderService = moneyHolderService;
        }
        [HttpGet]
        public IActionResult GetAllMoneyHolder()
        {
            var result = _moneyHolderService.GetAllMoneyHolder();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetMoneyHolder(Guid Id)
        {
            var result = _moneyHolderService.GetMoneyHolder(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateMoneyHolder(MoneyHolderDto request)
        {
            var result = _moneyHolderService.CreateMoneyHolder(request);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult EditMoneyHolder(MoneyHolderDto request)
        {
            var result = _moneyHolderService.EditMoneyHolder(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteMoneyHolder(Guid Id)
        {
            var result =_moneyHolderService.DeleteMoneyHolder(Id);
            return Ok(result);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _moneyHolderService.Search(request);
			return Ok(result);
		}
	}
}
