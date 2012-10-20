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
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;
        private const string DefaulImg = "/Content/img/imgThumbs/Fluor/Coffe.png";
        private const string DefaulDetailImg = "/Content/img/imgFull/Fluor/Coffe.png";
        private const string _productBennerTemplate = "<div class='template'>" +
                   " <div class='span8'>" +
                   " <a href='/Product/Details?id={0}'>" +
                   " <div class='box_main_item'>" +
                   " <div class='box_main_item_img'>" +
                   "  <div class='box_main_item_img_bg'>" +
                   "     <span>Детальніше</span>" +
                   "  </div>" +
                   " <img src='{1}' />" +
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
        private const string _productOrderBennerTemplate = "<div class='template'>" +
                  " <div class='span8'>" +
                  " <a href='/Order/Create?id={0}'>" +
                  " <div class='box_main_item'>" +
                  " <div class='box_main_item_img'>" +
                  "  <div class='box_main_item_img_bg'>" +
                  "     <span>Замовити</span>" +
                  "  </div>" +
                  " <img src='{1}' />" +
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
        private const string _productDetailTemplate =
             "<div class='span24'>" +
       " <img src='{0}' /></div>" +

         " </div>";

        public ProductController()
        {
            StoreRepository = new Repository<Store>();
            WebTemplateRepository = new Repository<WebTemplate>();
            ProductsRepository = new Repository<Product>();
            EmployeeRepository = new Repository<Employee>();
            UserRepository = new Repository<User>();
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
            try
            {
                return View(CreateProductModel(id));
            }
            catch
            {
                return View();
            }
        }

        private ProductCreateModel CreateProductModel(int id)
        {
            List<Store> stores = StoreRepository.GetAll().ToList();

            ProductCreateModel productCreateModel = new ProductCreateModel()
            {
                HtmlBanner = string.Format(_productBennerTemplate, '0', DefaulImg, "Назва товару", "Ціна грн"),
                HtmlDetail = string.Format(_productDetailTemplate, DefaulDetailImg),
                ImagePath = DefaulImg,
                Name = "Назва товару",
                Price = 100,
                EmployeeId = id,
                Stores = stores,
                StoreID = stores.FirstOrDefault().Id
            };
            return productCreateModel;
        }


        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(ProductCreateModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    CreateProduct(model);
                    return RedirectToAction("Edit", "Employee");
                }
                catch
                {
                    return View(model);
                }
            }
            else
            {
                List<Store> stores = StoreRepository.GetAll().ToList();

                ProductCreateModel productCreateModel = new ProductCreateModel()
                {
                    HtmlBanner = string.Format(_productBennerTemplate, '0', DefaulImg, "Назва товару", "Ціна грн"),
                    HtmlDetail = string.Format(_productDetailTemplate, DefaulDetailImg),
                    ImagePath = DefaulImg,
                    Name = "Назва товару",
                    Price = 100,
                    EmployeeId = model.EmployeeId,
                    Stores = stores,
                    StoreID = stores.FirstOrDefault().Id
                };
                return View(productCreateModel);
            }
        }

        private void CreateProduct(ProductCreateModel model)
        {

            Product product = new Product() { Name = model.Name, Price = model.Price, ImagePath = model.ImagePath, HtmlDetail = Server.HtmlEncode(model.HtmlDetail) };
            product.OwnerEmployee = EmployeeRepository.Get(e => e.Id.Equals(model.EmployeeId)).SingleOrDefault();
            product.Store=StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();
            Product productSaved = ProductsRepository.SaveOrUpdate(product);
            CreateHtml(productSaved);
            ProductsRepository.SaveOrUpdate(productSaved);

        }

        private void CreateHtml(Product product)
        {
            product.HtmlBanner = Server.HtmlEncode(string.Format(_productBennerTemplate, product.Id, product.ImagePath,
                product.Name, product.Price + " грн."));
            if (product.OwnerEmployee != null)
            {
                product.HtmlDetail = Server.HtmlEncode(string.Format(_productOrderBennerTemplate,
                    product.Id, product.ImagePath, product.Name, product.Price + " грн.")) +
                    product.OwnerEmployee.HtmlBanner + product.HtmlDetail;
            }
            else
            {
                product.HtmlDetail = Server.HtmlEncode(string.Format(_productOrderBennerTemplate, product.Id,
                    product.ImagePath, product.Name, product.Price + " грн.")) + product.HtmlDetail;
            }
        }

        private void SaveTemplate(ProductCreateModel model)
        {

            WebTemplate item = new WebTemplate();
            item.Html = model.HtmlDetail;
            item.Name = model.Name;
            WebTemplateRepository.SaveOrUpdate(item);
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
            ProductEditModel ProductEditModel = ProductToEditModel(id);
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
            product.HtmlBanner = Server.HtmlEncode(string.Format(_productBennerTemplate, productEditModel.Id, productEditModel.ImagePath, productEditModel.Name, productEditModel.Price + " грн."));
            product.HtmlDetail = Server.HtmlEncode(productEditModel.HtmlDetail);
            return product;
        }

        private ProductEditModel ProductToEditModel(int id)
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

