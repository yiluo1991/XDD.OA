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
    [Authorize(Roles= "管理员,社交管理")]
    public class BBSCategoryController : Controller
    {
        XDDDbContext Context = new XDDDbContext();
        // GET: BBSCategory
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword,int page=1,int rows=15) {
            IQueryable<BBSCategory> query = Context.BBSCategories;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword));
            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.SN).Skip((page - 1) * rows).Take(rows).Select(s => new { s.Enable,s.Icon,s.Id,s.Name,s.Option,s.Required,s.SN ,s.TimeAreaOneStart,s.TimeAreaOneEnd,s.TimeAreaTwoStart,s.TimeAreaTwoEnd}).ToList()
            },JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult Add(BBSCategory category)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string uploadPath = Server.MapPath("~/Images/BBSCategory/");
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
                    category.Icon = domain + "/Images/BBSCategory/" + name + Path.GetExtension(file.FileName).ToLower();
                 
                }
                else
                {
                    return Json(new { ResultCode = 0, message = "参数有误" });
                }
            }
            Context.BBSCategories.Add(category);
            Context.SaveChanges();
            return Json(new { ResultCode = 1, message = "添加成功" });
        }

        public JsonResult Edit(BBSCategory category)
        {
            var target = Context.BBSCategories.FirstOrDefault(s => s.Id == category.Id);
            if ((int)(category.Required & category.Option) == 0) {
                if (target != null)
                {
                   
                    target.Enable = category.Enable;
                    target.Name = category.Name;
                    target.Option = category.Option;
                    target.SN = category.SN;
                    target.Required = category.Required;
                    target.TimeAreaOneStart = category.TimeAreaOneStart;
                    target.TimeAreaOneEnd = category.TimeAreaOneEnd;
                    target.TimeAreaTwoStart = category.TimeAreaTwoStart;
                    target.TimeAreaTwoEnd = category.TimeAreaTwoEnd;
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        string uploadPath = Server.MapPath("~/Images/BBSCategory/");
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
                            target.Icon = domain + "/Images/BBSCategory/" + name + Path.GetExtension(file.FileName).ToLower();
                        }
                        else
                        {
                            return Json(new { ResultCode = 0, message = "图片格式有误" });
                        }
                    }
                    Context.SaveChanges();
                    return Json(new { ResultCode = 1, message = "修改成功" });
                }
                else
                {
                    return Json(new { ResultCode = 0, message = "没有找到指定的频道" });
                }
            }
            else
            {
                return Json(new { ResultCode = -1, message = "参数有误" });
            }
        }

        public JsonResult Remove(int id)
        {
            var target = Context.BBSCategories.FirstOrDefault(s => s.Id == id);
            if (target != null) {
                if (target.BBSArticles.Count == 0)
                {

                    Context.BBSCategories.Remove(target);
                    Context.SaveChanges();

                    return Json(new { ResultCode = 1, message = "删除成功" });
                }
                else
                {
                    return Json(new { ResultCode = 0, message = "有文章的频道无法删除，请删除频道下所有文章或将频道设为禁用" });
                }
            }
            else
            {
                return Json(new { ResultCode = 0, message = "没有找到指定的频道" });
            }
        }
    }
}