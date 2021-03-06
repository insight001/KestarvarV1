﻿// <auto-generated />
using System;
using Kestavar.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kestavar.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190816100336_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kestavar.DataEntities.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountNumber");

                    b.Property<string>("AccountType");

                    b.Property<string>("BusinessDistrict");

                    b.Property<string>("CustRefrence");

                    b.Property<string>("CustomerAddress");

                    b.Property<string>("CustomerMeterId");

                    b.Property<string>("CustomerName");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("OtherName");

                    b.Property<string>("PaymentType");

                    b.Property<string>("Phone");

                    b.Property<string>("Receiver");

                    b.Property<string>("ThirdPartyCode");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Kestavar.DataEntities.Permission", b =>
                {
                    b.Property<string>("PermissionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PermissionName");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionId = "b7366a74",
                            PermissionName = "ALL"
                        },
                        new
                        {
                            PermissionId = "db3e2c4a",
                            PermissionName = "GET_USERS"
                        },
                        new
                        {
                            PermissionId = "9856512e",
                            PermissionName = "GET_USER"
                        },
                        new
                        {
                            PermissionId = "20bb9115",
                            PermissionName = "CREATE_USER"
                        },
                        new
                        {
                            PermissionId = "738aa120",
                            PermissionName = "REMOVE_USER"
                        },
                        new
                        {
                            PermissionId = "139e76dd",
                            PermissionName = "CREATE_ROLES"
                        },
                        new
                        {
                            PermissionId = "5bf637bb",
                            PermissionName = "REMOVE_ROLE"
                        },
                        new
                        {
                            PermissionId = "447511e0",
                            PermissionName = "GET_ROLES"
                        },
                        new
                        {
                            PermissionId = "6a1374a2",
                            PermissionName = "GET_ROLE"
                        },
                        new
                        {
                            PermissionId = "217760cb",
                            PermissionName = "CREATE_CUSTOMERS"
                        },
                        new
                        {
                            PermissionId = "8543f249",
                            PermissionName = "GET_CUSTOMERS"
                        },
                        new
                        {
                            PermissionId = "9230b4cb",
                            PermissionName = "GET_CUSTOMER"
                        },
                        new
                        {
                            PermissionId = "afcc5f08",
                            PermissionName = "GET_TRANSACTIONS"
                        },
                        new
                        {
                            PermissionId = "d74419f0",
                            PermissionName = "GET_TRANSACTION"
                        });
                });

            modelBuilder.Entity("Kestavar.DataEntities.RoleModel", b =>
                {
                    b.Property<string>("RoleModelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Permissions");

                    b.Property<string>("RoleName");

                    b.HasKey("RoleModelId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleModelId = "4e03af4e-b090-4",
                            Permissions = "ALL",
                            RoleName = "Super Admin"
                        },
                        new
                        {
                            RoleModelId = "793cee63-3e7f-4d65-8a98-e17a54dee705",
                            Permissions = "GET_TRANSACTIONS,GET_TRANSACTION,GET_CUSTOMERS,GET_CUSTOMER",
                            RoleName = "RESELLER"
                        });
                });

            modelBuilder.Entity("Kestavar.DataEntities.Transaction", b =>
                {
                    b.Property<string>("TransactionId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.Property<string>("BillerStatus");

                    b.Property<string>("CustomerMeterID");

                    b.Property<DateTime?>("Date");

                    b.Property<string>("Status");

                    b.Property<string>("UserId");

                    b.Property<string>("amount");

                    b.Property<string>("customerAddress");

                    b.Property<string>("customerName");

                    b.Property<string>("description");

                    b.Property<bool>("isTestData");

                    b.Property<string>("orderNumber");

                    b.Property<string>("receiptNumber");

                    b.Property<string>("refNumber");

                    b.Property<string>("reference");

                    b.Property<string>("respCode");

                    b.Property<string>("respDescription");

                    b.Property<string>("tariff");

                    b.Property<string>("tax");

                    b.Property<double>("token");

                    b.Property<string>("tokenString");

                    b.Property<string>("units");

                    b.Property<string>("unitsType");

                    b.Property<string>("value");

                    b.Property<string>("value1");

                    b.HasKey("TransactionId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Kestavar.DataEntities.UserModel", b =>
                {
                    b.Property<string>("UserModelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FullName");

                    b.Property<string>("Password");

                    b.Property<double>("Percentage");

                    b.Property<string>("RoleId");

                    b.Property<string>("UserName");

                    b.Property<bool>("isActive");

                    b.HasKey("UserModelId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserModelId = "65bf1236-4141-4608-bae9-7f956f045b4a",
                            FullName = "Super Admin",
                            Password = "Adetola@1!",
                            Percentage = 0.0,
                            RoleId = "4e03af4e-b090-4",
                            UserName = "SuperAdmin@Kestavar.com",
                            isActive = true
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
