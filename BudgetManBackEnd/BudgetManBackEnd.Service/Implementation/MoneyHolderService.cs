using System;
using System.Collections.Generic;
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
using Npgsql.Internal.TypeHandlers.NumericHandlers;

namespace BudgetManBackEnd.Service.Implementation
{
    public class MoneyHolderService : IMoneyHolderService
    {
        private readonly IMoneyHolderRepository _moneyHolderRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MoneyHolderService(IMoneyHolderRepository moneyHolderRepository, IAccountInfoRepository accountInfoRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _moneyHolderRepository = moneyHolderRepository;
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public AppResponse<MoneyHolderDto> CreateMoneyHolder(MoneyHolderDto request)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var UserId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                //var role = ClaimHelper.GetClainByName(_httpContextAccessor, "Role");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == UserId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var moneyHolder = _mapper.Map<MoneyHolder>(request);
                moneyHolder.AccountId = accountInfo.Id;
                moneyHolder.Id = Guid.NewGuid();

                request.Id = moneyHolder.Id;
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteMoneyHolder(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var moneyHolder = _moneyHolderRepository.Get(Id);
                moneyHolder.IsDeleted =  true;

                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult("Delete Sucessfuly");
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneyHolderDto> EditMoneyHolder(MoneyHolderDto request)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var moneyHolder = _mapper.Map<MoneyHolder>(request);
                _moneyHolderRepository.Edit(moneyHolder);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<MoneyHolderDto>> GetAllMoneyHolder()
        {
            var result = new AppResponse<List<MoneyHolderDto>>();
            string userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            try
            {
                var query = _moneyHolderRepository.GetAll().Where(m => m.Account.UserId == userId);
                var list = query.Select(m => new MoneyHolderDto
                {
                    BankName = m.BankName,
                    Id = m.Id,
                    Name = m.Name,
                }).ToList();
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<MoneyHolderDto> GetMoneyHolder(Guid Id)
        {
            var result = new AppResponse<MoneyHolderDto>();
            try
            {
                var moneyHolder = _moneyHolderRepository.Get(Id);
                var data = _mapper.Map<MoneyHolderDto>(moneyHolder);
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
