using BudgetManBackEnd.CommonClass.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class UserSettingDto
    {
        public bool? IsCreateBaseData { get; set; }
        public string? Language { get; set; }
        public string? Currency { get; set; }
        public Guid? DefaultMoneyHolderId { get; set; }
        public string? ChatUserId { get; set; }
        public string? MemberList { get; set; }
    }
}
