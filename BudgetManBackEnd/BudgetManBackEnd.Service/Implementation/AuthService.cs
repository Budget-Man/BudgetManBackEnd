using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthService(IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> AuthenticateUser(UserModel login)
        {
            UserModel user = null;

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (login.UserName == "may.nghien@gmail.com" && login.Password == "CdzuOsSbBH")
            {
                user = new UserModel { UserName = "Super admin", EmailAddress = "may.nghien@gmail.com" };
            }
            else
            {
                var identityUser = await _userManager.FindByNameAsync(login.UserName);
                if (identityUser != null)
                {
                    if (await _userManager.CheckPasswordAsync(identityUser, login.Password))
                    {
                        user = new UserModel { UserName = identityUser.UserName, EmailAddress = identityUser.Email };

                    }
                }
            }
            if (user != null)
            {
                var tokenString = await GenerateJSONWebToken(user);
                return tokenString;
            }
            return "";
        }


        private async Task<string> GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: await GetClaims(userInfo),
              expires: DateTime.Now.AddHours(18),
              // subject: new ClaimsIdentity( await _userManager.GetClaimsAsync(userInfo)),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<List<Claim>> GetClaims(UserModel user)
        {
            //var userTenantMappings = (await _userTenantMapingRepository.FindByAsync(u => u.User.Id == user.Id)).ToList().FirstOrDefault(t => t.IsUsing);
            var claims = new List<Claim>
            {
                new Claim("UserName", user.UserName),

                new Claim("Email", user.EmailAddress),

            };
            //var roles = await _userManager.GetRolesAsync(user);
            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim("Role", role));
            //}
            return claims;
        }

    }
}
