using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TolokaStudio;
using TolokaStudio.Repository.Interfaces;
using TolokaStudio.Repository;
using TolokaStudio.Models;
using TolokaStudio.Entities;


namespace TolokaStudio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        
        // Constructs our home controller
        public HomeController()
        {
            StoreRepository = new Repository<Store>();
        }


        public ActionResult Index()
        {
           
               StoreRepository.SaveOrUpdate(new Store(){Name="Test"});
               return View(new HomeModel() { Stores = StoreRepository.GetAll().ToList() });
          
        }

    }
}
