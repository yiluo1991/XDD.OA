using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员")]
    public class NavIconController : Controller
    {
        // GET: Icon
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword = "")
        {
            XDDDbContext ctx = new XDDDbContext();
            return Json(ctx.NavIcons.Where(s => s.Name.Contains(keyword)).OrderBy(s => s.SN).Select(s => new { s.Id, s.Name, s.SN, s.Src, s.Url, s.Enable }).ToList(), JsonRequestBehavior.AllowGet);
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

        public JsonResult Add(NavIcon navicon)
        {
            XDDDbContext ctx = new XDDDbContext();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string uploadPath = Server.MapPath("~/Images/NavIcon/");
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
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 200, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];
                    navicon.Src = domain + "/Images/NavIcon/" + name + Path.GetExtension(file.FileName).ToLower();
                    ctx.NavIcons.Add(navicon);
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


        public JsonResult Edit(NavIcon navicon)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.NavIcons.FirstOrDefault(s => s.Id == navicon.Id);
            if (target == null)
            {
                return Json(new { ResultCode = -1, message = "没有找到要操作的数据" });
            }
            else
            {
                target.Name = navicon.Name;
                target.SN = navicon.SN;
                target.Url = navicon.Url;
                target.Enable = navicon.Enable;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string uploadPath = Server.MapPath("~/Images/NavIcon/");
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
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 200, 9999, "W", imageFormat);
                        string domain = WebConfigurationManager.AppSettings["domain"];
                        target.Src = domain + "/Images/NavIcon/" + name + Path.GetExtension(file.FileName).ToLower();
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


        public JsonResult Remove(Int32 Id)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.NavIcons.FirstOrDefault(s => s.Id == Id);
            if (target == null)
            {
                return Json(new { ResultCode = 0, message = "没有找到图片" });
            }
            else
            {
                ctx.NavIcons.Remove(target);
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "删除成功" });
            }
        }
    }
}