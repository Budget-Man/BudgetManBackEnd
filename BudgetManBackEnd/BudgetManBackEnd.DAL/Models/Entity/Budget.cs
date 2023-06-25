using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class Budget:BaseAccountEntity
    {
        [ForeignKey("BudgetCategory")]
        public Guid BudgetCategoryId { get; set; }
        [ForeignKey("BudgetCategoryId")]
        public virtual BudgetCategory BudgetCategory { get; set; }

        public double Balance { get; set; }
    }
}
