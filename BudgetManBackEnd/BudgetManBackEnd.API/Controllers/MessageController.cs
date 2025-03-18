using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("handle")]
        public async Task<IActionResult> HandleMessage([FromBody] MessageRequest request)
        {
            try
            {
                var response = await _messageService.HandleMessage(request.Message, request.Images);
                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class MessageRequest
    {
        public string Message { get; set; }
        public List<byte[]>? Images { get; set; }
    }
}