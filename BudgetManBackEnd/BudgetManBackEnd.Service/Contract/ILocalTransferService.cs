using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface ILocalTransferService
    {
        AppResponse<LocalTransferDto> GetLocalTransfer(Guid Id);
        AppResponse<List<LocalTransferDto>> GetAllLocalTransfery();
        AppResponse<LocalTransferDto> CreateLocalTransfer(LocalTransferDto request);
        AppResponse<LocalTransferDto> EditLocalTransfer(LocalTransferDto request);
        AppResponse<string> DeleteLocalTransfer(Guid Id);
    }
}
