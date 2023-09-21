using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IMoneySpendDetailService
    {
        AppResponse<MoneySpendDetailDto> GetMoneySpendDetail(Guid Id);
        AppResponse<List<MoneySpendDetailDto>> GetAllMoneySpendDetail();
        AppResponse<MoneySpendDetailDto> CreateMoneySpendDetail(MoneySpendDetailDto request);
        AppResponse<MoneySpendDetailDto> EditMoneySpendDetail(MoneySpendDetailDto request);
        AppResponse<string> DeleteMoneySpendDetail(Guid Id);
    }
}
