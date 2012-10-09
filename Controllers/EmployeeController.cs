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

namespace TolokaStudio.Controllers
{
    [ValidateInput(false)]
    public class EmployeeController : Controller
    {

        private readonly IRepository<Store> StoreRepository;
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        private readonly IRepository<Employee> EmployeeRepository;
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

        public EmployeeController()
        {
            StoreRepository = new Repository<Store>();
            WebTemplateRepository = new Repository<WebTemplate>();
            EmployeeRepository = new Repository<Employee>();
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
        // GET: /Employee/Create

        public ActionResult Create(int id)
        {
            return View(new EmployeeCreateModel()
            {
                StoreID = id,
                FirstName = "Ім'я",
                LastName = "Прізвище",
                ImagePath = DefaulImg,
                Email = "galaynavistovska@gmail.com",
                HtmlBanner = string.Format(_authorTemplate, '0', DefaulImg, "Ім'я та Прізвище", "galaynavistovska@gmail.com"),
            });
        }

        //
        // POST: /employee0/Create

        [HttpPost]
        public ActionResult Create(EmployeeCreateModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    Employee employee = CreateModelToEmployee(model);
                    EmployeeRepository.SaveOrUpdate(employee);
                    employee.HtmlBanner = Server.HtmlEncode(string.Format(_authorTemplate, employee.Id, employee.ImagePath, employee.FirstName, employee.LastName));
                    employee.Name= employee.FirstName+"  "+employee.LastName;
                    EmployeeRepository.SaveOrUpdate(employee);
                    Store store = StoreRepository.Get(s => s.Id.Equals(model.StoreID)).SingleOrDefault();
                    store.AddEmployee(employee);
                    StoreRepository.SaveOrUpdate(store);
                    return RedirectToAction("Edit", "Store", new { id = model.StoreID});
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

        private void SaveTemplate(EmployeeCreateModel model)
        {

            WebTemplate item = new WebTemplate();
            item.Html = model.HtmlDetail;
            item.Name = model.FirstName + "  " + model.LastName + "Author";
            WebTemplateRepository.SaveOrUpdate(item);
        }
        //
        // GET: /Employee/Edit/5

        public ActionResult Edit(int id)
        {
            EmployeeEditModel model = EmployeeToEditModel(id);

            return View(model);
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
                    WebTemplateRepository.SaveOrUpdate(new WebTemplate() { Name = "Employee" + employee.FirstName + " " + employee.LastName, Html = employee.HtmlBanner });
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
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id)
        {
            return View(EmployeeRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
        }

        //
        // POST: /Employee/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                EmployeeRepository.Delete(EmployeeRepository.Get(s => s.Id.Equals(id)).SingleOrDefault());
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private Employee EditModelToEmployee(EmployeeEditModel employeeEditModel)
        {
            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(employeeEditModel.Id)).SingleOrDefault();
            employee.FirstName = employeeEditModel.FirstName;
            employee.LastName= employeeEditModel.LastName;
            employee.ImagePath = employeeEditModel.ImagePath;
            employee.HtmlBanner = Server.HtmlEncode(string.Format(_authorTemplate, employeeEditModel.Id, 
             employeeEditModel.ImagePath, employeeEditModel.FirstName, employeeEditModel.LastName));
            employee.HtmlDetail = Server.HtmlEncode(employeeEditModel.HtmlDetail);
            return employee;
        }

        private Employee CreateModelToEmployee(EmployeeCreateModel employeeCreateModel)
        {
            Employee employee = new Employee()
            {
                FirstName = employeeCreateModel.FirstName,
                LastName = employeeCreateModel.LastName,
                ImagePath = employeeCreateModel.ImagePath,
                HtmlDetail = Server.HtmlEncode(employeeCreateModel.HtmlDetail),
                Email = employeeCreateModel.Email
            };
            return employee;
        }
        private EmployeeEditModel EmployeeToEditModel(int id)
        {
            Employee employee = EmployeeRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            EmployeeEditModel employeeEditModel = new EmployeeEditModel()
            {
                ImagePath = employee.ImagePath,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HtmlBanner = HttpUtility.HtmlDecode(employee.HtmlBanner),
                HtmlDetail = HttpUtility.HtmlDecode(employee.HtmlDetail),
                Email = employee.Email,
                Id = id
            };
            return employeeEditModel;
        }


    }
}
