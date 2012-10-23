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


        public ActionResult EditDetails(int id)
        {
            string html = HttpUtility.HtmlDecode(EmployeeRepository.Get(s => s.Id == id).SingleOrDefault().HtmlDetail);
            EmployeeDetailsModel model = new EmployeeDetailsModel();
            model.EmployeeId = id;
            model.HtmlDetail = html != null ? html : "";
            return View(model);
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult EditDetails(EmployeeDetailsModel model)
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
        [TolokaAuthorizeAsAuthorAttribute]
        public ActionResult Edit()
        {
            EmployeeEditModel model = EmployeeToEditModel();

            return View(model);
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult EditAuthor(int id)
        {
            EmployeeEditModel model = EmployeeToEditModel(id);

            return View("Edit", model);
        }
        //
        // POST: /Employee/Edit/5

        [HttpPost]
        public ActionResult Edit(EmployeeEditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Employee employee = EditModelToEmployee(model);
                    EmployeeRepository.SaveOrUpdate(employee);

                    return RedirectToAction("Index", "Home");
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
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id)
        {
            return View(EmployeeRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
        }
        [TolokaAuthorizeAsAuthorAttribute]
        public ActionResult EditProduct(int id)
        {
            return RedirectToAction("Edit", "Product", new { id = id });
        }
        //
        // POST: /Employee/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Employee employee = EmployeeRepository.Get(s => s.Id == id).SingleOrDefault();
                if (employee != null)
                {
                    employee.HtmlBanner = null;
                    List<Product> Products = ProductsRepository.Get(p => p.OwnerEmployee.Id == employee.Id).ToList();
                    foreach (var item in Products)
                    {
                        ProductsRepository.Delete(item);
                    }
                    EmployeeRepository.SaveOrUpdate(employee);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }
        [TolokaAuthorizeAsAuthorAttribute]
        public ActionResult DeleteProduct(int id)
        {
            return RedirectToAction("Delete", "Product", new { id = id });
        }
        private Employee EditModelToEmployee(EmployeeEditModel employeeEditModel)
        {
            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(employeeEditModel.Id)).SingleOrDefault();
            employee.FirstName = employeeEditModel.FirstName;
            employee.LastName = employeeEditModel.LastName;
            employee.ImagePath = employeeEditModel.ImagePath;
            employee.HtmlBanner = Server.HtmlEncode(string.Format(_authorTemplate, employeeEditModel.Id,
             employeeEditModel.ImagePath, employeeEditModel.FirstName, employeeEditModel.LastName));
            employee.HtmlDetail = Server.HtmlEncode(employeeEditModel.HtmlDetail);
            return employee;
        }


        private EmployeeEditModel EmployeeToEditModel()
        {
            Employee employee = UserRepository.Get(u => u.UserName.Equals(User.Identity.Name)).SingleOrDefault().Employee;
            EmployeeEditModel employeeEditModel = new EmployeeEditModel();

            if (employee.HtmlBanner == null)
            {

                employeeEditModel.FirstName = employee.FirstName;
                employeeEditModel.LastName = " Прізвище";
                employeeEditModel.ImagePath = DefaulImg;
                employeeEditModel.Email = employee.Email;
                employeeEditModel.Id = employee.Id;
                employeeEditModel.HtmlBanner = string.Format(_authorTemplate, employee.Id, DefaulImg, employeeEditModel.FirstName, employeeEditModel.Email);
                employeeEditModel.Products = new List<Product>();
            }
            else
            {
                employeeEditModel.ImagePath = employee.ImagePath;
                employeeEditModel.FirstName = employee.FirstName;
                employeeEditModel.LastName = employee.LastName;
                employeeEditModel.HtmlBanner = HttpUtility.HtmlDecode(employee.HtmlBanner);
                employeeEditModel.HtmlDetail = HttpUtility.HtmlDecode(employee.HtmlDetail);
                employeeEditModel.Email = employee.Email;
                employeeEditModel.Id = employee.Id;
                employeeEditModel.Products = ProductsRepository.Get(p => p.OwnerEmployee.Id == employee.Id).ToList();

            }
            return employeeEditModel;
        }

        private EmployeeEditModel EmployeeToEditModel(int id)
        {
            Employee employee = EmployeeRepository.Get(u => u.Id == id).SingleOrDefault();
            EmployeeEditModel employeeEditModel = new EmployeeEditModel();

            if (employee.HtmlBanner == null)
            {

                employeeEditModel.FirstName = employee.FirstName;
                employeeEditModel.LastName = " Прізвище";
                employeeEditModel.ImagePath = DefaulImg;
                employeeEditModel.Email = employee.Email;
                employeeEditModel.Id = employee.Id;
                employeeEditModel.HtmlBanner = string.Format(_authorTemplate, employee.Id, DefaulImg, employeeEditModel.FirstName, employeeEditModel.Email);
                employeeEditModel.Products = new List<Product>();
            }
            else
            {
                employeeEditModel.ImagePath = employee.ImagePath;
                employeeEditModel.FirstName = employee.FirstName;
                employeeEditModel.LastName = employee.LastName;
                employeeEditModel.HtmlBanner = HttpUtility.HtmlDecode(employee.HtmlBanner);
                employeeEditModel.HtmlDetail = HttpUtility.HtmlDecode(employee.HtmlDetail);
                employeeEditModel.Email = employee.Email;
                employeeEditModel.Id = employee.Id;
                employeeEditModel.Products = ProductsRepository.Get(p => p.OwnerEmployee.Id == employee.Id).ToList();

            }
            return employeeEditModel;
        }
    }
}
