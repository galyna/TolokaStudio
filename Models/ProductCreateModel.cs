using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data.Entities;
using System.ComponentModel.DataAnnotations;
using SumkaWeb.Resources.Views;
using SumkaWeb.Models.CombinedHTML;
using SumkaWeb.Resources.Models.Product;
using SumkaWeb.Resources.Views.Home;

namespace SumkaWeb.Models
{
    public class ProductCreateModel
    {
        [Required(ErrorMessageResourceName = "Required_Name",
                ErrorMessageResourceType = typeof(ProductCreate))]
        [Display(Name = "Name", ResourceType = typeof(ProductCreate))]
        public string Name { get; set; }
        public string HtmlBanner { get; set; }
        [Required(ErrorMessageResourceName = "Required_Price",
            ErrorMessageResourceType = typeof(ProductCreate))]
        [Display(Name = "Price", ResourceType = typeof(ProductCreate))]
        public double Price { get; set; }
        [Required(ErrorMessageResourceName = "Required_Image",
        ErrorMessageResourceType = typeof(Home))]
        public virtual string ImagePath { get; set; }
        public int StoreID { get; set; }
        public string HtmlDetail { get; set; }
        public CombinedHTMLItem CombinedHTMLItem { get; set; }
    }
}