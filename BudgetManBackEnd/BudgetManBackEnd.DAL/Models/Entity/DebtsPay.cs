
using BudgetManBackEnd.CommonClass.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class DebtsPay:BaseAccountEntity
    {
        [ForeignKey("Debts")]
        public Guid DebtsId { get; set; }
        [ForeignKey("DebtsId")]
        public virtual Debts Debts { get; set; }
        public double? PaidAmount { get; set; }
        public double? Interest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
