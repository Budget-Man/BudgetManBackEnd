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

        public string? language { get; set; }

        public string? currency { get; set; }

        public string defaultMoneyHolderId { get; set; }
    }
}
