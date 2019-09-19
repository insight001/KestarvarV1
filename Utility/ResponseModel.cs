using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kestavar.Utility
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string respCode { get; set; }
        public string respDescription { get; set; }
    }

    public class SessionResponse
    {
        public string respCode { get; set; }
        public string respDescription { get; set; }
        public string sessionid { get; set; }
    }
    public class ValidateCustomerResponseModel : ResponseModel
    {
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
        public string CustomerId { get; set; }
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
        public string MinimumAmount { get; set; }
        public string CreditLimit { get; set; }
        public string OutstandingAmount { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        //

        //public string AgentId { get; set; }
        //public string AgentKey { get; set; }
        //public string hash { get; set; }



    }
    public class AirtimeResponseModel : ResponseModel
    {
        public string refNumber { get; set; }
        public string amount { get; set; }
        public string trxRef { get; set; }
        public string serviceType { get; set; }
        public string phoneNumber { get; set; }

    }

    public class BuyPowerResponseModel :ResponseModel
    {
        public string refNumber { get; set; }
        public string amount { get; set; }
        public string tokenAmount { get; set; }
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
        public string Status { get; set; }

    }

}
