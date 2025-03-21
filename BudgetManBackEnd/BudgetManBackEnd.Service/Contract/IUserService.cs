﻿using BudgetManBackEnd.Model.Dto;
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
    public interface IUserService
    {
        public Task<AppResponse<List<UserModel>>> GetAllUser();
        public Task<AppResponse<string>> CreateUser(UserModel model);
        public Task<AppResponse<string>> EditUser(UserModel model);
        public Task<AppResponse<string>> ResetUser(UserModel model);
        public Task<AppResponse<string>> DeleteUser(string id);
        public Task<AppResponse<SearchUserResponse>> Search(SearchRequest request);
        public Task<AppResponse<UserModel>> GetUser(string id);
        public Task<AppResponse<string>> RunTestUser(UserModel model);
        
    }
}
