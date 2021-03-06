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
       [Authorize(Roles = "管理员,票务管理")]
    public class AgentController : Controller
    {
        XDDDbContext cxt = new XDDDbContext();
        /// <summary>
        /// 代理列表
        /// </summary>
        /// <returns></returns>
     
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(string keyword, int page = 1, int rows = 15)
        {
            IQueryable<Member> query = cxt.Members.Where(s=>s.Status.HasFlag(MemberStatus.Agant)&&s.IsVirtualMember==false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => (s.NickName.Contains(keyword)||s.Captain.RealName.Contains(keyword) || s.RealName.Contains(keyword) || s.PlatformBindPhone.Contains(keyword)));
            }
            return Json(new
            {
                total = query.Count(),
                rows = query.OrderBy(s => s.CaptainId).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Id,
                    s.AppOpenId,
                    s.WebOpenId,
                    s.UnionId,
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
                    s.Session_key,
                    s.Status,
                    s.CaptainId,
                    CaptionName=s.CaptainId==null?null:s.Captain.RealName,
                    s.IsVirtualMember,
                  

                }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTicketOrder( int id, DateTime? start, DateTime? end, string keyword = null, int page = 1, int rows = 10)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<TicketOrder> list = new List<TicketOrder>();
            IQueryable<TicketOrder> query;
            query = ctx.TicketOrders.Where(s => s.AgentId == id && (s.Status == OrderStatus.CheckOut || s.Status == OrderStatus.Paied || s.Status == OrderStatus.Success));
            if (start.HasValue)
            {
                query = query.Where(s => s.CreateTime >= start.Value);
            }
            if (end.HasValue)
            {
                end = end.Value.AddDays(1);
                query = query.Where(s => s.CreateTime < end.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                //有传递keyword并且keyword的值不为空字符串
                query = query.Where(s => s.OrderNum.Contains(keyword));
            }//查询
                return Json(
            new
            {
                total = query.Count(),
                totalMoney=query.Sum(s=>(decimal?)s.OrderPrice)??0,
                rows = query.OrderByDescending(s=>s.CreateTime).Skip((page - 1) * rows).Take(rows).ToList().Select(s => new
               {
                   s.Id,
                   s.OrderNum,
                   s.OrderPrice,
                   s.CreateTime,
                   s.Status,
                   TicetPackage = s.TicetPackage.Ticket.Name+"-"+ s.TicetPackage.Name,
                   s.L1BalanceCharges,
                   s.L2BalanceCharges
                   //获取数据
               })

            }, JsonRequestBehavior.AllowGet);
        }

    }
}

