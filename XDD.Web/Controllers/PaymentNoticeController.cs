using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Payment;
using System.Transactions;
using XDD.Core.Model;
using System.Text;
using XDD.Web.Infrastructure;
namespace XDD.Web.Controllers
{
    public class PaymentNoticeController : Controller
    {
        // GET: PaymentNotice
        public ContentResult Index()
        {
            PayOrderResult result = Payment.PaymentProvider.ReviceNotice();
            if (result.OrderSN.StartsWith("T"))
            {
                if (result.Success)
                {
                    XDDDbContext ctx = new XDDDbContext();
                    var now = DateTime.Now;
                    var order = ctx.TicketOrders.FirstOrDefault(s => s.OrderNum == result.OrderSN && s.Member.AppOpenId == result.OpenId && (s.Status == OrderStatus.None || s.Status == OrderStatus.Fail) && s.Supplier.MemberId != null && s.OrderPrice == result.Money);
                    if (order != null)
                    {
                        PayHandler.PaySuccess(now, result, order, ctx);
                    }
                }
            }
            else if(result.OrderSN.StartsWith("C"))
            {
                if (result.Success)
                {
                    XDDDbContext ctx = new XDDDbContext();
                    var now = DateTime.Now;
                    var order = ctx.CommodityOrders.FirstOrDefault(s => s.OrderNum == result.OrderSN && s.Member.AppOpenId == result.OpenId && (s.CommodityOrderStatus == CommodityOrderStatus.None || s.CommodityOrderStatus == CommodityOrderStatus.Fail||s.CommodityOrderStatus==CommodityOrderStatus.TotalCancel)  && s.OrderPrice == result.Money);
                    if (order != null)
                    {
                        PayHandler.PayCommoditySuccess(now, result, order, ctx);
                    }
                }
            }
          
            return Content("<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>");

        }
    }
}