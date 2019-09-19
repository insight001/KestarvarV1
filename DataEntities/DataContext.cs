using Kestavar.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kestavar.DataEntities
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :base(options)
        {


        }


        public  DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Logger> Logs { get; set; }
        public DbSet<CreditToken> Tokens { get; set; }
        public DbSet<CreditHistory> Histories { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<RoleModel>().HasData(new RoleModel
        //    {
        //        RoleModelId = "4e03af4e-b090-4",
        //        RoleName = "Super Admin",
        //        Permissions = "ALL"
        //    },
        //    new RoleModel
        //    {
        //        RoleModelId = Guid.NewGuid().ToString(),
        //        RoleName = "RESELLER",
        //        Permissions = "GET_TRANSACTIONS,GET_TRANSACTION,GET_CUSTOMERS,GET_CUSTOMER"
        //    });
        //    modelBuilder.Entity<UserModel>().HasData(new UserModel
        //    {
        //        FullName = "Super Admin",
        //        Password = "Adetola@1!",
        //        RoleId = "4e03af4e-b090-4",
        //        UserModelId = Guid.NewGuid().ToString(),
        //        UserName = "SuperAdmin@Kestavar.com",
        //        isActive = true

        //    });

        //    modelBuilder.Entity<Permission>().HasData(new Permission
        //    {
        //        PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //        PermissionName = "ALL"
        //    },
        //    new Permission
        //    {
        //        PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //        PermissionName = "GET_USERS"
        //    },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_USER"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "CREATE_USER"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "REMOVE_USER"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "CREATE_ROLES"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "REMOVE_ROLE"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_ROLES"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_ROLE"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "CREATE_CUSTOMERS"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_CUSTOMERS"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_CUSTOMER"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_TRANSACTIONS"
        //     },
        //     new Permission
        //     {
        //         PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
        //         PermissionName = "GET_TRANSACTION"
        //     }

        //    );
        //}
    }
}
