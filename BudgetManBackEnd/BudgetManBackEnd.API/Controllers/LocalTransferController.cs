using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LocalTransferController : Controller
    {
        private ILocalTransferService _localTransferService;
        public LocalTransferController(ILocalTransferService localTransferService)
        {
            _localTransferService = localTransferService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _localTransferService.GetAllLocalTransfery();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var result =_localTransferService.GetLocalTransfer(Id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult Create(LocalTransferDto request)
        {
            var result = _localTransferService.CreateLocalTransfer(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("{Id}")]
        public IActionResult Edit(LocalTransferDto request)
        {
            var result =_localTransferService.EditLocalTransfer(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var result =_localTransferService.DeleteLocalTransfer(Id);
            return Ok(result);
        }
    }
}
