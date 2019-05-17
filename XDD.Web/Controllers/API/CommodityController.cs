using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Payment;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API
{

    /// <summary>
    ///     二手交易接口
    /// </summary>
    [RoutePrefix("api/Commodity")]
    public class CommodityController : ApiController
    {
        XDDDbContext ctx = new XDDDbContext();
        /// <summary>
        ///     获取分类列表
        /// </summary>
        /// <returns>商品分类列表</returns>
        [Route("GetCategories")]
        public object GetCategories()
        {

            return ctx.CommodityCategories.Where(s => s.Enable).OrderBy(s => s.SN).Select(s => new { Enable = s.Enable, Icon = s.Icon, Id = s.Id, Name = s.Name, s.SN }).ToList();

        }

        /// <summary>
        ///     分页获取二手收售信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetCommodities"), HttpPost]
        public PageResponse GetCommodities(PageRequest req)
        {
            IQueryable<Commodity> query = ctx.Commodities.Where(s => s.Enable && s.OnSale && !s.Member.Status.HasFlag(MemberStatus.Freeze)&&s.CommodityCategory.Enable);
            if (req.id.HasValue)
            {
                query = query.Where(s => s.CommodityCategoryId == req.id.Value);
            }
            if (!string.IsNullOrEmpty(req.keyword))
            {
                query = query.Where(s => s.Name.Contains(req.keyword) || s.Member.NickName.Contains(req.keyword));
            }
            if (req.tradetype.HasValue)
            {
                query = query.Where(s => s.Type == req.tradetype.Value);
            }
            var total = query.Count();
            IOrderedQueryable<Commodity> orderedQuery = query.OrderBy(s => s.SN);


            switch (req.sort)
            {
                case "ta":
                    orderedQuery = orderedQuery.ThenByDescending(s => s.CreateTime);
                    break;
                case "td":
                    orderedQuery = orderedQuery.ThenBy(s => s.CreateTime);
                    break;
                case "pa":
                    orderedQuery = orderedQuery.ThenBy(s => s.Price);
                    break;
                case "pd":
                    orderedQuery = orderedQuery.ThenByDescending(s => s.Price);
                    break;
                default:
                    orderedQuery = orderedQuery.ThenByDescending(s => s.CreateTime);
                    break;

            }
            return new PageResponse()
            {
                Total = total,
                More = req.tradetype,
                Rows = orderedQuery.Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.CommodityCategoryId,
                    s.Cover,
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
                    s.ViewCount
                }).ToList()
            };
        }

        /// <summary>
        /// 获取单条收售信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetCommodity"), HttpPost]
        public DetailResponse GetCommodity(SimpleStatusRequest req, bool add = false)
        {
            var target = ctx.Commodities.FirstOrDefault(s => s.Id == req.Id && s.Enable == true && !s.Member.Status.HasFlag(MemberStatus.Freeze) && s.CommodityCategory.Enable);
            if (target != null)
            {
                if (add)
                {
                    try
                    {
                        target.ViewCount++;
                        ctx.SaveChanges();
                    }
                    catch
                    {


                    }
                }


                return new DetailResponse
                {
                    Message = "获取成功",
                    ResultCode = 1,
                    Detail = new
                    {
                        target.OnSale,
                        target.Id,
                        CommodityCategoryName = target.CommodityCategory.Name,
                        target.Member.AvatarUrl,
                        target.CommodityCategoryId,
                        target.Content,
                        target.Cover,
                        target.CreateTime,
                        target.Enable,
                        target.Paths,
                        target.Price,
                        target.SN,
                        target.Type,
                        Saled=target.CommodityOrders.Count(u=>u.CommodityOrderStatus!=CommodityOrderStatus.TotalCancel)>0?true:false,
                        target.ViewCount,
                        target.NewLevel,
                        target.Name,
                        target.MemberId,
                        target.Member.NickName,
                        MemberStatus = target.Member.Status
                    }
                };
            }
            else
            {
                return new DetailResponse
                {
                    Detail = null,
                    ResultCode = 0,
                    Message = "商品不存在"
                };
            }
        }

        /// <summary>
        ///     添加订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("AddCommodityOrder"), HttpPost]
        [TokenAuthorize]
        public TicketOrderResponse AddOrder(AddCommodityOrderRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var target = ctx.Commodities.FirstOrDefault(s => s.Id == req.CommodityId && s.Enable == true && !s.Member.Status.HasFlag(MemberStatus.Freeze) && s.OnSale && s.CommodityCategory.Enable);
            DateTime now = DateTime.Now;
            if (target != null)
            {
                var order = new CommodityOrder()
                {
                    Address = req.Address,
                    Area = req.Area,
                    City = req.City,
                    Province = req.Province,
                    RealName = req.RealName,
                    CommodityId = target.Id,
                    Mobile = req.Mobile,
                    OrderNum = now.ToString("CyyyyMMddHHmmssfff") + (Guid.NewGuid().ToString().Split('-')[0]),
                    OrderPrice = target.Price,
                    Remark = now.ToString("yyyy-MM-dd HH:mm:ss") + "：下单"
                };
                order.CreateTime = DateTime.Now;
                order.CommodityOrderStatus = CommodityOrderStatus.None;
                order.MemberId = mid;
                //商品设置为不在售
                //target.OnSale = false;
                try
                {
                    ctx.CommodityOrders.Add(order);
                    ctx.SaveChanges();
                    var member = ctx.Members.First(s => s.Id == order.MemberId);
                    Dictionary<string, string> dic = PaymentProvider.Pay(now, member.AppOpenId, order.OrderNum, order.OrderPrice, "校园小叮当-线上支付");
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
                return new TicketOrderResponse() { Message = "没有找到商品", ResultCode = 0, SignBody = null };
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
            var target = ctx.CommodityOrders.FirstOrDefault(s => s.Id == req.Id && s.MemberId == id && s.CommodityOrderStatus == CommodityOrderStatus.None);
            if (member.Status.HasFlag(MemberStatus.Freeze))
            {

                return new TicketOrderResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            if (target != null)
            {
                var result = Payment.PaymentProvider.Query(target.OrderNum);
                if (result.Success && result.Money == target.OrderPrice)
                {
                    var order = ctx.CommodityOrders.FirstOrDefault(s => s.OrderNum == result.OrderSN && s.Member.AppOpenId == result.OpenId && (s.CommodityOrderStatus == CommodityOrderStatus.None || s.CommodityOrderStatus == CommodityOrderStatus.Fail) && s.OrderPrice == result.Money);
                    if (order != null)
                    {

                        PayHandler.PayCommoditySuccess(now, result, order, ctx);
                        return new TicketOrderResponse { Message = "订单已支付", ResultCode = 0 };
                    }
                    else
                    {
                        return new TicketOrderResponse { Message = "没有找到订单或订单不在可付款状态", ResultCode = 0 };
                    }
                }
                else
                {
                    Dictionary<string, string> dic = PaymentProvider.Pay(now, member.AppOpenId, target.OrderNum, target.OrderPrice, "校园小叮当-线上支付");
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
        ///     获取订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetOrders"), HttpPost]
        public PageResponse GetOrders(PageRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            IQueryable<CommodityOrder> query = ctx.CommodityOrders;
            switch (req.id)
            {
                case 1:
                    query = query.Where(s => s.MemberId == id);
                    break;
                case 2:
                    query = query.Where(s => (s.CommodityOrderStatus == CommodityOrderStatus.None || s.CommodityOrderStatus == CommodityOrderStatus.Fail) && s.MemberId == id);
                    break;
                case 3:
                    query = query.Where(s => s.MemberId == id && (s.CommodityOrderStatus == CommodityOrderStatus.Paied || s.CommodityOrderStatus == CommodityOrderStatus.Express || s.CommodityOrderStatus == CommodityOrderStatus.BackDeny));
                    break;
                case 4:
                    query = query.Where(s => s.MemberId == id && (s.CommodityOrderStatus == CommodityOrderStatus.BackAllow || s.CommodityOrderStatus == CommodityOrderStatus.BackExpress || s.CommodityOrderStatus == CommodityOrderStatus.BackRequest));
                    break;
                case 5:
                    query = query.Where(s => s.MemberId == id && (s.CommodityOrderStatus == CommodityOrderStatus.BackDelivery || s.CommodityOrderStatus == CommodityOrderStatus.Delivery || s.CommodityOrderStatus == CommodityOrderStatus.TotalCancel || s.CommodityOrderStatus == CommodityOrderStatus.TotalSuccess));
                    break;
                case 6:
                    query = query.Where(s => s.Commodity.MemberId == id);
                    break;
                case 7:
                    query = query.Where(s => s.Commodity.MemberId == id && s.CommodityOrderStatus == CommodityOrderStatus.Paied);
                    break;
                case 8:
                    query = query.Where(s => s.Commodity.MemberId == id && s.CommodityOrderStatus == CommodityOrderStatus.Express || s.CommodityOrderStatus == CommodityOrderStatus.BackDeny);
                    break;
                case 9:
                    query = query.Where(s => s.Commodity.MemberId == id && (s.CommodityOrderStatus == CommodityOrderStatus.BackAllow || s.CommodityOrderStatus == CommodityOrderStatus.BackExpress || s.CommodityOrderStatus == CommodityOrderStatus.BackRequest));
                    break;
                case 10:
                    query = query.Where(s => s.Commodity.MemberId == id && (s.CommodityOrderStatus == CommodityOrderStatus.BackDelivery || s.CommodityOrderStatus == CommodityOrderStatus.Delivery || s.CommodityOrderStatus == CommodityOrderStatus.TotalCancel || s.CommodityOrderStatus == CommodityOrderStatus.TotalSuccess));
                    break;

            }
            return new PageResponse
            {
                Total = query.Count(),
                Rows = query.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new
                {
                    s.OrderPrice,
                    s.RealName,
                    s.Mobile,
                    s.CreateTime,
                    s.Id,
                    s.Commodity.NewLevel,
                    CommodityName = s.Commodity.Name,
                    SaleMemberName = s.Commodity.Member.NickName,
                    BuyMemberName = s.Member.NickName,
                    SaleMemberId = s.Commodity.MemberId,
                    BuyMemberId = s.MemberId,
                    s.CommodityId,
                    s.CommodityOrderStatus,
                    s.Freeze,
                    s.Commodity.Cover,
                    s.OrderNum
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
            var s = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && (q.MemberId == id || q.Commodity.MemberId == id));
            if (s != null)
            {
                return new DetailResponse
                {
                    Detail = new
                    {
                        ViewType = s.MemberId == id ? "buy" : "sale",
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
        ///     关闭交易
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("CloseOrder"), HttpPost]
        public SimpleStatusResponse CloseOrder(SimpleStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.Commodity.MemberId == id && q.CommodityOrderStatus == CommodityOrderStatus.Paied && !q.Freeze);
            if (order != null && order.Commodity.Member.Status.HasFlag(MemberStatus.Freeze))
            {

                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可关闭状态", ResultCode = 0 };
                }
                else
                {

                    var now = DateTime.Now;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：卖家关闭了订单，支付的款项返回买家账户";
                    order.CommodityOrderStatus = CommodityOrderStatus.TotalCancel;
                    PayHandler.CommodityOrderCancelWidthoutSaveChange(order, now, ctx);
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }

            }
        }

        /// <summary>
        ///     允许退货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BackAllow"), HttpPost]
        public SimpleStatusResponse BackAllow(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.Commodity.MemberId == id && q.CommodityOrderStatus == CommodityOrderStatus.BackRequest && !q.Freeze);
            if (order != null && order.Commodity.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.BackProvince = req.Province;
                    order.BackCity = req.City;
                    order.BackArea = req.Area;
                    order.BackAddress = req.Address;
                    order.BackRealName = req.RealName;
                    order.BackMobile = req.Mobile;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：卖家通过了退货请求，请按照退货地址寄出退货物品";
                    order.CommodityOrderStatus = CommodityOrderStatus.BackAllow;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }

        /// <summary>
        ///     拒绝退货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BackDeny"), HttpPost]
        public SimpleStatusResponse BackDeny(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.Commodity.MemberId == id && q.CommodityOrderStatus == CommodityOrderStatus.BackRequest && !q.Freeze);
            if (order != null && order.Commodity.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.BackFeedback = req.Other;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：卖家拒绝了退货请求，理由见记录";
                    order.CommodityOrderStatus = CommodityOrderStatus.BackDeny;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }

        /// <summary>
        ///     退货申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BackRequest"), HttpPost]
        public SimpleStatusResponse BackRequest(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.MemberId == id && (q.CommodityOrderStatus == CommodityOrderStatus.Express || q.CommodityOrderStatus == CommodityOrderStatus.BackDeny && !q.Freeze));
            if (order != null && order.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.BackQequest = req.Other;
                    order.BackProvince = null;
                    order.BackCity = null;
                    order.BackArea = null;
                    order.BackAddress = null;
                    order.BackRealName = null;
                    order.BackMobile = null;
                    order.BackFeedback = null;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：买家提交了退货申请，理由见记录";
                    order.CommodityOrderStatus = CommodityOrderStatus.BackRequest;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }


        /// <summary>
        ///     确认收货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("Delivery"), HttpPost]
        public SimpleStatusResponse Delivery(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.MemberId == id && (q.CommodityOrderStatus == CommodityOrderStatus.Express || q.CommodityOrderStatus == CommodityOrderStatus.BackDeny && !q.Freeze));
            if (order != null && order.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：买家确认收货，交易完成";
                    order.CommodityOrderStatus = CommodityOrderStatus.Delivery;
                    PayHandler.CommodityOrderSuccessWidthoutSaveChange(order, now, ctx);
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }


        /// <summary>
        ///     发货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("Express"), HttpPost]
        public SimpleStatusResponse Express(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.Commodity.MemberId == id && (q.CommodityOrderStatus == CommodityOrderStatus.Paied && !q.Freeze));
            if (order != null && order.Commodity.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.CommodityOrderStatus = CommodityOrderStatus.Express;
                    order.ExpressName = req.ExpressName;
                    order.ExpressNo = req.ExpressNo;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：卖家已发货";
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }

        /// <summary>
        ///     退货发货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BackExpress"), HttpPost]
        public SimpleStatusResponse BackExpress(SetOrderStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.MemberId == id && (q.CommodityOrderStatus == CommodityOrderStatus.BackAllow && !q.Freeze));
            if (order != null && order.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可操作状态", ResultCode = 0 };
                }
                else
                {
                    var now = DateTime.Now;
                    order.CommodityOrderStatus = CommodityOrderStatus.BackExpress;
                    order.BackExpressName = req.ExpressName;
                    order.BackExpressNo = req.ExpressNo;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：买家退货物品已发货";
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }
            }
        }

        /// <summary>
        ///     退货收货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BackDelivery"), HttpPost]
        public SimpleStatusResponse BackDelivery(SimpleStatusRequest req)
        {
            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var order = ctx.CommodityOrders.FirstOrDefault(q => q.Id == req.Id && q.Commodity.MemberId == id && q.CommodityOrderStatus == CommodityOrderStatus.BackExpress && !q.Freeze);
            if (order != null && order.Commodity.Member.Status.HasFlag(MemberStatus.Freeze))
            {
                return new SimpleStatusResponse { Message = "您的账户已禁用，如有问题请联系管理员", ResultCode = 0 };
            }
            else
            {
                if (order == null)
                {
                    return new SimpleStatusResponse { Message = "没有找到订单或订单不在可关闭状态", ResultCode = 0 };
                }
                else
                {

                    var now = DateTime.Now;
                    order.Remark += "@|@" + now.ToString("yyyy-MM-dd HH:mm:ss") + "：退货已收货，交易的款项返回买家账户";
                    order.CommodityOrderStatus = CommodityOrderStatus.BackDelivery;
                    PayHandler.CommodityOrderCancelWidthoutSaveChange(order, now, ctx);
                    ctx.SaveChanges();
                    return new SimpleStatusResponse { Message = "操作成功", ResultCode = 1 };
                }

            }
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [IdentityAuthorize]
        [Route("AddCommodity"), HttpPost]
        public SimpleStatusResponse AddCommodity(AddCommodityRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            Commodity c = new Commodity()
            {
                CommodityCategoryId = req.CommodityCategoryId,
                Content = req.Content,
                Cover = req.Paths.Split('|')[0].Replace("/b-","/s-"),
                Paths = req.Paths,
                CreateTime = DateTime.Now,
                Enable = true,
                MemberId = mid,
                OnSale = true,
                Name = req.Name,
                NewLevel = req.NewLevel,
                Price = req.Price,
                SN = 100,
                Type = req.Type,
                ViewCount = 0
            };
            ctx.Commodities.Add(c);
            ctx.SaveChanges();
            return new SimpleStatusResponse() { Id = c.Id, ResultCode = 1, Message = "操作成功" };
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [IdentityAuthorize]
        [Route("EditCommodity"), HttpPost]
        public SimpleStatusResponse EditCommodity(AddCommodityRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var c = ctx.Commodities.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid && s.Enable);
            if (c != null)
            {
                if (c.CommodityOrders.Count(u=>u.CommodityOrderStatus!=CommodityOrderStatus.TotalCancel) > 0)
                {
                    return new SimpleStatusResponse() { ResultCode = 0, Message = "已售出的商品信息无法编辑" };
                }
                else
                {
                    c.CommodityCategoryId = req.CommodityCategoryId;
                    c.Content = req.Content;
                    c.Cover = req.Paths.Split('|')[0].Replace("/b-", "/s-");
                    c.Paths = req.Paths;
                    c.CreateTime = DateTime.Now;
                    c.MemberId = mid;
                    c.Name = req.Name;
                    c.NewLevel = req.NewLevel;
                    c.Price = req.Price;
                    c.Type = req.Type;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse() { Id = c.Id, ResultCode = 1, Message = "操作成功" };
                }
            }
            else
            {
                return new SimpleStatusResponse() { ResultCode = 0, Message = "没有找到要操作的信息" };
            }
        }

        /// <summary>
        ///     上架
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [IdentityAuthorize]
        [Route("OnSale"), HttpPost]
        public SimpleStatusResponse OnSale(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var c = ctx.Commodities.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid && s.Enable);
            if (c != null)
            {
                if (c.CommodityOrders.Count(s=>s.CommodityOrderStatus!=CommodityOrderStatus.TotalCancel) == 0)
                {
                    c.CreateTime = DateTime.Now;
                    c.OnSale = true;
                    ctx.SaveChanges();
                    return new SimpleStatusResponse() { Id = c.Id, ResultCode = 1, Message = "操作成功" };
                }
                else
                {
                    return new SimpleStatusResponse() { ResultCode = 0, Message = "已售出的商品无法修改和上架" };
                }
            }
            else
            {
                return new SimpleStatusResponse() { ResultCode = 0, Message = "没有找到要操作的信息" };
            }
        }

        /// <summary>
        ///     下架
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [IdentityAuthorize]
        [Route("OffSale"), HttpPost]
        public SimpleStatusResponse OffSale(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var c = ctx.Commodities.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid && s.Enable);
            if (c != null)
            {
                c.OnSale = false;
                ctx.SaveChanges();
                return new SimpleStatusResponse() { Id = c.Id, ResultCode = 1, Message = "操作成功" };
            }
            else
            {
                return new SimpleStatusResponse() { ResultCode = 0, Message = "没有找到要操作的信息" };
            }
        }

       /// <summary>
       /// 禁用
       /// </summary>
       /// <param name="req"></param>
       /// <returns></returns>
        [TokenAuthorize]
        [Route("Disable"), HttpPost]
        public SimpleStatusResponse Disable(SimpleStatusRequest req)
        {
            int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var target = ctx.Commodities.FirstOrDefault(s => s.Id == req.Id && s.MemberId == mid && s.CommodityOrders.Count() == 0);
            if (target != null)
            {
                target.Enable = false;
                target.OnSale = false;
                ctx.SaveChanges();
                return new SimpleStatusResponse() { ResultCode = 1, Message = "操作成功" };
            }
            else {
                return new SimpleStatusResponse() { ResultCode = 0, Message = "没有找到要操作的信息或该信息已有交易" };
            }
            
        }


        /// <summary>
        ///     分页获取我发布的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetMineCommodities"),HttpPost]
        public PageResponse GetMineCommodities(PageRequest req)
        {
              int mid = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
              var q=ctx.Commodities.Where(s => s.MemberId == mid && s.Enable);
              return new PageResponse
              {
                  Total = q.Count(),
                  Rows = q.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 10).Take(10).Select(s => new {

                      s.CommodityCategoryId,
                      s.Cover,
                      s.CreateTime,
                      CommodityCategoryName= s.CommodityCategory.Name,
                      Saled=s.CommodityOrders.Count(u=>u.CommodityOrderStatus!=CommodityOrderStatus.TotalCancel)>0?true:false,
                      s.Enable,
                      s.Id,
                      s.Member.NickName,
                      s.MemberId,
                      s.Name,
                      s.NewLevel,
                      s.OnSale,
                      s.Price,
                      s.SN,
                      s.Type,
                      s.ViewCount
                  }).ToList()
              };
        }
    }
}
