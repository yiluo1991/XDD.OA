using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using System.Web.Configuration;
namespace XDD.Web.Controllers
{

        [Authorize(Roles = "管理员")]
    public class PlatformStatementController : Controller
    {
        // GET: PlatformStatement
        [Authorize(Roles = "管理员")]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetStatements(String show, String Begin, String End, String Keyword, int page, int rows)
        {
            if (String.IsNullOrEmpty(show))
                show = "true";
            XDDDbContext ctx = new XDDDbContext();
            if (show == "true")
            {
                IQueryable<AccountStatement> query;
                var typestring = WebConfigurationManager.AppSettings["IncomeKeys"];
                List<string> types = typestring.Split('|').ToList();
                query = ctx.AccountStatements.Where(s => types.Contains(s.Type));
                DateTime bt;
                DateTime et;
                if (String.IsNullOrEmpty(Begin))
                    bt = DateTime.MinValue;
                else bt = Convert.ToDateTime(Begin);
                if (String.IsNullOrEmpty(End))
                    et = DateTime.MaxValue;
                else et = Convert.ToDateTime(End).AddDays(1);
                query = query.Where(s => DateTime.Compare(s.CreateTime, bt) >= 0 && DateTime.Compare(s.CreateTime, et) <= 0);
                if(!String.IsNullOrEmpty(Keyword))
                {
                    query = query.Where(s => s.Member.RealName.Contains(Keyword)||s.Member.NickName.Contains(Keyword));
                }
                Decimal? totalAmount = query.Sum(s => (Decimal?)s.Money);
                return Json(
                  new
                  {
                      total = query.Count(),
                      rows = query.OrderByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                      {   //获取数据
                          s.Type,
                          MemberName = s.Member.RealName==null?s.Member.NickName:s.Member.RealName,
                          s.Money,
                          s.CreateTime,
                      }),
                      totalAmount
                  }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IQueryable<Withdraw> query;
                query = ctx.Withdraws.Where(s => s.Status==WithdrawStatus.Success);
                DateTime bt;
                DateTime et;
                if (String.IsNullOrEmpty(Begin))
                    bt = DateTime.MinValue;
                else bt = Convert.ToDateTime(Begin);
                if (String.IsNullOrEmpty(End))
                    et = DateTime.MaxValue;
                else et = Convert.ToDateTime(End);
                query = query.Where(s => DateTime.Compare(s.CreateTime, bt) >= 0 && DateTime.Compare(s.CreateTime, et) <= 0);
                if (!String.IsNullOrEmpty(Keyword))
                {
                    query = query.Where(s => s.Member.RealName.Contains(Keyword)||s.Member.NickName.Contains(Keyword));
                }
                Decimal? totalAmount = query.Sum(s => (Decimal?)s.Money);
                return Json(
                  new
                  {
                      total = query.Count(),
                      rows = query.OrderByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
                      {   //获取数据
                          Type = "提现",
                          MemberName = s.Member.RealName,
                          s.Money,
                          s.CreateTime,
                      }),
                      totalAmount
                  }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}