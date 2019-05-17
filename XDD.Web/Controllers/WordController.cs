using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{

    [Authorize(Roles = "管理员,社交管理")]
    public class WordController : Controller
    {

        // GET: Word

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Get(string keyword, string id, int page = 1, int rows = 15)
        {
            XDDDbContext ctx = new XDDDbContext();
            IQueryable<Word> query = ctx.Words;

            if (id == null)
            {
                if (keyword != null)
                {
                    return Json(new
                    {
                        total = query.Where(s => s.Content.Contains(keyword)).Count(),
                        rows = query.Where(s => s.Content.Contains(keyword)).ToList().Select(s => new
                        {
                            Id = s.Id,
                            Content = HtmlSaferAnalyser.ToSafeHtml(s.Content, false),
                            SN = s.SN,
                            CreateTime = s.CreateTime,
                            Tags = s.Tags
                        }).OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows)
                    });
                }
                else {
                    return Json(new
                    {
                        total = query.Count(),
                        rows = query.ToList().Select(s => new { Id = s.Id, Content = HtmlSaferAnalyser.ToSafeHtml(s.Content, false), SN = s.SN, CreateTime = s.CreateTime, Tags = s.Tags }).OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows)
                    });

                }
               
            }
            else
            {
                int bianhao = Convert.ToInt32(id);
                return Json(new
                {
                    total = query.Where(s => s.Id == bianhao).Count(),
                    rows = query.ToList().Select(s => new { Id = s.Id, Content = HtmlSaferAnalyser.ToSafeHtml(s.Content, false), SN = s.SN, CreateTime = s.CreateTime, Tags = s.Tags }).Where(s => s.Id == bianhao).OrderBy(s => s.SN).ThenBy(s => s.CreateTime)
                });
            }
        }
        public JsonResult GetComments(int id) {
            XDDDbContext ctx = new XDDDbContext();
            
            return Json(ctx.WordComments.Where(s=>s.WordId==id).OrderBy(s => s.CreateTime).ToList().Select(s=> new{Id= s.Id,Content=HtmlSaferAnalyser.ToSafeHtml( s.Content,false),CreateTime=s.CreateTime}));
        }
        public JsonResult Remove(List<int> ids)
        {
            XDDDbContext ctx = new XDDDbContext();
            var list = ctx.Words.Where(s => ids.Contains(s.Id)).ToList();
            for (int i=0;i<ids.Count;i++) {
              list.Where(s => s.RefferId == ids[i]);
            }
            list.ForEach(s => { s.RefferId = null; s.RefferHasDeleted = true; });
            ctx.Words.RemoveRange(list);
            ctx.SaveChanges();
            return Json(new { ResultCode = 1, message = "保存成功" });
        }
        public JsonResult removeComments(List<int> ids)
        {
            XDDDbContext ctx = new XDDDbContext();
            var list = ctx.WordComments.Where(s => ids.Contains(s.Id)).ToList();
            var id = list[0].WordId;
            var shuliang= ctx.Words.FirstOrDefault(s => s.Id == id).CommentsCount;
            var listt = ctx.Words.FirstOrDefault(s => s.Id == id);
            listt.CommentsCount = shuliang - 1;
            ctx.WordComments.RemoveRange(list);
            ctx.SaveChanges();
          
            return Json(new { ResultCode = 1, message = "保存成功" });
        }

        public JsonResult editpaixu(Word word) {
            XDDDbContext ctx = new XDDDbContext();
            var list = ctx.Words.FirstOrDefault(s => s.Id == word.Id);
            if (list != null) {
                list.SN = word.SN;
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "修改成功" });
            }
            else
            {
                return Json(new { ResultCode = 0, message = "修改失败" });
            }
        }
    }
}