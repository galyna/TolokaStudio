using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;
using TolokaStudio.Models;
using Core.Data.Entities;


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
           
            return View(new HomeModel() { Stores = StoreRepository.GetAll().ToList()});
        }

    }
}
