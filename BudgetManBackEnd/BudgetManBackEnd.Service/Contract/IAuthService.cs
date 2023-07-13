using BudgetManBackEnd.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IAuthService
    {
        Task<string> AuthenticateUser(UserModel login);
    }
}
