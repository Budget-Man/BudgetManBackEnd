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

        [HttpPost]
        public async Task<IActionResult> HandleMessage([FromBody] MessageRequest request)
        {
            try
            {
                var imageBytes = request.Images?.Select(base64 => Convert.FromBase64String(base64)).ToList();
                var response = await _messageService.HandleMessage(request.Message, imageBytes);
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
        public List<string>? Images { get; set; } // Nhận base64 thay vì byte[]
    }
}