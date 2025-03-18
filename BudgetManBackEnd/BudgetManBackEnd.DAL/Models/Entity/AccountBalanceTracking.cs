using BudgetManBackEnd.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class AccountBalanceTracking : BaseAccountEntity
    {
        [ForeignKey("MoneyHolder")]
        public Guid MoneyHolderId { get; set; }
        [ForeignKey("MoneyHolderId")]
        public virtual MoneyHolder MoneyHolder { get; set; }


        [ForeignKey("Budget")]
        public Guid? BudgetId { get; set; }
        [ForeignKey("BudgetId")]
        public virtual Budget? Budget { get; set; }
        public ChangeType ChangeType { get; set; }


        public double CurrentBalance { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }


    }
}
