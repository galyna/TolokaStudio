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

namespace SumkaWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> OrdersRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private const string _orderTemplate = "<div class='template'>" +
                   " <div class='span8'>" +
                   " <a href='/Order/Details?id={0}''>" +
                   " <div class='box_main_item'>" +
                   " <div class='box_main_item_img'>" +
                   "  <div class='box_main_item_img_bg'>" +
                   "     <span>Замовити</span>" +
                   "  </div>" +
                   " <img src='{1}' alt='img_box' />" +
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



        //
        // GET: /Product/Create/5

        public ActionResult Create(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            List<Product> products = new List<Product>();
            products.Add(product);
            OrderCreateModel orderCreateModel = new OrderCreateModel()
            {
                Employee = product.OwnerEmployee,
                Product = product,
                ProductId= product.Id,
                EmployeeId = product.OwnerEmployee.Id
            };

            return View(orderCreateModel);
        }

        //k
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(OrderCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Product product = ProductsRepository.Get(s => s.Id.Equals(model.ProductId)).SingleOrDefault();
                    Employee employee = EmployeeRepository.Get(s => s.Id.Equals(model.EmployeeId)).SingleOrDefault();

                    Order orcder = new Order() { Product =product, Employee = employee,ContactEmail=model.ContactEmail };
                    OrdersRepository.SaveOrUpdate(orcder);

                    NotificateEmployee(orcder);


                    return RedirectToAction("Index", "Order");
                }
                catch
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        private static void NotificateEmployee(Order order)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("mail.infobox.ru");
            SmtpServer.Credentials = new System.Net.NetworkCredential("order@tolokastudio.pp.ua", "atlzatlz1");
            SmtpServer.Port = 25;
            //SmtpServer.EnableSsl = true;

            mail.From = new MailAddress("order@tolokastudio.pp.ua");
            mail.To.Add("verh@yandex.ru");//(order.Employee.Email);
            mail.Subject = "new order";
            mail.Body = "Your product " + order.Product.Name + " was ordered. Please, contact your customer."
                + "\r\nCustomer email: " + order.ContactEmail;

            SmtpServer.Send(mail);
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
