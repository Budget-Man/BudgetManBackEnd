using BudgetManBackEnd.Model.Dto;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IAuthService
    {
        Task<AppResponse<LoginResult>> AuthenticateUser(UserModel login);
        Task<AppResponse<string>> CreateUser(UserModel user);

        Task<AppResponse<LoginResult>> LoginByGoogle(GoogleLoginDto token);
    }
}
