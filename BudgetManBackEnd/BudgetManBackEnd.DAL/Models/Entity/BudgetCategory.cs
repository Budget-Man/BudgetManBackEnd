using MayNghien.Common.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class BudgetCategory: BaseAccountEntity
    {
        public int Name { get; set; }

    }
}
