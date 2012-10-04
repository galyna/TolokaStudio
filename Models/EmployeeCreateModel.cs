using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SumkaWeb.Resources.Views.Home;
using SumkaWeb.Resources.Models.Employee;
using System.Web.Mvc;

namespace SumkaWeb.Models
{
    public class EmployeeCreateModel
    {
        [Required(ErrorMessageResourceName = "Required_FirstName",
                ErrorMessageResourceType = typeof(EmployeeCreate))]
        [Display(Name = "FirstName", ResourceType = typeof(EmployeeCreate))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required_LastName",
               ErrorMessageResourceType = typeof(EmployeeCreate))]
        [Display(Name = "LastName", ResourceType = typeof(EmployeeCreate))]
        public string LastName { get; set; }

         [Required(ErrorMessageResourceName = "Required_Image",
        ErrorMessageResourceType = typeof(Home))]
         [HiddenInput(DisplayValue = false)]
        public virtual string ImagePath { get; set; }

         [Required(ErrorMessageResourceName = "Required_Email",
                ErrorMessageResourceType = typeof(EmployeeCreate))]
         [Display(Name = "Email", ResourceType = typeof(EmployeeCreate))]
         public virtual string Email { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int StoreID { get; set; }

        public string HtmlBanner { get; set; }
        public string HtmlDetail { get; set; }

    }
}