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
    public interface IMoneySpendDetailService
    {
        AppResponse<MoneySpendDetailDto> GetMoneySpendDetail(Guid Id);
        AppResponse<List<MoneySpendDetailDto>> GetAllMoneySpendDetail();
        AppResponse<MoneySpendDetailDto> CreateMoneySpendDetail(MoneySpendDetailDto request);
        AppResponse<MoneySpendDetailDto> EditMoneySpendDetail(MoneySpendDetailDto request);
        AppResponse<string> DeleteMoneySpendDetail(Guid Id);
        AppResponse<SearchResponse<MoneySpendDetailDto>> Search(SearchRequest request);
	}
}
