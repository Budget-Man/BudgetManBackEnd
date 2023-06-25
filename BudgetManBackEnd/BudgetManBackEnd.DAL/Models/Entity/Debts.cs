using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class Debts:BaseAccountEntity
    {
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double? RemainAmount { get; set; }
        public double? PaidAmount { get; set; }
        public double? TotalInterest { get; set; }
        public double InterestRate { get; set; } = 0;
        public string RatePeriod { get; set; }
    }
}
