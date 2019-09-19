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
    [Migration("20190909185708_WalletFix")]
    partial class WalletFix
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kestavar.DataEntities.CreditHistory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<DateTime>("IssuedDate");

                    b.Property<string>("IssuerId");

                    b.Property<string>("ResellerId");

                    b.HasKey("Id");

                    b.ToTable("Histories");
                });

            modelBuilder.Entity("Kestavar.DataEntities.CreditToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<DateTime>("ExpiryDate");

                    b.Property<DateTime>("GeneratedDate");

                    b.Property<string>("ResellerId");

                    b.Property<string>("Token");

                    b.Property<bool>("isUsed");

                    b.HasKey("Id");

                    b.ToTable("Tokens");
                });

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

            modelBuilder.Entity("Kestavar.DataEntities.Logger", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CustomerId");

                    b.Property<string>("InstanceId");

                    b.Property<string>("MerchantFk");

                    b.Property<string>("Status");

                    b.Property<DateTime>("date");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Kestavar.DataEntities.Permission", b =>
                {
                    b.Property<string>("PermissionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PermissionName");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("Kestavar.DataEntities.RoleModel", b =>
                {
                    b.Property<string>("RoleModelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Permissions");

                    b.Property<string>("RoleName");

                    b.HasKey("RoleModelId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Kestavar.DataEntities.Transaction", b =>
                {
                    b.Property<string>("TransactionId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.Property<string>("BillerStatus");

                    b.Property<string>("CustomerMeterID");

                    b.Property<DateTime?>("Date");

                    b.Property<double>("ResellerPercent");

                    b.Property<string>("Status");

                    b.Property<string>("UserId");

                    b.Property<string>("amount");

                    b.Property<string>("customerAddress");

                    b.Property<string>("customerName");

                    b.Property<string>("description");

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

                    b.Property<double>("Balance");

                    b.Property<string>("FullName");

                    b.Property<string>("Password");

                    b.Property<double>("Percentage");

                    b.Property<string>("RoleId");

                    b.Property<int>("Subscription");

                    b.Property<string>("UserName");

                    b.Property<bool>("isActive");

                    b.HasKey("UserModelId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
