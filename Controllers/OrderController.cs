using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data.Entities;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;
using SumkaWeb.Models;
using System.Net.Mail;
using TolokaStudio.Common;


namespace SumkaWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> OrdersRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;
 
        private const string DefaulImgBascetChecked = "/Content/img/q/button_ok_4699.png";
        private const string _orderTemplate = "<div class='template ordered{0}' >" +
                 " <div class='span8'>" +
   " <div class='box_main_item'>" +
            " <img src='{4}' />" +
          
                 "  </div>" +
                 " <a href='/Order/Create?id={0}'>" +
                 " <div class='box_main_item'>" +
                 " <div class='box_main_item_img'>" +
                 "  <div class='box_main_item_img_bg'>" +
                 "     <span>Замовити</span>" +

                 "  </div>" +
                 " <img src='{1}' />" +
                 " </div>" +
                 " <div class='box_main_item_text'>" +
                 "   <h3>" +
                 "       {2}</h3>" +
                 "     <span>{3}</span>" +

                 "  </div>" +
                 " </div>" +
                 " </a>" +
                 "</div>" +
                 " </div>";
        private const string _productOrderedBennerTemplateNone = "<div class='template ordered{0}' >" +
                 " <div class='span8'>" +
   " <div class='box_main_item'>" +
            " <img src='{5}' />" +
            //" <img src='{4}' />" +
                 "  </div>" +
                 " <a href='/Order/Create?id={0}'>" +
                 " <div class='box_main_item'>" +
                 " <div class='box_main_item_img'>" +
                 "  <div class='box_main_item_img_bg'>" +
                 "     <span>Замовити</span>" +

                 "  </div>" +
                 " <img src='{1}' />" +
                 " </div>" +
                 " <div class='box_main_item_text'>" +
                 "   <h3>" +
                 "       {2}</h3>" +
                 "     <span>{3}</span>" +

                 "  </div>" +
                 " </div>" +
                 " </a>" +
                 "</div>" +
                 " </div>";


        public OrderController()
        {
            OrdersRepository = new Repository<Order>();
            EmployeeRepository = new Repository<Employee>();
            ProductsRepository = new Repository<Product>();
            UserRepository = new Repository<User>();
        }

        public ActionResult Index()
        {
            IList<Order> orders = OrdersRepository.GetAll().ToList();
            return View(orders);
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

            return View(product);
        }



        [TolokaAuthorizeAsSimpleUserAttribute]
        [HttpPost]
        public ActionResult Create(NewOrder newOrder)
        {
            User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            if (user != null)
            {
                Product product = ProductsRepository.Get(s => s.Id == newOrder.ProductId).SingleOrDefault();
                product.Ordered = true;
                ProductsRepository.SaveOrUpdate(product);
                CreateHtml(product);
                Order orcder = new Order() { Product = product, Employee = product.OwnerEmployee, Comments = "" };
                orcder.User = user;
                OrdersRepository.SaveOrUpdate(orcder);
                return Json("//Product/Index");
            }

            return RedirectToAction("LogOn", "Account");
        }
        private void CreateHtml(Product product)
        {


            product.HtmlBannerOrdered = Server.HtmlEncode(string.Format(_orderTemplate, product.Id, product.ImagePath,
                  product.Name, product.Price + " грн.",  DefaulImgBascetChecked));


        }

        public class NewOrder
        {
            public int ProductId { get; set; }
        }
        //k
        // POST: /Product/Create

        //[HttpPost]
        //public ActionResult Create(OrderCreateModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            Product product = ProductsRepository.Get(s => s.Id.Equals(model.ProductId)).SingleOrDefault();
        //            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(model.EmployeeId)).SingleOrDefault();

        //            Order orcder = new Order() { Product = product, Employee = employee, Comments = model.ContactEmail };
        //            OrdersRepository.SaveOrUpdate(orcder);




        //            return RedirectToAction("Index", "Order");
        //        }
        //        catch
        //        {
        //            return View(model);
        //        }
        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //}

        private static void NotificateEmployee(Order order)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("mail.infobox.ru");
            SmtpServer.Credentials = new System.Net.NetworkCredential("order@tolokastudio.pp.ua", "atlzatlz1");
            SmtpServer.Port = 25;
            //SmtpServer.EnableSsl = true;

            mail.From = new MailAddress("order@tolokastudio.pp.ua");
            mail.To.Add(order.Employee.Email);
            mail.Subject = "new order";
            mail.Body = "Your product " + order.Product.Name + " was ordered. Please, contact your customer."
                + "\r\nCustomer email: " + order.Comments;

            SmtpServer.Send(mail);
        }
        //
        // GET: /Product/Create/5

        public ActionResult MakeOrder(int id, string comments)
        {
            Order order = OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            order.Comments = comments;
            NotificateEmployee(order);
            ViewBag.Messege= "Про ваше замовлення повідомлено автора. Скоро з вами сконтактуються.";
            return RedirectToAction("Index", "Bascet");
        }
        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            return View(OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
        }

        //
        // POST: /Product/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                OrdersRepository.Delete(OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                return RedirectToAction("Index", "Order");
            }
            catch
            {
                return View();
            }
        }


    }
}
