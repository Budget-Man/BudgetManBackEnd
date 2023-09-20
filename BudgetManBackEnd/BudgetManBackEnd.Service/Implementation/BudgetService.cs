using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;

namespace BudgetManBackEnd.Service.Implementation
{
    public class BudgetService: IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;
        private IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BudgetService(IBudgetRepository budgetRepository, IAccountInfoRepository accountInfoRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBudgetCategoryRepository budgetCategoryRepository)
        {
            _budgetRepository = budgetRepository;
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _budgetCategoryRepository = budgetCategoryRepository;
        }

        public AppResponse<BudgetDto> CreateBudget(BudgetDto request)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.BudgetCategoryId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var budgetCategories = _budgetCategoryRepository.FindBy(m => m.Id == request.BudgetCategoryId && m.IsDeleted != true);
                if (budgetCategories.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }

                var budget = _mapper.Map<Budget>(request);
                budget.Id = Guid.NewGuid();
                budget.AccountId = accountInfo.Id;
                budget.BudgetCategory = budgetCategories.First();
                _budgetRepository.Add(budget);
                request.Id = budget.Id;
                result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;

            
        }

        public AppResponse<string> DeleteBudget(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var budget = _budgetRepository.Get(Id);
                budget.IsDeleted = true;

                _budgetRepository.Edit(budget);
                result.BuildResult("Delete successfully");
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<BudgetDto> EditBudget(BudgetDto request)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var budget = _mapper.Map<Budget>(request);

                _budgetRepository.Edit(budget);

                result.BuildResult(request);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<BudgetDto>> GetAllBudget()
        {
            var result =  new AppResponse<List<BudgetDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _budgetRepository.GetAll()
                    .Where(x => x.Account.UserId == userId)
                    .Include(x=>x.BudgetCategory);
                var list = query
                    .Select(x=> new BudgetDto
                    {
                        BudgetCategoryName = x.BudgetCategory.Name,
                        Balance = x.Balance,
                        Id = x.Id,
                        BudgetCategoryId = x.BudgetCategory.Id,
                    } )

                    .ToList();
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<BudgetDto> GetBudget(Guid Id)
        {
            var result = new AppResponse<BudgetDto>();
            try
            {
                var query = _budgetRepository.FindBy(x=>x.Id == Id);
                var data = query.Select(x => new BudgetDto
                {
                    BudgetCategoryName= x.BudgetCategory.Name,
                    Balance= x.Balance,
                    BudgetCategoryId = x.BudgetCategory.Id,
                    Id = x.Id
                }).First();
                result.BuildResult(data);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message );
            }
            return result;
        }
    }
}
