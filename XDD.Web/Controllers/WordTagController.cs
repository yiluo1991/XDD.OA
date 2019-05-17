using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员,社交管理")]
    public class WordTagController : Controller
    {
        // GET: WordTag
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword)
        {
            XDDDbContext ctx = new XDDDbContext();
            if (keyword != null)
            {
                return Json(ctx.WordTags.Where(s => s.Name.Contains(keyword)).OrderBy(s => s.SN).ToList());
            }
            return Json(ctx.WordTags.OrderBy(s => s.SN).ToList());
        }
        public JsonResult GetTags( string keyword, DateTime? start, DateTime? end) {
            XDDDbContext ctx = new XDDDbContext();
            var query = ctx.Words.Select(s=>new { Id = s.Id, CreateTime = s.CreateTime,Tags=s.Tags });
            if (end != null)
            {end = end.Value.AddDays(1);
                query = query.Where(s => s.CreateTime < end.Value);
            }
            if (start != null)
            {
                query = query.Where(s => s.CreateTime > start.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(s => s.Tags.Contains(keyword));
            }
            var lists = query.ToList();
            Dictionary < string,int> dic = new Dictionary<string, int>();
            foreach (var i in lists) {
                //string[] sArray = i.Tags.Split('#');
                //if (!dic.ContainsKey(sArray[1]))
                //{
                //    dic.Add(sArray[1], 1);
                //}
                //else {
                //    dic[sArray[1]]++;
                //}
                var matchs = Regex.Matches(i.Tags, @"#(?<Tag>[^#\|]{1,})#");
                foreach (Match match in matchs)
                {
                    if (!dic.ContainsKey(match.Groups["Tag"].Value))
                    {
                        dic.Add(match.Groups["Tag"].Value, 1);
                    }
                    else
                    {
                        dic[match.Groups["Tag"].Value]++;
                    }
                }
            }
           var listss= dic.OrderByDescending(s=>s.Value);
           return Json(listss);
        }

        public JsonResult AddWordTag(WordTag tag) {
            XDDDbContext ctx = new XDDDbContext();
            var targer = ctx.WordTags.FirstOrDefault(s => s.Id == tag.Id);
            if (targer == null)
            {
                ctx.WordTags.Add(tag);
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "添加成功" });
            }
            else {
                return Json(new { ResultCode = 0, message = "添加失败" });

            }

        }
        public JsonResult Add(List<string> Tags, WordTag tag)
        {
            XDDDbContext ctx = new XDDDbContext();
            foreach (var i in Tags) {
                var target = ctx.WordTags.AsNoTracking().FirstOrDefault(s => s.Name ==i);
                if (target == null) {
                    tag.Name = i;
                    ctx.WordTags.Add(tag);
                    ctx.SaveChanges();
                }
            }
            //var target = ctx.WordTags.Where(s => Tags.Contains(s.Name)).ToList();
            //if (target == null) {
            return Json(new { ResultCode = 1, message = "添加成功" });
            //}
            //else {
            //    return Json(new { ResultCode = 0, Message = "添加失败" });
            //    }
        
        }

        public JsonResult Edit(WordTag tag)
        {
            XDDDbContext ctx = new XDDDbContext();
            var target = ctx.WordTags.FirstOrDefault(s => s.Id == tag.Id);
            if (target == null)
            {
                return Json(new { ResultCode = 0, message = "没有找到要修改的项" });
            }
            else
            {
                target.Name = tag.Name;
                target.SN = tag.SN;
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "修改成功" });
            }
        }

        public JsonResult Remove(List<int> ids) {
            XDDDbContext ctx = new XDDDbContext();
            var list = ctx.WordTags.Where(s =>ids.Contains(s.Id)).ToList();
            ctx.WordTags.RemoveRange(list);
            ctx.SaveChanges();
            return Json(new { ResultCode = 1, message = "保存成功" });
        }
    }
}