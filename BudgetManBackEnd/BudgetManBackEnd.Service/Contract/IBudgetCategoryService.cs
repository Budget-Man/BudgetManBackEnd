﻿using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IBudgetCategoryService
    {
        AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory();
        AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto request);
        AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto request);
        AppResponse<string> DeletebudgetCategory(Guid budgetCategoryId);
        AppResponse<BudgetCategoryDto> GetBudgetCategory(Guid budgetCategoryId);
        AppResponse<SearchResponse<BudgetCategoryDto>> Search(SearchRequest request);

	}
}
