using MayNghien.DAL.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class MoneyHolder: BaseAccountEntity
    {
        public string Name { get; set; }
        public string? BankName { get; set; }
       
    }
}
