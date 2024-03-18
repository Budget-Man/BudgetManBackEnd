using BudgetManBackEnd.CommonClass.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class LoginResponseModel
    {
        public string? userName { get; set; }
        public string? accessToken { get; set; }
        public bool isNewUser { get; set; } = false;
        public Languages? language { get; set; }
        public Currencies? currency { get; set; }
        public Guid? defaultMoneyHolderId { get; set; }
    }
}
