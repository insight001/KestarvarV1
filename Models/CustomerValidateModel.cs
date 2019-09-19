using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kestavar.Models
{
    public class CustomerValidateModel
    {
        public string customerId { get; set; }
        public int MerchantFK { get; set; }
        public int accountType { get; set; }
    }
    public class CustomerValidateRequestModel
    {
        public string customerId { get; set; }
        public int MerchantFK { get; set; }
        public int accountType { get; set; }
        public string hashValue { get; set; }
    }
    public class FailureModel
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
    }
    public class SuccessModel
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class BuyPowerModel
    {
        public string customerId { get; set; }
        public int MerchantFK { get; set; }
        public int accountType { get; set; }
        public string customerName { get; set; }
        public string phoneNumber { get; set; }
        public string amount { get; set; }
        public string emailAddress { get; set; }
        public string reference { get; set; }

    }
    public class BuyPowerRequestModel: BuyPowerModel
    {
        public string hashValue { get; set; }
        public string refNumber { get; set; }

    }

    public class TransactionDetailsModel
    {
        public string refNumber { get; set; }
        public string amount { get; set; }
        public int MerchantFK { get; set; }

    }
    public class TransactionDetailsRequestModel : TransactionDetailsModel
    {
        public string hashValue { get; set; }
    }

    public class AirtimeModel
    {
        public string amount { get; set; }
       public string refNumber { get; set; }
        public string serviceType { get; set; }
        public string phoneNumber { get; set; }
    }
    public class AirtimeRequestModel: AirtimeModel
    {
        public string hashValue { get; set; }
    }
}
