using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using MayNghien.Models.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MoneySpendDetailController : Controller
    {
        private readonly IMoneySpendDetailService _moneySpendDetailService;
        public MoneySpendDetailController(IMoneySpendDetailService moneySpendDetailService)
        {
            _moneySpendDetailService = moneySpendDetailService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _moneySpendDetailService.GetAllMoneySpendDetail();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var result = _moneySpendDetailService.GetMoneySpendDetail(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult Create(MoneySpendDetailDto request)
        {
            var result = _moneySpendDetailService.CreateMoneySpendDetail(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult Edit(MoneySpendDetailDto request)
        {
            var result = _moneySpendDetailService.EditMoneySpendDetail(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var reusult = _moneySpendDetailService.DeleteMoneySpendDetail(Id);
            return Ok(reusult);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _moneySpendDetailService.Search(request);
			return Ok(result);
		}
	}
}
