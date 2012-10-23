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

namespace TolokaStudio.Controllers
{
    [TolokaAuthorizeAsSimpleUserAttribute]
    public class BascetController : Controller
    {
        private readonly IRepository<User> UserRepository;


        public BascetController()
        {
            UserRepository = new Repository<User>();
        }
       
        public ActionResult Index(string message)
        {
            var currentUser = base.ControllerContext.HttpContext.User.Identity.Name;

            User user = UserRepository.Get(u => u.UserName == currentUser).SingleOrDefault();
            BascetModel bascetModel = new BascetModel();
            bascetModel.Orders = user.Orders;
            bascetModel.Message = message != null ? message : "";
            return View("Index", bascetModel);
        }

        public ActionResult Add(int id)
        {
            return RedirectToAction("Create", "Order", new { id = id });
          
        }
       
    }
}
