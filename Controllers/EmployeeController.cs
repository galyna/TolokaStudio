using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Data.Repository.Interfaces;
using Core.Data.Entities;
using Core.Data.Repository;
using TolokaStudio.Models;
using System.IO;
using TolokaStudio.Common;

namespace TolokaStudio.Controllers
{

    public class EmployeeController : Controller
    {

        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Employee> EmployeeRepository;
        private readonly IRepository<User> UserRepository;
        private readonly IRepository<Product> ProductsRepository;
        private const string DefaulImg = "/Content/img/_C3D9074.png";
        private const string DefaulDetailImg = "/Content/img/imgFull/Fluor/Coffe.png";
        private const string DefaulImgUpload = "/Content/img/q/image_upload.png";
        private const string DefaulImgDelete = "/Content/img/q/delete.png";
        private const string DefaulImgAddProduct = "/Content/img/q/add.png";
        private const string DefaulImgSave = "/Content/img/q/save.png";
        private const string DefaulImgEdit = "/Content/img/q/edit.png";
        private const string _rootImagesFolderPath = "/Content/img/";
        private const string _authorTemplate = "<div class='template'>" +
                    " <div class='span8'>" +
                    " <a href='/Employee/Details?id={0}'>" +
                    " <div class='box_main_item'>" +
                    " <div class='box_main_item_img'>" +
                    "  <div class='box_main_item_img_bg'>" +
                    "     <span>Про автора</span>" +
                    "  </div>" +
                    " <img src='{1}' alt='img_box' />" +
                    " </div>" +
                    " <div class='box_main_item_text'>" +
                    "   <h3>" +
                    "       {2}</h3>" +
                    "     <span>{3}</span>" +
                     "     <span>{4}</span>" +
                    "  </div>" +
                    " </div>" +
                    " </a>" +
                    "</div>" +
                    " </div>";
        private const string _authorEdit = "<div class='template'>" +
                    " <div class='span8'>" +
            " <img class='addProduct'  title='Додати продукт {2}' alt='{0}'   src='" + DefaulImgAddProduct + "' />" +  
            " <img class='editDetailsBtn' src='" + DefaulImgEdit + "' alt='{0}'  title='Редагувати Сторінку{2}'/>" +
                    " <a href='/Employee/Edit?id={0}'>" +
                    " <div class='box_main_item'>" +
                    " <div class='box_main_item_img'>" +
                    "  <div class='box_main_item_img_bg'>" +
                    "     <span>Peдагувати</span>" +
                    "  </div>" +
                    " <img src='{1}' alt='img_box' />" +
                    " </div>" +
                    " <div class='box_main_item_text'>" +
                     "   <h3 class='firstname'>" +
                    "       {2}</h3>" +
                    "     <span class='lastName'>{3}</span>" +
                     "     <span class='email'>{4}</span>" +
                    "  </div>" +
                    " </div>" +
                    " </a>" +
                    "</div>" +
                    " </div>";

        public Dictionary<int, string> staff { get; set; }

        public EmployeeController()
        {
            StoreRepository = new Repository<Store>();
            WebTemplateRepository = new Repository<WebTemplate>();
            EmployeeRepository = new Repository<Employee>();
            UserRepository = new Repository<User>();
            ProductsRepository = new Repository<Product>();
        }

        public ActionResult Index()
        {
            IList<Employee> employees = EmployeeRepository.GetAll().ToList();
            return View(employees);
        }

        //
        // GET: /Employee/Details/5

        public ActionResult Details(int id)
        {
            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();

            return View(employee);
        }

        //
        // GET: /Employee/Details/5

        public ActionResult Create(int userId)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            User userauthor = UserRepository.Get(u => u.Id == userId).SingleOrDefault();
            if (user != null && user.Role.IsAdmin && userauthor != null)
            {


                userauthor.Role.IsAuthor = true;
                Employee employee = new Employee();
                employee.Email = user.Email;
                employee.FirstName = "Ім'я";
                employee.LastName = "Прізвище";
                employee.ImagePath = DefaulImg;
                Employee employeeSaved = EmployeeRepository.SaveOrUpdate(employee);
                employee.HtmlBannerEdit = Server.HtmlEncode(string.Format(_authorEdit, employeeSaved.Id, DefaulImg, employee.FirstName, employee.LastName, employee.Email));
                employee.HtmlBanner = Server.HtmlEncode(string.Format(_authorTemplate, employeeSaved.Id, DefaulImg, employee.FirstName, employee.LastName, user.Email));

                userauthor.Employee = employeeSaved;
                UserRepository.SaveOrUpdate(userauthor);

                return RedirectToAction("Edit", "Employee", new { id = employeeSaved.Id });

            }

