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
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize(Roles= "管理员,二手管理,社交管理,票务管理")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetChart()
        {
            XDDDbContext ctx = new XDDDbContext();
            long dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
            var list = ctx.TicketOrders.Where(s => s.Status != OrderStatus.None && s.Status != OrderStatus.Fail && s.Status != OrderStatus.TurnBack).Select(s => new { p = s.OrderPrice, s = s.TicetPackage.Ticket.ServiceType, t = DbFunctions.TruncateTime(s.CreateTime) }).ToList();
            var moneyList = list.GroupBy(s => s.t).OrderBy(s=>s.Key.Value).Select(s => new List<object> { (s.Key.Value.Ticks - dtFrom) / 10000, s.Sum(t => t.p) }).ToList();
            var countList = list.GroupBy(s => s.t).OrderBy(s => s.Key.Value).Select(s => new List<object> { (s.Key.Value.Ticks - dtFrom) / 10000, s.Count() }).ToList();
            return Json(new List<object>{
              new{   name="订单金额", data=moneyList},
            new{   name="订单数量", data=countList}
           }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Info() {
           XDDDbContext ctx = new XDDDbContext();
           DateTime today = DateTime.Now.Date;
           DateTime oneMonth = today.AddMonths(-1);
           return Json(new
           {
               OrderInfo = new { 
                   Today=  ctx.TicketOrders.Where(s=> s.Status != OrderStatus.None && s.Status != OrderStatus.Fail && s.Status != OrderStatus.TurnBack&&s.CreateTime>today).Count(),
                   Month = ctx.TicketOrders.Where(s => s.Status != OrderStatus.None && s.Status != OrderStatus.Fail && s.Status != OrderStatus.TurnBack && s.CreateTime > oneMonth).Count(),
                   UnCheckOut=ctx.TicketOrders.Where(s => s.Status != OrderStatus.None && s.Status != OrderStatus.Fail && s.Status != OrderStatus.TurnBack&&s.Status!=OrderStatus.CheckOut ).Count()
               },
               CommodityInfo = new
               {
                   Today = ctx.CommodityOrders.Where(s => s.CommodityOrderStatus != CommodityOrderStatus.None && s.CommodityOrderStatus != CommodityOrderStatus.Fail && s.CommodityOrderStatus != CommodityOrderStatus.TotalCancel && s.CommodityOrderStatus != CommodityOrderStatus.BackDelivery && s.CreateTime > today).Count(),
                   Month = ctx.CommodityOrders.Where(s => s.CommodityOrderStatus != CommodityOrderStatus.None && s.CommodityOrderStatus != CommodityOrderStatus.Fail && s.CommodityOrderStatus != CommodityOrderStatus.TotalCancel && s.CommodityOrderStatus != CommodityOrderStatus.BackDelivery && s.CreateTime > oneMonth).Count(),
                   Freeze = ctx.CommodityOrders.Where(s => s.Freeze).Count()
               },
               More = new
               {
                   Identity = ctx.IdentityVerifies.Where(s => s.Status==VerifyStatus.None).Count(),
                   Agent = ctx.AgentApplys.Where(s => s.Status==VerifyStatus.None).Count(),
                   Withdraw = ctx.Withdraws.Where(s =>s.Status==WithdrawStatus.None).Count()
               },

           }, JsonRequestBehavior.AllowGet);
        }
    }
}