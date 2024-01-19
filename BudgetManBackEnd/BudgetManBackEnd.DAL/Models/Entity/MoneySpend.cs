using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class MoneySpend:BaseAccountEntity
    {
        [ForeignKey("Budget")]
        public Guid BudgetId { get; set; }
        [ForeignKey("BudgetId")]
        public virtual Budget Budget { get; set; }

        public double Amount { get; set; }

        [ForeignKey("MoneyHolder")]
        public Guid MoneyHolderId { get; set; }
        [ForeignKey("MoneyHolderId")]
        public virtual MoneyHolder MoneyHolder { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
