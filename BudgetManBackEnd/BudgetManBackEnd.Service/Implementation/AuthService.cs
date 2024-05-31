using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Response.Base;
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
using static MayNghien.Common.CommonMessage.AuthResponseMessage;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.Common.Enum;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Oauth2.v2.Data;

namespace BudgetManBackEnd.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountInfoRepository _accountInfoRepository;
        public AuthService(IConfiguration config, UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, IAccountInfoRepository accountInfoRepository)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountInfoRepository = accountInfoRepository;
        }
        public async Task<AppResponse<string>> CreateUser(UserModel user)
        {
            var result = new AppResponse<string>();
            try
            {
                if(string.IsNullOrEmpty(user.Email))
                {
                    return result.BuildError(ERR_MSG_EmailIsNullOrEmpty);
                }
                var identityUser = await _userManager.FindByNameAsync(user.UserName);
                if (identityUser != null)
                {
                    return result.BuildError(ERR_MSG_UserExisted);
                }
                var newIdentityUser = new IdentityUser { Email = user.Email, UserName = user.Email };
                var createResult = await _userManager.CreateAsync(newIdentityUser);
                await _userManager.AddPasswordAsync(newIdentityUser, user.Password);

                newIdentityUser = await _userManager.FindByEmailAsync(user.Email);
                if (newIdentityUser != null)
                {
                    var AccountInfo = new AccountInfo()
                    {
                        Id = Guid.NewGuid(),
                        Balance = 0,
                        Email = user.Email,
                        CreatedBy = user.Email,
                        CreatedOn = DateTime.Now,
                        Name = user.UserName,
                        IsDeleted = false,
                        UserId = newIdentityUser.Id,
                        IsNewUser = true
                    };
                    _accountInfoRepository.Add(AccountInfo, "");
                }
                return result.BuildResult(INFO_MSG_UserCreated);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }

        }
        public async Task<AppResponse<LoginResult>> AuthenticateUser(UserModel login)
        {
            var result = new AppResponse<LoginResult>();
            try
            {
                UserModel user = null;
                IdentityUser identityUser = new IdentityUser();
                //Validate the User Credentials    
                //Demo Purpose, I have Passed HardCoded User Information    
                if (!(await _roleManager.RoleExistsAsync(nameof(UserRoleEnum.SuperAdmin))))
                {
                    IdentityRole role = new IdentityRole { Name = nameof(UserRoleEnum.SuperAdmin) };
                    await _roleManager.CreateAsync(role);
                }
                if (!(await _roleManager.RoleExistsAsync(nameof(UserRoleEnum.TenantAdmin))))
                {
                    IdentityRole role = new IdentityRole { Name = nameof(UserRoleEnum.TenantAdmin) };
                    await _roleManager.CreateAsync(role);
                }
                if (!(await _roleManager.RoleExistsAsync(nameof(UserRoleEnum.Admin))))
                {
                    IdentityRole role = new IdentityRole { Name = nameof(UserRoleEnum.Admin) };
                    await _roleManager.CreateAsync(role);
                }
                identityUser = await _userManager.FindByNameAsync(login.UserName);
                if (identityUser != null )
                {
                    //if (identityUser.EmailConfirmed != true)
                    //{
                    //    return result.BuildError(ERR_MSG_UserNotConFirmed);
                    //}
                    if(await _userManager.IsLockedOutAsync(identityUser))
                    {
                        return result.BuildError(ERR_MSG_UserLockedOut);

                    }
                    if (await _userManager.CheckPasswordAsync(identityUser, login.Password))
                    {
                        user = new UserModel { 
                            UserName = identityUser.UserName,
                            Email = identityUser.Email,
                            Id = identityUser.Id,
                            Role = (await _userManager.GetRolesAsync(identityUser)).FirstOrDefault(),
                            };

                    }

                }
                else if (login.UserName == "may.nghien@gmail.com")
                {
                    var newIdentity = new IdentityUser { UserName = login.UserName, Email = login.Email, EmailConfirmed = true };
                    await _userManager.CreateAsync(newIdentity);
                    await _userManager.AddPasswordAsync(newIdentity, "CdzuOsSbBH");
                    
                    await _userManager.AddToRoleAsync(newIdentity, "superadmin");
                    //var accountInfor = new AccountInfo
                    //{
                    //    Id = Guid.NewGuid(),
                    //    Balance = 0,
                    //    Email = newIdentity.Email,
                    //    CreatedBy = newIdentity.Email,
                    //    CreatedOn = DateTime.Now,
                    //    Name = newIdentity.UserName,
                    //    IsDeleted = false,
                    //    UserId = newIdentity.Id,
                    //};
                    //_accountInfoRepository.Add(accountInfor, "");

                }
                if (user != null)
                {
                    var tokenString = await GenerateJSONWebToken(user, identityUser);
                    var loginResult = new LoginResult();
                    loginResult.Token = tokenString;
                    loginResult.UserName = user.UserName;
                    loginResult.Roles = (await _userManager.GetRolesAsync(identityUser)).ToArray();
                    return result.BuildResult(loginResult);
                }
                else
                {
                    return result.BuildError(ERR_MSG_UserNotFound);
                }
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }


        }


        private async Task<string> GenerateJSONWebToken(UserModel userInfo, IdentityUser identityUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: await GetClaims(userInfo, identityUser),
              expires: DateTime.Now.AddHours(18),
              // subject: new ClaimsIdentity( await _userManager.GetClaimsAsync(userInfo)),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<List<Claim>> GetClaims(UserModel user, IdentityUser identityUser)
        {
            //var userTenantMappings = (await _userTenantMapingRepository.FindByAsync(u => u.User.Id == user.Id)).ToList().FirstOrDefault(t => t.IsUsing);
            var claims = new List<Claim>
            {
                new Claim("UserName", identityUser.UserName),
                new Claim("UserId", identityUser.Id),

                new Claim("Email", identityUser.Email),

            };
            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim("Role", role));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        public async Task<AppResponse<LoginResult>> LoginByGoogle(GoogleLoginDto loginInfo)
        {
            var result = new AppResponse<LoginResult>();
            var webAppClientId = "807507486424-ios762laefni6l7u7fgnl41a1fifgj4v.apps.googleusercontent.com";
            var webAppClientSecret = "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";
            var clientSecrets = new ClientSecrets
            {
                ClientId = webAppClientId,
                ClientSecret = webAppClientSecret
            };

            // Set up the authorization code flow
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { Oauth2Service.Scope.UserinfoProfile, Oauth2Service.Scope.UserinfoEmail },
                DataStore = new FileDataStore("Store") // You may want to store tokens securely
            });
            TokenResponse tokenResponse;
            try
            {
                // Exchange the authorization code for tokens
                tokenResponse = await flow.ExchangeCodeForTokenAsync(webAppClientId, loginInfo.code,
                    loginInfo.redirectUri, CancellationToken.None);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                // Handle the exception or log the details
                return result.BuildError(ex.ToString());
            }
            // Create a user credential from the token response
            var credential = new UserCredential(flow, "user", tokenResponse);

            // Create the Oauth2Service using the user's credential
            var service = new Oauth2Service(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Budget Managment"
            });

            // Retrieve user information
            var userInfoRequest = service.Userinfo.Get();
            var userInfo = await userInfoRequest.ExecuteAsync();

            // Use userInfo and perform server-side logic 
            var identityUser = await _userManager.FindByEmailAsync(userInfo.Email);
            LoginResult loginResponse = new LoginResult()
            {
                UserName = userInfo.Email
            };

            if (identityUser != null)
            {
                if (await _userManager.IsLockedOutAsync(identityUser))
                {
                    return result.BuildError(ERR_MSG_UserLockedOut);

                }
                else
                {
                    loginResponse.Roles = (await _userManager.GetRolesAsync(identityUser)).ToArray();
                    var accountInfo = _accountInfoRepository.FindBy(x => x.UserId == identityUser.Id).FirstOrDefault();
                    if (accountInfo!=null)
                    {
                        loginResponse.Language = accountInfo.Language;
                        loginResponse.Currency = accountInfo.Currency;
                        loginResponse.DefaultMoneyHolderId = accountInfo.DefaultMoneyHolderId;
                    }

                }
            }
            else
            {
                identityUser = new IdentityUser { Email = userInfo.Email, UserName = userInfo.Email };
                var createResult = await _userManager.CreateAsync(identityUser);
                //await _userManager.AddPasswordAsync(newIdentityUser, user.Password);

                identityUser = await _userManager.FindByEmailAsync(userInfo.Email);
                if (identityUser != null)
                {
                    var AccountInfo = new AccountInfo()
                    {
                        Id = Guid.NewGuid(),
                        Balance = 0,
                        Email = userInfo.Email,
                        CreatedBy = userInfo.Email,
                        CreatedOn = DateTime.Now,
                        Name = userInfo.Name,
                        IsDeleted = false,
                        UserId = identityUser.Id,
                    };
                    _accountInfoRepository.Add(AccountInfo, "");
                    loginResponse.IsNewUser = true;
                }
                else
                {
                    return result.BuildError(ERR_MSG_CanNotCreateUser);
                }
            }
            var user = new UserModel
            {
                UserName = userInfo.Email,
                Email = userInfo.Email,
                Id = identityUser.Id,
            };

            var tokenString = await GenerateJSONWebToken(user, identityUser);
            loginResponse.Token = tokenString;
            return result.BuildResult(loginResponse);
        }
    }
}
