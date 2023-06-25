using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class MoneyHolder:BaseEntity
    {
        public string Name { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public AccountInfo Account { get; set; }
    }
}
