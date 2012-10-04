using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data.Entities;

namespace SumkaWeb.Models
{
    public class ProductEditModel
    {
        public IList<WebTemplate> ProductTemplates { get; set; }
        public Product Product { get; set; }
    }
}