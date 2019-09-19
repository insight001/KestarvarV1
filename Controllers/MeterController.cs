using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kestavar.DataEntities;
using Kestavar.Models;
using Kestavar.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kestavar.Controllers
{
    public class MeterController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly DataContext _context;
        public MeterController(IOptions<MyConfig> config, DataContext context)
        {
            this.config = config;
            _context = context;
           
        } 
        [Route("api/v1/Borrow/MeterEligibility")]
        public IActionResult Index()
        {
            return View();
        }

      

       

        public class temp
        {
            public object v { get; set; }
            public string MerchantId { get; set; }
            public string ProductId { get; set; }
        }
       
        [Route("api/test")]
        public async Task<IActionResult> test()
        {
            string agentId = config.Value.AgentId;
            string agentKey = config.Value.AgentKey;
            string signature = Utilities.GenerateHash(agentId+agentKey+config.Value.AgentEmail);
            string path = $"paelyt/AccountTransact/GetSessionId?agentid={agentId}&agentkey={agentKey}&signature={signature}";
            var response = await Utilities.MakeCallGet(path);
            var res = JsonConvert.DeserializeObject<SessionResponse>(await response.Content.ReadAsStringAsync());
            return Ok(res.sessionid);
        }
        [HttpGet]
        [Route("api/Test/Time/{Inst}")]
        public IActionResult Time([FromRoute]string Inst)
        {
            return Ok(_context.Logs.Where(x => x.InstanceId == Inst).ToList());
        }
        [HttpGet]
        [Route("api/Test/OnTest")]
        public async Task<IActionResult> TIDEAsync()
        {
            //config.Value.AgentEmail = "new email";
            var SessionId = await Utilities.GetSessionId(config);
            HttpContext.Session.SetString("sessionId", SessionId);
            return Ok(config.Value.AgentEmail);
        }

        

        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Customer/Validate")]
        public async Task<IActionResult> Validate([FromBody]CustomerValidateModel validateModel)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            string path = "paelyt/PowerTransact/ValidateCustomerID";

            var Customer = new CustomerValidateRequestModel
            {
                accountType = validateModel.accountType,
                customerId = validateModel.customerId,
                MerchantFK = validateModel.MerchantFK,
                hashValue = Utilities.GenerateHash(config.Value.AgentId+config.Value.AgentKey+validateModel.customerId)
            };
          //  var data = JsonConvert.SerializeObject(Customer);
            var stringContent = new StringContent(JsonConvert.SerializeObject(Customer),Encoding.UTF8,"application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if(SId == null)
            {
                 SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
           
            var resp = await Utilities.MakeCallPost(path, config,stringContent, SId);
            var res = JsonConvert.DeserializeObject<ValidateCustomerResponseModel>(await resp.Content.ReadAsStringAsync());
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription,ResponseCode = "99" });
            return Ok(new SuccessModel { Message = res.respDescription, ResponseCode ="00", Data = res});
        }
        

        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Transaction/Buy")]
        public async Task<IActionResult> BuyPower([FromBody]BuyPowerModel buyPower)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            var UserId = User.FindFirstValue("Id");
            var Reseller = _context.Users.Where(x => x.UserModelId == UserId).FirstOrDefault();
            if (Reseller == null)
                return Unauthorized();
            var inst = Guid.NewGuid().ToString();
            var Log = new Logger
            {
                CustomerId = buyPower.customerId,
                date = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                MerchantFk = buyPower.MerchantFK.ToString(),
                InstanceId = inst,
                Status = "Connected to my endpoint"

            };
            _context.Logs.Add(Log);
         
            var refChecker = _context.Transactions.Where(x => x.reference == buyPower.reference).FirstOrDefault();
            bool isRequery = false;
            string refNumber;

            if (refChecker != null && refChecker.value != "AWAITING_SERVICE_PROVIDER")
            {
                isRequery = false;
                return Ok(new SuccessModel { Message = refChecker.respDescription, ResponseCode = "94", Data = refChecker });
            }
            else if(refChecker != null & refChecker.value == "AWAITING_SERVICE_PROVIDER")
            {
                //Perform a call to the details transactions endpoint here
                var response  = await TransactionDetails(new TransactionDetailsModel { amount = refChecker.amount, MerchantFK = 1, refNumber = refChecker.refNumber });
                return response;
            }
            refNumber = Guid.NewGuid().ToString().Substring(0, 8);

            if (Reseller.Subscription == SubscriptionType.Prepaid && Reseller.Balance < System.Convert.ToDouble(buyPower.amount))
                return Ok(new FailureModel { Message ="Account Balance not Sufficient", ResponseCode ="93" });


            
            string path = "paelyt/PowerTransact/BuyPower";
           
            var Power = new BuyPowerRequestModel
            {
                accountType = buyPower.accountType,
                customerId = buyPower.customerId,
                amount = buyPower.amount,
                customerName = buyPower.customerName,
                emailAddress = buyPower.emailAddress,
                phoneNumber = buyPower.phoneNumber,
                MerchantFK = buyPower.MerchantFK,
                hashValue = Utilities.GenerateHash(buyPower.customerId + buyPower.amount + config.Value.AgentId + config.Value.AgentKey),
                refNumber = refNumber,
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(Power), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);
            var res = JsonConvert.DeserializeObject<BuyPowerResponseModel>(await resp.Content.ReadAsStringAsync());
            res.Status = inst.ToString();



            //Deduct amount from wallet balance if account is Postpaid.
            if (!isRequery)
            {
                double ResellerBalance = Reseller.Balance;
                Reseller.Balance = ResellerBalance - System.Convert.ToDouble(buyPower.amount);
                _context.Entry(Reseller).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            
              
            
          
            var Logg = new Logger
            {
                CustomerId = buyPower.customerId,
                date = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                MerchantFk = buyPower.MerchantFK.ToString(),
                InstanceId = inst,
                Status = "Returned response"

            };
            _context.Logs.Add(Logg);

           
            res.refNumber = buyPower.reference;
            Transaction trans = Convert(res);
            trans.UserId = User.FindFirstValue("Id");
            trans.CustomerMeterID = Power.customerId;
            trans.AccountType = buyPower.accountType;
            var refff = _context.Transactions.Where(x => x.reference == buyPower.reference).ToList();
          if(isRequery)
            {
                refChecker.value = trans.value;
                refChecker.value1 = trans.value;
                refChecker.description = "success";
                _context.Entry(refChecker).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                _context.Transactions.Add(trans);
            }
            trans.ResellerPercent = Reseller.Percentage;
            trans.refNumber = refNumber;
          
            if(res.respCode.Length > res.respDescription.Length)
            {
                var code = res.respDescription;
                res.respDescription = res.respCode;
                res.respCode = code;
            }
                
            _context.SaveChanges();
            res.refNumber = buyPower.reference;
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription, ResponseCode = "99" });
          
          
            

            return Ok(new SuccessModel { Message = res.respDescription,ResponseCode = "00", Data =res});
        }


        //Review this Later :: Turn method to post, request for refNumber only
        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Transaction/Details")]
        public async Task<IActionResult> TransactionDetails([FromBody]TransactionDetailsModel transaction)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            string path = "paelyt/PowerTransact/GetTransactionDetails";
            string refNumber = _context.Transactions.Where(x => x.reference == transaction.refNumber).Select(x => x.refNumber).FirstOrDefault();
            var Transaction = new TransactionDetailsRequestModel
            {
                amount = transaction.amount,
                MerchantFK = transaction.MerchantFK,
                refNumber = refNumber,
                hashValue = Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey + refNumber + transaction.amount)
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(Transaction), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);
            var res = JsonConvert.DeserializeObject<BuyPowerResponseModel>(await resp.Content.ReadAsStringAsync());
            res.refNumber = transaction.refNumber;
            if (res.respCode != "00")
            {
                var TransLocal = _context.Transactions.Where(x => x.reference == transaction.refNumber).FirstOrDefault();
                if (TransLocal != null)
                    return Ok(new SuccessModel { Message = res.respDescription, ResponseCode = "00", Data = TransLocal });
            }
            return Ok(new SuccessModel { Message = res.respDescription, ResponseCode = "00", Data = res });
           

        }


        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Borrow/Eligible")]
        public async Task<IActionResult> CheckEligibility([FromBody]CustomerValidateModel validateModel)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            string path = "paelyt/PowerTransact/CheckMeterEligibilty";
