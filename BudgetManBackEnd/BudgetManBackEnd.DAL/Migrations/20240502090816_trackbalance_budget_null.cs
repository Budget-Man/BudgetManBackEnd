using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class trackbalance_budget_null : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BudgetId",
                table: "AccountBalanceTrackings",
                 nullable: true


                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
