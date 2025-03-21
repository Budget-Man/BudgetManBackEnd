﻿using System;
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
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using static MayNghien.Common.Helpers.SearchHelper;

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
                    return result.BuildError("category Cannot be null");
                }
                if (request.Name == null)
                {
                    return result.BuildError("Name Cannot be null");
                }
                var budgetCategories = _budgetCategoryRepository.FindBy(m => m.Id == request.BudgetCategoryId && m.IsDeleted != true).FirstOrDefault();
                if (budgetCategories == null)
                {
                    return result.BuildError("Cannot find category");
                }

                var budget = _mapper.Map<Budget>(request);
                budget.Id = Guid.NewGuid();
                budget.AccountId = accountInfo.Id;
                budget.IsActive = true;
                budget.BudgetCategory = null;
                budget.MonthlyLimit = budgetCategories.MonthlyLimit;
                budget.Balance = budgetCategories.MonthlyLimit;
                _budgetRepository.Add(budget, accountInfo.Name);
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
                if (request.Id == null)
                {
                    return result.BuildError("id Cannot be null");
                }
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.BudgetCategoryId == null)
                {
                    return result.BuildError("category Cannot be null");
                }
                if (request.Name == null)
                {
                    return result.BuildError("Name Cannot be null");
                }
                var budgetCategories = _budgetCategoryRepository.FindBy(m => m.Id == request.BudgetCategoryId && m.IsDeleted != true);
                if (budgetCategories.Count() == 0)
                {
                    return result.BuildError("Cannot find category");
                }

                var budget = _budgetRepository.Get((Guid)request.Id);
                budget.BudgetCategoryId = request.BudgetCategoryId;
                budget.Balance = (double)request.Balance;
                budget.Name = request.Name;
                budget.ModifiedOn = DateTime.UtcNow;
                budget.Modifiedby = accountInfo.Email;
                budget.UseCredit = (double)request.UseCredit;
                budget.MonthlyLimit = request.MonthlyLimit;
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
			var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
			if (accountInfoQuery.Count() == 0)
			{
				return result.BuildError("Cannot find Account Info by this user");
			}
			try
            {
                var query = _budgetRepository.GetAll()
                    .Where(x => x.Account.UserId == userId && x.IsDeleted!=true && x.IsActive)
                    .Include(x=>x.BudgetCategory);
                var list = query
                    .Select(x=> new BudgetDto
                    {
                        BudgetCategoryName = x.BudgetCategory.Name,
                        Balance = x.Balance,
                        Id = x.Id,
                        BudgetCategoryId = x.BudgetCategory.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        UseCredit = x.UseCredit,
                    } )
                    .ToList();
                result.BuildResult(list);
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
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name= x.Name,
                    UseCredit= x.UseCredit,
                }).First();
                result.BuildResult(data);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message );
            }
            return result;
        }
		public AppResponse<SearchResponse<BudgetDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<BudgetDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = _budgetRepository.CountRecordsByPredicate(query);
				var model = _budgetRepository.FindByPredicate(query).Include(x=>x.BudgetCategory).OrderByDescending(x => x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new BudgetDto
                    {
                        BudgetCategoryName = x.BudgetCategory.Name,
                        Balance = x.Balance,
                        BudgetCategoryId = x.BudgetCategory.Id,
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        UseCredit = x.UseCredit,
                        MonthlyLimit = x.MonthlyLimit,
                        BalanceInfo = x.Balance.ToString("n0")+"/"+(x.MonthlyLimit!=null? x.MonthlyLimit.Value.ToString("n0"):"0"),
                    })
					.ToList();


				var searchUserResult = new SearchResponse<BudgetDto>
				{
					TotalRows = 0,
					TotalPages = CalculateNumOfPages(0, pageSize),
					CurrentPage = pageIndex,
					Data = List,
				};
				result.BuildResult(searchUserResult);
			}
			catch (Exception ex)
			{
				result.BuildError(ex.Message);
			}
			return result;
		}
		private ExpressionStarter<Budget> BuildFilterExpression(IList<Filter> Filters , Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<Budget>(true);
                if(Filters != null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						case "BudgetCategoryName":
							predicate = predicate.And(m => m.BudgetCategory.Name.Contains(filter.Value) && m.AccountId == accountId);
							break;
						default:
							break;
					}
				}
				predicate = predicate.And(m => m.IsDeleted == false);
				predicate = predicate.And(m => m.AccountId == accountId);
				return predicate;
			}
			catch (Exception)
			{

				throw;
			}
		}
        public AppResponse<string> StatusChange(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var budget = _budgetRepository.Get(Id);
                budget.IsActive = !budget.IsActive;

                _budgetRepository.Edit(budget);
                if (budget.IsActive)
                    result.BuildResult("activated");
                else
                    result.BuildResult("disabled");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        public AppResponse<string> MonthReset()
        {
            var result = new AppResponse<string>();
            var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
            if (accountInfoQuery.Count() == 0)
            {
                return result.BuildError("Cannot find Account Info by this user");
            }
            var accountInfo = accountInfoQuery.First();

            #region disable old budget
            var activeBudgets = _budgetRepository.FindBy(m=>m.AccountId == accountInfo.Id && m.IsActive).ToList();

            foreach ( var budget in activeBudgets)
            {
                budget.IsActive = false;
            }
            _budgetRepository.EditRange(activeBudgets, false);

            #endregion disable

            var cates = _budgetCategoryRepository.FindBy(m => m.AccountId == accountInfo.Id).ToList();
            
            var newBudgets = new List<Budget>();
            foreach (var cate in cates)
            {
                var budget = new Budget
                {
                    AccountId = accountInfo.Id,
                    Balance = cate.MonthlyLimit,
                    MonthlyLimit = cate.MonthlyLimit,
                    BudgetCategoryId = cate.Id,
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Name = cate.Name + " - " + DateTime.UtcNow.AddDays(10).ToString("yy/MM"),
                    UseCredit = 0,
                    IsDeleted = false,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,

                };
                newBudgets.Add(budget);
            }

            _budgetRepository.AddRange(newBudgets);
            return result.BuildResult("ok");
        }

    }
}
