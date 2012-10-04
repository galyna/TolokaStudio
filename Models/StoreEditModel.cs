using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data.Entities;

namespace SumkaWeb.Models
{
    public class StoreEditModel
    {
        public IList<WebTemplate> ProductTemplates { get; set; }
        public Store Store { get; set; }
    }
}