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
        private static string DefaultComments = "Ви можете задати запитання автору щодо замовлення. Ми будемо вдячні якщо ви вкажете додаткові кантактні дані для отримання товару поштою чи для обговорення деталей подальшої співпраці з автором.";



        public OrderController()
        {
            OrdersRepository = new Repository<Order>();
            EmployeeRepository = new Repository<Employee>();
            ProductsRepository = new Repository<Product>();
            UserRepository = new Repository<User>();
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
                Comments = DefaultComments
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
            mail.Subject = "Order from " + order.Email;
            mail.Body = @"Your product " + order.Product.Name + " was ordered. Please, contact your customer."
                + "\\r\\n Customer email: " + order.Email+ " on "+order.ProcessDateTime;
            if (order.Comments != DefaultComments)
            {
                mail.Body = mail.Body + "Customer added Comments." + order.Comments;
            }
            SmtpServer.Send(mail);
        }
        public class OrderMakerModel
        {
            public int OrderId { get; set; }
            public string Comments { get; set; }
            public string Email { get; set; }
        }
        // GET: /Product/MakeOrder
        [HttpPost]
        public ActionResult MakeOrder(OrderMakerModel model)
        {
            Order order = OrdersRepository.Get(s => s.Id.Equals(model.OrderId)).SingleOrDefault();
            order.Comments = model.Comments;
            User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            string success = "Про ваше замовлення :" + order.Product.Name
                 + " повідомлено автора. Скоро з вами сконтактуються.";
            string fail = "Нажаль спроба замовлення " + order.Product.Name
                 + "невдала.";
            BascetModel bascetModel = new BascetModel();

            if (order != null && IsValid(user.Email))
            {
                order.ProcessDateTime = DateTime.Now.ToString();
                order.Status = "Ordered";
                order = OrdersRepository.SaveOrUpdate(order);
                NotificateEmployee(order);
                user.DeleteOrder(order);
                user.OrdersHistory.Add(order);
                UserRepository.SaveOrUpdate(user);



                return Json("\\Order\\OrderMaked?message=" + success);

            }
            else
            {
                return Json("\\Order\\OrderMaked?message=" + fail);
            }
        }

        public ActionResult OrderMaked(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        private bool IsValid(string emailaddress)
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
                User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
                Order order = OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
                string success = "Успішно видалено " + order.Product.Name + " з кошика";

                if (order != null)
                {
                    order.ProcessDateTime = DateTime.Now.ToString();
                    order.Status = "Ordered";
                    order = OrdersRepository.SaveOrUpdate(order);
                    user.DeleteOrder(order);
                    UserRepository.SaveOrUpdate(user);
                }
                ViewBag.Message = success;
                return View("OrderMaked");

            }
            catch
            {
                ViewBag.Message = "Не вдалось видалити товар з корзини";
                return View("OrderMaked");

            }

        }



    }
}
