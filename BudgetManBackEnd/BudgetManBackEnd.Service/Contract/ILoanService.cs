using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Contract
{
    public interface ILoanService
    {
        AppResponse<List<LoanDto>> GetAllLoan();
        AppResponse<LoanDto> GetLoan(Guid Id);
        AppResponse<LoanDto> CreateLoan(LoanDto request);
        AppResponse<LoanDto> EditLoan(LoanDto request);
        AppResponse<string> DeleteLoan(Guid Id);
    }
}
