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
using System.Web.Security;
using TolokaStudio.Models;


namespace SumkaWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> OrdersRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;



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


        [HttpPost]
        public ActionResult Create(NewOrder newOrder)
        {

            Product product = ProductsRepository.Get(s => s.Id == newOrder.ProductId).SingleOrDefault();
            if (product != null)
            {
                User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
                if (user == null)
                {
                    return null;
                }
                Order order = AddOrderToBascet(user, product);
                List<int> ids = new List<int>();
                foreach (var item in user.Orders)
                {
                    ids.Add(item.Product.Id);
                }
                return Json(new { Url = Request.UrlReferrer.AbsoluteUri, id = ids.ToArray() });
            }

            return Json(new { Url = Request.UrlReferrer.AbsoluteUri });
        }

        private Order AddOrderToBascet(User user, Product product)
        {

            Order order = new Order()
            {
                Email = !string.IsNullOrEmpty(user.Email) ? user.Email : "galynavistovska@gmail.com",
                User = user,
                Product = product,
                Employee = product.OwnerEmployee,
                Comments = "Ви можете задати запитання автору щодо замовлення. Ми будемо вдячні якщо ви вкажете додаткові кантактні дані для отримання товару поштою чи для обговорення деталей подальшої співпраці з автором."
            };

            order = OrdersRepository.SaveOrUpdate(order);

            user.AddOrder(order);
            UserRepository.SaveOrUpdate(user);

            return order;
        }

        public class NewOrder
        {
            public int ProductId { get; set; }
        }

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
        public class OrderMaker
        {
            public int OrderId { get; set; }
            public string Comments { get; set; }
            public string Email { get; set; }
        }
        // GET: /Product/Create/5
        [HttpPost]
        public ActionResult MakeOrder(OrderMaker model)
        {
            Order order = OrdersRepository.Get(s => s.Id.Equals(model.OrderId)).SingleOrDefault();
            order.Comments = model.Comments;
            User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            string success = "Про ваше замовлення :" + order.Product.Name
                 + " повідомлено автора. Скоро з вами сконтактуються.";
            BascetModel bascetModel = new BascetModel();

            if (order != null && IsValid(user.Email))
            {
                NotificateEmployee(order);
                user.DeleteOrder(order);
                user.Message = success;
                order.OrderDateTime = DateTime.Now.ToString();
                user.OrdersHistory.Add(order);
                UserRepository.SaveOrUpdate(user);
                bascetModel.Message = success;
                bascetModel.Orders = user.Orders;
                return Json("\\Order\\OrderMaked?id=" + order.Id);

            }
            else
            {
                return null;
            }
        }

        public ActionResult OrderMaked(int id)
        {
            string success = "Про ваше замовлення :" + OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault().Product.Name
                   + " повідомлено автора. Скоро з вами сконтактуються.";

            ViewBag.Message = success;
            return View();
        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                Order order = OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
                OrdersRepository.Delete(OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());

                return RedirectToAction("Index", "Bascet", new { message = "Успішно вдалено " + order.Product.Name + " з кошика" });
            }
            catch
            {
                return RedirectToAction("Index", "Bascet", new { message = "" });
            }

        }



    }
}
