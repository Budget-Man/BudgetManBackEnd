using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Service.Implementation
{
    public class LocalTransferService : ILocalTransferService
    {
        private readonly ILocalTransferRepository _loanPayRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IMoneyHolderRepository _loanRepository;
        public LocalTransferService(ILocalTransferRepository loanPayRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneyHolderRepository loanRepository)
        {
            _loanPayRepository = loanPayRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _loanRepository = loanRepository;
        }

        public AppResponse<LocalTransferDto> GetLocalTransfer(Guid Id)
        {
            var result = new AppResponse<LocalTransferDto>();
            try
            {
                var query = _loanPayRepository.FindBy(x => x.Id == Id).Include(x => x.ToMoneyHolder);
                var loanPay = query.First();
                var data = _mapper.Map<LocalTransferDto>(loanPay);
                data.MoneyHolderIdBankName = loanPay.ToMoneyHolder.Name;
                data.MoneyHolderIdName = loanPay.FromMoneyHolder.Name;
                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<LocalTransferDto>> GetAllLocalTransfery()
        {
            var result = new AppResponse<List<LocalTransferDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _loanPayRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.ToMoneyHolder).Include(x => x.FromMoneyHolder);
                var list = query
                    .Select(x => new LocalTransferDto
                    {
                        Id = x.Id,
                        MoneyHolderIdBankName = x.ToMoneyHolder.Name,
                        MoneyHolderIdName = x.FromMoneyHolder.BankName,
                        FromMoneyHolderId = accountInfo.Id,
                        ToMoneyHolderId = accountInfo.Id,
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

        public AppResponse<LocalTransferDto> CreateLocalTransfer(LocalTransferDto request)
        {
            var result = new AppResponse<LocalTransferDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.ToMoneyHolderId == null)
                {
                    return result.BuildError("Debt Cannot be null");
                }
                var loan = _loanRepository.FindBy(m => m.Id == request.ToMoneyHolderId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                var loan2 = _loanRepository.FindBy(m => m.Id == request.FromMoneyHolderId && m.IsDeleted != true);
                if (loan.Count() == 0)
                {
                    return result.BuildError("Cannot find debt");
                }
                 var loanPay = _mapper.Map<LocalTransfer>(request);
                loanPay.Id = Guid.NewGuid();
                loanPay.AccountId = accountInfo.Id;
                loanPay.ToMoneyHolder = loan.First();
                loanPay.FromMoneyHolder = loan.First();
                _loanPayRepository.Add(loanPay, accountInfo.Name);

                request.Id = loanPay.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<LocalTransferDto> EditLocalTransfer(LocalTransferDto request)
        {
            var result = new AppResponse<LocalTransferDto>();
            try
            {
                var loanPay = _mapper.Map<LocalTransfer>(request);
                _loanPayRepository.Edit(loanPay);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteLocalTransfer(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var loanPay = _loanPayRepository.Get(Id);
                loanPay.IsDeleted = true;
                _loanPayRepository.Edit(loanPay);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
