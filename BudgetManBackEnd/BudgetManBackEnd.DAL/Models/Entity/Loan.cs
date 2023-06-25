using BudgetManBackEnd.CommonClass.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class Loan:BaseAccountEntity
    {
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double? RemainAmount { get; set; }
        public double? LoanAmount { get; set; }
        public double? TotalInterest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }
    }
}
