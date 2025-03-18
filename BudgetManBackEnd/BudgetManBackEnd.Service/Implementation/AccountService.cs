using AutoMapper;
using BudgetManBackEnd.Common.Enum;
using BudgetManBackEnd.CommonClass.Enum;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Model.Response.User;
using BudgetManBackEnd.Service.Contract;
using Intercom.Data;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection.Metadata;
using static MayNghien.Common.CommonMessage.AuthResponseMessage;

namespace BudgetManBackEnd.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMoneyHolderRepository _moneyHolderRepository;


        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(IConfiguration config, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IAccountInfoRepository accountInfoRepository,
            IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IBudgetCategoryRepository budgetCategoryRepository, IBudgetRepository budgetRepository,
            IMoneyHolderRepository moneyHolderRepository)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _accountInfoRepository = accountInfoRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _budgetCategoryRepository = budgetCategoryRepository;
            _budgetRepository = budgetRepository;
            _moneyHolderRepository = moneyHolderRepository;
        }
        public async Task<AppResponse<string>> SaveSetting(UserSettingDto model)
        {
            var result = new AppResponse<string>();
            var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");


            try
            {
                var qUserInfo = _accountInfoRepository.FindBy(m => m.UserId == userId && m.IsDeleted == false);
                if (qUserInfo.Count() > 0 && qUserInfo.FirstOrDefault() != null)
                {
                    var accountInfo = qUserInfo.FirstOrDefault();
                    accountInfo.Language = (Languages)Enum.Parse(typeof(Languages), model.Language, true);
                    accountInfo.Currency = (Currencies)Enum.Parse(typeof(Currencies), model.Currency, true);
                    accountInfo.IsNewUser = false;
                    accountInfo.ChatUserId = model.ChatUserId;
                    accountInfo.MemberList = model.MemberList;
                    _accountInfoRepository.Edit(accountInfo);

                    if (model.IsCreateBaseData.HasValue && model.IsCreateBaseData.Value)
                    {
                        var createResult = await CreateBaseData(accountInfo);

                        if (!string.IsNullOrEmpty(createResult))
                        {
                            return result.BuildError(createResult);
                        }
                    }

                    return result.BuildResult("ok");
                }
                return result.BuildError("AccountInfo is not existed");
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.ToString());
            }
        }

        private async Task<string> CreateBaseData(AccountInfo accountInfo)
        {
            var baseName = BaseNameAttribute.GetBaseName(accountInfo.Language.Value);
            //var existedBugetCate = _budgetCategoryRepository.CountRecordsByPredicate(x => x.Account == accountInfo);
            //if (existedBugetCate > 0)
            //{
            //    return string.Empty;
            //}
            var existedMoneyHolder = _moneyHolderRepository.CountRecordsByPredicate(x => x.Account == accountInfo && x.IsDeleted==false);
            if (existedMoneyHolder > 0)
            {
                return string.Empty;
            }
            var budgetcat = new BudgetCategory
            {
                Name = baseName,
                Id = Guid.NewGuid(),
                AccountId = accountInfo.Id
            };
            _budgetCategoryRepository.Add(budgetcat, accountInfo.Name);

            var budgetCategories = _budgetCategoryRepository.Get(budgetcat.Id);
            if (budgetCategories == null) return "cannot create budgetCategories";
            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                AccountId = accountInfo.Id,
                IsActive = true,
                BudgetCategory = budgetCategories
            };
            _budgetRepository.Add(budget, accountInfo.Name);

            var moneyHolder = new MoneyHolder
            {
                Name = baseName,
                AccountId = accountInfo.Id,
                Id = Guid.NewGuid(),
                Balance = 0
            };
            _moneyHolderRepository.Add(moneyHolder, accountInfo.Name);

            return string.Empty;
        }

    }
}
