using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Models.Entity;
using BudgetManBackEnd.Model.Dto;
using BudgetManBackEnd.Service.Contract;
using MayNghien.Models.Response.Base;

namespace BudgetManBackEnd.Service.Implementation
{
    public class DebtService : IDebtService
    {
        private IDebtRepository _debtRepository;
        private IMapper _mapper;

        public DebtService(IDebtRepository debtRepository, IMapper mapper)
        {
            _debtRepository = debtRepository;
            _mapper = mapper;
        }

        public AppResponse<DebtDto> CreateDebt(DebtDto request)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var debt = _mapper.Map<Debt>(request);
                debt.Id = Guid.NewGuid();

                request.Id = debt.Id;

                _debtRepository.Add(debt);

                result.BuildResult(request);


            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<string> DeleteDebt(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var debt = _debtRepository.Get(Id);
                debt.IsDeleted = true;
                _debtRepository.Edit(debt);

                result.BuildResult("đã xóa");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DebtDto> EditDebt(DebtDto request)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var debt = _mapper.Map<Debt>(request);

                _debtRepository.Edit(debt);

                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DebtDto>> GetAllDebt()
        {
            var result = new AppResponse<List<DebtDto>>();
            try
            {
                var query = _debtRepository.GetAll();
                var list =  query.Select(x => new DebtDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    TotalAmount = x.TotalAmount,
                    TotalInterest = x.TotalInterest,
                    RemainAmount = x.RemainAmount,
                    PaidAmount = x.PaidAmount,
                    InterestRate = x.InterestRate,
                    RatePeriod = x.RatePeriod,
                }).ToList();

                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DebtDto> GetDebt(Guid Id)
        {
            var result = new AppResponse<DebtDto>();
            try
            {
                var debt = _debtRepository.Get(Id);
                var data = _mapper.Map<DebtDto>(debt);
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
