using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kestavar.DataEntities;
using Kestavar.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
namespace Kestavar.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<TokenHelper> config;
        private readonly DataContext _context;
        public AccountController(IOptions<TokenHelper> config, DataContext context)
        {
            this.config = config;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class loginresponse
        {
            public string Token { get; set; }
            public UserModelDTO user { get; set; }
        }
        public class UserModelDTO
        {
            public string Username { get; set; }
            public RoleModel Role { get; set; }
           
        }

        [HttpPost]
        [Route("api/Account/Login")]
        public IActionResult Login([FromBody]LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var User = _context.Users.Where(x=>x.UserName.ToLower() == login.Email.ToLower() && x.isActive == true).FirstOrDefault();
            if (User == null)
                return BadRequest("Invalid Login Attempt .");
            if (User.Password != login.Password)
                return BadRequest("Invalid Login Attempt");
            var role = _context.Roles.Where(x => x.RoleModelId == User.RoleId).FirstOrDefault();
           
            return Ok(new loginresponse { Token = GenerateJwtToken(login.Email, User), user = new UserModelDTO { Username = User.FullName, Role = role } });
        }


        [HttpGet]
        [Authorize]
        [Route("api/v1/Users")]
        public IActionResult Users()
        {
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_USERS", RoleId);
            if (checker == false)
                return Unauthorized();
            return Ok(_context.Users.Where(x => x.isActive == true).ToList());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/User/{Id}")]
        public IActionResult GetUser([FromRoute] string Id)
        {
            if (Id == null)
                return NotFound("Resource Missing");
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_USER", RoleId);
            if (checker == false)
                return Unauthorized();
            var Userr = _context.Users.Where(x => x.isActive && x.UserModelId == Id).FirstOrDefault();
            return Ok(Userr);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/User/Create")]
        public IActionResult Create([FromBody]UserModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("CREATE_USER", RoleId);
            if (checker == false)
                return Unauthorized();
            user.UserModelId = Guid.NewGuid().ToString();
            var exist = _context.Users.Where(x => x.UserName == user.UserName && x.isActive == true).FirstOrDefault();
            if (exist != null)
                return BadRequest("User exist");
            user.isActive = true;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok( user);
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/User/Remove")]
        public IActionResult RemoveUser([FromBody]UserModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters, Check and try again");

            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("REMOVE_USER", RoleId);
            if (checker == false)
                return Unauthorized();
            var ActualUser = _context.Users.Where(x => x.UserName == user.UserName).FirstOrDefault();
            if (ActualUser == null || ActualUser.isActive == false)
                return NotFound("User Not Found");
            ActualUser.isActive = false;
            _context.Entry(ActualUser).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok("User Deleted"); 

        }

        [HttpPost]
        [Route("api/v1/User/update")]
        public IActionResult UpdateUser([FromBody]UserModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters, Check and try again");
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("REMOVE_USER", RoleId);
            if (checker == false)
                return Unauthorized();
            var ActualUser = _context.Users.Where(x => x.UserName == user.UserName).FirstOrDefault();
            if (ActualUser == null || ActualUser.isActive == false)
                return NotFound("User Not Found");
            ActualUser.Password = user.Password ?? ActualUser.Password;
            ActualUser.Subscription = user.Subscription;
            ActualUser.RoleId = user.RoleId  ?? ActualUser.RoleId;
            ActualUser.UserName = user.UserName ?? ActualUser.UserName;
            ActualUser.FullName = user.FullName ?? ActualUser.FullName;
            ActualUser.Percentage = user.Percentage;
            _context.Entry(ActualUser).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(ActualUser);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/GenerateCreditToken")]
        public IActionResult TokenGen([FromBody]TokenQuick quick)
        {
            var Ispermitted = isPermitted("ALL", User.FindFirstValue("RoleId"));
            if (!Ispermitted)
                return Unauthorized();
            string Tok = Guid.NewGuid().ToString();
            var Token = new CreditToken
            {
                Amount = quick.Amount,
                ExpiryDate = DateTime.Now.AddHours(2),
                GeneratedDate = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                isUsed = false,
                ResellerId = quick.ResellerId,
                Token = Tok


            };
            // Send Token to EmailAddress 

            _context.Tokens.Add(Token);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        [Authorize]
        [Route("api/v1/CreditWallet")]
        public IActionResult CreditWallet([FromBody]CreditModel credit)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var Ispermitted = isPermitted("ALL", User.FindFirstValue("RoleId"));
            if (!Ispermitted)
                return Unauthorized();
            var Token = _context.Tokens.Where(x => x.Token == credit.VerificationToken).FirstOrDefault();
            if(Token == null)
            {
                return BadRequest("Invalid Token");

            }

            if(Token.isUsed || Token.ExpiryDate <= DateTime.Now)
            {
                return BadRequest("Invalid Token");
            }

           if(Token.Amount != credit.Amount)
            {
                return BadRequest("Token Invalid for Specified Amount");
            }
            if (Token.ResellerId != credit.ResellerId)
                return BadRequest("Token is for different reseller");

            var Reseller = _context.Users.Where(x => x.UserModelId == credit.ResellerId).FirstOrDefault();
            if (Reseller == null)
                return BadRequest("Reseller not recognised");
            Reseller.Balance += credit.Amount;
            _context.Entry(Reseller).State = EntityState.Modified;
            Token.isUsed = true;
            _context.Entry(Token).State = EntityState.Modified;


            var Histo = new CreditHistory
            {
                Amount = credit.Amount,
                Id = Guid.NewGuid().ToString(),
                IssuedDate = DateTime.Now,
                IssuerId = User.FindFirstValue("Id"),
                ResellerId = credit.ResellerId,


            };
            _context.Histories.Add(Histo);
            _context.SaveChanges();


            return Ok("Wallet Creditied Successfully");
        }

        public class CreditModel
        {
            public string ResellerId { get; set; }
            public string VerificationToken { get; set; }
            public double Amount { get; set; }
        } 


       


        [HttpGet]
        [Authorize]
        [Route("api/v1/Roles")]
        public IActionResult GetRoles()
        {
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_ROLES", RoleId);
            if (checker == false)
                return Unauthorized();
            return Ok(_context.Roles.ToList());
        }
        [HttpGet]
        [Authorize]
        [Route("api/v1/Role/{RoleId}")]
        public IActionResult GetRole(string RoleId)
        {
            var RoleIdd = User.FindFirstValue("Role");
            if (RoleIdd == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_ROLE", RoleIdd);
            if (checker == false)
                return Unauthorized();
            return Ok(_context.Roles.Where(q => q.RoleModelId == RoleId).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/Role/Create")]
        public IActionResult CreateRole([FromBody]RoleModel role)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("CREATE_ROLES", RoleId);
            if (checker == false)
                return Unauthorized();
            role.RoleModelId = Guid.NewGuid().ToString();
           
            _context.Roles.Add(role);
            _context.SaveChanges();

            return Ok(role);
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/Role/Remove")]
        public IActionResult RemoveRole([FromBody]RoleModel role)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters, Check and try again");

            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("REMOVE_ROLE", RoleId);
            if (checker == false)
                return Unauthorized();

            var ActualUser = _context.Roles.Where(x => x.RoleModelId == role.RoleModelId).FirstOrDefault();
            if (ActualUser == null)
                return NotFound("Role Not Found");

            _context.Roles.Remove(ActualUser);
            _context.SaveChanges();
            return Ok("Role Removed Successfully");

        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/Role/Update")]
        public IActionResult UpdateRole([FromBody]RoleModel role)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters, Check and try again");

            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("REMOVE_ROLE", RoleId);
            if (checker == false)
                return Unauthorized();
           

            var ActualUser = _context.Roles.Where(x => x.RoleModelId == role.RoleModelId).FirstOrDefault();
            if (ActualUser == null)
                return NotFound("Role Not Found");

            ActualUser.Permissions = role.Permissions;
            ActualUser.RoleName = role.RoleName;
            _context.Entry(ActualUser).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(ActualUser);

        }

        public  bool isPermitted(string permission, string RoleId)
        {
            var Perm = _context.Roles.Where(x => x.RoleModelId == RoleId).FirstOrDefault();
            if (Perm == null)
                return false;
            var permissions = Perm.Permissions.Split(',');
            if (permissions.Contains(permission) || permissions.Contains("ALL"))
                return true;
            return false;
        }

        [HttpGet]
        [Route("api/fix")]
        public IActionResult FixDuplicate()
        {
            List<Permission> permissions = new List<Permission>()
         {
             new Permission
            {
                PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                PermissionName = "ALL"
            },
            new Permission
            {
                PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                PermissionName = "GET_USERS"
            },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_USER"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "CREATE_USER"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "REMOVE_USER"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "CREATE_ROLES"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "REMOVE_ROLE"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_ROLES"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_ROLE"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "CREATE_CUSTOMERS"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_CUSTOMERS"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_CUSTOMER"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_TRANSACTIONS"
             },
             new Permission
             {
                 PermissionId = Guid.NewGuid().ToString().Substring(0, 8),
                 PermissionName = "GET_TRANSACTION"
             }
         };

            foreach(var perm in permissions)
            {
                _context.Permissions.Add(perm);
                _context.SaveChanges();

            }

            return Ok(_context.Permissions.ToList());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/Transactions/Share")]
        public IActionResult Share([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate, [FromQuery]String ResellerName)
        {
            var RoleId = User.FindFirstValue("Role");
            var UserId = User.FindFirstValue("Id");
            var checker = isPermitted("ALL", RoleId);
            var ResellerNameCheck = _context.Users.Where(x => x.FullName.ToLower() == ResellerName.ToLower()).FirstOrDefault();
            if (ResellerNameCheck == null)
                return NotFound("Reseller with the Name Specified not found");

            if (checker)
            {
                var Seller = _context.Users.Where(x => x.FullName.ToLower() == ResellerName.ToLower()).FirstOrDefault();
                if (Seller == null)
                    return NotFound("Reseller with specified name not found");
                return Ok(sharemoney(StartDate, EndDate, Seller));

            }
            else
            {
                var Seller = _context.Users.Where(x => x.UserModelId == UserId).FirstOrDefault();
                if (Seller == null)
                    return NotFound("Seller's Data Missing");
                return Ok(sharemoney(StartDate, EndDate, Seller));
            }

           
        }

        public class ShareModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public String ResellerName { get; set; }
            public double ResellPercet { get; set; }
            public double KestarvarPercent { get; set; }
            public double ResellAmount { get; set; }
            public double KestarvarAmount { get; set; }
            public double TotalAmount { get; set; }
            public double NetAmount { get; set; }
        }


        public ShareModel sharemoney(DateTime StartDate, DateTime EndDate, UserModel Reseller )
        {
            
                var TotalAmount = _context.Transactions.Where(x => x.Date.Value.Date >= StartDate.Date && x.Date.Value <= EndDate.Date && x.respCode == "00" && x.UserId == Reseller.UserModelId).Select(x => x.token).Sum();
                var RessellerPercentage = Reseller.Percentage / 100;
                var ResellerPercent = (0.02 * TotalAmount) * (RessellerPercentage);
            var kestarvarPercentage = (0.02 * TotalAmount) - ResellerPercent;
            return new ShareModel
            {
                EndDate = EndDate,
                KestarvarAmount = kestarvarPercentage,
                ResellerName = Reseller.FullName,
                ResellAmount = ResellerPercent,
                StartDate = StartDate,
                ResellPercet = Reseller.Percentage,
                KestarvarPercent = 100 - Reseller.Percentage,
                TotalAmount = TotalAmount,
                NetAmount = TotalAmount + kestarvarPercentage +ResellerPercent
            };
        }

       
        

        [HttpGet]
        [Authorize]
        [Route("api/v1/Customers")]
        public IActionResult Customers([FromQuery]int offset, [FromQuery]int limit)
        {
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_CUSTOMERS", RoleId);
            if (checker == false)
                return Unauthorized();

            var Customers = _context.Customers.ToList().Skip(offset).Take(limit);
            return Ok(Customers);
        }



        [HttpPost]
        [Authorize]
        [Route("api/v1/Customer/Create")]
        public IActionResult AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("CREATE_CUSTOMERS", RoleId);
            if (checker == false)
                return Unauthorized();
            customer.CustomerId = Guid.NewGuid().ToString();
            _context.Customers.Add(customer);
            return Ok(customer);
        }

      
        [HttpGet]
        [Authorize]
        [Route("api/v1/Customer/{Id}")]
        public IActionResult Customer([FromRoute] string Id)
        {
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_CUSTOMER", RoleId);
            if (checker == false)
                return Unauthorized();

            return Ok(_context.Customers.Where(x => x.CustomerId == Id).FirstOrDefault());
        }

       

        [HttpGet]
        [Route("api/v1/Trans")]
        public IActionResult Trans()
        {
            return Ok(_context.Transactions.ToList().OrderBy(x => x.Date));
        }


        [HttpGet]
        [Authorize]
        [Route("api/v1/Transactions")]
        public IActionResult Transactions([FromQuery]string Search, [FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate,int Filter =1, [FromQuery]int offset = 0, [FromQuery]int limit =10)
        {
            var RoleId = User.FindFirstValue("Role");
            var UserId = User.FindFirstValue("Id");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
           
            if (isPermitted("ALL", RoleId))
            {
                List<Transaction> trans = new List<Transaction>();
                if(Filter == 1)
                {
                    trans = _context.Transactions.Where(x=>x.value != null && x.value != "").OrderBy(x=>x.Date.Value).ToList(); 
                }
                else if(Filter ==3)
                {
                     trans = _context.Transactions.Where(x=>x.value == null || x.value =="").OrderBy(x => x.Date.Value).ToList();
                }
                else if(Filter == 2)
                {
                    trans = _context.Transactions.OrderBy(x => x.Date.Value).ToList();
                }
              
            
                if (Search != null)
                {
                    trans = trans.Where(x => x.CustomerMeterID == Search).ToList();
                }
                if(StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {
                    trans = trans.Where(x => x.Date.Value.Date >= StartDate && x.Date.Value.Date <= EndDate).ToList();
                }
                else
                {
                    trans = trans.Where(x => x.Date.Value.Date == DateTime.Now.Date).ToList();
                }
                var count = trans.Count();
                //  var sum = trans.Sum(x => x.token);
                var TheSum = trans.Sum(x => x.token);
                trans = trans.Skip(offset).Take(limit).ToList();
               

                return Ok(new ArrayList { trans, count, TheSum});
                
             
            }
            else
            {
                if (isPermitted("GET_TRANSACTIONS", RoleId))
                {
                    List<Transaction> trans = new List<Transaction>();
                    if (Filter == 1)
                    {
                        trans = _context.Transactions.Where(x => (x.value != null && x.value != "") && x.UserId == UserId).OrderBy(x => x.Date).ToList();
                    }
                    else if(Filter == 3)
                    {
                        trans = _context.Transactions.Where(x => (x.value == null || x.value == "")&& x.UserId == UserId).OrderBy(x => x.Date).ToList();
                    }
                    else if(Filter == 2)
                    {
                        trans = _context.Transactions.Where(x=>x.UserId == UserId).OrderBy(x => x.Date).ToList();
                    }


                
                    if (Search != null)
                    {
                        trans = trans.Where(x => x.CustomerMeterID == Search).ToList();
                    }
                    if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                    {
                        trans = trans.Where(x => x.Date.Value.Date >= StartDate && x.Date.Value.Date <= EndDate).ToList();
                    }
                    else
                    {
                        trans = trans.Where(x => x.Date.Value.Date == DateTime.Now.Date).ToList();
                    }
                    var TheSum = trans.Sum(x => x.token);
                    var count = trans.Count();
                   
                    trans = trans.Skip(offset).Take(limit).ToList();
                 

                    return Ok(new ArrayList { trans, count, TheSum});
                }
                return Unauthorized();
            }
          
        }
      



        [HttpGet]
        [Route("api/v1/fixInterest")]
        public IActionResult fixInterest([FromRoute]int offset, [FromRoute]int limit)
        {
            var Trans = _context.Transactions;
            foreach (var item in Trans)
            {
                var Userpercent = _context.Users.Where(x => x.UserModelId == item.UserId).FirstOrDefault().Percentage;
                item.ResellerPercent = Userpercent;
                _context.Entry(item).State = EntityState.Modified;

            }
            _context.SaveChanges();


            return Ok(Trans.Count());
        }
        [HttpGet]
        [Route("api/v1/fixBalance")]
        public IActionResult FixBalance()
        {
            var Users = _context.Users.ToList();
            foreach(var item in Users)
            {
                var trans = _context.Transactions.Where(x => x.UserId == item.UserModelId && x.value != null && x.value != "").Sum(x=>x.token);
                item.Balance = 0 - trans;
                _context.Entry(item).State = EntityState.Modified;
        }
            _context.SaveChanges();
            return Ok();
        }
           

        [HttpGet]
        [Route("api/v2/Transaction/Clear")]
        public IActionResult Clear()
        {
            var transData = _context.Transactions.ToList();
            foreach(var item in transData)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }
            _context.SaveChanges();
            return Ok();
        }
       

        [HttpGet]
        [Authorize]
        [Route("api/v1/Transaction/{Id}")]
        public IActionResult Transaction([FromRoute] string Id)
        {
            var RoleId = User.FindFirstValue("Role");
            if (RoleId == null)
                return BadRequest("User Role is Missing"); //Change this later
            bool checker = isPermitted("GET_TRANSACTION", RoleId);
            if (checker == false)
                return Unauthorized();
            return Ok(_context.Transactions.Where(x => x.TransactionId == Id).FirstOrDefault());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/Transaction/ByMeterNumber/{CustomerMeterId}")]
        public IActionResult TransByMeterNumber([FromRoute] string CustomerMeterId)
        {
            var RoleId = User.FindFirstValue("Role");
            var perm = _context.Roles.Where(x => x.RoleModelId == RoleId).Select(x => x.Permissions).FirstOrDefault().Split(',');
            var UserId = User.FindFirstValue("Id");
            if (RoleId == null)
                return BadRequest("User's Role is Missing");
            bool checker = isPermitted("GET_TRANSACTION", RoleId);
            if (checker == false)
                return Unauthorized();
            if (perm.Contains("ALL"))
            {
                return Ok(_context.Transactions.Where(x => x.CustomerMeterID == CustomerMeterId).FirstOrDefault());
            }
            else
            {
                return Ok(_context.Transactions.Where(x => x.CustomerMeterID == CustomerMeterId && x.UserId == UserId).FirstOrDefault());
            }
            
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/Transaction/Filter")]
        public IActionResult FilterTrans([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var RoleId = User.FindFirstValue("Role");
            var UserId = User.FindFirstValue("Id");
            var perm = _context.Roles.Where(x => x.RoleModelId == RoleId).Select(x => x.Permissions).FirstOrDefault().Split(',');
            if (RoleId == null)
                return BadRequest("User's Role is Missing");
            bool checker = isPermitted("GET_TRANSACTION", RoleId);
            if (checker == false)
                return Unauthorized();
            if (perm.Contains("ALL"))
            {
                return Ok(_context.Transactions.Where(x => x.Date >= StartDate && x.Date <= EndDate).ToList());
            }
            else
            {
                return Ok(_context.Transactions.Where(x => x.Date >= StartDate && x.Date <= EndDate && x.UserId == UserId).ToList());
            }
            
        }
      

        [HttpGet]
        [Authorize]
        [Route("api/v1/Customer/transactions")]
        public IActionResult CusTrans([FromQuery]string CustId, [FromQuery]int offset = 0, [FromQuery]int limit = 10)
        {
            var cust = _context.Customers.Where(x => x.CustomerId == CustId).FirstOrDefault().CustomerMeterId;
            var transactions = _context.Transactions.Where(x => x.CustomerMeterID == cust).ToList().Skip(offset).Take(limit);
            return Ok(transactions);
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/Permissions")]
        public IActionResult permissions()
        {
            return Ok(_context.Permissions.ToList());
        }
        [HttpGet]
        [Authorize]
        [Route("api/v1/DashboardData")]
        public IActionResult Dashboard()
        {
            var UserId = User.FindFirstValue("Id");
            var RoleId = User.FindFirstValue("Role");
            if (isPermitted("ALL", RoleId))
            {
                var MonthTotalSuccess = _context.Transactions.Where(x => x.tokenString != null && x.Date.Value.Date.Month == DateTime.Now.Date.Month).ToList();
                if (MonthTotalSuccess == null)
                    return Ok();
                var MonthAll = _context.Transactions.Where(q=>q.Date.Value.Month == DateTime.Now.Month).Count();
                var MonthPrepaidAll = _context.Transactions.Where(q => q.Date.Value.Date.Month == DateTime.Now.Date.Month && q.AccountType ==1).Count();
                var MonthPostpaidAll = _context.Transactions.Where(q => q.Date.Value.Date.Month == DateTime.Now.Date.Month && q.AccountType == 2).Count();
                if (MonthPrepaidAll == 0)
                    MonthPrepaidAll = 1;
                if (MonthAll == 0)
                    MonthAll = 1;
                if (MonthPostpaidAll == 0)
                    MonthPostpaidAll = 1;
                var MonthRate = (MonthTotalSuccess.Count()/MonthAll)*100;
                var MonthValue = MonthTotalSuccess.Select(x => x.token).Sum();

             
                

                var MonthPrePaidVolume = MonthTotalSuccess.Where(x => x.AccountType == 1).Count();
                var MonthPrepaidRate = (MonthPrePaidVolume / MonthPrepaidAll) * 100;
                var MonthPrepaidValue = MonthTotalSuccess.Where(x => x.AccountType == 1).Select(a => a.token).Sum();
                var MonthPostPaidVolume = MonthTotalSuccess.Where(x => x.AccountType == 2).Count();
                var MonthPostpaidRate = (MonthPostPaidVolume/ MonthPostpaidAll) * 100;
                var MonthPostpaidValue = MonthTotalSuccess.Where(x => x.AccountType == 2).Select(a => a.token).Sum();


                var TodayTotalSuccess = _context.Transactions.Where(x => x.tokenString != null && x.Date.Value.Date == DateTime.Today.Date).ToList();
                if (TodayTotalSuccess == null)
                    return Ok();
                var TodayAll = _context.Transactions.Where(x => x.Date.Value.Day == DateTime.Today.Day).Count();
                var TodayPrepaidAll = _context.Transactions.Where(q => q.Date.Value.Date == DateTime.Today.Date && q.AccountType == 1).Count();
                var TodayPostPaidAll = _context.Transactions.Where(q => q.Date.Value.Date == DateTime.Today.Date && q.AccountType == 2).Count();

                if (TodayAll == 0)
                    TodayAll = 1;
                if (TodayPrepaidAll == 0)
                    TodayPrepaidAll = 1;
                if (TodayPostPaidAll == 0)
                    TodayPostPaidAll = 1;


                var TodayRate = (TodayTotalSuccess.Count() / TodayAll) * 100;
                var TodayPrePaidVolume = TodayTotalSuccess.Where(x => x.AccountType == 1).Count();
                var TodayPrepaidRate = (TodayPrePaidVolume / TodayPrepaidAll) * 100;
                var TodayPrepaidValue = TodayTotalSuccess.Where(x => x.AccountType == 1).Select(a => a.token).Sum();
                var TodayPostPaidVolume = TodayTotalSuccess.Where(x => x.AccountType == 2).Count();
                var TodayPostpaidRate = (TodayPostPaidVolume / TodayPostPaidAll) * 100;
                var TodayPostpaidValue = TodayTotalSuccess.Where(x => x.AccountType == 2).Select(a => a.token).Sum();
                var TodayValue = TodayTotalSuccess.Select(x => x.token).Sum();

                var result = new DasboardValues
                {

                    Month = new MonthData { Rate = MonthRate, Volume = MonthTotalSuccess.Count(), Value = MonthValue, PrepaidRate = MonthPrepaidRate, PrepaidVolume = MonthPrePaidVolume, PostRate = MonthPostpaidRate, PostVolume = MonthPostPaidVolume, PrepaidValue = MonthPrepaidValue, PostValue = MonthPostpaidValue },
                    Today = new MonthData { Rate = TodayRate, Volume = TodayTotalSuccess.Count(), Value = TodayValue, PrepaidRate = TodayPrepaidRate, PrepaidVolume = TodayPrePaidVolume, PostRate = TodayPostpaidRate, PostVolume = TodayPostPaidVolume, PrepaidValue = TodayPrepaidValue, PostValue = TodayPostpaidValue }
                };


                return Ok(result);
            }
            else if(isPermitted("GET_TRANSACTION", RoleId))
            {
                var MonthTotalSuccess = _context.Transactions.Where(x =>x.UserId == UserId && x.tokenString != null && x.Date.Value.Date.Month == DateTime.Now.Date.Month).ToList();
                if (MonthTotalSuccess == null)
                    return Ok();
                var MonthAll = _context.Transactions.Where(q => q.Date.Value.Month == DateTime.Now.Month).Count();
                var MonthPrepaidAll = _context.Transactions.Where(q => q.Date.Value.Date.Month == DateTime.Now.Date.Month && q.AccountType == 1).Count();
                var MonthPostpaidAll = _context.Transactions.Where(q => q.Date.Value.Date.Month == DateTime.Now.Date.Month && q.AccountType == 2).Count();

                var MonthRate = (MonthTotalSuccess.Count() / MonthAll) * 100;
                var MonthValue = MonthTotalSuccess.Select(x => x.token).Sum();

                if (MonthPrepaidAll == 0)
                    MonthPrepaidAll = 1;
                if (MonthAll == 0)
                    MonthAll = 1;
                if (MonthPostpaidAll == 0)
                    MonthPostpaidAll = 1;


                var MonthPrePaidVolume = MonthTotalSuccess.Where(x => x.AccountType == 1).Count();
                var MonthPrepaidRate = (MonthPrePaidVolume / MonthPrepaidAll) * 100;
                var MonthPrepaidValue = MonthTotalSuccess.Where(x => x.AccountType == 1).Select(a => a.token).Sum();
                var MonthPostPaidVolume = MonthTotalSuccess.Where(x => x.AccountType == 2).Count();
                var MonthPostpaidRate = (MonthPostPaidVolume / MonthPostpaidAll) * 100;
                var MonthPostpaidValue = MonthTotalSuccess.Where(x => x.AccountType == 2).Select(a => a.token).Sum();


                var TodayTotalSuccess = _context.Transactions.Where(x =>x.UserId == UserId && x.tokenString != null && x.Date.Value.Date == DateTime.Today.Date).ToList();
                if (TodayTotalSuccess == null)
                    return Ok();
                var TodayAll = _context.Transactions.Where(x => x.Date.Value.Date == DateTime.Today.Date).Count();
                var TodayPrepaidAll = _context.Transactions.Where(q => q.Date.Value.Date == DateTime.Today.Date && q.AccountType == 1).Count();
                var TodayPostPaidAll = _context.Transactions.Where(q => q.Date.Value.Date == DateTime.Today.Date && q.AccountType == 2).Count();

                if (TodayAll == 0)
                    TodayAll = 1;
                if (TodayPrepaidAll == 0)
                    TodayPrepaidAll = 1;
                if (TodayPostPaidAll == 0)
                    TodayPostPaidAll = 1;


                var TodayRate = (TodayTotalSuccess.Count() / TodayAll) * 100;
                var TodayPrePaidVolume = TodayTotalSuccess.Where(x => x.AccountType == 1).Count();
                var TodayPrepaidRate = (TodayPrePaidVolume / TodayPrepaidAll) * 100;
                var TodayPrepaidValue = TodayTotalSuccess.Where(x => x.AccountType == 1).Select(a => a.token).Sum();
                var TodayPostPaidVolume = TodayTotalSuccess.Where(x => x.AccountType == 2).Count();
                var TodayPostpaidRate = (TodayPostPaidVolume / TodayPostPaidAll) * 100;
                var TodayPostpaidValue = TodayTotalSuccess.Where(x => x.AccountType == 2).Select(a => a.token).Sum();
                var TodayValue = TodayTotalSuccess.Select(x => x.token).Sum();

                var result = new DasboardValues
                {

                    Month = new MonthData { Rate = MonthRate, Volume = MonthTotalSuccess.Count(), Value = MonthValue, PrepaidRate = MonthPrepaidRate, PrepaidVolume = MonthPrePaidVolume, PostRate = MonthPostpaidRate, PostVolume = MonthPostPaidVolume, PrepaidValue = MonthPrepaidValue, PostValue = MonthPostpaidValue },
                    Today = new MonthData { Rate = TodayRate, Volume = TodayTotalSuccess.Count(), Value = TodayValue, PrepaidRate = TodayPrepaidRate, PrepaidVolume = TodayPrePaidVolume, PostRate = TodayPostpaidRate, PostVolume = TodayPostPaidVolume, PrepaidValue = TodayPrepaidValue, PostValue = TodayPostpaidValue },
                    Balance = _context.Users.Where(x => x.UserModelId == UserId).FirstOrDefault().Balance
                    
                };


                return Ok(result);
            }
            return Unauthorized();
        }








        public class MonthData
        {
            public int Rate { get; set; }
            public int Volume { get; set; }
            public double Value { get; set; }
            public int PrepaidRate { get; set; }
            public int PrepaidVolume { get; set; }
            public double PrepaidValue { get; set; }
            public int PostRate { get; set; }
            public int PostVolume { get; set; }
            public double PostValue { get; set; }

        }

        public class DasboardValues
        {
            public MonthData Month { get; set; }
            public MonthData Today { get; set; }
            public double Balance { get; set; }
        }


        public class RoleOut
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            public string[] Permissions { get; set; } 
        }





        private string GenerateJwtToken(string email, UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.FullName),
                new Claim("Role",user.RoleId),
                new Claim("Id",user.UserModelId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(config.Value.AccessExpiration));

            var token = new JwtSecurityToken(config.Value.Issuer, config.Value.Audience, claims, expires: expires, signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}