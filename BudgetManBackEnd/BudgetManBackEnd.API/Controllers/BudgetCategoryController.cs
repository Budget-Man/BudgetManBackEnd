﻿using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BudgetManBackEnd.API.Controllers
{

    [Route("api/campaign")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BudgetCategoryController : Controller
    {
        private readonly IBudgetCategoryService _budgetCategoryService;
        public BudgetCategoryController(IBudgetCategoryService budgetCategoryService)
        {
            _budgetCategoryService = budgetCategoryService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            var result = _budgetCategoryService.GetAllBudgetCategory();

            return Ok(result);

        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetBugdgetCate(Guid id)
        {

            var result = _budgetCategoryService.GetBudgetCategory(id);

            return Ok(result);

        }
        [HttpPost]
        [Route("{id}")]
        public IActionResult CreateCampain([FromBody] BudgetCategoryDto request)
        {

            var result = _budgetCategoryService.CreatebudgetCategory(request);

            return Ok(result);

        }
        [HttpPut]
        [Route("{id}")]

        public IActionResult EditCampain([FromBody] BudgetCategoryDto request)
        {

            var result = _budgetCategoryService.EditbudgetCategory(request);

            return Ok(result);

        }
        [HttpDelete]
        public IActionResult DeleteCampain(Guid id)
        {

            var result = _budgetCategoryService.DeletebudgetCategory(id);

            return Ok(result);

        }
    }
}