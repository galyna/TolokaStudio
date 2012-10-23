using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TolokaStudio.Common;
using Core.Data.Repository.Interfaces;
using Core.Data.Entities;
using Core.Data.Repository;
using TolokaStudio.Models;
using System.Web.Security;

namespace TolokaStudio.Controllers
{

    public class BascetController : Controller
    {
        private readonly IRepository<User> UserRepository;

        public BascetController()
        {
            UserRepository = new Repository<User>();
        }

        public ActionResult Index(string message)
        {
            BascetModel bascetModel = new BascetModel();
            var currentUser = base.ControllerContext.HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(currentUser))
            {

                User user = UserRepository.Get(u => u.UserName == currentUser).SingleOrDefault();
                if (user == null)
                {
                    int count = UserRepository.GetAll().Count();
                    User userNew = new User();
                    userNew.UserName = "Гість" + count;
                    userNew.Email = "Гість" + count;
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(userNew.Email, false/* createPersistentCookie */);
                    user = UserRepository.SaveOrUpdate(userNew);
                    user.Orders = new List<Order>();
                }

                bascetModel.Message = message;
                bascetModel.Orders = user.Orders;
               
                return View(bascetModel);
            }

            return RedirectToAction("Index", "Product");
        }

        public ActionResult Add(int id)
        {
            return RedirectToAction("Create", "Order", new { id = id });

        }

    }
}
