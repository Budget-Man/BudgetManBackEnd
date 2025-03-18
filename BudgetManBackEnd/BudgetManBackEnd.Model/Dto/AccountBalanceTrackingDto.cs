using BudgetManBackEnd.Common.Enum;
using MayNghien.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class AccountBalanceTrackingDto : BaseDto
    {
        public Guid MoneyHolderId { get; set; }
        public string? MoneyHolderName { get; set; }

        public int ChangeType { get; set; }
        public string? ChangeTypeName { get; set; }


        public double CurrentBalance { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
    }
}
