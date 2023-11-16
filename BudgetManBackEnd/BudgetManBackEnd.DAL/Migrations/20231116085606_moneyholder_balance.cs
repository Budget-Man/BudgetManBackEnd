using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class moneyholder_balance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "MoneyHolders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "MoneyHolders");
        }
    }
}
