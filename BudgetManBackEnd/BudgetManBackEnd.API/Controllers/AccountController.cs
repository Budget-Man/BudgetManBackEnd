using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManBackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AccountController : Controller
    {
        IAuthService _authService;
        IAccountService _accountService;
        public AccountController(IAuthService authService, IAccountService accountService)
        {
            _authService = authService;
            _accountService = accountService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserModel login)
        {
            var result = await _authService.AuthenticateUser(login);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Regisger(UserModel login)
        {
            var result = await _authService.CreateUser(login);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> LoginByGoogle(GoogleLoginDto login)
        {
            var result = await _authService.LoginByGoogle(login);

            return Ok(result);
        }

        [HttpPut]
        [Route("savesetting")]
        public async Task<IActionResult> SaveSetting([FromBody] UserSettingDto request)
        {
            var result = await _accountService.SaveSetting(request);

            return Ok(result);
        }
    }
}
