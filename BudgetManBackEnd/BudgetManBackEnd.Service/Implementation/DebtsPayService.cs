using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;

namespace BudgetManBackEnd.Service.Implementation
{
    public class DebtsPayService : IDebtsPayService
    {
        private readonly IDebtsPayRepository _debtsPayRepository;
        private IMapper _mapper;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DebtsPayService(IDebtsPayRepository debtsPayRepository, IMapper mapper, IAccountInfoRepository accountInfoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _debtsPayRepository = debtsPayRepository;
            _mapper = mapper;
            _accountInfoRepository = accountInfoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public AppResponse<List<DebtsPayDto>> GetAllDebtsPay()
        {
            var result = new AppResponse<List<DebtsPayDto>>();
            try
            {
                var query = _debtsPayRepository.GetAll()
                    .Include(x=>x.Debts);
                var list = query.Select(x=> new DebtsPayDto
                {
                    Id = x.Id,
                    PaidAmount = x.PaidAmount,
                    Interest = x.Interest,
                    InterestRate = x.InterestRate,
                    IsPaid = x.IsPaid,
                    DebtsId = x.DebtsId,
                    DebtsName = x.Debts.Name,
                    RatePeriod = x.RatePeriod,
                })
                    .ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> GetDebtsPay(Guid Id)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var debtsPay = _debtsPayRepository.FindBy(x => x.Id == Id).Include(x => x.Debts);
                var data = _mapper.Map<DebtsPayDto>(debtsPay);
                result.BuildResult(data);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> EditDebtsPay(DebtsPayDto request)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var debtsPay = _mapper.Map<DebtsPay>(request);
                _debtsPayRepository.Edit(debtsPay);

                result.BuildResult(request);
            }
            catch( Exception ex )
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteDebtsPay(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var debtsPay = _debtsPayRepository.Get(Id);
                debtsPay.IsDeleted = true;

                _debtsPayRepository.Edit(debtsPay);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<DebtsPayDto> CreateDebtsPay(DebtsPayDto request)
        {
            var result = new AppResponse<DebtsPayDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var debtsPay = _mapper.Map<DebtsPay>(request);
                debtsPay.Id = Guid.NewGuid();
                debtsPay.AccountId = accountInfo.Id;

                _debtsPayRepository.Add(debtsPay);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
