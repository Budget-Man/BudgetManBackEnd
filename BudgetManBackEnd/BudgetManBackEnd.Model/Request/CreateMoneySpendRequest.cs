using BudgetManBackEnd.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Request
{
    public class CreateMoneySpendRequest
    {
        public Guid BudgetId { get; set; }
        public double Amount { get; set; }
        public Guid MoneyHolderId { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }

        public List<MoneySpendDetailDto> Details { get; set; }
    }
}
