using MayNghien.Common.Models;

namespace BudgetManBackEnd.Model.Dto
{
    public class BudgetCategoryDto: BaseDto
    {
        public string Name { get; set; }

        public double MonthlyLimit { get; set; }
    }
}
