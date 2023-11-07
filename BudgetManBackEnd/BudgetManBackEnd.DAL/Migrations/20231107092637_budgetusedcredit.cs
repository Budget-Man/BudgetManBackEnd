using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class budgetusedcredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "UseCredit",
                table: "Budgets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCredit",
                table: "Budgets");
        }
    }
}
