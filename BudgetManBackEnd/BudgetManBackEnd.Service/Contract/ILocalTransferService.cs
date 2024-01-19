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
    public interface ILocalTransferService
    {
        AppResponse<LocalTransferDto> GetLocalTransfer(Guid Id);
        AppResponse<List<LocalTransferDto>> GetAllLocalTransfery();
        AppResponse<LocalTransferDto> CreateLocalTransfer(LocalTransferDto request);
        AppResponse<LocalTransferDto> EditLocalTransfer(LocalTransferDto request);
        AppResponse<string> DeleteLocalTransfer(Guid Id);
        AppResponse<SearchResponse<LocalTransferDto>> Search(SearchRequest request);

	}
}
