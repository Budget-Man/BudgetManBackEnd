using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Request;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MySqlConnector;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IMoneySpendService
    {
        AppResponse<MoneySpendDto> GetMoneySpend(Guid Id);
        AppResponse<List<MoneySpendDto>> GetAllMoneySpend();
        AppResponse<CreateMoneySpendRequest> CreateMoneySpend(CreateMoneySpendRequest request);
        AppResponse<MoneySpendDto> EditMoneySpend(MoneySpendDto request);
        AppResponse<string> DeleteMoneySpend(Guid Id);
        AppResponse<SearchResponse<MoneySpendDto>> Search(SearchRequest request);

	}
}
