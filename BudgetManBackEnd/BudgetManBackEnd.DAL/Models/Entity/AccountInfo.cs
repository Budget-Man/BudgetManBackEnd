using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class AccountInfo : BaseEntity
    {
        public string UserId { get; set; }

        public string Name { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }

    }
}
