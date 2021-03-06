using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Models.WeChat;
using System.Web.Configuration;
using XDD.Web.Infrastructure;
using XDD.Payment;
namespace XDD.Web.Controllers.API
{
    /// <summary>
    ///     门票接口
    /// </summary>
    [RoutePrefix("api/Ticket")]
    public class TicketController : ApiController
    {

        XDDDbContext ctx = new XDDDbContext();

        /// <summary>
        ///     分页获取门票列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetTickets"), HttpPost]
        public PageResponse GetTickets(PageRequest req)
        {
            IQueryable<Ticket> query = ctx.Tickets.Where(s => s.Enable && s.OnSale && s.TicketCategory.Enable);
            if (req.id.HasValue)
            {
                query = query.Where(s => s.TicketCategoryId == req.id.Value);
            }
            if (!string.IsNullOrEmpty(req.keyword))
            {
                query = query.Where(s => s.TicketCategory.Name.Contains(req.keyword) || s.ShopName.Contains(req.keyword) || s.Address.Contains(req.keyword));
            }
            return new PageResponse()
            {
                Total = query.Count(),
                Rows = query.OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Address,
                    s.Name,
                    s.Enable,
                    s.OnSale,
                    s.Pic,
                    s.Price,
                    s.SaleNum,
                    s.ShopName,
                    s.OrginPrice,
                    s.EarlyDay,
                    s.SN,
                    TicketCategoryName = s.TicketCategory.Name,
                    s.Lat,
                    s.Id,
                    s.Tags,
                    s.Lng,
                    s.ServiceType
                }).ToList()
            };
        }

        /// <summary>
        ///     分页获取热门门票，SN小于100
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetHotTickets"), HttpPost]
        public PageResponse GetHotTickets(PageRequest req)
        {
            IQueryable<Ticket> query = ctx.Tickets.Where(s => s.Enable && s.OnSale && s.TicketCategory.Enable && s.SN < 100);
            if (req.id.HasValue)
            {
                query = query.Where(s => s.TicketCategoryId == req.id.Value);
            }
            if (!string.IsNullOrEmpty(req.keyword))
            {
                query = query.Where(s => s.TicketCategory.Name.Contains(req.keyword) || s.ShopName.Contains(req.keyword) || s.Address.Contains(req.keyword)||s.Name.Contains(req.keyword));
            }
            return new PageResponse()
            {
                Total = query.Count(),
                Rows = query.OrderBy(s => s.SN).ThenByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Address,
                    s.Name,
                    s.Enable,
                    s.OnSale,
                    s.Pic,
                    s.Price,
                    s.EarlyDay,
                    s.SaleNum,
                    s.ShopName,
                    s.OrginPrice,
                    s.SN,
                    TicketCategoryName = s.TicketCategory.Name,
                    s.Lat,
                    s.Id,
                    s.Tags,
                    s.Lng,
                    s.ServiceType
                }).ToList()
            };
        }

        /// <summary>
        ///     获取门票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetTicket"), HttpPost]
        public DetailResponse GetTicket(SimpleStatusRequest req)
        {
            var target = ctx.Tickets.FirstOrDefault(s => s.Id == req.Id && s.TicketCategory.Enable && s.Enable);
            if (target != null)
            {
                return new DetailResponse
                {
                    Detail = new
                    {
                        target.Address,
                        target.Content,
                        target.CreateTime,
                        target.Enable,
                        target.Id,
                        target.Lat,
                        target.Lng,
                        target.Name,
                        target.OnSale,
                        target.Pic,
                        target.Price,
                        target.SaleNum,
                        target.Tags,
                        target.OrginPrice,
                        Start = DateTime.Now.AddDays(target.EarlyDay).ToString("yyyy-MM-dd"),
                        target.EarlyDay,
                        target.MoreUrl,
                        target.ServiceType,
                        target.ShopName,
                        target.SN,
                        TicketCategoryName = target.TicketCategory.Name,
                        target.TicketCategoryId,
                        TicketPackages = target.TicketPackages.Where(s => s.OnSale && s.Enable&&s.Supplier.Enable && s.Supplier.MemberId != null && !s.Supplier.Member.Status.HasFlag(MemberStatus.Freeze)).Select(t => new { t.Name, t.Price, t.Stock, t.SN, t.Id }).OrderBy(s => s.SN).ToList()
                    },
                    Message = "获取成功",
                    ResultCode = 1
                };
            }
            else
            {
                return new DetailResponse { Detail = null, Message = "没有找到门票信息", ResultCode = 0 };
            }
        }

        /// <summary>
        ///     获取文章分类列表
        /// </summary>
        /// <returns>文章分类列表</returns>
        [Route("GetCategories")]
        public object GetCategories()
        {
            return ctx.TicketCategories.Where(s => s.Enable).OrderBy(s => s.SN).Select(s => new { Enable = s.Enable, Icon = s.Icon, Id = s.Id, Name = s.Name, s.SN }).ToList();
        }

        /// <summary>
        ///     获取门票
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetPackage"), HttpPost]
        public DetailResponse GetPackage(SimpleStatusRequest req)
        {
            var target = ctx.TicketPackages.FirstOrDefault(s => s.OnSale && s.Id == req.Id && s.Enable && s.Ticket.Enable && s.Ticket.TicketCategory.Enable&&s.Supplier.Enable && s.Supplier.MemberId != null && !s.Supplier.Member.Status.HasFlag(MemberStatus.Freeze));
            if (target != null)
            {
                return new DetailResponse
                {
                    Detail = new
                    {
                        OnSale = target.OnSale && target.Ticket.OnSale,
                        target.Id,
                        target.Name,
                        target.Price,
                        target.Ticket.Tags,
                        target.Stock,
                        target.LastDay,
                        target.TicketId,
                        target.Ticket.EarlyDay,
                        target.Ticket.ServiceType,
                        Start = DateTime.Now.AddDays(target.Ticket.EarlyDay).ToString("yyyy-MM-dd"),
                        End = DateTime.Now.AddDays(target.LastDay).ToString("yyyy-MM-dd"),
                    },
                    Message = "获取成功",
                    ResultCode = 1
                };
            }
            else
            {
                return new DetailResponse { Detail = null, Message = "没有找到门票信息", ResultCode = 0 };
            }
        }

        /// <summary>
        ///     提交订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("SubmitOrder"), HttpPost]
        [TokenAuthorize]
        public TicketOrderResponse SubmitOrder(TicketOrderRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var target = ctx.TicketPackages.FirstOrDefault(s => s.Id == req.Id && s.Enable && s.Ticket.Enable && s.Ticket.TicketCategory.Enable && s.Supplier.MemberId != null&&s.Supplier.Enable && !s.Supplier.Member.Status.HasFlag(MemberStatus.Freeze));
            if (target != null)
            {
                if (target.Ticket.ServiceType == ServiceType.Package) req.Quantity = 1;
                if (target.OnSale && target.Ticket.OnSale)
                {
                    if (target.Stock >= req.Quantity && req.Quantity > 0)
                    {
                        var now = DateTime.Now;
                        var order = new TicketOrder()
                        {
                            DeliveryPrice = target.DeliveryPrice * req.Quantity,
                            CreateTime = DateTime.Now,
                            Detail = "门票",
                            Description = target.Ticket.Name + " - " + target.Name + "  * " + req.Quantity,
                            MemberId = id,
                            Mobile = req.Mobile,
                            OrderNum = now.ToString("TyyyyMMddHHmmssfff") + (Guid.NewGuid().ToString().Split('-')[0]),
                            RealName = req.Name,
                            OrderPrice = target.Price * req.Quantity,
                            Price = target.Price,
                            UseDate = req.UseDate,
                            Status = OrderStatus.None,
                            SupplierId = target.SupplierId,
                            Address=req.Address,
                            PackageCode=req.PackageCode,

                            TicketPackageId = req.Id,
                            L1AgentCharges = target.L1AgentCharges,
                            L1AgentChargesPercent = target.L1AgentChargesPercent,
                            L1AgentMoreCharges = target.L1AgentMoreCharges,
                            L1AgentMoreChargesPercent = target.L1AgentMoreChargesPercent,
                            L2AgentCharges = target.L2AgentCharges,
                            L2AgentChargesPercent = target.L2AgentChargesPercent,
                            L1BalanceCharges = 0,
                            L2BalanceCharges = 0,
                            Quantity = req.Quantity

                        };
                        #region 记录代理信息
                        if (req.RefferId.HasValue)
                        {
                            var reffer = ctx.Members.FirstOrDefault(s => s.Id == req.RefferId.Value);
                            if (reffer != null && reffer.Id != id && !reffer.Status.HasFlag(MemberStatus.Freeze) && reffer.Status.HasFlag(MemberStatus.Agant))
                            {
                                order.AgentId = req.RefferId.Value;
                                if (reffer.CaptainId != null)
                                {
                                    //是二级代理
                                    if (order.L2AgentCharges > 0)
                                    {
                                        order.L2BalanceCharges = order.L2AgentCharges;
                                    }
                                    else if (order.L2AgentChargesPercent > 0)
                                    {
                                        order.L2BalanceCharges = Decimal.Round(order.OrderPrice * (order.L2BalanceCharges / 100), 2);
                                    }
                                    if (order.L1AgentMoreCharges > 0)
                                    {
                                        order.L1BalanceCharges = order.L1AgentMoreCharges;
                                    }
                                    else if (order.L1AgentMoreChargesPercent > 0)
                                    {
                                        order.L1BalanceCharges = Decimal.Round(order.OrderPrice * (order.L1AgentMoreChargesPercent / 100), 2);
                                    }
                                }
                                else
                                {
                                    //是一级代理
                                    if (order.L2AgentCharges > 0)
                                    {
                                        order.L1BalanceCharges = order.L1AgentCharges;
                                    }
                                    else if (order.L2AgentChargesPercent > 0)
                                    {
                                        order.L1BalanceCharges = order.OrderPrice * Decimal.Round((order.L1BalanceCharges / 100), 2);
                                    }
                                }
                            }
                        }
                        #endregion
                        target.Stock = target.Stock - req.Quantity;
                        ctx.TicketOrders.Add(order);
                        try
                        {
                            ctx.SaveChanges();
                            var member = ctx.Members.First(s => s.Id == order.MemberId);
                            Dictionary<string, string> dic = PaymentProvider.Pay(now, member.AppOpenId, order.OrderNum, order.OrderPrice, "校园小叮当-景区门票");
                            if (dic != null)
                            {
                                CacheManager.SetPrepay_id(order.MemberId, dic["package"].Replace("prepay_id=", ""));
                            }
                            return new TicketOrderResponse { Message = "提交成功", ResultCode = 1, SignBody = dic };
                        }
                        catch (Exception e)
                        {
                            return new TicketOrderResponse { Message = "系统繁忙，请重试", ResultCode = 0 };
                        }

                    }
                    else
                    {
                        return new TicketOrderResponse { Message = "门票剩余库存不足", ResultCode = 0 };
                    }
                }
                else
                {
                    return new TicketOrderResponse { Message = "门票已下架", ResultCode = 0 };
                }
            }
            else
            {
                return new TicketOrderResponse { Message = "门票不存在", ResultCode = 0 };
            }
        }

        /// <summary>
        ///     申请退货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("AddRequest"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse AddRequest(SetOrderStatusRequest req)
        {
           int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var s = ctx.TicketOrders.FirstOrDefault(q => q.Id == req.Id && q.MemberId == id);
            if (s != null) {
                if ((s.Status == OrderStatus.Success||s.Status==OrderStatus.Handled)&&!s.HasRequest)
                {
                    s.HasRequest = true;
                    s.Request = req.Other;
                    ctx.SaveChanges();
                    if (!string.IsNullOrEmpty(s.Supplier.Member.PlatformBindPhone))
                    {
                        SMS.SMSManager.SendNoticeSMS(new List<string> { "","一笔新的门票/服务订单退款申请",s.Mobile, "-票券商核销平台"}, s.Supplier.Member.PlatformBindPhone);
                    }
                    return new SimpleStatusResponse { ResultCode = 1, Message = "提交成功" };
                }
                else
                {
                    return new SimpleStatusResponse { ResultCode = 0, Message = "当前订单状态不允许提交退款申请" };
                }
            }
            else
            {
                return new SimpleStatusResponse {  ResultCode = 0, Message = "没有找到订单" };
            }
        }


        /// <summary>
        ///     获取订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetOrders"), HttpPost]
        public PageResponse GetOrders(PageRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            IQueryable<TicketOrder> query = ctx.TicketOrders.Where(s => s.MemberId == id);
            switch (req.id)
            {
                case 1:
                    query = query.Where(s => s.Status == OrderStatus.None);
                    break;
                case 2:
                    query = query.Where(s => (s.Status == OrderStatus.Paied || s.Status == OrderStatus.Success||s.Status==OrderStatus.Handled));
                    break;
                case 3:
                    query = query.Where(s => s.Status == OrderStatus.CheckOut);
                    break;
                case 4:
                    query = query.Where(s => s.Status == OrderStatus.TurnBack);
                    break;
                default:
                    break;
            }
            return new PageResponse
            {
                Total = query.Count(),
                Rows = query.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.Status,
                    s.OrderPrice,
                    PackageName = s.TicetPackage.Name,
                    TicketName = s.TicetPackage.Ticket.Name,
                    s.Quantity,
                    s.RealName,
                    s.TicetPackage.Ticket.ServiceType,
                    s.Mobile,
                    s.Address,
                    s.PackageCode,
                    s.CreateTime,
                    s.Price,
                    
                    s.Request,
                    s.HasRequest,
                    s.TicetPackage.Ticket.Pic,
                    s.Id
                }).ToList()
            };
        }


        /// <summary>
        ///     获取订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetOrder"), HttpPost]
        public DetailResponse GetOrder(SimpleStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var s = ctx.TicketOrders.FirstOrDefault(q => q.Id == req.Id && q.MemberId == id);
            if (s != null)
            {
                return new DetailResponse
                {
                    Detail = new
                        {
                            s.Status,
                            s.OrderPrice,
                            PackageName = s.TicetPackage.Name,
                            TicketName = s.TicetPackage.Ticket.Name,
                            s.Quantity,
                            s.RealName,
                            s.PackageCode,
                            s.Address,
                            s.TicetPackage.Ticket.ServiceType,
                            s.Mobile,
                            s.Price,
                            s.Id,
                            CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            s.TicetPackage.Ticket.Pic,
                            UseDate = s.UseDate.ToString("yyyy-MM-dd"),
                            s.OrderNum,
                            s.HasRequest,
                            s.Request,
                            s.TicetPackage.Ticket.Name,
                            s.VCode
                        },
                    Message = "获取成功",
                    ResultCode = 1
                };
            }
            else
            {
                return new DetailResponse { Detail = null, ResultCode = 0, Message = "没有找到订单" };
            }
        }

        /// <summary>
        ///     继续支付
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("ContinuePay"), HttpPost]
        public TicketOrderResponse ContinuePay(SimpleStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var now = DateTime.Now;
            var member = ctx.Members.FirstOrDefault(s => s.Id == id);
            var target = ctx.TicketOrders.FirstOrDefault(s=>s.Id==req.Id && s.MemberId==id&& s.Status == OrderStatus.None &&s.Supplier.Enable&& s.TicetPackage.Supplier.MemberId != null);
            if (member.Status.HasFlag(MemberStatus.Freeze))
            {

                return new TicketOrderResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            if (target != null)
            {
               var result=  Payment.PaymentProvider.Query(target.OrderNum);
               if (result.Success && result.Money == target.OrderPrice)
               {
                   var order = ctx.TicketOrders.FirstOrDefault(s => s.OrderNum == result.OrderSN && s.Member.AppOpenId == result.OpenId && (s.Status == OrderStatus.None || s.Status == OrderStatus.Fail) && s.Supplier.MemberId != null && s.OrderPrice == result.Money);
                   if (order != null)
                   {

                       PayHandler.PaySuccess(now, result, order,ctx);
                       return new TicketOrderResponse { Message = "订单已支付", ResultCode = 0 };
                   }
                   else
                   {
                       return new TicketOrderResponse { Message = "没有找到订单或订单不在可付款状态", ResultCode = 0 };
                   }
               }
               else
               {
                   Dictionary<string, string> dic = PaymentProvider.Pay(now, member.AppOpenId, target.OrderNum, target.OrderPrice, "校园小叮当-景区门票");
                   if (dic != null)
                   {
                       CacheManager.SetPrepay_id(member.Id, dic["package"].Replace("prepay_id=", ""));
                   }
                   return new TicketOrderResponse { Message = "提交成功", ResultCode = 1, SignBody = dic };
               }

                
            }
            else
            {
                return new TicketOrderResponse { Message = "没有找到订单或订单不在可付款状态", ResultCode = 0 };
            }
          
        }

    
        /// <summary>
        ///     获取代理佣金
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetAgentCharges"), HttpPost]
        public DetailResponse GetAgentCharges(SimpleStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var member = ctx.Members.FirstOrDefault(s => s.Id == id);
            if (member != null && member.Status.HasFlag(MemberStatus.Agant) && !member.Status.HasFlag(MemberStatus.Freeze))
            {
                var ticket = ctx.Tickets.FirstOrDefault(s => s.Enable && s.OnSale && s.Id == req.Id && s.TicketCategory.Enable);
                if (ticket != null)
                {
                    if (ticket.OnSale)
                    {
                        var packages = ticket.TicketPackages.Where(s => s.Enable == true && s.OnSale == true &&s.Supplier.Enable&& s.Supplier.MemberId != null && !s.Supplier.Member.Status.HasFlag(MemberStatus.Freeze)).ToList();
                        var isL1Agent = member.CaptainId == null;
                        if (isL1Agent)
                        {
                            return new DetailResponse
                            {
                                ResultCode = 1,
                                Message = "获取成功",
                                Detail = new { ticket.Name,ticket.Pic, Packages = packages.Select(s => new { s.Name, Words = "您每推销售一张门票预计可以获得佣金" + (s.L1AgentChargesPercent != 0 ? (Decimal.Round(s.Price * (s.L1AgentChargesPercent / 100), 2)) : s.L1AgentCharges) + "元，您的队员销售成功您可以获得" + (s.L1AgentMoreChargesPercent != 0 ? (Decimal.Round(s.Price * (s.L1AgentMoreChargesPercent / 100), 2)) : s.L1AgentMoreCharges) + "元" }).ToList() }
                            };
                        }
                        else
                        {
                            return new DetailResponse
                            {
                                ResultCode = 1,
                                Message = "获取成功",
                                Detail = new { ticket.Name, ticket.Pic, Packages = packages.Select(s => new { s.Name, Words = "您每推销售一张门票预计可以获得佣金" + (s.L2AgentChargesPercent != 0 ? (Decimal.Round(s.Price * (s.L2AgentChargesPercent / 100), 2)) : s.L2AgentCharges) + "元，您的队长也可获得" + (s.L1AgentMoreChargesPercent != 0 ? (Decimal.Round(s.Price * (s.L1AgentMoreChargesPercent / 100), 2)) : s.L1AgentMoreCharges) + "元" }).ToList() }
                            };
                        }
                    }
                    else
                    {
                        return new DetailResponse { Message = "当前门票已下架", ResultCode = 0 };
                    }

                }
                else
                {
                    return new DetailResponse { Message = "没有找到门票信息", ResultCode = 0 };

                }
            }
            else
            {
                return new DetailResponse { Message = "您无法获取代理佣金信息", ResultCode = 0 };
            }
        }
    }
}
