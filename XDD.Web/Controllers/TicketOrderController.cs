using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;

namespace XDD.Web.Controllers
{

    [Authorize(Roles = "管理员,票务管理")]
    public class TicketOrderController : Controller
    {
        /// <summary>
        ///     购票订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public void Export(string keyword)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<TicketOrder> list = new List<TicketOrder>();
            IQueryable<TicketOrder> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.TicketOrders;
                //没有传递keyword或者keyword为""
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.TicketOrders.Where(s => s.OrderNum.Contains(keyword) || s.RealName.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Agent.RealName.Contains(keyword) || s.Agent.NickName.Contains(keyword) || s.Id.ToString() == keyword || s.TicetPackage.Name.Contains(keyword) || s.Address.Contains(keyword) || s.Mobile.Contains(keyword) || s.PackageCode.Contains(keyword));

            }//查询

            var orders = query.Select(s => new
                {
                    s.Id,
                    s.RealName,
                    s.OrderNum,
                    s.OrderPrice,
                    s.Price,
                    s.CreateTime,
                    s.Quantity,
                    s.Mobile,
                    s.Address,
                    s.PackageCode,
                    s.VCode,
                    s.DeliveryPrice,
                    s.TicetPackage.Ticket.ServiceType,
                    Member = new { s.Member.RealName, s.Member.NickName, s.Member.Id },
                    Status = s.Status,
                    s.AgentId,
                    Agent = s.AgentId == null ? null : new { s.Agent.RealName, s.Agent.NickName, s.Agent.Id },
                    Supplier = s.Supplier.MemberId == null ? null : new { s.Supplier.Member.NickName, s.Supplier.Member.RealName, s.SupplierId, s.Supplier.MemberId },
                    TicetPackage = new { s.TicetPackage.Name, TicketName = s.TicetPackage.Ticket.Name },
                    s.L1BalanceCharges,
                    s.L2BalanceCharges
                    //获取数据
                }).ToList();
            XSSFWorkbook workbook = null;
            MemoryStream ms = null;
            ISheet sheet = null;
            try
            {

           
                workbook = new XSSFWorkbook();
                ms = new MemoryStream();
                sheet = workbook.CreateSheet();
                XSSFRow dataRow0 = (XSSFRow)sheet.CreateRow(0);
                List<string> headers = new List<string> {
                    "下单时间",
                    "订单号",
                    "状态",
                    "门票套餐",
                    "订单价格", 
                    "单价",
                    "数量", 
                    "收货人",
                    "收货人手机号",
                    "地址",
                    "取件码" ,
                    "购票会员ID | 昵称 | 姓名", 
                    "代理会员ID | 姓名",
                    "一级代理佣金",
                    "二级代理佣金", 
                    "供应商ID | 会员ID | 姓名",
                    "供应商货款" };
                for (int i = 0; i < headers.Count; i++)
                {
                    dataRow0.CreateCell(i, CellType.String).SetCellValue(headers[i]);
                }
                string[] status=new string[7]{"未付款","已付款","付款超时","已出票","已处理","已核销","退票"};
                for (int i = 0; i < orders.Count; i++)
                {
                    XSSFRow row = (XSSFRow)sheet.CreateRow(i+1);
                    row.CreateCell(0).SetCellValue(orders[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(1).SetCellValue(orders[i].OrderNum);
                    row.CreateCell(2).SetCellValue(status[(int)orders[i].Status]);
                    row.CreateCell(3).SetCellValue(orders[i].TicetPackage.TicketName + "-" + orders[i].TicetPackage.Name);
                    row.CreateCell(4).SetCellValue((double)orders[i].OrderPrice);
                    row.CreateCell(5).SetCellValue((double)orders[i].Price);
                    row.CreateCell(6).SetCellValue(orders[i].Quantity);
                    row.CreateCell(7).SetCellValue(orders[i].RealName);
                    row.CreateCell(8).SetCellValue(orders[i].Mobile);
                    row.CreateCell(9).SetCellValue(orders[i].Address);
                    row.CreateCell(10).SetCellValue(orders[i].PackageCode);
                    row.CreateCell(11).SetCellValue(orders[i].Member.Id+" | "+orders[i].Member.NickName+" | "+orders[i].Member.RealName);
                    if (orders[i].Agent != null)
                    {
                        row.CreateCell(12).SetCellValue(orders[i].AgentId + " | " + orders[i].Agent.RealName);
                    }
                   
                    row.CreateCell(13).SetCellValue((double)orders[i].L1BalanceCharges);
                    row.CreateCell(14).SetCellValue((double)orders[i].L1BalanceCharges);
                    if (orders[i].Supplier != null)
                    {
                        row.CreateCell(15).SetCellValue(orders[i].Supplier.SupplierId + " | " + orders[i].Supplier.MemberId + " | " + orders[i].Supplier.RealName);
                    }
                    row.CreateCell(16).SetCellValue((double)orders[i].DeliveryPrice);
                }
                    for (int i = 0; i <= 16; ++i)
                        sheet.AutoSizeColumn(i);
                workbook.Write(ms);
                ms.Flush();
            }
            catch (Exception)
            {

            }
            var filename=HttpUtility.UrlEncode(Encoding.UTF8.GetBytes("订单记录导出_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + filename));
            Response.AddHeader("Content-Length", ms.ToArray().Length.ToString());
            Response.AddHeader("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;chartset=utf-8");
            Response.BinaryWrite(ms.ToArray());
            Response.Flush();
            ms.Close();
            ms.Dispose();
        }
        public JsonResult Get(string keyword = null, int page = 1, int rows = 10)
        {
            XDDDbContext ctx = new XDDDbContext();
            List<TicketOrder> list = new List<TicketOrder>();
            IQueryable<TicketOrder> query;
            if (string.IsNullOrEmpty(keyword))
            {
                query = ctx.TicketOrders;
                //没有传递keyword或者keyword为""
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.TicketOrders.Where(s => s.OrderNum.Contains(keyword) || s.RealName.Contains(keyword) || s.Member.RealName.Contains(keyword) || s.Member.NickName.Contains(keyword) || s.Agent.RealName.Contains(keyword) || s.Agent.NickName.Contains(keyword) || s.Id.ToString() == keyword || s.TicetPackage.Name.Contains(keyword) || s.Address.Contains(keyword) || s.Mobile.Contains(keyword) || s.PackageCode.Contains(keyword));

            }//查询
            return Json(
            new
            {
                total = query.Count(),
                rows = query.OrderByDescending(s => s.CreateTime).Skip((page - 1) * rows).Take(rows).Select(s => new
                {
                    s.Id,
                    s.RealName,
                    s.OrderNum,
                    s.OrderPrice,
                    s.Price,
                    s.CreateTime,
                    s.Quantity,
                    s.Mobile,
                    s.Address,
                    s.PackageCode,
                    s.VCode,
                    s.Request,
                    s.HasRequest,
                    s.DeliveryPrice,
                    s.TicetPackage.Ticket.ServiceType,
                    Member = new { s.Member.RealName, s.Member.NickName, s.Member.Id },
                    Status = s.Status,
                    Agent = s.AgentId == null ? null : new { s.Agent.RealName, s.Agent.NickName, s.Agent.Id },
                    Supplier = s.Supplier.MemberId == null ? null : new { s.Supplier.Member.NickName, s.Supplier.Member.RealName, s.SupplierId, s.Supplier.MemberId },
                    TicetPackage = new { s.TicetPackage.Name, TicketName = s.TicetPackage.Ticket.Name },
                    s.L1BalanceCharges,
                    s.L2BalanceCharges
                    //获取数据
                }).ToList()


            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckOut(int id) {
            XDDDbContext ctx = new XDDDbContext();
            var order = ctx.TicketOrders.FirstOrDefault(s => s.Id == id && (s.Status == OrderStatus.Success || s.Status == OrderStatus.Handled));
            if (order != null)
            {
                if (!order.HasRequest)
                {
                    try
                    {
                        var now = DateTime.Now;
                        PayHandler.DealAgentCharges(now, order, ctx);
                        order.Status = OrderStatus.CheckOut;
                        ctx.SaveChanges();
                        return Json( new  { ResultCode = 1, Message = "核销成功" });
                    }
                    catch (Exception)
                    {
                        return Json( new{ ResultCode = 0, Message = "系统忙请重试" });
                    }
                }
                else
                {
                    return  Json( new{ ResultCode = 0, Message = "请先通知供应商处理该订单的退款申请！" });

                }
            }
            else
            {
                return  Json( new { ResultCode = -1, Message = "没有找到订单或订单不在可核销状态" });
            }
        }

    }
}



