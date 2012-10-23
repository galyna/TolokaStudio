
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Net;

using Core.Data.Entities;
using SquishIt.Framework;
using TolokaStudio.Models;
using Core.Data.Repository;
using Core.Data.Repository.Interfaces;

namespace TolokaStudio.Controllers
{

    public class CombinedHTMLController : Controller
    {
        #region Private firlds
        private string _getTemplateQuery = "CombinedHTML/GetTemplate?id={0}";
        private const string _rootImagesFolder = "Root";
        private const string _rootImagesFolderPath = "Content/img/";
        private readonly IRepository<WebTemplate> WebTemplateRepository;
        #endregion
        public CombinedHTMLController()
        {
            WebTemplateRepository = new Repository<WebTemplate>();
        }

        #region Templates Init
        public JsonResult GetTinyMceInitSettings()
        {
            var model = new TinymceInitSettingsModel();
            model.templates = GetTemplatesList();
            model.scripts = GetTinyMceExternallScripts();
            model.rootImagesFolderPath = Path.Combine(Request.Url.Scheme + "://" + Request.Url.DnsSafeHost + ":" + Request.Url.Port + Request.ApplicationPath, _rootImagesFolderPath);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHtmlFromPage(string pageUrl)
        {
            WebClient wc = new WebClient();
            byte[] raw = wc.DownloadData(pageUrl);

            string webData = System.Text.Encoding.UTF8.GetString(raw);
            var template =
                 new TemplateModel
                 {
                     title = "t",
                     src = webData,
                     description = "d"
                 };
            return Json(template, JsonRequestBehavior.AllowGet);
        }

        private List<TemplateModel> GetTemplatesList()
        {
            IList<WebTemplate> templatesDB = WebTemplateRepository.GetAll().ToList();
            List<TemplateModel> templates = new List<TemplateModel>();
            foreach (var item in templatesDB)
            {
                var template =
                     new TemplateModel
                     {
                         title = item.Name,
                         src = string.Format(_getTemplateQuery, item.Id),
                         description = item.Name
                     };
                templates.Add(template);
            }
            return templates;
        }

        public ActionResult GetTemplate(int id)
        {
            WebTemplate template = WebTemplateRepository.Get(s => s.Id.Equals(id)).SingleOrDefault();
            return Content(Server.HtmlDecode(template.Html));
        }

        public ActionResult GetTemplateCss()
        {
            var file = File("~/Content/style/toloka_content.css", "text/css", Server.UrlEncode("toloka_content.css"));
            return file;
        }

        private List<string> GetTinyMceExternallScripts()
        {
            List<string> scripts = new List<string>();
            //SquishIt
            var squishIt = Bundle.JavaScript()
           .Add("~/Content/assets/js/jquery.js")
           .Add("~/Content/assets/js/google-code-prettify/prettify.js")
           .Add("~/Content/js/events_init.js")
           .Add("~/Content/js/scroller_load_img.js")
           .Add("~/Content/js/menu_full_width.js")
           .Add("~/Content/assets/js/google-code-prettify/prettify.js")
           .ForceRelease()
           .Render("~/Content/js/combined_.js");
            var squishItVersion = squishIt.Substring(squishIt.IndexOf("combined_") + 9, 32);
            scripts.Add(Path.Combine(Request.Url.Scheme + "://" + Request.Url.DnsSafeHost + ":" + Request.Url.Port + Request.ApplicationPath, "Content/js/combined_.js"));
            return scripts;
        }
        #endregion
        #region Templates Images
        public ActionResult GetImagesList(string selectedFolder)
        {
            List<ImageModel> imageList =  FillImagesList(selectedFolder);
            return Json(imageList, JsonRequestBehavior.AllowGet);
        }

        private List<ImageModel> FillImagesList(string sourceDir)
        {
            List<ImageModel> imageList = new List<ImageModel>();
            string[] files = Directory.GetFiles(sourceDir);
            var urlPrefix = Path.Combine(Request.Url.Scheme + "://" + Request.Url.DnsSafeHost + ":" + Request.Url.Port + Request.ApplicationPath + _rootImagesFolderPath, new DirectoryInfo(sourceDir).Name);
            if (IsRootFolder(sourceDir))
            {
                urlPrefix = Request.ApplicationPath + _rootImagesFolderPath;
            }

            foreach (string filePath in files)
            {
                string fullName = Path.GetFileName(filePath);
                string extention = Path.GetExtension(filePath);

                if (extention.Equals(".png") || extention.Equals(".jpg") || extention.Equals(".jpeg") || extention.Equals(".gif"))
                {
                    imageList.Add(new ImageModel() { name = fullName, src = Path.Combine(urlPrefix, fullName) });
                }
            }
            return imageList;
        }
        public ActionResult GetLinksList()
        {
            var pages = new List<string>();// _pagePropertiesService.GetPageURLs();
            Dictionary<string, string> tinyMCELinkList = new Dictionary<string, string>();
            foreach (var page in pages)
            {
                var value = "";//Request.ApplicationPath + page.Key;
                string key = "";// page.Value + "(" + page.Key + ")";
                tinyMCELinkList.Add(key, value);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("var tinyMCELinkList = new Array(");
            foreach (var link in tinyMCELinkList)
            {
                sb.Append("[" + "'" + link.Key + "'" + "," + "'" + link.Value + "'" + "],");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            return JavaScript(sb.ToString());
        }

        public ActionResult GetImage(string filename)
        {
            string[] nameDotSplited = filename.Split('.');

            var file = File(Request.ApplicationPath + _rootImagesFolderPath + filename, "image/" + nameDotSplited[1], Server.UrlEncode(filename));
            return file;

        }

        #region Templates ImageUpload
        public ActionResult ImageUpload()
        {
            return PartialView(UpdateFolders(new CombinedHTMLImageUpload(), null));
        }

        [HttpPost, ActionName("ImageUpload")]
        public ActionResult ImageUpload(string selectedFolderUrl, HttpPostedFileBase fileUpload)
        {
            var fileUploaded = (fileUpload != null && fileUpload.ContentLength > 0) ? true : false;
            var viewModel = new CombinedHTMLImageUpload();
            viewModel = UpdateFolders(viewModel, null);

            try
            {
                if (string.IsNullOrEmpty(selectedFolderUrl))
                {
                    viewModel.Message = string.Format("Selected directory{0} not valid.", selectedFolderUrl);
                    Console.WriteLine(viewModel.Message);
                    return PartialView("ImageUpload", viewModel);
                }
                // Determine whether the directory exists.
                if (!Directory.Exists(selectedFolderUrl))
                {
                    viewModel.Message = string.Format("Selected directory{0} does not exists.", selectedFolderUrl);
                    Console.WriteLine(viewModel.Message);
                    return PartialView("ImageUpload", viewModel);
                }

                if (!fileUploaded)
                {
                    viewModel.Message = string.Format("Upload failed.");
                    Console.WriteLine(viewModel.Message);
                    return PartialView("ImageUpload", viewModel);
                }

                string fileName = Path.GetFileName(fileUpload.FileName);

                // Try to save image.
                fileUpload.SaveAs(Path.Combine(selectedFolderUrl, fileName));
                viewModel = UpdateFolders(viewModel, new DirectoryInfo(selectedFolderUrl).Name);
                viewModel.Message = string.Format("Image {0} was succecfully uploaded.", fileName);
            }
            catch (Exception e)
            {
                viewModel.Message = string.Format("The process failed: {0}", e.ToString());
                Console.WriteLine(viewModel.Message);
                return PartialView("ImageUpload", viewModel);
            }

            return PartialView(viewModel);
        }
        #endregion

        #region Templates CreateImageFolder
        [HttpPost, ActionName("CreateImageFolder")]
        public ActionResult CreateImageFolder(string folderName)
        {
            string path = Path.Combine(Server.MapPath("~" + _rootImagesFolderPath), folderName);
            var viewModel = new CombinedHTMLImageUpload();
            viewModel = UpdateFolders(viewModel, null);
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    viewModel.Message = string.Format("The directory{0} exists already.", folderName);
                    Console.WriteLine(viewModel.Message);
                    return PartialView("ImageUpload", viewModel);
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                viewModel.Message = string.Format("The directory{0} was created successfully at {1}.", folderName, Directory.GetCreationTime(path));
                Console.WriteLine(viewModel.Message);

            }
            catch (Exception e)
            {
                viewModel.Message = string.Format("The process failed: {0}", e.ToString());
                Console.WriteLine(viewModel.Message);
                return PartialView("ImageUpload", viewModel);
            }

            viewModel = UpdateFolders(viewModel, folderName);

            return PartialView("ImageUpload", viewModel);

        }

        private CombinedHTMLImageUpload UpdateFolders(CombinedHTMLImageUpload viewModel, string selected)
        {
            viewModel.Folders = GetImageFoldrs();
            if (!string.IsNullOrEmpty(selected))
            {
                viewModel.SelectedFolder = viewModel.Folders.Where(x => x.Name == selected).Last();
            }
            else
            {
                viewModel.SelectedFolder = viewModel.Folders.Where(x => x.Name == _rootImagesFolder).Last();
            }

            return viewModel;
        }
        #endregion

        #region Templates GetImageFoldrs

        private bool IsRootFolder(string path)
        {
            var rootPhysicalPath = Server.MapPath("~/" + _rootImagesFolderPath);
            if (rootPhysicalPath.Equals(path))
            {
                return true;
            }
            return false;
        }

        private List<ImagesFolder> GetImageFoldrs()
        {
            List<ImagesFolder> imgFolders = new List<ImagesFolder>();
            // Process the list of files found in the directory. 
            string rootPath = Server.MapPath("~/" + _rootImagesFolderPath);
            string[] fileEntries = Directory.GetDirectories(rootPath);
            foreach (string fileName in fileEntries)
            {
                ImagesFolder imagesFolder = new ImagesFolder() { Name = new DirectoryInfo(fileName).Name, Url = Path.Combine(rootPath, fileName) };
                imgFolders.Add(imagesFolder);
            }
            //addd root
            ImagesFolder imagesFolderRoot = new ImagesFolder() { Name = _rootImagesFolder, Url = rootPath };
            imgFolders.Add(imagesFolderRoot);

            return imgFolders;
        }

        #endregion

        #endregion


        #region Templates Save

        public ActionResult SaveWebTemplate(string html, string name)
        {
            WebTemplate item = new WebTemplate();
            item.Html = html;
            item.Name= name;
            WebTemplateRepository.SaveOrUpdate(item);
            var model = new TinymceInitSettingsModel();
            model.templates = GetTemplatesList();
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        #endregion

    }
    #region JavaScript models
    public class TemplateModel
    {
        public string title { get; set; }
        public string src { get; set; }
        public string description { get; set; }
    }
    public class ImageModel
    {
        public string name { get; set; }
        public string src { get; set; }
        public string description { get; set; }
    }

    public class TinymceInitSettingsModel
    {
        public List<TemplateModel> templates { get; set; }
        public List<string> scripts { get; set; }
        public string rootImagesFolderPath { get; set; }

    }
    #endregion


}
