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
    public interface ILoanPayService
    {
        AppResponse<LoanPayDto> GetLoanPay(Guid Id);
        AppResponse<List<LoanPayDto>> GetAllLoanPay();
        AppResponse<LoanPayDto> CreateLoanPay(LoanPayDto request);
        AppResponse<string> DeleteLoanPay(Guid Id);
        AppResponse<SearchResponse<LoanPayDto>> Search(SearchRequest request);

	}
}
