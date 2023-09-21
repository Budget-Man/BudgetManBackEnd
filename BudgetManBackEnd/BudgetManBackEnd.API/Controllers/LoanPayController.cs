﻿using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LocalTransferController : Controller
    {
        private ILocalTransferService _loanPayService;

        public LocalTransferController(ILocalTransferService loanPayService)
        {
            _loanPayService = loanPayService;
        }
        [HttpGet]
        public IActionResult GetAllLoanPay()
        {
            var result = _loanPayService.GetAllLocalTransfery();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult GetLoanPay(Guid Id)
        {
            var result = _loanPayService.GetLocalTransfer(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateLoanPay(LocalTransferDto request)
        {
            var result = _loanPayService.CreateLocalTransfer(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult EditLoanPay(LocalTransferDto request)
        {
            var result = _loanPayService.EditLocalTransfer(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeleteLoanPay(Guid Id)
        {
            var result = _loanPayService.DeleteLocalTransfer(Id);
            return Ok(result);
        }
    }
}
