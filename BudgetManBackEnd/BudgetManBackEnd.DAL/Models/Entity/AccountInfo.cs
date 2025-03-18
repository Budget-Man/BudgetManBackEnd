using BudgetManBackEnd.CommonClass.Enum;
using MayNghien.Common.Models.Entity;

namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class AccountInfo : BaseEntity
    {
        public string UserId { get; set; }

        public string Name { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }

        public double Balance { get; set; }
        public Languages? Language { get; set; }
        public Currencies? Currency { get; set; }
        public Guid? DefaultMoneyHolderId { get; set; }
        public bool IsNewUser { get; set; }
        public string? ChatUserId { get; set; }
        public string? MemberList { get; set; }
    }
}
