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
       [Authorize(Roles = "管理员,社交管理")]
    public class BBSArticleController : Controller
    {
        XDDDbContext Context = new XDDDbContext();
        // GET: BBSArticle
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Remove(Int32 Id)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == Id);
            if (target == null)
            {
                return Json(new { ResultCode = 0, message = "删除失败" });
            }
            else
            {
                Context.BBSArticles.Remove(target);
                Context.SaveChanges();
                return Json(new { ResultCode = 1, message = "删除成功" });
            }
        }

        public JsonResult SetView(int id,int count)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == id);
            if (target == null)
            {
                return Json(new { ResultCode = 0, message = "没有找到要设置的记录" });
            }
            else
            {
                target.ReadCount = count;
                Context.SaveChanges();
                return Json(new { ResultCode = 1, message = "设置成功" });
            }
        }

        public JsonResult AllowGet(string keyword, int categoryId = 0, int page = 1, int rows = 15)//搜索

        {
            IQueryable<BBSArticle> query = Context.BBSArticles;
            if (categoryId != 0)
            {
                query = query.Where(s => s.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                int keywordId;
                bool isNumber = int.TryParse(keyword, out keywordId);
                if (isNumber)
                {
                    query = query.Where(s => s.Title.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.BBSCategory.Name.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Subject.Contains(keyword) || s.Address.Contains(keyword) || s.Member.NickName == keyword || s.SN.ToString() == keyword || s.Member.Id == keywordId || s.Id == keywordId);
                }
                else
                {
                    query = query.Where(s => s.Title.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.BBSCategory.Name.Contains(keyword) ||s.Member.NickName.Contains(keyword) || s.Subject.Contains(keyword) || s.Address.Contains(keyword) || s.Member.NickName == keyword || s.SN.ToString() == keyword || s.Member.Id == keywordId || s.Id == keywordId);
                }
            }
            return Json(new
            {
                total = query.Count(),
               
                rows = query.OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                {
                    s.AllowComment,
                    s.CategoryId,
                    s.CommentCount,
                    s.Counter,
                    s.CreateTime,
                    s.Id,
                    s.IsBackgroundArticle,
                    MemberId = s.Member.NickName,
                    M=s.MemberId,
                    RealName=s.Member.RealName,
                    s.ReadCount,
                    s.ShowComment,
                    s.SN,
                    s.Subject,
                   Title= HtmlSaferAnalyser.ToSafeHtml(s.Title,false),
                    //s.MemberId,
                    NickName = s.Member.NickName,
                    s.Paths,
                    s.Address,
                     CategoryName = s.BBSCategory.Name,
                    s.DateTime,
                    s.Payment,
                    s.PeopleEnd,
                    s.PeopleStart,
                    s.Content,s.HomeShow
                })
            });
        }
        [ValidateInput(false)]
        public JsonResult Add(BBSArticle BBSArticle)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id==BBSArticle.Id);
            if (target == null)
            {
                BBSArticle.CategoryId =BBSArticle.CategoryId;
                BBSArticle.CreateTime = DateTime.Now;
                BBSArticle.DateTime = BBSArticle.DateTime;
                BBSArticle.MemberId = Context.Members.FirstOrDefault(x=>x.IsVirtualMember==true).Id;
                BBSArticle.IsBackgroundArticle = true;
                BBSArticle.ShowComment = true;
                BBSArticle.AllowComment = true;
                Context.BBSArticles.Add(BBSArticle);
                Context.SaveChanges();
                return Json(new { ResultCode = 1, message = "添加成功" });
            }
            else {
                return Json(new { ResultCode = 0, message = "添加失败" });
            }
        }
        [ValidateInput(false)]
        public JsonResult Change(BBSArticle BBSArticle)
        {

            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == BBSArticle.Id);
            if (target != null)
            {
                target.CategoryId = BBSArticle.CategoryId;
                target.Title = BBSArticle.Title;
                target.Subject = BBSArticle.Subject;
                target.DateTime = BBSArticle.DateTime;
                target.Address = BBSArticle.Address;
                target.PeopleStart = BBSArticle.PeopleStart;
                target.PeopleEnd = BBSArticle.PeopleEnd;
                target.Payment = BBSArticle.Payment;
                target.SN = BBSArticle.SN;
                target.Content = BBSArticle.Content;
            
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    string uploadPath = Server.MapPath("~/Images/BBSCategories/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string name = DateTime.Now.ToString("b-yyyyMMddHHmmssfff");
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
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                        ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name.Replace("b-", "s-"), Path.GetExtension(file.FileName).ToLower()), 360, 9999, "W", imageFormat);
                        string domain = WebConfigurationManager.AppSettings["domain"];
                        target.Paths = domain + "/Images/BBSCategories/" + name + Path.GetExtension(file.FileName).ToLower();
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
                return Json(new { ResultCode = 0, message = "参数错误" });
            }
        }
        public JsonResult editwork(int id,  bool select) {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id ==id);
            if (target != null)
            {
                target.IsBackgroundArticle = select;
                Context.SaveChanges();
                return Json(new { ResultCode = 0, message = "修改成功" });

            }
            else {
                return Json(new { ResultCode = 0, message = "修改失败" });

            }
        }
        public JsonResult editkepinglun(int id, bool select)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                target.AllowComment = select;
                Context.SaveChanges();
                return Json(new { ResultCode = 0, message = "修改成功" });

            }
            else
            {
                return Json(new { ResultCode = 0, message = "修改失败" });

            }
        }
        public JsonResult editshowpinglun(int id, bool select)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                target.ShowComment = select;
                Context.SaveChanges();
                return Json(new { ResultCode = 0, message = "修改成功" });

            }
            else
            {
                return Json(new { ResultCode = 0, message = "修改失败" });

            }
        }
        public JsonResult editzhuye(int id, bool select)
        {
            var target = Context.BBSArticles.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                target.HomeShow = select;
                Context.SaveChanges();
                return Json(new { ResultCode = 0, message = "修改成功" });

            }
            else
            {
                return Json(new { ResultCode = 0, message = "修改失败" });

            }
        }

        public JsonResult GetCategoryComboData()
        {
            return Json(Context.BBSCategories.Where(s => s.Enable).OrderBy(s => s.SN).Select(s => new
            {
                v = s.Id,
                t = s.Name
            }).ToList(),JsonRequestBehavior.AllowGet);
        }
        //public JsonResult removeComments(int id)
        //{
        //    var target = Context.BBSComments.FirstOrDefault(s => s.Id == id);
        //    if (target == null)
        //    {
        //        return Json(new { ResultCode = 0, message = "删除失败" });
        //    }
        //    else
        //    {
        //        //var list = Context.BBSComments.Where(s => s.Id == id).ToList();
        //        //list.ForEach(s => { s.RefferId = null; s.RefferHasDeleted = true; });

        //        var ids = target.BBSArticleId;
        //        var shuliang = Context.BBSArticles.FirstOrDefault(s => s.Id == ids).CommentCount;
        //        var listt = Context.BBSArticles.FirstOrDefault(s => s.Id == ids);
        //        listt.CommentCount = shuliang - 1;
        //        Context.BBSComments.Remove(target);
        //        Context.SaveChanges();
        //        return Json(new { ResultCode = 1, message = "删除成功" });
        //    }
        //}
        public JsonResult removeComments(int id)
        {
            var target = Context.BBSComments.FirstOrDefault(s => s.Id == id);
            if (target != null)
            {
                var list = Context.BBSComments.Where(s => s.RefferId == id).ToList();
                list.ForEach(s => { s.RefferId = null; s.RefferHasDeleted = true; });
                Context.BBSComments.Remove(target);
                var ids = target.BBSArticleId;
                var listt = Context.BBSArticles.FirstOrDefault(s => s.Id == ids);
                listt.CommentCount = listt.CommentCount  - 1;
                //Context.SaveChanges();
                try
                {  Context.SaveChanges();

                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
                return Json(new { ResultCode = 1, message = "删除成功" });
            }
            else
            {
                return Json(new { ResultCode = 0, message = "删除失败" });
            }
        }


        public JsonResult GetComments(int id, int page = 1, int rows = 15) {
            XDDDbContext ctx = new XDDDbContext();
            IQueryable<BBSComment> query = ctx.BBSComments;
            if (id != 0)
            {
                query = query.Where(s => s.BBSArticleId == id);
                return Json(new
                {
                    total = query.Count(),
                    rows = query.OrderBy(s => s.CreateTime).ToList().Select(s => new { Id = s.Id, Content = HtmlSaferAnalyser.ToSafeHtml(s.Comment, false), CreateTime = s.CreateTime ,Paths=s.Paths}).Skip((page - 1) * rows).Take(rows)
                });
            }
            return Json(new
            {
                ResultCode = 0,
                message = "获取评论失败"
            });
          
        }
        public JsonResult UploadImg()
        {
            List<String> list = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var p = "/Images/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                string uploadPath = Server.MapPath(p);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string name = DateTime.Now.ToString("HHmmssfff" + Guid.NewGuid().ToString().Split('-')[0]);
                if (ImageWorker.IsValidImage(file))
                {  ImageFormat imageFormat;
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
                    ImageWorker.MakeThumbnail(file, string.Format("{0}{1}{2}", uploadPath, name, Path.GetExtension(file.FileName).ToLower()), 900, 9999, "W", imageFormat);
                    string domain = WebConfigurationManager.AppSettings["domain"];
                    list.Add(domain + p + name + Path.GetExtension(file.FileName).ToLower());
                }

            }
            return Json(new
            {
                errno = 0,
                data = list
            });

        }
    }
   


}