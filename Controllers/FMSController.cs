using FMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FMSController : ControllerBase
    {
        LTIFMSContext context;

        public FMSController(LTIFMSContext _context)
        {
            context = _context;
        }

        [HttpGet]

        public IActionResult GetAll()
        {
            return Ok(context.Registration);

        }
        [HttpPost]
        public IActionResult Adduser(Registration userdata)
        {
            var d = context.Registration.Where(x => x.Email == userdata.Email && x.UserName == userdata.UserName).FirstOrDefault();
            var e = context.Registration.Where(y => y.Email == userdata.Email).FirstOrDefault();
            if (d == null && e==null)
            {
               
                    context.Registration.Add(userdata);
                    context.SaveChanges();
                    return Ok(context.Registration);

                
                
            }
            else
            {
                return BadRequest("username already exist");
            }
        }




        [HttpPost("Userlogin")]
        public IActionResult login(Registration user)
        {

            var c = context.Registration.Where(x => x.UserName == user.UserName && x.Password == user.Password ).FirstOrDefault();
            if (c != null)
            {
                if (c.Status == "Yes")
                {
                    return Ok();

                }
                else
                {
                    return BadRequest("your application is still under processing");
                }
                
            }
            else
            {
                return BadRequest("invalid login!");
            }
        }

        [HttpPut("{username}")]

        public IActionResult GetCust(Registration customer,string username)
        {
            var user=context.Registration.Where(x => x.UserName == username).FirstOrDefault();
            user.Status = customer.Status;
            //user.Name = customer.Name;
            //user.DateOfBirth = customer.DateOfBirth;
            //user.Email = customer.Email;
            //user.Phone = customer.Phone;
            //user.UserName = customer.UserName;
            //user.Password = customer.Password;
            //user.Address = customer.Address;
            //user.CardType = customer.CardType;
            //user.Bank = customer.Bank;
            //user.Account = customer.Account;
            //user.Ifsc = customer.Ifsc;
            //user.Status = customer.Status;
            context.Registration.Update(user);
            context.SaveChanges();
            return Ok(context.Registration);



        }

        [HttpPost("createCustomer")]
        
        public IActionResult createCustomer(Customer customer)
        {
            
            Customer cust= context.Customer.Where(x => x.UserName == customer.UserName).FirstOrDefault();
            var reguser = context.Registration.Where(y => y.UserName == customer.UserName).FirstOrDefault();
            DateTime theDate = DateTime.Now;
            DateTime yearInTheFuture = theDate.AddYears(10);

            if (cust == null && reguser.Status=="Yes")
            {
                cust = new Customer();
                cust.UserName = customer.UserName;
                cust.CardNumber = GenRandom();
                cust.ValidTill = yearInTheFuture;
                cust.Limit = reguser.CardType == "gold" ? 50000 : 100000;
                cust.Balance= reguser.CardType == "gold" ? 50000 : 100000;
                context.Customer.Add(cust);
                context.SaveChanges();
                return Ok(context.Customer);
            }
            else
            {
                return BadRequest();
            }



            
        }
        [NonAction]

        public long GenRandom()
        {

            Random rnd = new Random();
            long R;

            
            R=rnd.Next();
            

            return R;


        }

        [HttpPost("Adminlogin")]
        public IActionResult Alogin(Admin user)
        {
            var c = context.Admin.Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefault();
            if (c != null)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

      

        [HttpGet("{UserName}")]
        public IActionResult GetByName(string UserName)
        {
            return Ok(context.Registration.Where(x => x.UserName == UserName).FirstOrDefault());

        }

        [HttpGet("GetDetails/{UserName}")]
        
        public IActionResult GetDetails(string UserName)
        {
            return Ok(context.Customer.Where(x => x.UserName == UserName).FirstOrDefault());

        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            return Ok(context.Products);
        }

        [HttpGet]
        [Route("GetByProductId/{productId}")]

        public IActionResult Get(int productId) { 
            return Ok(context.Products.Where(x => x.ProductId == productId).FirstOrDefault());

        }

        [HttpPost]
        [Route("PlaceOrder/{prodId}/{username}/{emi}")]
        public IActionResult PlaceOrder(int prodId,string username,int emi)
        {
            var custId = context.Customer.Where(x => x.UserName == username).FirstOrDefault();
            var Prod = context.Products.Where(y => y.ProductId == prodId).FirstOrDefault();
            if (Prod.Cost <= custId.Balance)
            {
                DateTime theDate = DateTime.Now;
                decimal PFee = (Prod.Cost) * Convert.ToDecimal(0.02);
                Orders order = new Orders();
                order.ProductName = Prod.ProductName;
                order.CustomerId = custId.CustomerId;
                order.ProductId = prodId;
                order.PurchasedDate = theDate;
                order.EmiTenure = emi;
                order.MonthlyEmi = (Prod.Cost) / emi;
                order.ProcessingFee = decimal.ToDouble(PFee);
                order.EmiPaid = 0;
                order.AmountPaid = 0;
                context.Orders.Add(order);
                context.SaveChanges();



                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("updatebalance/{prodId}/{username}")]
        
        public IActionResult updatebalance(int prodId,string username)
        {
            var prod = context.Products.Where(x => x.ProductId == prodId).FirstOrDefault();
            var user = context.Customer.Where(y => y.UserName == username).FirstOrDefault();
            
            var cost = prod.Cost;
            
                user.Balance = user.Balance - cost;
                context.Customer.Update(user);
                context.SaveChanges();
            return Ok();



             

           

        }

        [HttpGet("GetOrderByUserName/{username}")]
        public IActionResult GetOrdersDetails(string username)
        {
            var user = context.Customer.Where(x => x.UserName == username).FirstOrDefault();
            var order = context.Orders.Where(y => y.CustomerId == user.CustomerId);

            return Ok(order);
        }

        [HttpPut]
        [Route("updateOrder/{OrderId}/{UserName}")]
        public IActionResult updateorderdetails(int OrderId,string UserName)
        {
            var order = context.Orders.Where(x => x.OrderId == OrderId).FirstOrDefault();
            //var productname = context.Products.Where(x => x.ProductId == order.ProductId).FirstOrDefault();
            order.EmiPaid += 1;
            if (order.EmiPaid <= order.EmiTenure +1)
            {
                
                order.AmountPaid = Convert.ToDouble(order.EmiPaid) * Convert.ToDouble(order.MonthlyEmi);
                var customer = context.Customer.Where(y => y.UserName == UserName).FirstOrDefault();
                customer.Balance = customer.Balance + Convert.ToDecimal(order.MonthlyEmi);
                
                context.Orders.Update(order);
                context.Customer.Update(customer);
                context.SaveChanges();
                

                return Ok();
            }
            
            else
            {
                return BadRequest("you have already paid all emi's");
            }

            
        }


        [HttpPost("forgotpassword")]
        public IActionResult ForgotUserlogin(Registration userDetails)
        {
            string tomail = userDetails.Email;
            string subject = "Your OTP";
            //_context.UserDetails.Add(userDetails);
            var res = context.Registration.Where(x => x.Email == userDetails.Email).FirstOrDefault();

            if (res != null)
            {
                var random = new Random();
                int code = random.Next(1000, 9999);
                string Body = "hi heres your OTP as per your request for re-setting password " + code;
                //codeget = code;
                SendMail("sush200003@gmail.com", tomail, subject, Body);
                //status.Add("Success", true);
                // return Ok(status);
                return Ok(new { status = true, rnum = code });
            }
            else
            {
                //status.Add("Success", false);
                // codeget = -1;
                //return Ok(status);
                return Ok(new { status = false, rnum = -1 });
            }
            // return CreatedAtAction("GetUserDetails", new { id = userDetails.UserId }, userDetails);
        }
        [NonAction]
        public static void SendMail(string from, string To, String Subject, string Body)
        {
            MailMessage mail = new MailMessage(from, To);
            mail.Subject = Subject;
            mail.Body = Body;

            //Attachment attachment = new Attachment(@"");
            //mail.Attachments.Add(attachment);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

            client.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "sush200003@gmail.com",
                Password = "happinessinfinity"

            };
            client.EnableSsl = true;
            client.Send(mail);

        }














    }
}
