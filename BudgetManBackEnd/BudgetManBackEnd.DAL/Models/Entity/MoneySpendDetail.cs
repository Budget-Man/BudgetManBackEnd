using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class MoneySpendDetail:BaseAccountEntity
    {
        [ForeignKey("MoneySpend")]
        public Guid MoneySpendId { get; set; }
        [ForeignKey("MoneySpendId")]
        public virtual MoneySpend MoneySpend { get; set; }
        public double? Price { get; set; }
        public double? Quantity { get; set; }
        public double Amount { get; set; }
        public string Reason { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
