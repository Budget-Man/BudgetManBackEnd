using MayNghien.DAL.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class LocalTransfer: BaseAccountEntity
    {
        [ForeignKey("FromMoneyHolder")]
        public Guid FromMoneyHolderId { get; set; }
        [ForeignKey("FromMoneyHolderId")]
        public virtual MoneyHolder FromMoneyHolder { get; set; }




        [ForeignKey("ToMoneyHolder")]
        public Guid ToMoneyHolderId { get; set; }
        [ForeignKey("ToMoneyHolderId")]
        public virtual MoneyHolder ToMoneyHolder { get; set; }


        public double Amount { get; set; }
    }
}
