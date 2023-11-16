using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManBackEnd.CommonClass.Enum;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class LoanDto:BaseDto
    {
        public string Name { get; set; }
        public double? TotalAmount { get; set; }
        public double? RemainAmount { get; set; }
        public double? LoanAmount { get; set; }
        public double? TotalInterest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }
    }
}
