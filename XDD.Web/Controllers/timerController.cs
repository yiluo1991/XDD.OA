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

    public class TimerController : Controller
    {
        // GET: timer
        public JsonResult Index()
        {
            XDDDbContext ctx = new XDDDbContext();
            var now = DateTime.Now;
            var orders = ctx.TicketOrders.Where(s => DbFunctions.DiffMinutes(s.CreateTime, now) > (2 * 60 - 6) && s.Status == OrderStatus.None).ToList();
            var corders = ctx.CommodityOrders.Where(s => DbFunctions.DiffMinutes(s.CreateTime, now) > (2 * 60 - 6) && s.CommodityOrderStatus == CommodityOrderStatus.None).ToList();
            orders.ForEach(s => { s.Status = OrderStatus.Fail; });

            corders.ForEach(s => {

                s.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：支付超时，交易关闭";
                s.CommodityOrderStatus = CommodityOrderStatus.TotalCancel; 
            
            });
            try
            {
                ctx.SaveChanges();
                return Json(new { Message = "处理成功" },JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "事务冲突" },JsonRequestBehavior.AllowGet);
            }
      
        }
    }
}