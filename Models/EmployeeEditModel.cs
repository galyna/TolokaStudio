using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SumkaWeb.Models
{
    public class EmployeeEditModel:EmployeeCreateModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string ImagePath { get; set; }
    }
}