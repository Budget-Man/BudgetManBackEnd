using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Common.Models.Entity;

namespace BudgetManBackEnd.Model.Dto
{
    public class MoneySpendDto:BaseEntity
    {
        public Guid BudgetId { get; set; }
        public string BudgetName { get; set; }

        public double Amount { get; set; }
        public Guid MoneyHolderId { get; set; }
        public string MoneyHolderName { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}
