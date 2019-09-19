using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kestavar.DataEntities
{
    public class UserModel
    {
        public string UserModelId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
        public double Percentage { get; set; }
        public bool isActive { get; set; }
        public double Balance { get; set; }
        public SubscriptionType Subscription { get; set; }
    }

    public enum SubscriptionType 
    {
        Postpaid,
        Prepaid

    }
    public class RoleModel
    {
        public string RoleModelId { get; set; }
        public string RoleName { get; set; }
        public string Permissions { get; set; }
    }
    public class CreditToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public double Amount { get; set; }
        public bool isUsed { get; set; }
        public string ResellerId { get; set; }
    }
    public class TokenQuick
    {
        public string ResellerId { get; set; }
        public double Amount { get; set; }
    }
    public class CreditHistory
    {
        public string Id { get; set; }
        public DateTime IssuedDate{get;set;}
        public double Amount { get; set; }
        public string ResellerId { get; set; }
        public string IssuerId { get; set; }



    }
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CustomerMeterId { get; set; }
        public string CustRefrence { get; set; }
        public string ThirdPartyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string OtherName { get; set; }
        public string PaymentType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BusinessDistrict { get; set; }
        public string Receiver { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
      
    }
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string refNumber { get; set; }
        public string reference { get; set; }
        public string amount { get; set; }
        public double token { get; set; }
        public string tokenString { get; set; }
        public string description { get; set; }
        public string receiptNumber { get; set; }
        public string tariff { get; set; }
        public string tax { get; set; }
        public string units { get; set; }
        public string unitsType { get; set; }
        public string value { get; set; }
        public string value1 { get; set; }
        public string orderNumber { get; set; }
        public string BillerStatus { get; set; }
        public string customerName { get; set; }
        public string customerAddress { get; set; }
        public string UserId { get; set; }
        public DateTime? Date { get; set; }
        public string CustomerMeterID { get; set; }
        public string Status { get; set; }
        public string respCode { get; set; }
        public string respDescription { get; set; }
        public int AccountType { get; set; }
        public double ResellerPercent { get; set; }
        //public bool isTestData { get; set; }

    }
    public class Permission
    {
        public string PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
    
}
