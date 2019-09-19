using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kestavar.Migrations
{
    public partial class Logger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    MerchantFk = table.Column<string>(nullable: true),
                    CustomerId = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    InstanceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
