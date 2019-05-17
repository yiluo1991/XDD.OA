using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;
using System.Web.Configuration;
namespace XDD.OA.Controllers
{
       [Authorize(Roles = "管理员")]
    public class BannerController : Controller
    {
      
        // GET: Banner
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword = "")
        {
            XDDDbContext ctx = new XDDDbContext();
            return Json(ctx.Banners.Where(s => s.Name.Contains(keyword)).OrderBy(s => s.SN).Select(s => new { s.Id, s.Name, s.SN, s.Src, s.Url, s.Enable }).ToList(), JsonRequestBehavior.AllowGet);
        }
        private bool IsValidImage(System.Web.HttpPostedFileBase postedFile)
        {
            string sMimeType = postedFile.ContentType.ToLower();
            string ext = null;
            if (postedFile.FileName.IndexOf('.') > 0)
            {
                string[] fs = postedFile.FileName.Split('.');
                ext = fs[fs.Length - 1];
            }
            if (!new List<string>() { "jpg", "jpeg", "png", "gif" }.Contains(ext.ToLower()))
            {
                return false;
            }
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(postedFile.InputStream);
                if (img.Width * img.Height < 1)
                    return false;

                img.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public JsonResult Add(Banner banner)
        {
            XDDDbContext ctx = new XDDDbContext();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string uploadPath = Server.MapPath("~/Images/Banner/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (IsValidImage(file))
                {

                    ImageFormat imageFormat;
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
                            return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
                    }

                    //保存
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];
                    banner.Src = domain + "/Images/Banner/" + name + Path.GetExtension(file.FileName).ToLower();
                    ctx.Banners.Add(banner);
                    ctx.SaveChanges();
                    return Json(new { ResultCode = 1, message = "添加成功" });
                }
                else
                {
                    return Json(new { ResultCode = 0, message = "参数有误" });
                }

            }
            else
            {
                return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
            }
        }


        public JsonResult Edit(Banner banner)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.Banners.FirstOrDefault(s => s.Id == banner.Id);
            if (target == null)
            {
                return Json(new { ResultCode = -1, message = "没有找到要操作的数据" });
            }
            else
            {
                target.Name = banner.Name;
                target.SN = banner.SN;
                target.Url = banner.Url;
                target.Enable = banner.Enable;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string uploadPath = Server.MapPath("~/Images/Banner/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    if (IsValidImage(file))
                    {

                        ImageFormat imageFormat;
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
                                return Json(new { ResultCode = 0, message = "只接受jpg/png/gif图片" });
                        }

                        //保存
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                        string domain = WebConfigurationManager.AppSettings["domain"];
                        target.Src = domain + "/Images/Banner/" + name + Path.GetExtension(file.FileName).ToLower();
                    }
                    else
                    {
                        return Json(new { ResultCode = 0, message = "图片格式有误" });
                    }
                }
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "修改成功" });
            }
        }


        public JsonResult Remove(Int32 Id) {
            XDDDbContext ctx = new XDDDbContext();
           var target= ctx.Banners.FirstOrDefault(s => s.Id == Id);
           if (target == null)
           {
               return Json(new { ResultCode = 0, message = "没有找到图片" });
           }
           else
           {
               ctx.Banners.Remove(target);
               ctx.SaveChanges();
               return Json(new { ResultCode =1, message = "删除成功" });
           }
        }
    }
}