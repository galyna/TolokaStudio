using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SumkaWeb.Models
{
    public class ImagesFolder
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
    public class CombinedHTMLImageUpload
    {
        public  CombinedHTMLImageUpload()
        {
            this.Message="";
            this.SelectLabel = "Виберіть зображення для завантаження";
            this.ImageUploaded = ""; ;
        }
        public IEnumerable<ImagesFolder> Folders { get; set; }
        public ImagesFolder SelectedFolder { get; set; }
        public string Message { get; set; }
        public string ImageUploaded { get; set; }
        public string SelectLabel { get; set; }
      
    }
}