using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;

namespace XDD.Web.Controllers
{
     [Authorize(Roles = "管理员,二手管理")]
    public class CommodityController : Controller
    {
        XDDDbContext ctx = new XDDDbContext();
        // GET: Commodity
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetComodities(string keyword,TradeType? type,int page=1,int rows=10) {
            IQueryable<Commodity> query = ctx.Commodities.Where(s => s.Enable==true);
            int id;
            var isNum = Int32.TryParse(keyword, out id);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword) || s.Member.NickName.Contains(keyword)||s.CommodityCategory.Name.Contains(keyword)||(isNum?s.MemberId==id:false));
            }
            if (type.HasValue)
            {
                query = query.Where(s => s.Type ==type.Value);
            }
            var total = query.Count();
            IOrderedQueryable<Commodity> orderedQuery = query.OrderBy(s => s.SN).ThenByDescending(s=>s.CreateTime);
            return Json(new
            {
                total = total,
             
                rows = orderedQuery.Skip((page - 1) * 10).Take(rows).Select(s => new
                {
                    s.CommodityCategoryId,
                    s.Cover,
                    s.Content,
                    CommodityCategoryName = s.CommodityCategory.Name,
                    s.CreateTime,
                    s.Enable,
                    s.Id,
                    s.Member.NickName,
                    s.MemberId,
                    s.Name,
                    s.NewLevel,
                    s.OnSale,
                    s.Paths,
                    s.Price,
                    s.SN,
                    s.Type,
                    s.ViewCount,
                   
                }).ToList()
            });
        }

        public JsonResult Disable(int id)
        {
            var target = ctx.Commodities.FirstOrDefault(s => s.Id == id);
            if (null!=target)
            {
                target.OnSale = false;
                target.Enable = false;
                ctx.SaveChanges();
                return Json(new { ResultCode = 1, message = "操作成功" });
            }
            else
            {
                return Json(new { ResultCode = 0, message = "没有找到要修改的信息" });
            }
        }
     }
}