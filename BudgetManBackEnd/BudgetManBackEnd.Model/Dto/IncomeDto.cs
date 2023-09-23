using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class IncomeDto:BaseDto
    {
        public string? Name { get; set; }


        public Guid MoneyHolderId { get; set; }

        public string? MoneyHolderName { get; set; }
    }
}
