using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IDebtService
    {
        AppResponse<List<DebtDto>> GetAllDebt();
        AppResponse<DebtDto> GetDebt(Guid Id);
        AppResponse<DebtDto> EditDebt(DebtDto request);
        AppResponse<string> DeleteDebt(Guid Id);
        AppResponse<DebtDto> CreateDebt(DebtDto request);
        AppResponse<SearchResponse<DebtDto>> Search(SearchRequest request);

	}
}
