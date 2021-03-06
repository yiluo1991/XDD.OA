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
       [Authorize(Roles = "管理员,二手管理")]
    public class CommodityCategoryController : Controller
    {
        // GET: CommodityCategory
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword = "")//搜索
        {
            int keywordId;
            bool isId = int.TryParse(keyword, out keywordId);
            XDDDbContext ctx = new XDDDbContext();
            return Json(ctx.CommodityCategories.Where(s => (isId ? s.Id == keywordId : false) || s.Name.Contains(keyword) || s.SN.ToString() == keyword).OrderBy(s => s.SN).Select(s => new { s.Id, s.Name, s.SN, s.Icon, s.Enable }).ToList(), JsonRequestBehavior.AllowGet);
        }


     

        public JsonResult Add(CommodityCategory category)
        {
            XDDDbContext ctx = new XDDDbContext();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string uploadPath = Server.MapPath("~/Images/CommodityCategory/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (ImageWorker.IsValidImage(file))
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
                    category.Icon = domain + "/Images/CommodityCategory/" + name + Path.GetExtension(file.FileName).ToLower();
                    ctx.CommodityCategories.Add(category);
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

        public JsonResult Edit(CommodityCategory category)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.CommodityCategories.FirstOrDefault(s => s.Id == category.Id);
            if (target == null)
            {
                return Json(new { ResultCode = -1, message = "没有找到要操作的数据" });
            }
            else
            {
                target.Name = category.Name;
                target.SN = category.SN;

                target.Enable = category.Enable;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string uploadPath = Server.MapPath("~/Images/CommodityCategory/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    if (ImageWorker.IsValidImage(file))
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
                        target.Icon = domain + "/Images/CommodityCategory/" + name + Path.GetExtension(file.FileName).ToLower();
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
    }
}