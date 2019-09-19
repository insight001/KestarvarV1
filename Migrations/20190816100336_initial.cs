using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kestavar.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    CustomerMeterId = table.Column<string>(nullable: true),
                    CustRefrence = table.Column<string>(nullable: true),
                    ThirdPartyCode = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerAddress = table.Column<string>(nullable: true),
                    OtherName = table.Column<string>(nullable: true),
                    PaymentType = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    BusinessDistrict = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    AccountType = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<string>(nullable: false),
                    PermissionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleModelId = table.Column<string>(nullable: false),
                    RoleName = table.Column<string>(nullable: true),
                    Permissions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleModelId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(nullable: false),
                    refNumber = table.Column<string>(nullable: true),
                    reference = table.Column<string>(nullable: true),
                    amount = table.Column<string>(nullable: true),
                    token = table.Column<double>(nullable: false),
                    tokenString = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    receiptNumber = table.Column<string>(nullable: true),
                    tariff = table.Column<string>(nullable: true),
                    tax = table.Column<string>(nullable: true),
                    units = table.Column<string>(nullable: true),
                    unitsType = table.Column<string>(nullable: true),
                    value = table.Column<string>(nullable: true),
                    value1 = table.Column<string>(nullable: true),
                    orderNumber = table.Column<string>(nullable: true),
                    BillerStatus = table.Column<string>(nullable: true),
                    customerName = table.Column<string>(nullable: true),
                    customerAddress = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    CustomerMeterID = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    respCode = table.Column<string>(nullable: true),
                    respDescription = table.Column<string>(nullable: true),
                    AccountType = table.Column<int>(nullable: false),
                    isTestData = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserModelId = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    Percentage = table.Column<double>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserModelId);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "PermissionName" },
                values: new object[,]
                {
                    { "b7366a74", "ALL" },
                    { "d74419f0", "GET_TRANSACTION" },
                    { "afcc5f08", "GET_TRANSACTIONS" },
                    { "9230b4cb", "GET_CUSTOMER" },
                    { "8543f249", "GET_CUSTOMERS" },
                    { "217760cb", "CREATE_CUSTOMERS" },
                    { "447511e0", "GET_ROLES" },
                    { "6a1374a2", "GET_ROLE" },
                    { "139e76dd", "CREATE_ROLES" },
                    { "738aa120", "REMOVE_USER" },
                    { "20bb9115", "CREATE_USER" },
                    { "9856512e", "GET_USER" },
                    { "db3e2c4a", "GET_USERS" },
                    { "5bf637bb", "REMOVE_ROLE" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleModelId", "Permissions", "RoleName" },
                values: new object[,]
                {
                    { "793cee63-3e7f-4d65-8a98-e17a54dee705", "GET_TRANSACTIONS,GET_TRANSACTION,GET_CUSTOMERS,GET_CUSTOMER", "RESELLER" },
                    { "4e03af4e-b090-4", "ALL", "Super Admin" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserModelId", "FullName", "Password", "Percentage", "RoleId", "UserName", "isActive" },
                values: new object[] { "65bf1236-4141-4608-bae9-7f956f045b4a", "Super Admin", "Adetola@1!", 0.0, "4e03af4e-b090-4", "SuperAdmin@Kestavar.com", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
