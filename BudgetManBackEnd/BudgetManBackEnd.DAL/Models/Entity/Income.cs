using MayNghien.Common.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class Income: BaseAccountEntity
    {
        public string Name { get; set; }

        [ForeignKey("MoneyHolder")]
        public Guid MoneyHolderId { get; set; }
        [ForeignKey("MoneyHolderId")]
        public virtual MoneyHolder MoneyHolder { get; set; }

        public double Amount { get; set; }

    }
}