            return null;
        }
     
        public ActionResult EditDetails(int id)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                string html = HttpUtility.HtmlDecode(EmployeeRepository.Get(s => s.Id == id).SingleOrDefault().HtmlDetail);
                EmployeeDetailsModel model = new EmployeeDetailsModel();
                model.EmployeeId = id;
                model.HtmlDetail = html != null ? html : "";
                return View(model);
            }

            return null;
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult EditDetails(EmployeeDetailsModel model)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {

                try
                {
                    Employee employee = EmployeeRepository.Get(s => s.Id.Equals(model.EmployeeId)).SingleOrDefault();
                    employee.HtmlDetail = Server.HtmlEncode(model.HtmlDetail);
                    EmployeeRepository.SaveOrUpdate(employee);

                    return RedirectToAction("Edit");
                }
                catch
                {
                    return View(model);
                }
            }

            return null;

        }

        public ActionResult Cabinet()
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            Employee IsAuthor = EmployeeRepository.Get(u => u.Id == user.Employee.Id).SingleOrDefault();
            if (user != null && IsAuthor != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                EmployeeEditModel model = EmployeeToEditModel(IsAuthor);
                return View("Edit",model);
            }

            return null;
        }


        public ActionResult Edit(int id)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            Employee IsAuthor = EmployeeRepository.Get(u => u.Id == id).SingleOrDefault();
            if (user != null && IsAuthor != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                EmployeeEditModel model = EmployeeToEditModel(IsAuthor);
                return View(model);
            }

            return null;
        }

        private EmployeeEditModel EmployeeToEditModel(Employee employee)
        {
            EmployeeEditModel employeeEditModel = new EmployeeEditModel();
            employeeEditModel.ImagePath = employee.ImagePath;
            employeeEditModel.FirstName = employee.FirstName;
            employeeEditModel.LastName = employee.LastName;
            employeeEditModel.HtmlBanner = HttpUtility.HtmlDecode(employee.HtmlBanner);
            employeeEditModel.HtmlDetail = HttpUtility.HtmlDecode(employee.HtmlDetail);
            employeeEditModel.HtmlBannerEdit = HttpUtility.HtmlDecode(employee.HtmlBannerEdit);
            employeeEditModel.Email = employee.Email;
            employeeEditModel.Id = employee.Id;
            employeeEditModel.Products = ProductsRepository.Get(p => p.Employee.Id == employee.Id).ToList();


            return employeeEditModel;
        }
        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult Edit(EmployeeEditModel model)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        Employee employee = EditModelToEmployee(model);
                        EmployeeRepository.SaveOrUpdate(employee);

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

            return null;

        }
        //
        // GET: /Employee/Delete/5

        public class DeleteModel{
            public int Id { get; set; }
        }
    
        public ActionResult Delete(int Id)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                Employee gg = EmployeeRepository.Delete(EmployeeRepository.Get(s => s.Id == Id).SingleOrDefault());             
            }

            return RedirectToAction("Index","Store");
        }

        public ActionResult EditProduct(int id)
        {
            User user = UserRepository.Get(u => u.UserName == User.Identity.Name).SingleOrDefault();
            if (user != null && user.Role.IsAdmin || user.Role.IsAuthor)
            {
                return RedirectToAction("Edit", "Product", new { id = id });
            }
            return null;
        }
        //
        // POST: /Employee/Delete/5

       
     

        private Employee EditModelToEmployee(EmployeeEditModel employeeEditModel)
        {
            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(employeeEditModel.Id)).SingleOrDefault();
            employee.FirstName = employeeEditModel.FirstName;
            employee.LastName = employeeEditModel.LastName;
            employee.ImagePath = employeeEditModel.ImagePath;

            employee.HtmlBannerEdit = Server.HtmlEncode(string.Format(_authorEdit, employeeEditModel.Id, employeeEditModel.ImagePath, employeeEditModel.FirstName, employeeEditModel.LastName, employee.Email, DefaulImgUpload, DefaulImgAddProduct, DefaulImgDelete));
            employee.HtmlBanner = Server.HtmlEncode(string.Format(_authorTemplate, employeeEditModel.Id, employeeEditModel.ImagePath, employeeEditModel.FirstName, employeeEditModel.LastName, employee.Email));
            employee.HtmlDetail = Server.HtmlEncode(employeeEditModel.HtmlDetail);
            return employee;
        }

    }
}
