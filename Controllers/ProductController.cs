using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TolokaStudio.Entities;
using TolokaStudio.Models;
using TolokaStudio.Repository.Interfaces;
using TolokaStudio.Repository;

namespace TolokaStudio.Controllers
{
    [ValidateInput(false)]
    public class ProductController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
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
                return View(CreateCreateModel(id));
            }
            catch
            {
                ProductCreateModel productCreateModel = CreateEmptyCreateModel(id);
                return View(productCreateModel);
            }
        }

        private ProductCreateModel CreateCreateModel(int id)
        {
            Store Store = StoreRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            ViewBag.Employees = Store.Staff;

            ProductCreateModel productCreateModel = new ProductCreateModel()
            {
                HtmlBanner = string.Format(_productBennerTemplate, '0', DefaulImg, "Назва товару", "Ціна грн"),
                HtmlDetail = string.Format(_productDetailTemplate, DefaulDetailImg),
                ImagePath = DefaulImg,
                Name = "Назва товару",
                Price = 100,
                StoreID = id,
                EmployeeId = Store.Staff.FirstOrDefault().Id
            };
            return productCreateModel;
        }

        private ProductCreateModel CreateEmptyCreateModel(int id)
        {
            ProductCreateModel productCreateModel = new ProductCreateModel()
            {
                HtmlBanner = string.Format(_productBennerTemplate, '0', DefaulImg, "Назва товару", "Ціна грн"),
                HtmlDetail = string.Format(_productDetailTemplate, DefaulDetailImg),
                ImagePath = DefaulImg,
                Name = "Назва товару",
                Price = 100,
                StoreID = id
            };
            return productCreateModel;
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

                    CreateProduct(model);

                    return RedirectToAction("Edit", "Store", new { id = model.StoreID });
                }
                catch
                {
                    return View(model);
                }
            }
            else
            {
                model.HtmlBanner = Server.HtmlEncode(string.Format(_productBennerTemplate, 0, model.ImagePath, model.Name, model.Price + " грн."));
                return View(model);
            }
        }

        private void CreateProduct(ProductCreateModel model)
        {
            Store store = StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();
            Product product = new Product() { Name = model.Name, Price = model.Price, ImagePath = model.ImagePath, HtmlDetail = Server.HtmlEncode(model.HtmlDetail) };
            product.OwnerEmployee = store.Staff.Where(e => e.Id == model.EmployeeId).SingleOrDefault();
            store.AddProduct(product);
            StoreRepository.SaveOrUpdate(store);

            //ID  Is received

            ViewBag.Employees = store.Staff;
            Product product1 = ProductsRepository.Get(p => p.Name == product.Name && p.Price == product.Price && p.ImagePath == product.ImagePath).FirstOrDefault();
            CreateHtml(product1);
          
            ProductsRepository.SaveOrUpdate(product1);
            SaveTemplate(model);
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

