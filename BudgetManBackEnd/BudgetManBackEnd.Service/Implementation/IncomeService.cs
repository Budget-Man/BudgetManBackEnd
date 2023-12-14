using System.Data.Entity;
using System.Security.Cryptography;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using Intercom.Data;
using LinqKit;
using MayNghien.Common.Helpers;
using MayNghien.Models.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using static MayNghien.Common.Helpers.SearchHelper;

namespace BudgetManBackEnd.Service.Implementation
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAccountInfoRepository _accountInfoRepository;
        private IMoneyHolderRepository _moneyHolderRepository;
        public IncomeService(IIncomeRepository incomeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountInfoRepository accountInfoRepository, IMoneyHolderRepository moneyHolderRepository)
        {
            _incomeRepository = incomeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountInfoRepository = accountInfoRepository;
            _moneyHolderRepository = moneyHolderRepository;
        }

        public AppResponse<IncomeDto> GetIncome(Guid Id)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var query = _incomeRepository.FindBy(x => x.Id == Id).Include(x => x.MoneyHolder);
                var data = query.Select(x => new IncomeDto
                {
                    Id = x.Id,
                    MoneyHolderId = x.MoneyHolderId,
                    MoneyHolderName = x.MoneyHolder.Name,
                    Name = x.Name,
                }).First();

                result.BuildResult(data);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<IncomeDto>> GetAllIncome()
        {
            var result = new AppResponse<List<IncomeDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();

                var query = _incomeRepository.GetAll().Where(x => x.AccountId == accountInfo.Id && x.IsDeleted != true).Include(x => x.MoneyHolder);
                var list = query
                    .Select(x => new IncomeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MoneyHolderId = accountInfo.Id,
                        MoneyHolderName = x.MoneyHolder.Name,
                        CreatedOn = x.CreatedOn.Value.Date
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

        public AppResponse<IncomeDto> CreateIncome(IncomeDto request)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var accountInfo = accountInfoQuery.First();
                if (request.MoneyHolderId == null)
                {
                    return result.BuildError("Money holder Cannot be null");
                }
                var query = _moneyHolderRepository.FindBy(m => m.Id == request.MoneyHolderId && m.IsDeleted != true);
                if (query.Count() == 0)
                {
                    return result.BuildError("Cannot find money holder");
                }
                var income = _mapper.Map<Income>(request);
                income.Id = Guid.NewGuid();
                income.AccountId = accountInfo.Id;
                income.MoneyHolder = null;
                income.Amount = request.Amount;
                _incomeRepository.Add(income, accountInfo.Name);
                var moneyHolder = query.First();
                if (moneyHolder.Balance == 0) moneyHolder.Balance = 0;
                moneyHolder.Balance += request.Amount;
                _moneyHolderRepository.Edit(moneyHolder);
                request.Id = income.Id;
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<IncomeDto> EditIncome(IncomeDto request)
        {
            var result = new AppResponse<IncomeDto>();
            try
            {
                var income = _incomeRepository.FindBy(x => x.Id == request.Id.Value)
                .Include(x => x.MoneyHolder)
                .Include(x => x.MoneyHolder.Balance).FirstOrDefault();
                var moneyHolder = _moneyHolderRepository.FindBy(x => x.Id == request.MoneyHolderId)
                //.Include(x => x.Balance)
                .Include(x => x.Balance).FirstOrDefault();
                if (moneyHolder == null)
                {
                    return result.BuildError("Cannot find Money Holder");
                }
                if (request.MoneyHolderId == income.MoneyHolderId)
                {
                    income.Amount = request.Amount;
                    moneyHolder.Balance -= income.Amount;
                    var oldmoneyhoder = _moneyHolderRepository.FindByPredicate(x => x.Id == income.MoneyHolderId).First();
                    oldmoneyhoder.Balance += income.Amount;
                    _moneyHolderRepository.Edit(oldmoneyhoder);

                }
                if (request.MoneyHolderId != income.MoneyHolderId)
                {
                    income.Amount = request.Amount;
                    moneyHolder.Balance += income.Amount;
                    var oldmoneyhoder = _moneyHolderRepository.FindByPredicate(x => x.Id == income.MoneyHolderId).First();
                    oldmoneyhoder.Balance -= income.Amount;
                    _moneyHolderRepository.Edit(oldmoneyhoder);
                }
                income.Name = request.Name;

                income.MoneyHolderId = request.MoneyHolderId;

                _moneyHolderRepository.Edit(moneyHolder);
                _incomeRepository.Edit(income);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteIncome(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var income = _incomeRepository.Get(Id);
                income.IsDeleted = true;
                _incomeRepository.Edit(income);
                result.BuildResult("Delete Sucessfuly");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        public AppResponse<SearchResponse<IncomeDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<IncomeDto>>();
            try
            {
                var userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
                var accountInfoQuery = _accountInfoRepository.FindBy(m => m.UserId == userId);
                if (accountInfoQuery.Count() == 0)
                {
                    return result.BuildError("Cannot find Account Info by this user");
                }
                var query = BuildFilterExpression(request.Filters, (accountInfoQuery.First()).Id);
                var numOfRecords = -_incomeRepository.CountRecordsByPredicate(query);
                var model = _incomeRepository.FindByPredicate(query).Include(x => x.MoneyHolder).OrderByDescending(x => x.CreatedOn);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new IncomeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MoneyHolderId = x.MoneyHolderId,
                        MoneyHolderName = x.MoneyHolder.Name,
                        Amount = x.Amount,
                        CreatedOn = x.CreatedOn.Value.Date,
                    })
                    .ToList();


                var searchUserResult = new SearchResponse<IncomeDto>
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
        private ExpressionStarter<Income> BuildFilterExpression(IList<Filter> Filters, Guid accountId)
        {
            try
            {
                var predicate = PredicateBuilder.New<Income>(true);
                predicate = predicate.And(m => m.IsDeleted == false);
                predicate = predicate.And(m => m.AccountId == accountId);
                if (Filters != null)
                {
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                predicate = predicate.And(m => m.Name.Contains(filter.Value));
                                break;
                            case "moneyHolderId":
                                predicate = predicate.And(m => m.MoneyHolderId.ToString() == filter.Value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ExportToExcel(SearchRequest request)
        {
            var data = this.Search(request);
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SelectedRows");
                var UserName = ClaimHelper.GetClainByName(_httpContextAccessor, "UserName");

                // Group data by date
                var groupedData = data.Data.Data.GroupBy(d => d.CreatedOn.Value.Date);

                // Iterate through each group and add data to the Excel file
                int rowNumber = 2; // Starting row for data entries
                foreach (var group in groupedData)
                {
                    // Calculate total amount for this group
                    var totalAmount = group.Sum(dto => dto.Amount);

                    // Set header row for the group
                    worksheet.Cells[rowNumber, 1].Value = "Thời Gian";
                    worksheet.Cells[rowNumber, 2].Value = "Sự Kiện";
                    worksheet.Cells[rowNumber, 3].Value = "Nơi Giữ Tiền";
                    worksheet.Cells[rowNumber, 4].Value = "Số Tiền";
                    rowNumber++;

                    // Set data rows for each income in the group
                    foreach (var income in group)
                    {
                        worksheet.Cells[rowNumber, 1].Value = income.CreatedOn.Value.ToString("dd/MM/yyyy");
                        worksheet.Cells[rowNumber, 2].Value = income.Name;
                        worksheet.Cells[rowNumber, 3].Value = income.MoneyHolderName;
                        worksheet.Cells[rowNumber, 4].Value = income.Amount;
                        rowNumber++;
                    }

                    // Set total row for the group
                    worksheet.Cells[rowNumber, 1].Value = "Tổng:";
                    worksheet.Cells[rowNumber, 2].Value = totalAmount;
                    rowNumber += 4;  // Add space between groups

                }

                return package.GetAsByteArray();
            }
        }

    }
}
