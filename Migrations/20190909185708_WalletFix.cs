using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kestavar.Migrations
{
    public partial class WalletFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Users",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Subscription",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ResellerPercent",
                table: "Transactions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IssuedDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    ResellerId = table.Column<string>(nullable: true),
                    IssuerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    GeneratedDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    isUsed = table.Column<bool>(nullable: false),
                    ResellerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Subscription",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResellerPercent",
                table: "Transactions");
        }
    }
}
