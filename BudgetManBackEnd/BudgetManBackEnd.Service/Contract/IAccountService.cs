using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Response.User;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Contract
{
    public interface IAccountService
    {
        public Task<AppResponse<string>> SaveSetting(UserSettingDto model);
        
    }
}
