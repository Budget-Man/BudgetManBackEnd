using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class budget_MonthlyLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MonthlyLimit",
                table: "Budgets",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyLimit",
                table: "Budgets");
        }
    }
}
