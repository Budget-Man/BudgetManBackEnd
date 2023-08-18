using AutoMapper;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Mapper
{
    public class MappingsProfile: Profile
    {
        public MappingsProfile()
        {
            CreateMap();
        }
        protected void CreateMap()
        {
            CreateMap<BudgetCategory, BudgetCategoryDto>()
                .ReverseMap();
            CreateMap<IdentityUser, UserModel>()
                .ReverseMap();

        }
    }
}
