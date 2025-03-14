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
    public interface IMoneyHolderService
    {
        AppResponse<List<MoneyHolderDto>> GetAllMoneyHolder();
        AppResponse<MoneyHolderDto> GetMoneyHolder(Guid Id);
        AppResponse<MoneyHolderDto> CreateMoneyHolder(MoneyHolderDto request);
        AppResponse<MoneyHolderDto> EditMoneyHolder(MoneyHolderDto request);
        AppResponse<string> DeleteMoneyHolder(Guid Id);
        AppResponse<SearchResponse<MoneyHolderDto>> Search(SearchRequest request);
        AppResponse<List<MoneyHolderDto>> GetMoneyHolderByUser(string userId);


    }
}
