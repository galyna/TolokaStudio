using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data.Entities;
using System.ComponentModel.DataAnnotations;
using ViewRes.Store;

namespace SumkaWeb.Models
{
    public class StoreCreateModel
    {
        [Required(ErrorMessageResourceName = "Required_Name",
         ErrorMessageResourceType = typeof(Create))]
        [Display(Name = "Name", ResourceType = typeof(Create))]
        public string Name { get; set; }
        public string HtmlBanner { get; set; }
        [Required(ErrorMessageResourceName = "Required_Description",
            ErrorMessageResourceType = typeof(Create))]
        public string Description { get; set; }
        [Required(ErrorMessageResourceName = "Required_Image",
        ErrorMessageResourceType = typeof(Create))]
        public string ImagePath { get; set; }
    }
}