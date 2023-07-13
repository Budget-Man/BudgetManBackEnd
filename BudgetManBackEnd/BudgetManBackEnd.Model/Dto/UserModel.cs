using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? EmailAddress { get; set; }
    }
}
