using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data.Entities;
using TolokaStudio.Models;
using Core.Data.Repository.Interfaces;
using Core.Data.Repository;
using System.Web.Security;

namespace TolokaStudio.Controllers
{

    public class ProductController : Controller
    {
        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;
        private const string DefaulImgName = "Coffe.png";
        public const string HtmlBannerOrdered = _productOrderedBennerTemplateNone;
        private const string DefaulImg = "/Content/img/imgThumbs/Fluor/" + DefaulImgName;
        private const string DefaulImgBascet = "/Content/img/q/shopping_cart_1.gif";
        private const string DefaulImgBascetChecked = "/Content/img/q/button_ok_4699.png";
        private const string DefaulDetailImg = "/Content/img/imgFull/Fluor/" + DefaulImgName;
        private const string _productBennerTemplate = "<div class='template order{0}'>" +

                   " <div class='span8'>" +
            " <div class='box_main_item'>" +


                  " <img class='orderBtn' title='{0}' src='{4}' />" +
                  "  </div>" +
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
        private const string _productOrderedBennerTemplateNone = "<div class='template ordered{0}' style='diasplay:none;' >" +
                  " <div class='span8'>" +
    " <div class='box_main_item'>" +
             " <img src='{5}' />" +
            //" <img src='{4}' />" +
                  "  </div>" +
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
        private const string _productOrderedBennerTemplate = "<div class='template' style='display:none;'>" +
                " <div class='span8'>" +
  " <div class='box_main_item'>" +
           " <img src='{5}' />" +

                "  </div>" +
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
                 " <img src='{4}' />" +

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
            ProductList model = new ProductList();
            model.Products = products.ToList<Product>();
            return View(model);
        }


       
        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            if (id != null)
            {
                Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

                return View(product);
            }


            return null;
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
                HtmlBanner = string.Format(_productBennerTemplate, '0', DefaulImg, "Назва товару", "Ціна грн", DefaulImgBascet, DefaulImgBascetChecked),
                HtmlDetail = string.Format(_productDetailTemplate, DefaulDetailImg),

                ImagePath = DefaulImg,
                Name = "Назва товару",
                Price = 100,
                EmployeeId = id,
                StoreID = stores.FirstOrDefault().Id
            };
            productCreateModel.staff = new Dictionary<int, string>();
            foreach (var staffType in stores)
            {
                productCreateModel.staff.Add(staffType.Id, staffType.Name);
            }
            return productCreateModel;
        }


        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(ProductCreateModel model)
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

        private void CreateProduct(ProductCreateModel model)
        {

            Product product = new Product() { Name = model.Name, Price = model.Price, ImagePath = model.ImagePath, HtmlDetail = Server.HtmlEncode(model.HtmlDetail) };
            product.OwnerEmployee = EmployeeRepository.Get(e => e.Id.Equals(model.EmployeeId)).SingleOrDefault();
            product.Store = StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();

            Product productSaved = ProductsRepository.SaveOrUpdate(product);
            product.Store.AddProduct(productSaved);

            CreateHtml(productSaved);

            ProductsRepository.SaveOrUpdate(productSaved);

        }

        private void CreateHtml(Product product)
        {
            product.HtmlBanner = Server.HtmlEncode(string.Format(_productBennerTemplate, product.Id, product.ImagePath,
                product.Name, product.Price + " грн.", DefaulImgBascet, DefaulImgBascetChecked));
            product.HtmlBannerOrderedNot = Server.HtmlEncode(string.Format(_productBennerTemplate, product.Id, product.ImagePath,
               product.Name, product.Price + " грн.", DefaulImgBascet, DefaulImgBascetChecked));
            product.HtmlBannerOrdered = Server.HtmlEncode(string.Format(_productOrderedBennerTemplate, product.Id, product.ImagePath,
                product.Name, product.Price + " грн.", DefaulImgBascet, DefaulImgBascetChecked));

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
        public ActionResult StateBascetDeleted(int id)
        {

            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            if (product != null)
            {
                product.HtmlBanner = product.HtmlBannerOrderedNot;
                product.Ordered = false;
                ProductsRepository.SaveOrUpdate(product);
                return RedirectToAction("Index", "Bascet", new { message = "Ви видалили з корзини " + product.Name });
            }
            return null;
        }
        //
        // POST: /Product/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                ProductsRepository.Delete(ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                return RedirectToAction("Edit", "Employee");
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
            product.HtmlBanner = Server.HtmlEncode(string.Format(_productBennerTemplate, productEditModel.Id, productEditModel.ImagePath, productEditModel.Name, productEditModel.Price + " грн.", DefaulImgBascet, DefaulImgBascetChecked));
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

