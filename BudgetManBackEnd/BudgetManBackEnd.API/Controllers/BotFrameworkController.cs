using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetManBackEnd.BotFramework;
using MayNghien.Models.Request.Base;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/BotFramework")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class BotFrameworkController : Controller
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        public BotFrameworkController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            _adapter.ProcessAsync(Request, Response, _bot);
        }

    }
}
