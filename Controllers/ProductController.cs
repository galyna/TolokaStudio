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

        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;
        private const string DefaulImgName = "Coffe.png";
        private const string DefaulImg = "/Content/img/imgThumbs/Fluor/" + DefaulImgName;
        private const string DefaulImgBascet = "/Content/img/q/shopping_cart_1.gif";
        private const string DefaulImgBascetDelete = "/Content/img/q/delete.png";
        private const string DefaulImgBascetOrder = "/Content/img/q/order.png";
        private const string DefaulImgUpload = "/Content/img/q/image_upload.png";
        private const string DefaulImgDelete = "/Content/img/q/delete.png";
        private const string DefaulImgAddProduct = "/Content/img/q/add.png";
        private const string DefaulImgSave = "/Content/img/q/save.png";
        private const string DefaulImgEdit = "/Content/img/q/edit.png";
        private const string DefaulDetailImg = "/Content/img/imgFull/Fluor/" + DefaulImgName;
        private const string _productBenner = "<div class='template order{0}'>" +
                   " <div class='span8'>" +
                " <div class='box_main_item'>" +
            " <img class='orderBtn' title='Додати в кошик Назва'  alt='{0}'  src='" + DefaulImgBascet + "' />" +
            " <img class='makeOrder'  title='Замовити Назва' alt='{0}' style='display:none;'  src='" + DefaulImgBascetOrder + "' />" +
            " <img class='deleteBtn' alt='{0}'  style='display:none;' title='Видалити з кошика Назва' src='" + DefaulImgBascetDelete + "'/>" +
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
                  "   <h3>{2}</h3>" +
                  "     <span>{3}</span>" +
                  "  </div>" +
                  " </div>" +
                  " </a>" +
                  "</div>" +
                  " </div>";


        private const string _productBennerOrder = "<div class='template order{0}'>" + " <div class='span8'>" +
                " <div class='box_main_item'>" +
                  " <img class='orderBtn' title='Додати в кошик Назва'  alt='{0}'  src='" + DefaulImgBascet + "' />" +
           " <img class='makeOrder'  title='Замовити Назва' alt='{0}'  src='" + DefaulImgBascetOrder + "' />" +
           " <img class='deleteBtn' alt='{0}'  title='Видалити з кошика Назва' src='" + DefaulImgBascetDelete + "'/>" +
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
                  "   <h3>{2}</h3>" +
                  "     <span>{3}</span>" +
                  "  </div>" +
                  " </div>" +
                  " </a>" +
                  "</div>" +
                  " </div>";

        private const string _productBennerTemplate = "<div class='template order{0}'>" +
                   " <div class='span8'>" +
                " <div class='box_main_item'>" +
             " <img class='uploadBtn' title='Завантажити зображення Назва'  alt='{0}'  src='" + DefaulImgUpload + "' />" +
            " <img class='deleteBtn' src='" + DefaulImgDelete + "' alt='{0}'  title='Видалити товар Назва'/>" +
            " <img class='editDetailsBtn' src='" + DefaulImgEdit + "' alt='{0}'  title='Редагувати Сторінку Назва'/>" +
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
                  "   <h3>{2}</h3>" +
                  "     <span>{3}</span>" +
                  "  </div>" +
                  " </div>" +
                  " </a>" +
                  "</div>" +
                  " </div>";

        private const string _productDetailTemplate =
             "<div class='span10'>" +
       " <img src='{0}' /></div>" +
                  " </div>";

        public ProductController()
        {
            StoreRepository = new Repository<Store>();
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
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            return View(product);
        }
        //
        // GET: /Product/Create/5

        public ActionResult Create(int employeeId)
        {
            try
            {
              Product p=  CreateProduct(employeeId);
                return Json("\\Product\\Edit?employeeId=" + p.Id, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return View();
            }
        }

        private Product CreateProduct(int employeeId)
        {

            Product product = new Product() { Name = "Назва", Price = 100, ImagePath = DefaulImg };
            product.Employee = EmployeeRepository.Get(e => e.Id == employeeId).SingleOrDefault();
            product.Store = StoreRepository.GetAll().First();

            Product productSaved = ProductsRepository.SaveOrUpdate(product);
            product.Store.AddProduct(productSaved);

            productSaved = ProductsRepository.SaveOrUpdate(productSaved);
            return productSaved;

        }

        private void CreateHtml(ref Product product)
        {
            var HtmlBanner = string.Format(_productBennerTemplate, product.Id, product.ImagePath, product.Name, product.Price + " грн.");
            var HtmlBannerOrderedNot = string.Format(_productBenner, product.Id, product.ImagePath, product.Name, product.Price + " грн.");
            var HtmlBannerOrdered = string.Format(_productBennerOrder, product.Id, product.ImagePath, product.Name, product.Price + " грн.");
            var HtmlBannerEdit = string.Format(_productBennerTemplate, product.Id, product.ImagePath, product.Name, product.Price + " грн.");
            product.HtmlBanner = Server.HtmlEncode(HtmlBanner);
            product.HtmlBannerOrderedNot = Server.HtmlEncode(HtmlBannerOrderedNot);
            product.HtmlBannerOrdered = Server.HtmlEncode(HtmlBannerOrdered);
            product.HtmlBannerEdit = Server.HtmlEncode(HtmlBannerEdit);

        }


        public ActionResult Edit(int employeeId)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                ProductEditModel ProductEditModel = ProductToEditModel(employeeId);
                return View(ProductEditModel);
            }
            return null;
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductEditModel productEditModel)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();

            if (ModelState.IsValid && user != null && user.Role.IsAdmin || user.Role.IsAuthor)
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

            CreateHtml(ref product);

            return product;
        }

        private ProductEditModel ProductToEditModel(int id)
        {
            Product product = ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            CreateHtml(ref product);
            ProductEditModel ProductEditModel = new ProductEditModel()
            {
                ImagePath = product.ImagePath,
                Name = product.Name,
                Price = product.Price,
                Id = id,
                HtmlBannerEdit = product.HtmlBannerEdit,
                HtmlDetail = product.HtmlDetail,
                HtmlBanner = product.HtmlBanner
            };
            return ProductEditModel;
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {

            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                return View(ProductsRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
            }
            return null;
        }

        //
        // POST: /Product/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
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
            return null;
        }
        //
        // GET: /Product/Edit/5
    }
}

