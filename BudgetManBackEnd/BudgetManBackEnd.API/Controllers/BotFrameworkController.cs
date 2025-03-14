using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetManBackEnd.MessageCronJob;
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
    public class BotFrameworkController : Controller, ActivityHandler
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        //private ISkype _loanService;
        //private MyBot myBot;
        public BotFrameworkController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
            //myBot = new MyBot();
        }

        [HttpPost]
        public async Task PostAsync()
        {
            //await myBot.ProcessAsync(Request, Response);
            _adapter.ProcessAsync(Request, Response, _bot);
        }

    }
}
