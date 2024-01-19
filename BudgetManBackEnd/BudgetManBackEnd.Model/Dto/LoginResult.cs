using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class LoginResult
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string[] Roles { get; set; }
    }
}
