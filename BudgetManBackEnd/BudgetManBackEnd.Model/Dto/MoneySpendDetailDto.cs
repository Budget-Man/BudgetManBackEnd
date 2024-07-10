using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class MoneySpendDetailDto : BaseDto
    {
        public double? Price { get; set; }
        public double? Quantity { get; set; }
        public double Amount { get; set; }
        public string Reason { get; set; }

        public DateTime? CreateOn { get; set; } 
    }
}
