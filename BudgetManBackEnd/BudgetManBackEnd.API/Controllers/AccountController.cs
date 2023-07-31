using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
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

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserModel login)
        {
            var result = await _authService.AuthenticateUser(login);

            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Regisger(UserModel login)
        {
            var result = await _authService.CreateUser(login);

            return Ok(result);
        }

    }
}
