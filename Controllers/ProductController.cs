using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data.Entities;
using TolokaStudio.Models;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;

namespace TolokaStudio.Controllers
{
    [ValidateInput(false)]
    public class ProductController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Product> ProductsRepository;
        private const string _productTemplate = "<div class='template'>" +
                   " <div class='span8'>" +
                   " <a href='/Product/Details?id={0}''>" +
                   " <div class='box_main_item'>" +
                   " <div class='box_main_item_img'>" +
                   "  <div class='box_main_item_img_bg'>" +
                   "     <span>Детальніше</span>" +
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
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

            return View(product);
        }



        //
        // GET: /Product/Create/5

        public ActionResult Create(int id)
        {
            ProductCreateModel productCreateModel = new ProductCreateModel()
            {
                HtmlBanner = string.Format(_productTemplate, '0', "/Content/img/Podushka.png", "Назва товару", "Ціна грн"),
                StoreID = id
            };

            return View(productCreateModel);
        }

        //k
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(ProductCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Store store = StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();
                    Product product0 = new Product() { Name = model.Name, Price = model.Price };

                    ProductsRepository.SaveOrUpdate(product0);
                    product0.HtmlBanner = Server.HtmlEncode(string.Format(_productTemplate, product0.Id, model.ImagePath, model.Name, model.Price + " грн."));
                    product0.HtmlDetail = Server.HtmlEncode(model.HtmlDetail);
                    ProductsRepository.SaveOrUpdate(product0);
                    Product product = ProductsRepository.GetAll().Where(p => p.Name == model.Name && p.Price == model.Price).SingleOrDefault();
                    store.AddProduct(product);
                    StoreRepository.SaveOrUpdate(store);
                    return RedirectToAction("Index", "Store");
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
                return RedirectToAction("Index", "Store");
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
            ProductEditModel ProductEditModel = StoreToEditModel(id);
            return View(ProductEditModel);
        }

        

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductEditModel productEditModel)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    Product product = EditModelToProduct(productEditModel);
                    ProductsRepository.SaveOrUpdate(product);
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(productEditModel);
                }
            }
            else
            {
                return View(productEditModel);
            }
        }

        private Product EditModelToProduct(ProductEditModel productEditModel)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(productEditModel.Id)).SingleOrDefault();
            product.Name = productEditModel.Name;
            product.Price = productEditModel.Price;
            product.ImagePath = productEditModel.ImagePath;
            product.HtmlBanner = Server.HtmlEncode(string.Format(_productTemplate, productEditModel.Id, productEditModel.ImagePath, productEditModel.Name, productEditModel.Price + " грн."));
            product.HtmlDetail = Server.HtmlEncode(productEditModel.HtmlDetail);
            return product;
        }

        private ProductEditModel StoreToEditModel(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            ProductEditModel ProductEditModel = new ProductEditModel()
            {
                ImagePath = product.ImagePath,
                Name = product.Name,
                Price = product.Price,
                HtmlBanner = HttpUtility.HtmlDecode(product.HtmlBanner),
                HtmlDetail = HttpUtility.HtmlDecode(product.HtmlDetail),
                Id = id
            };
            return ProductEditModel;
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

