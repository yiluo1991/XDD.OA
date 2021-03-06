using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员")]
    public class MessageController : Controller
    {
        // GET: Message
        [Authorize(Roles = "管理员")]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Get(string keyword = null, int page = 1, int rows = 10)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<Message> list = new List<Message>();
            IQueryable<Message> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.Messages;
                
                //没有传递keyword或者keyword为""
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.Messages.Where(s => s.FromId.ToString().Contains(keyword) || s.ToId.ToString().Contains(keyword) || s.SendTime.ToString().Contains(keyword) || s.Content.Contains(keyword)||s.From.NickName.Contains(keyword)||s.From.RealName.Contains(keyword)||s.To.NickName.Contains(keyword)||s.To.RealName.Contains(keyword)); 
            }//查询
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderByDescending(s => s.SendTime).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Id,
                    FromName=s.FromId==null?null:new { s.From.NickName, s.FromId,s.From.RealName},
                    ToName = s.ToId == null ? null : new { s.To.NickName, s.ToId ,s.To.RealName},
                    //从他表获取数据
                    s.SendTime,
                    s.Content,
                    s.HasRead,
                   
                    //获取数据
                })

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Account( int Id,string keyword = null, int page = 1, int rows = 10)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<Member> list = new List<Member>();
            IQueryable<Member> query;
            query = ctx.Members;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.Members;

                //没有传递keyword或者keyword为""
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.Members.Where(s => s.NickName.Contains(keyword) );
            }//查询
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.Id).Select(s => new
                {
                    s.NickName,
                    

                    //获取数据
                })

            }, JsonRequestBehavior.AllowGet);
        }

    }
    }