using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;
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
        private readonly ILocalTransferRepository _localTransferRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IMoneyHolderRepository _moneyHolderRepository;
        public LocalTransferService(ILocalTransferRepository localTransferRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneyHolderRepository moneyHolderRepository)
        {
            _localTransferRepository = localTransferRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneyHolderRepository = moneyHolderRepository;
        }

        public AppResponse<LocalTransferDto> GetLocalTransfer(Guid Id)
        {
            var result = new AppResponse<LocalTransferDto>();
            try
            {
                var query = _localTransferRepository.FindBy(x => x.Id == Id).Include(x => x.ToMoneyHolder);
                var data = query.Select(x=>new LocalTransferDto
                {
                    Amount = x.Amount,
                    FromMoneyHolderId = x.FromMoneyHolderId,
                    Id = x.Id,
                    FromMoneyHolderName  = x.FromMoneyHolder.BankName,
                    ToMoneyHolderName = x.ToMoneyHolder.Name,
                } ).First();
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

                var query = _localTransferRepository.GetAll().Where(x => x.AccountId == accountInfo.Id).Include(x => x.ToMoneyHolder).Include(x => x.FromMoneyHolder);
                var list = query
                    .Select(x => new LocalTransferDto
                    {
                        Id = x.Id,
                        ToMoneyHolderName = x.ToMoneyHolder.Name,
                        FromMoneyHolderName = x.FromMoneyHolder.BankName,
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
                    return result.BuildError("To money holder Cannot be null");
                }
                var moneyHolder = _moneyHolderRepository.FindBy(m => m.Id == request.ToMoneyHolderId && m.IsDeleted != true);
                if (moneyHolder.Count() == 0)
                {
                    return result.BuildError("Cannot find to money holder");
                }
                if(request.FromMoneyHolderName == null)
                {
                    return result.BuildError("From money holder cannot be null");
                }
                var moneyHolder2 = _moneyHolderRepository.FindBy(m => m.Id == request.FromMoneyHolderId && m.IsDeleted != true);
                if (moneyHolder2.Count() == 0)
                {
                    return result.BuildError("Cannot find from money holder");
                }
                 var localTransfer = _mapper.Map<LocalTransfer>(request);
                localTransfer.Id = Guid.NewGuid();
                localTransfer.AccountId = accountInfo.Id;
                _localTransferRepository.Add(localTransfer, accountInfo.Name);

                request.Id = localTransfer.Id;
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
                _localTransferRepository.Edit(loanPay);
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
                var localTransfer = _localTransferRepository.Get(Id);
                localTransfer.IsDeleted = true;
                _localTransferRepository.Edit(localTransfer);
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
