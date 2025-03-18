using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class trackbalance_budget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "AccountBalanceTrackings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalanceTrackings_BudgetId",
                table: "AccountBalanceTrackings",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBalanceTrackings_Budgets_BudgetId",
                table: "AccountBalanceTrackings",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBalanceTrackings_Budgets_BudgetId",
                table: "AccountBalanceTrackings");

            migrationBuilder.DropIndex(
                name: "IX_AccountBalanceTrackings_BudgetId",
                table: "AccountBalanceTrackings");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "AccountBalanceTrackings");
        }
    }
}
