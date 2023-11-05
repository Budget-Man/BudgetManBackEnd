using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class BudgetDto : BaseDto
    {
        public Guid BudgetCategoryId { get; set; }
        public string BudgetCategoryName { get; set; }

        public double Balance { get; set; }
        public string? Name { get; set; }
    }
}
