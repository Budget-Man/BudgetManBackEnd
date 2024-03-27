namespace BudgetManBackEnd.DAL.Models.Entity
{
    public class MoneyHolder : BaseAccountEntity
    {
        public string Name { get; set; }
        public string? BankName { get; set; }
        public double Balance { get; set; } = 0;

    }
}
