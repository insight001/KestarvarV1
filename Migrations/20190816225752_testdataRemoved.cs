using Microsoft.EntityFrameworkCore.Migrations;

namespace Kestavar.Migrations
{
    public partial class testdataRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(

                name: "isTestData",
                 table: "Transactions"

                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isTestData",
                table: "Transactions"
               
                );
        }
    }
}
