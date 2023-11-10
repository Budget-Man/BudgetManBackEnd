using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IDebtsPayService
    {
        AppResponse<List<DebtsPayDto>> GetAllDebtsPay();
        AppResponse<DebtsPayDto> GetDebtsPay(Guid Id);
        AppResponse<DebtsPayDto> CreateDebtsPay(DebtsPayDto request);
        AppResponse<string> DeleteDebtsPay(Guid Id);
        AppResponse<SearchResponse<DebtsPayDto>> Search(SearchRequest request);

	}
}
