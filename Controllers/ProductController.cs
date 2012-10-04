using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data.Entities;
using SumkaWeb.Models;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;

namespace SumkaWeb.Controllers
{
    [ValidateInput(false)]
    public class ProductController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Product> ProductsRepository;
        private const string _productTemplate = "<div class='template'>" +
                   " <div class='span8'>" +
                   " <a href='#'>" +
                   " <div class='box_main_item'>" +
                   " <div class='box_main_item_img'>" +
                   "  <div class='box_main_item_img_bg'>" +
                   "     <span>Замовити</span>" +
                   "  </div>" +
                   " <img src='{0}' alt='img_box' />" +
                   " </div>" +
                   " <div class='box_main_item_text'>" +
                   "   <h3>" +
                   "       {1}</h3>" +
                   "     <span>{2}</span>" +
                   "  </div>" +
                   " </div>" +
                   " </a>" +
                   "</div>" +
                   " </div>";


        public ProductController()
        {
            StoreRepository = new Repository<Store>();
            WebTemplateRepository = new Repository<WebTemplate>();
            ProductsRepository = new Repository<Product>();
        }

        public ActionResult Index()
        {
            IList<Product> products = ProductsRepository.GetAll().ToList();
            return View(products);
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            Product store = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

            return View(store);
        }



        //
        // GET: /Product/Create/5

        public ActionResult Create(int id)
        {
            ProductCreateModel productCreateModel = new ProductCreateModel()
            {

                StoreID = id
            };

            return View(productCreateModel);
        }

        //k
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(ProductCreateModel model)
        {
            try
            {
                Store store = StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();
                var htmlBanner = string.Format(_productTemplate, model.ImagePath, model.Name, model.Price);
                store.AddProduct(new Product() { Name = model.Name, Price = model.Price, HtmlBanner = Server.HtmlEncode(htmlBanner) });
                StoreRepository.SaveOrUpdate(store);

                StoreRepository.SaveOrUpdate(store);

                return RedirectToAction("Index","Store");
            }
            catch
            {
                return View(model);
            }
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            return View(ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
        }

        //
        // POST: /Product/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                ProductsRepository.Delete(ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                return RedirectToAction("Index","Store");
            }
            catch
            {
                return View();
            }
        }
        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            IList<WebTemplate> productTemplates = WebTemplateRepository.GetAll().ToList();

            ProductEditModel productEditModel = new ProductEditModel() { Product = product, ProductTemplates = productTemplates };

            return View(productEditModel);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
                product.Name = collection["Product.Name"];
                product.HtmlBanner = Server.HtmlEncode(collection["Product.HtmlBanner"]);

                ProductsRepository.SaveOrUpdate(product);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // Adds any products that we pass in to the store that we pass in
        public static void AddProductsToStore(Store store, params Product[] products)
        {
            foreach (var product in products)
            {
                store.AddProduct(product);
            }
        }

    }
}

