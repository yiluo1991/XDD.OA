using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API

{
    /// <summary>
    /// 核销订单导出
    /// </summary>
    [RoutePrefix("api/Cheakoutexport")]
    public class CheakOutExportController : ApiController
    {
        [TokenAuthorize]
        [Route("Export"), HttpGet]
        public  HttpResponseMessage Export(string keyword="",DateTime? datatime=null,string selected="")
        {
            XDDDbContext ctx = new XDDDbContext();
            List<TicketOrder> list = new List<TicketOrder>();
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            IQueryable<TicketOrder> query = ctx.TicketOrders.Where(s => s.Supplier.MemberId == id);
            var filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
            if (string.IsNullOrEmpty(keyword))
            {
              
            }
            else
            {
                //有传递keyword并且keyword的值不为空字符串
                query = ctx.TicketOrders.Where( s => s.RealName.Contains(keyword) || s.Mobile.Contains(keyword) || s.Address.Contains(keyword) || s.PackageCode.Contains(keyword));
            }
            if (datatime.HasValue)
            {
                query = query.Where(s => s.UseDate == datatime.Value);
            }
            switch (selected) {
                case "1":
                    query = query.Where(s => s.Status == OrderStatus.Paied || s.Status == OrderStatus.Success);
                     filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes("未核销记录导出"+DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
                    break;
                case "2":
                    query = query.Where(s => s.Status == OrderStatus.Handled);
                     filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes("已处理记录导出"+DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
                    break;
                case "3":
                    query = query.Where(s => s.Status == OrderStatus.CheckOut);
                    filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes("已核销记录导出" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
                    break;
                case "4":
                    query = query.Where(s => s.Status == OrderStatus.TurnBack || s.HasRequest);
                    filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes("退款单记录导出" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx"));
                    break;
                default:
                    break;
            }

            var orders = query.Select(s => new
            {
                s.RealName,
                s.CreateTime,
                s.UseDate,
                s.OrderNum,
                s.OrderPrice,
                s.Price,
                Status = s.Status,
                s.Quantity,
                s.Mobile,
                s.PackageCode,
                TicetPackage = new { s.TicetPackage.Name, TicketName = s.TicetPackage.Ticket.Name },

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
                string[] status = new string[7] { "未付款", "已付款", "付款超时", "已出票", "已处理", "已核销", "退票" };
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
                    "取件码",
                };
                for (int i = 0; i < headers.Count; i++)
                {
                    dataRow0.CreateCell(i, CellType.String).SetCellValue(headers[i]);
                }
                for (int i = 0; i < orders.Count; i++)
                {
                    XSSFRow row = (XSSFRow)sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(orders[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(1).SetCellValue(orders[i].OrderNum);
                    row.CreateCell(2).SetCellValue(status[(int)orders[i].Status]);
                    row.CreateCell(3).SetCellValue(orders[i].TicetPackage.TicketName + "-" + orders[i].TicetPackage.Name);
                    row.CreateCell(4).SetCellValue((double)orders[i].OrderPrice);
                    row.CreateCell(5).SetCellValue((double)orders[i].Price);
                    row.CreateCell(6).SetCellValue(orders[i].Quantity);
                    row.CreateCell(7).SetCellValue(orders[i].RealName);
                    row.CreateCell(8).SetCellValue(orders[i].Mobile);
                    row.CreateCell(9).SetCellValue(orders[i].PackageCode);
                }
                for (int i = 0; i <= 16; ++i)
                    sheet.AutoSizeColumn(i);
                workbook.Write(ms);
                ms.Flush();
            }
            catch (Exception e){

            }

            //var filename = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddhhmmss") ));
            //Response.BinaryWrite(ms.ToArray());
            //Response.Flush();
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =   new ByteArrayContent(ms.ToArray()),
            };
            resp.Content.Headers.Add("Content-Disposition", string.Format("attachment;filename=" + filename));
            resp.Content.Headers.Add("Content-Length", ms.ToArray().Length.ToString());
            resp.Content.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;chartset=utf-8");
            //resp.Content.Headers.Add("Content-Type", "application/vnd.ms-excel;charset=utf-8");

            return resp;

        }
    }
}
