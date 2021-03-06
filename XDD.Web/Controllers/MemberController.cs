using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员")]
    public class MemberController : Controller
    {
        
        /// <summary>
        ///     会员列表
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles="管理员")]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Get(string keyword = null, int page = 1, int rows = 10)
        {
            var sexList = new List<string> { "女", "男" };
            int keywordId;
            bool isId = int.TryParse(keyword, out keywordId);
            XDDDbContext ctx = new XDDDbContext();
            List<Member> list = new List<Member>();
            IQueryable<Member> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.Members;
                //没有传递keyword或者keyword为""
            }
            else
            {
                int index = sexList.FindIndex(s => s == keyword);
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.Members.Where(s =>(isId?s.Id==keywordId:false)|| s.NickName.Contains(keyword) || s.Country.Contains(keyword) || s.RealName.Contains(keyword) || s.Province.Contains(keyword) || s.City.Contains(keyword) || s.WeChatBindPhone.Contains(keyword) || s.PlatformBindPhone.Contains(keyword) || (int)s.Sex == index);
               // query = ctx.Members.Where(s => (int)s.Sex == index);
            }//查询
            query = query.Where(s => s.IsVirtualMember == false);
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.Id).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                {
                    s.Id,
                    s.NickName,
                    s.AvatarUrl,
                    s.RealName,
                    s.Sex,
                    s.Country,
                    s.Province,
                    s.City,
                    s.WeChatBindPhone,
                    s.PlatformBindPhone,
                    s.Account,
                    s.Status
                    //获取数据
                })

            }, JsonRequestBehavior.AllowGet);
        } //启用和禁用
        public bool put(int id, Boolean blea)
        {
            XDDDbContext ctx = new XDDDbContext();
            Member tager = ctx.Members.FirstOrDefault(s => s.Id == id);
            if (tager != null)
            {
                if (tager.Status.HasFlag(MemberStatus.Freeze))
                {
                    tager.Status -= MemberStatus.Freeze;
                }
                else
                {
                    tager.Status |= MemberStatus.Freeze;
                }

            }
            ctx.SaveChanges();
            return true;
        }
        public JsonResult Account(int id,DateTime? start, DateTime? end,string keyword = null, int page = 1, int rows = 10 )
        {
            XDDDbContext ctx = new XDDDbContext();
            List<AccountStatement> list = new List<AccountStatement>();
            IQueryable<AccountStatement> query;
            query = ctx.AccountStatements.Where(s=>s.MemberId==id);
            if (start.HasValue)
            {
                query = query.Where(s => s.CreateTime >= start.Value);
            }
            if (end.HasValue)
            {
                end=end.Value.AddDays(1);
                query = query.Where(s => s.CreateTime < end.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                //有传递keyword并且keyword的值不为空字符串
                query =query.Where(s => s.Type.Contains(keyword));
            }
            
            //查询
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderByDescending(s=>s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                {
                    s.Id,
                    MemberName=s.Member.NickName,
                    s.BeforeBalance,
                    s.Money,
                    s.CreateTime,
                    s.Type,
                    s.RefferId
                    //获取数据
                })

            }, JsonRequestBehavior.AllowGet);
        }

    }

}