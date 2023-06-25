using MayNghien.DAL.Models.Entity;
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

    }
}
