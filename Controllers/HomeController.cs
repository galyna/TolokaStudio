using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;
using SumkaWeb.Models;
using Core.Data.Entities;


namespace SumkaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        
        // Constructs our home controller
        public HomeController()
        {
            StoreRepository = new Repository<Store>();
        }

        private void Seed()
        {
            // Create a couple of Stores each with some Products and Employees
            var barginBasin = new Store { Name = "Bargin Basin" };
            var superMart = new Store { Name = "SuperMart" };

            var potatoes = new Product { Name = "Potatoes", Price = 3.60 };
            var fish = new Product { Name = "Fish", Price = 4.49 };
            var milk = new Product { Name = "Milk", Price = 0.79 };
            var bread = new Product { Name = "Bread", Price = 1.29 };
            var cheese = new Product { Name = "Cheese", Price = 2.10 };
            var waffles = new Product { Name = "Waffles", Price = 2.41 };

            var daisy = new Employee { FirstName = "Daisy", LastName = "Harrison" };
            var jack = new Employee { FirstName = "Jack", LastName = "Torrance" };
            var sue = new Employee { FirstName = "Sue", LastName = "Walkters" };
            var bill = new Employee { FirstName = "Bill", LastName = "Taft" };
            var joan = new Employee { FirstName = "Joan", LastName = "Pope" };

            // Add Products to the Stores
            // The Store-Product relationship is many-to-many
            AddProductsToStore(barginBasin, potatoes, fish, milk, bread, cheese);
            AddProductsToStore(superMart, bread, cheese, waffles);

            // Add Employees to the Stores
            // The Store-Employee relationship is one-to-many
            AddEmployeesToStore(barginBasin, daisy, jack, sue);
            AddEmployeesToStore(superMart, bill, joan);

            StoreRepository.SaveOrUpdateAll(barginBasin, superMart);
        }

        public ActionResult Index()
        {
            //Seed();
            return View(new HomeModel() { Stores = StoreRepository.GetAll().ToList()});
        }

        // Adds any products that we pass in to the store that we pass in
        public static void AddProductsToStore(Store store, params Product[] products)
        {
            foreach (var product in products)
            {
                store.AddProduct(product);
            }
        }

        // Adds any employees that we pass in to the store that we pass in
        public static void AddEmployeesToStore(Store store, params Employee[] employees)
        {
            foreach (var employee in employees)
            {
                store.AddEmployee(employee);
            }
        }
    }
}
