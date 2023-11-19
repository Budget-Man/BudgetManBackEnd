using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class debt_loan_removebudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Budgets_BudgetId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Budgets_BudgetId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BudgetId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Debts_BudgetId",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Debts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "Loans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "Debts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BudgetId",
                table: "Loans",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_BudgetId",
                table: "Debts",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Budgets_BudgetId",
                table: "Debts",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Budgets_BudgetId",
                table: "Loans",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
