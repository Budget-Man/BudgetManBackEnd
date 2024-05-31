using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManBackEnd.DAL.Migrations
{
    public partial class addnewfieldstoaccountInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "AccountInfos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultMoneyHolderId",
                table: "AccountInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNewUser",
                table: "AccountInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "AccountInfos",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "DefaultMoneyHolderId",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "IsNewUser",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "AccountInfos");
        }
    }
}
