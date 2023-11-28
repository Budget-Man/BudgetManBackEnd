using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class MoneyHolderDto:BaseDto
    {
        public string Name { get; set; }
        public string? BankName { get; set; }

        public double? Balance { get; set; }
    }
}
