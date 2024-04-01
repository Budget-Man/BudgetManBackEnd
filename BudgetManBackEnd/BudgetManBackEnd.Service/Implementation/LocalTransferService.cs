using System.Data.Entity;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
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
                    FromMoneyHolderName  = x.FromMoneyHolder.Name,
                    ToMoneyHolderName = x.ToMoneyHolder.Name,
                    ToMoneyHolderId = x.ToMoneyHolderId,
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

                var query = _localTransferRepository.GetAll().Where(x => x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x => x.ToMoneyHolder).Include(x => x.FromMoneyHolder);
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
                var MoneyHolderSource = moneyHolder.FirstOrDefault();
                var MoneyHolderDestination = moneyHolder2.FirstOrDefault();
                 var localTransfer = _mapper.Map<LocalTransfer>(request);
                localTransfer.Id = Guid.NewGuid();
                localTransfer.AccountId = accountInfo.Id;
                localTransfer.FromMoneyHolderId = MoneyHolderSource.Id;
                localTransfer.ToMoneyHolderId = MoneyHolderDestination.Id;
                _localTransferRepository.Add(localTransfer, accountInfo.Name);

                MoneyHolderSource.Balance -= request.Amount;
                MoneyHolderDestination.Balance += request.Amount;
                _moneyHolderRepository.Edit(MoneyHolderSource);
                _moneyHolderRepository.Edit(MoneyHolderDestination);
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
                var localTransfer = _localTransferRepository.Get((Guid)request.Id);
                localTransfer.FromMoneyHolderId = request.FromMoneyHolderId;
                localTransfer.ToMoneyHolderId = request.ToMoneyHolderId;
                localTransfer.Amount = request.Amount;
                _localTransferRepository.Edit(localTransfer);
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
		public AppResponse<SearchResponse<LocalTransferDto>> Search(SearchRequest request)
		{
			var result = new AppResponse<SearchResponse<LocalTransferDto>>();
			try
			{
				var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
				var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
				if (accountInfoQuery.Count() == 0)
				{
					return result.BuildError("Cannot find Account Info by this user");
				}
				var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
				var numOfRecords = -_localTransferRepository.CountRecordsByPredicate(query);
				var model = _localTransferRepository.FindByPredicate(query).OrderByDescending(x => x.CreatedOn);
				int pageIndex = request.PageIndex ?? 1;
				int pageSize = request.PageSize ?? 1;
				int startIndex = (pageIndex - 1) * (int)pageSize;
				var List = model.Skip(startIndex).Take(pageSize)
					.Select(x => new LocalTransferDto
					{
						Id = x.Id,
						
					})
					.ToList();


				var searchUserResult = new SearchResponse<LocalTransferDto>
				{
					TotalRows = numOfRecords,
					TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
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
		private ExpressionStarter<LocalTransfer> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
		{
			try
			{
				var predicate = PredicateBuilder.New<LocalTransfer>(true);
                if(Filters != null)
				foreach (var filter in Filters)
				{
					switch (filter.FieldName)
					{
						
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
	}
}
