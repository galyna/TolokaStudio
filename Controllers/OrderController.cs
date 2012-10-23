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

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

            return View(product);
        }




        [HttpPost]
        public ActionResult Create(NewOrder newOrder)
        {

            Product product = ProductsRepository.Get(s => s.Id == newOrder.ProductId).SingleOrDefault();
            if (product != null)
            {
                User user = AddToBascet(product);
                List<int> ids = new List<int>();
                foreach (var item in user.Orders)
                {
                    ids.Add(item.Product.Id);
                }
                return Json(new { Url = Request.UrlReferrer.AbsoluteUri, id = ids.ToArray() });
            }

            return Json(new { Url = Request.UrlReferrer.AbsoluteUri });
        }

        private User AddToBascet(Product product)
        {
            User user = CheckUserCanOrder(product);
            Order order = AddOrderToBascet(user, product);

            return user;
        }

        private User CheckUserCanOrder(Product product)
        {
            User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            if (user == null)
            {
                int count = UserRepository.GetAll().Count();
                User userNew = new User();
                userNew.UserName = "Гість" + count;
                userNew.Email = "Гість" + count;
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(userNew.Email, false/* createPersistentCookie */);
                user = UserRepository.SaveOrUpdate(userNew);
            }
            else
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(user.Email, false/* createPersistentCookie */);

            }
            return user;
        }

        private Order AddOrderToBascet(User user, Product product)
        {

            Order order = new Order()
            {
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
        //
        // GET: /Product/Create/5

        public ActionResult MakeOrder(int id, string comments)
        {
            Order order = OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            order.Comments = comments;
            User user = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault();

            if (user != null && order != null)
            {
               

                NotificateEmployee(order);
                string success = "Про ваше замовлення :" + order.Product.Name
                      + " повідомлено автора. Скоро з вами сконтактуються.";
                user.DeleteOrder(order);
                order.OrderDateTime = DateTime.Now.ToString();
                user.OrdersHistory.Add(order);
                UserRepository.SaveOrUpdate(user);
                return RedirectToAction("Index", "Bascet", new { message = success });
            }

            return RedirectToAction("Index", "Bascet", new { message = "Повторіть замовлення" });
        }
        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                Order order = OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
                if (order != null)
                {
                    Product p = order.Product;
                    OrdersRepository.Delete(OrdersRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                    return RedirectToAction("StateBascetDeleted", "Product", new { id = p.Id });
                }


            }
            catch
            {
                return RedirectToAction("Index", "Bascet");
            }
            return RedirectToAction("Index", "Bascet");
        }



    }
}
