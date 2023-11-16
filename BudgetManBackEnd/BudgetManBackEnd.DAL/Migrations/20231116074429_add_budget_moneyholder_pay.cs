using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class add_budget_moneyholder_pay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "LoanPays",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoneyHolderId",
                table: "LoanPays",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "DebtsPays",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MoneyHolderId",
                table: "DebtsPays",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanPays_BudgetId",
                table: "LoanPays",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPays_MoneyHolderId",
                table: "LoanPays",
                column: "MoneyHolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtsPays_BudgetId",
                table: "DebtsPays",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtsPays_MoneyHolderId",
                table: "DebtsPays",
                column: "MoneyHolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DebtsPays_Budgets_BudgetId",
                table: "DebtsPays",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DebtsPays_MoneyHolders_MoneyHolderId",
                table: "DebtsPays",
                column: "MoneyHolderId",
                principalTable: "MoneyHolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanPays_Budgets_BudgetId",
                table: "LoanPays",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanPays_MoneyHolders_MoneyHolderId",
                table: "LoanPays",
                column: "MoneyHolderId",
                principalTable: "MoneyHolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebtsPays_Budgets_BudgetId",
                table: "DebtsPays");

            migrationBuilder.DropForeignKey(
                name: "FK_DebtsPays_MoneyHolders_MoneyHolderId",
                table: "DebtsPays");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanPays_Budgets_BudgetId",
                table: "LoanPays");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanPays_MoneyHolders_MoneyHolderId",
                table: "LoanPays");

            migrationBuilder.DropIndex(
                name: "IX_LoanPays_BudgetId",
                table: "LoanPays");

            migrationBuilder.DropIndex(
                name: "IX_LoanPays_MoneyHolderId",
                table: "LoanPays");

            migrationBuilder.DropIndex(
                name: "IX_DebtsPays_BudgetId",
                table: "DebtsPays");

            migrationBuilder.DropIndex(
                name: "IX_DebtsPays_MoneyHolderId",
                table: "DebtsPays");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "LoanPays");

            migrationBuilder.DropColumn(
                name: "MoneyHolderId",
                table: "LoanPays");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "DebtsPays");

            migrationBuilder.DropColumn(
                name: "MoneyHolderId",
                table: "DebtsPays");
        }
    }
}