;            var Eligible = new CustomerValidateRequestModel
            {
                accountType = validateModel.accountType,
                customerId = validateModel.customerId,
                MerchantFK = validateModel.MerchantFK,
                hashValue = Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey + validateModel.customerId)
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(Eligible), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);
            var res = JsonConvert.DeserializeObject<ValidateCustomerResponseModel>(await resp.Content.ReadAsStringAsync());
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription, ResponseCode = "99" });
            return Ok(new SuccessModel {  Data = res, Message = res.ResponseDescription, ResponseCode = "00"});
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Borrow")]
        public async Task<IActionResult> BorrowPower([FromBody]BuyPowerModel buyPower)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel {  ResponseCode = "90", Message = "Bad request"});
            var refChecker = _context.Transactions.Where(x => x.reference == buyPower.reference).FirstOrDefault();
            if (refChecker != null)
                return Ok(new FailureModel { Message = refChecker.respDescription, ResponseCode = "94" });
            string path = "paelyt/PowerTransact/BorrowPower";
            string refNumber = Guid.NewGuid().ToString().Substring(0, 15);
            var Power = new BuyPowerRequestModel
            {
                accountType = buyPower.accountType,
                customerId = buyPower.customerId,
                amount = buyPower.amount,
                customerName = buyPower.customerName,
                emailAddress = buyPower.emailAddress,
                phoneNumber = buyPower.phoneNumber,
                MerchantFK = buyPower.MerchantFK,
                hashValue = Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey+ buyPower.customerId + buyPower.amount ),
                refNumber = refNumber
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(Power), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);

            var res = JsonConvert.DeserializeObject<BuyPowerResponseModel>(await resp.Content.ReadAsStringAsync());
            res.refNumber = buyPower.reference;
            Transaction trans = Convert(res);
            trans.UserId = User.FindFirstValue("Id");
            trans.CustomerMeterID = Power.customerId;
            trans.AccountType = buyPower.accountType;
            trans.reference = buyPower.reference;
            _context.Transactions.Add(trans);
            _context.SaveChanges();
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription, ResponseCode = "99" });
            return Ok(new SuccessModel { ResponseCode = "00", Message = res.respDescription, Data =res });
        }
        
        [HttpPost]
        [Authorize]
        [Route("api/v1/Power/Borrow/TransactionDetails")]
        public async Task<IActionResult> BorrowTransactionDetails([FromBody]TransactionDetailsModel transaction)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            string path = "paelyt/PowerTransact/GetBorrowTransactionDetails";
            string refNumber = _context.Transactions.Where(x => x.reference == transaction.refNumber).Select(x => x.refNumber).FirstOrDefault();
            var Transaction = new TransactionDetailsRequestModel
            {
                amount = transaction.amount,
                MerchantFK = transaction.MerchantFK,
                refNumber = refNumber,
                hashValue = Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey + transaction.refNumber + transaction.amount)
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(Transaction), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);
            var res = JsonConvert.DeserializeObject<BuyPowerResponseModel>(await resp.Content.ReadAsStringAsync());

            res.refNumber = transaction.refNumber;
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription, ResponseCode = "99" });
            return Ok(new SuccessModel {  Data = res, Message = res.respDescription, ResponseCode = "00"});
        }

       

        [HttpPost]
        [Authorize]
        [Route("api/v1/Airtime/Purchase")]
        public async  Task<IActionResult> Purchase([FromBody]AirtimeModel airtime)
        {
            if (!ModelState.IsValid)
                return Ok(new FailureModel { ResponseCode = "90", Message = "Bad request" });
            var path = "paelyt/AirtimeTransact/BuyAirtime";
            string refNumber = Guid.NewGuid().ToString().Substring(0, 15);
            var Airtime = new AirtimeRequestModel
            {
                amount = airtime.amount,
                phoneNumber = airtime.phoneNumber,
                refNumber = refNumber,
                serviceType = airtime.serviceType,
                hashValue = Utilities.GenerateHash(config.Value.AgentId + config.Value.AgentKey + refNumber)
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(Airtime), Encoding.UTF8, "application/json");
            var SId = HttpContext.Session.GetString("SessionId");
            if (SId == null)
            {
                SId = await Utilities.GetSessionId(config);
                HttpContext.Session.SetString("SessionId", SId);
            }
            var resp = await Utilities.MakeCallPost(path, config, stringContent,SId);
            var res = JsonConvert.DeserializeObject<AirtimeResponseModel>(await resp.Content.ReadAsStringAsync());
            if (res.respCode != "00")
                return Ok(new FailureModel { Message = res.respDescription, ResponseCode = "99"});
            return Ok(new SuccessModel {  ResponseCode = "00", Message = res.respDescription, Data = res});
        }






       





      


        public static Transaction Convert(BuyPowerResponseModel buyPower)
        {
            var trans = new Transaction
            {
                amount = buyPower.amount,
                BillerStatus = buyPower.BillerStatus,
                customerAddress = buyPower.customerAddress,
                customerName = buyPower.customerName,
                description = buyPower.description,
                Date = DateTime.Now,
                orderNumber = buyPower.orderNumber,
                receiptNumber = buyPower.receiptNumber,
                refNumber = buyPower.refNumber,
                tariff = buyPower.tariff,
                tax = buyPower.tax,
                tokenString = buyPower.tokenAmount,
                TransactionId = Guid.NewGuid().ToString(),
                units = buyPower.units,
                unitsType = buyPower.unitsType,
                value = buyPower.value,
                value1 = buyPower.value,
                respCode = buyPower.respCode,
                respDescription = buyPower.respDescription,
                Status = buyPower.Status
                
            };
            if (trans.tokenString != null)
                trans.token = System.Convert.ToDouble(trans.tokenString);
            return trans;
        }

    }

}