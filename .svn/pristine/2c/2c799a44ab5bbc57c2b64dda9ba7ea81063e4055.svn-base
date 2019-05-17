using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Web.Infrastructure;
using System.IO;
using System.Drawing.Imaging;
using System.Web.Configuration;
namespace XDD.Web.Controllers
{
    [Authorize(Roles = "管理员")]
    public class BasicController : Controller
    {
        // GET: Basic
        [Authorize(Roles = "管理员")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            return Json(Setting.MySetting);
        }


        public JsonResult Save(SettingRoot root)
        {
            var path = Server.MapPath("/views/config.json");
            try
            {
                string uploadPath = Server.MapPath("~/Images/Banner/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var i = 0;

                ImageFormat imageFormat;
                var attrNames = new List<string> { "Block1Pic", "Block2Pic", "Block3Pic", "Block4Pic" };
                foreach (var attrName in attrNames)
                {
                    var oldValue = typeof(SettingRoot).GetProperty(attrName.Replace("Pic", "Src")).GetValue(Setting.MySetting);

                     
                    if (Request.Files[attrName] != null && ImageWorker.IsValidImage(Request.Files[attrName]))
                    {
                        HttpPostedFileBase file = Request.Files[attrName];
                        i++;
                        string name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + i;
                        switch (Path.GetExtension(file.FileName).ToLower())
                        {
                            case ".jpeg":
                                imageFormat = ImageFormat.Jpeg;
                                break;
                            case ".png":
                                imageFormat = ImageFormat.Png;
                                break;
                            case ".gif":
                                imageFormat = ImageFormat.Gif;
                                break;
                            case ".jpg":
                                imageFormat = ImageFormat.Jpeg;
                                break;
                            default:
                                continue;
                        }
                        
                        //保存
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 300, 150, "HW", imageFormat);
                        string domain = WebConfigurationManager.AppSettings["domain"];
                        typeof(SettingRoot).GetProperty(attrName.Replace("Pic","Src")).SetValue(root, domain + "/Images/Banner/" + name + Path.GetExtension(file.FileName).ToLower());
                    }
                    else
                    {
                        typeof(SettingRoot).GetProperty(attrName.Replace("Pic", "Src")).SetValue(root, oldValue);
                    }
                }
             
                System.IO.File.WriteAllText(path, new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(root));
                return Json(new { ResultCode = 1, Message = "保存成功" });
            }
            catch (Exception)
            {

                return Json(new { ResultCode = 0, Message = "保存失败" });
            }


        }

        public IEnumerable<object> attrNames { get; set; }
    }
}