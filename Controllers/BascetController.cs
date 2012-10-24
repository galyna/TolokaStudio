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
                user.Message = message;
                if (user == null)
                {
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    bascetModel.Message = string.IsNullOrEmpty(message) ? message : user.Message; 
                    bascetModel.Orders = user.Orders;
                    bascetModel.User = user;
                    bascetModel.Comments = user.Orders.Any()?user.Orders.LastOrDefault().Comments:"";
                    bascetModel.Order = user.Orders.Any() ? user.Orders.LastOrDefault().Id : 0; 
                    return View(bascetModel);
                }             
            }

            return RedirectToAction("Index", "Product");
        }





        public ActionResult Add(int id)
        {
            return RedirectToAction("Create", "Order", new { id = id });

        }

    }
}
