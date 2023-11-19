using BudgetManBackEnd.CommonClass.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class Debt:BaseAccountEntity
    {
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double? RemainAmount { get; set; }
        public double? PaidAmount { get; set; }
        public double? TotalInterest { get; set; }
        public double InterestRate { get; set; } = 0;
        public TimePeriod RatePeriod { get; set; }

        [ForeignKey("MoneyHolder")]
        public Guid? MoneyHolderId { get; set; }
        [ForeignKey("MoneyHolderId")]
        public virtual MoneyHolder? MoneyHolder { get; set; }

    }
}
