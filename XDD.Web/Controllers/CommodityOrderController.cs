using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{
     [Authorize(Roles = "管理员,二手管理")]
    public class CommodityOrderController : Controller
    {
         XDDDbContext ctx = new XDDDbContext();
        // GET: commodityOrder
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrders(string keyword,CommodityOrderStatus? status,int page=1,int rows=10) {
            IQueryable<CommodityOrder> query = ctx.CommodityOrders;
            int id;
            var isNum = Int32.TryParse(keyword, out id);
            if (status.HasValue)
            {
                query = query.Where(s => s.CommodityOrderStatus == status.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.RealName.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.Commodity.Name.Contains(keyword) || s.Commodity.Member.NickName.Contains(keyword) || s.Commodity.Member.RealName.Contains(keyword) || s.BackRealName.Contains(keyword) || s.Mobile.Contains(keyword) || s.BackMobile.Contains(keyword)||s.OrderNum.Contains(keyword) || (isNum ? (s.MemberId == id || s.Commodity.MemberId == id) : false));
            }
            return Json(new{
             total = query.Count(),
                rows = query.OrderByDescending(s => s.CreateTime).Skip((page - 1) * 10).Take(rows).Select(s => new
                {
                    s.Province,
                    s.Area,
                    s.City,
                    s.OrderPrice,
                    s.RealName,
                    s.Mobile,
                    s.Commodity.NewLevel,
                    s.Commodity.Cover,
                    s.Address,
                    s.CreateTime,
                    s.Id,
                    CommodityName = s.Commodity.Name,
                    SaleMemberName = s.Commodity.Member.NickName,
                    BuyMemberName = s.Member.NickName,
                    SaleMemberId = s.Commodity.MemberId,
                    BuyMemberId = s.MemberId,
                    s.BackAddress,
                    s.BackArea,
                    s.BackCity,
                    s.BackDeliveryTime,
                    s.BackExpressName,
                    s.BackExpressNo,
                    s.BackFeedback,
                    s.BackMobile,
                    s.BackPicthres,
                    s.BackProvince,
                    s.BackQequest,
                    s.BackRealName,
                    s.CommodityId,
                    s.CommodityOrderStatus,
                    s.DeliveryTime,
                    s.ExpressName,
                    s.ExpressNo,
                    s.Freeze,
                    s.OrderNum,
                    s.Remark
                }).ToList()
            });

        }


        public JsonResult TotalSuccess(int id)
        {
            var target = ctx.CommodityOrders.Where(s => s.CommodityOrderStatus != CommodityOrderStatus.BackDelivery && s.CommodityOrderStatus != CommodityOrderStatus.Delivery && s.CommodityOrderStatus != CommodityOrderStatus.None && s.CommodityOrderStatus != CommodityOrderStatus.Fail && s.CommodityOrderStatus != CommodityOrderStatus.TotalCancel && s.CommodityOrderStatus != CommodityOrderStatus.TotalSuccess && s.Id == id).FirstOrDefault();
            if (target != null)
            {
                var now = DateTime.Now;
                target.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：平台客服将交易设置为完成";
                target.CommodityOrderStatus = CommodityOrderStatus.TotalSuccess;
                PayHandler.CommodityOrderSuccessWidthoutSaveChange(target, now, ctx);
                ctx.SaveChanges();
                return Json(new
                {
                    ResultCode =1,
                    Message = "操作成功"
                });
            }
            else
            {
                return Json(new { 
                    ResultCode=0,
                    Message="操作失败,没有找到记录或记录不在可修改的状态"
                });
            }
        }
        public JsonResult TotalCancel(int id)
        {
            var target = ctx.CommodityOrders.Where(s => s.CommodityOrderStatus != CommodityOrderStatus.BackDelivery && s.CommodityOrderStatus != CommodityOrderStatus.Delivery && s.CommodityOrderStatus != CommodityOrderStatus.None && s.CommodityOrderStatus != CommodityOrderStatus.Fail && s.CommodityOrderStatus != CommodityOrderStatus.TotalCancel && s.CommodityOrderStatus != CommodityOrderStatus.TotalSuccess && s.Id == id).FirstOrDefault();
            if (target != null)
            {
                var now = DateTime.Now;
                target.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：平台客服关闭了交易";
                target.CommodityOrderStatus = CommodityOrderStatus.TotalCancel;
                PayHandler.CommodityOrderCancelWidthoutSaveChange(target, now, ctx);
                ctx.SaveChanges();
                return Json(new
                {
                    ResultCode = 1,
                    Message = "操作成功"
                });
            }
            else
            {
                return Json(new
                {
                    ResultCode = 0,
                    Message = "操作失败,没有找到记录或记录不在可修改的状态"
                });
            }
        }

        public JsonResult Freezen(int id)
        {
            var target = ctx.CommodityOrders.Where(s => s.CommodityOrderStatus != CommodityOrderStatus.BackDelivery && s.CommodityOrderStatus != CommodityOrderStatus.Delivery && s.CommodityOrderStatus != CommodityOrderStatus.None && s.CommodityOrderStatus != CommodityOrderStatus.Fail && s.CommodityOrderStatus != CommodityOrderStatus.TotalCancel && s.CommodityOrderStatus != CommodityOrderStatus.TotalSuccess && s.Id == id).FirstOrDefault();
            if (target != null)
            {
                var now = DateTime.Now;
                if (target.Freeze)
                {
                    target.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：平台客服解除了交易锁定，交易继续";
                }
                else
                {
                    target.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：平台客服锁定了该交易，用户无法进行操作";
                }

                target.Freeze = !target.Freeze;
              
                ctx.SaveChanges();
                return Json(new
                {
                    ResultCode = 1,
                    Message = "操作成功"
                });
            }
            else
            {
                return Json(new
                {
                    ResultCode = 0,
                    Message = "操作失败,没有找到记录或记录不在可修改的状态"
                });
            }
        }
     }
}