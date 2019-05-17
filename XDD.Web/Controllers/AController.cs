using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XDD.Web.Controllers
{
    public class AController : Controller
    {
        // GET: A
        public ActionResult Index()
        {
            SMS.SMSManager.SendNoticeSMS(new List<string> { "", "一笔新的门票服务订单退款申请", "18559819573", "-票券商核销平台" },"18559819573");
            return View();
        }
    }
}