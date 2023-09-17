using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.CommonClass.Enum;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class LoanPayDto:BaseDto
    {
        public Guid LoanId { get; set; }
        public string LoanName { get; set; }
        public double? PaidAmount { get; set; }
        public double? Interest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
