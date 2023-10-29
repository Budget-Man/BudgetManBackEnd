﻿using BudgetManBackEnd.Model.Dto;
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
    public class LoanPayController : Controller
    {
        private ILoanPayService _loanPayService;

        public LoanPayController(ILoanPayService loanPayService)
        {
            _loanPayService = loanPayService;
        }
        [HttpGet]
        public IActionResult GetAllLoanPay()
        {
            var result = _loanPayService.GetAllLoanPay();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoanPay(Guid Id)
        {
            var result = _loanPayService.GetLoanPay(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoanPay(LoanPayDto request)
        {
            var result = _loanPayService.CreateLoanPay(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditLoanPay(LoanPayDto request)
        {
            var result = _loanPayService.EditLoanPay(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoanPay(Guid Id)
        {
            var result = _loanPayService.DeleteLoanPay(Id);
            return Ok(result);
        }
		[HttpPost]
		[Route("Search")]
		public IActionResult Search(SearchRequest request)
		{
			var result = _loanPayService.Search(request);
			return Ok(result);
		}
	}
}
