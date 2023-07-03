using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Implementation
{
    public class BudgetCategoryService : IBudgetCategoryService
    {
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly IMapper _mapper;
        public BudgetCategoryService(IBudgetCategoryRepository budgetCategoryRepository,IMapper mapper)
        {
            _budgetCategoryRepository = budgetCategoryRepository;
            _mapper = mapper;
        }

        public AppResponse<BudgetCategoryDto> CreatebudgetCategory(BudgetCategoryDto request)
        {
            var result = new AppResponse<BudgetCategoryDto>();
            try
            {
                var budgetcat = new BudgetCategory();
                budgetcat = _mapper.Map<BudgetCategory>(request);
                budgetcat.Id = Guid.NewGuid();
                _budgetCategoryRepository.Add(budgetcat);
                request.Id = budgetcat.Id;
                result.IsSuccess = true;
                result.Data = request;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message =ex.Message +":"+ ex.StackTrace;
                return result;
                
            }
            
        }

        public AppResponse<string> DeletebudgetCategory(Guid request)
        {
            var result = new AppResponse<string>();
            try
            {
                var budgetcat = new BudgetCategory();
                budgetcat = _budgetCategoryRepository.Get(request);
                budgetcat.IsDeleted = true;
                
                _budgetCategoryRepository.Edit(budgetcat);
                
                result.IsSuccess = true;
                result.Data = "Delete Sucessfuly";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message + ":" + ex.StackTrace;
                return result;

            }
        }

        public AppResponse<BudgetCategoryDto> EditbudgetCategory(BudgetCategoryDto request)
        {
            var result = new AppResponse<BudgetCategoryDto>();
            try
            {
                var budgetcat = new BudgetCategory();
                if (request.Id == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Id cannot be null";
                    return result;
                }
                budgetcat = _budgetCategoryRepository.Get(request.Id.Value);
                budgetcat.Name = request.Name;
                //budgetcat.Id = Guid.NewGuid();
                _budgetCategoryRepository.Edit(budgetcat);
                
                result.IsSuccess = true;
                result.Data = request;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message + ":" + ex.StackTrace;
                return result;

            }
        }

        public AppResponse<List<BudgetCategoryDto>> GetAllBudgetCategory(string userId)
        {
            var result = new AppResponse<List<BudgetCategoryDto>>();

            try
            {
                var query = _budgetCategoryRepository.GetAll().Where(m=>m.Account.UserId==userId);
                var list = query.Select(m => new BudgetCategoryDto
                {
                    Name = m.Name,
                    Id = m.Id,
                }).ToList();
                result.IsSuccess = true;
                result.Data = list;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.StackTrace;
                return result;
            }

        }
    }
}
