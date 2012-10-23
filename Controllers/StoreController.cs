using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TolokaStudio;
using Core.Data.Entities;
using Core.Data.Repository;
using TolokaStudio.Models;
using Core.Data.Repository.Interfaces;
using System.IO;
using TolokaStudio.Common;

namespace TolokaStudio.Controllers
{
   
    public class StoreController : Controller
    {
        #region
        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Product> ProductsRepository;
        private readonly IRepository<User> UserRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private const string DefaultWord = "a";
        private const string DefaultImg = "/Content/img/imgThumbs/Fluor/Tiger.png";
        private const string _rootImagesFolderPath = "/Content/img/";
        private const string _storeTemplate = "<div class='template'>" +
                    " <div class='span8'>" +
                    " <a href='/Store/Details?id={0}'>" +
                    " <div class='box_main_item'>" +
                    " <div class='box_main_item_img'>" +
                    "  <div class='box_main_item_img_bg'>" +
                    "     <span>Товари</span>" +
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
        #endregion
        public StoreController()
        {
            StoreRepository = new Repository<Store>();
            WebTemplateRepository = new Repository<WebTemplate>();
            UserRepository = new Repository<User>();
            ProductsRepository = new Repository<Product>();
            EmployeeRepository = new Repository<Employee>();
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Index()
        {
            StoreIndexModel storeIndexModel = new StoreIndexModel();
            storeIndexModel.Stores = StoreRepository.GetAll().ToList();
            storeIndexModel.Users = UserRepository.GetAll().ToList();
            storeIndexModel.Employees = EmployeeRepository.GetAll().ToList(); 
            return View(storeIndexModel);
        }
        //
        // GET: /Storage/Details/5

        public ActionResult Details(int id)
        {
            Store store = StoreRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            return View(store);
        }

        //
        // GET: /Storage/Create
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Create()
        {
            return View(new StoreCreateModel()
            {
                Name = "Назва розділу",
                Description = "Опис",
                ImagePath = DefaultImg,
                HtmlBanner = string.Format(_storeTemplate, '0', DefaultImg, "Назва розділу", "Опис"),
            });
        }

        //
        // POST: /Storage/Create
        [TolokaAuthorizeAsAdminAttribute]
        [HttpPost]
        public ActionResult Create(StoreCreateModel store)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Store savedStore = StoreRepository.SaveOrUpdate(new Store() { Name = store.Name, Description = store.Description, ImagePath = store.ImagePath });
                    var htmlBanner = string.Format(_storeTemplate, savedStore.Id, store.ImagePath, savedStore.Name, savedStore.Description);
                    savedStore.HtmlBanner = Server.HtmlEncode(htmlBanner);
                    Store savedStoreWithBanner = StoreRepository.SaveOrUpdate(savedStore);
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(store);
                }
            }
            else
            {
                return View(store);
            }

        }

        //
        // GET: /Storage/Edit/5
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Edit(int id)
        {
            StoreEditModel model = StoreToEditModel(id);
            return View(model);
        }


        //
        // POST: /Storage/Edit/5
        [TolokaAuthorizeAsAdminAttribute]
        [HttpPost]
        public ActionResult Edit(StoreEditModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    Store store = EditModelToStore(model);

                    StoreRepository.SaveOrUpdate(store);

                    return RedirectToAction("Index");
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
        // GET: /Storage/Delete/5
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Delete(int id)
        {
            return View(StoreRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
        }

        //
        // POST: /Storage/Delete/5
        [TolokaAuthorizeAsAdminAttribute]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                StoreRepository.Delete(StoreRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Storage/EditProduct/5
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult EditProduct(int id)
        {
            return RedirectToAction("Edit", "Product", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult AddProduct(int id)
        {
            return RedirectToAction("Create", "Product", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult DeleteProduct(int id)
        {
            return RedirectToAction("Delete", "Product", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult EditEmployee(int id)
        {
            return RedirectToAction("EditAuthor", "Employee", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult AddEmployee(int id)
        {
            return RedirectToAction("Create", "Employee", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult DeleteEmployee(int id)
        {
            return RedirectToAction("Delete", "Employee", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult SetAsAdmin(int id)
        {
            return RedirectToAction("Admin", "Account", new { id = id });
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult SetAsAuthor(int id)
        {
            return RedirectToAction("Author", "Account", new { id = id });
        }



        public ActionResult ImageUpload()
        {
            return PartialView("ImageUpload", new CombinedHTMLImageUpload());
        }


        [HttpPost, ActionName("ImageUpload")]
        public ActionResult ImageUpload(HttpPostedFileBase fileUpload)
        {
            var fileUploaded = (fileUpload != null && fileUpload.ContentLength > 0) ? true : false;
            var viewModel = new CombinedHTMLImageUpload();

            try
            {

                if (!fileUploaded)
                {
                    viewModel.Message = string.Format("Не вдалось завантажити зображення.");
                    Console.WriteLine(viewModel.Message);
                    return PartialView("ImageUpload", viewModel);
                }

                string fileName = Path.GetFileName(fileUpload.FileName);
                string saveLocation = Path.Combine(Server.MapPath(_rootImagesFolderPath), fileName);
                // Try to save image.
                fileUpload.SaveAs(saveLocation);
                viewModel.ImageUploaded = "<IMG id='ImageUploaded' src=" + Path.Combine(_rootImagesFolderPath, fileName) + " style='float: left;'/>";
                viewModel.Message = string.Format("Зображення {0} було успішно завантажено.{1}", fileName, Server.MapPath(_rootImagesFolderPath));
            }
            catch (Exception)
            {

                Console.WriteLine(viewModel.Message);
                return PartialView("ImageUpload", viewModel);
            }

            return PartialView(viewModel);
        }


        private StoreEditModel StoreToEditModel(int id)
        {
            Store store = StoreRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            StoreEditModel model = new StoreEditModel()
            {
                Id = store.Id,
                ImagePath = store.ImagePath,
                Name = store.Name,
                HtmlBanner = store.HtmlBanner,
                Description = store.Description,
                Products = store.Products
            };
            return model;
        }
        private Store EditModelToStore(StoreEditModel model)
        {
            Store store = StoreRepository.Get(s => s.Id.Equals(model.Id)).SingleOrDefault();
            if (store != null)
            {

                store.ImagePath = model.ImagePath;
                store.Name = model.Name;
                store.HtmlBanner = Server.HtmlEncode(string.Format(_storeTemplate, model.Id, model.ImagePath, model.Name, model.Description));
                store.Description = model.Description;
                store.Products = model.Products;
            }
            return store;
        }
    }
}
