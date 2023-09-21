using MayNghien.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManBackEnd.Model.Dto
{
    public class IncomeDto : BaseDto
    {
        public string? Name { get; set; }

       
        public Guid MoneyHolderId { get; set; }
     
        public string? MoneyHolderName { get; set;}
    }
}
