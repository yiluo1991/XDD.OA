using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using XDD.Web.Infrastructure;
using XDD.Web.Models.WeChat;

namespace XDD.Web.Controllers.API
{
    /// <summary>
    /// 供应商接口
    /// </summary>
    [RoutePrefix("api/Supplier")]
    public class SupplierController : ApiController
    {
        XDDDbContext ctx = new XDDDbContext();
        private bool IsEnabledSupplier(Member member)
        {
            return !member.Status.HasFlag(MemberStatus.Freeze) && member.Supplier.Count > 0;
        }

        private Member GetEnabledMember(int id)
        {
            return ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
        }


        /// <summary>
        ///    绑定供应商账号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("BindMember"), HttpPost]
        public DetailResponse BindMember(BindSupplierRequest req)
        {

            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            XDDDbContext ctx = new XDDDbContext();
            var member = ctx.Members.FirstOrDefault(s => s.Id == id && !s.Status.HasFlag(MemberStatus.Freeze));
            if (member == null)
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
            else
            {
                if (member.Supplier.Count > 0)
                {
                    return new DetailResponse { ResultCode = 0, Message = "用户已是供应商，无需重复绑定" };
                }
                else
                {
                    //开始验证
                    if (string.IsNullOrEmpty(member.PlatformBindPhone))
                    {
                        //需验证手机
                        if (!CacheManager.CheckMobileCode(id, req.Mobile, req.Code))
                        {
                            //手机验证有误
                            return new DetailResponse { ResultCode = -1, Message = "手机验证码有误，请重试或重新获取" };
                        }
                        else
                        {
                            CacheManager.ClearMobileCode(id);
                            if (ctx.Members.Count(s => s.PlatformBindPhone == req.Mobile) > 0)
                            {
                                return new DetailResponse { ResultCode = -1, Message = "手机号已被绑定，请更换手机号" };
                            }
                            else
                            {
                                member.PlatformBindPhone = req.Mobile;
                            }
                        }
                    }
                  
                    if (string.IsNullOrEmpty(member.RealName))
                    {
                        if (string.IsNullOrEmpty(req.RealName))
                        {
                            return new DetailResponse { ResultCode = -1, Message = "姓名有误" };
                        }
                        else
                        {
                            member.RealName = req.RealName;
                        }
                    }
                    if (string.IsNullOrEmpty(req.BindCode))
                    {
                        return new DetailResponse { ResultCode = -1, Message = "供应商绑定码有误" };
                    }
                    int supplierId = CacheManager.CheckSupplierCode(req.BindCode.ToUpper());
                    if (supplierId > 0)
                    {
                        var supplier = ctx.Suppliers.FirstOrDefault(s => s.Id == supplierId && s.MemberId == null);
                        if (supplier != null)
                        {
                            supplier.MemberId = member.Id;
                        }
                        else
                        {
                            return new DetailResponse { ResultCode = -1, Message = "绑定码有误" };
                        }
                    }
                    else
                    {
                        return new DetailResponse { ResultCode = -1, Message = "绑定码有误" };
                    }
                    if (!member.Status.HasFlag(MemberStatus.Supplier)) {
                        member.Status |= MemberStatus.Supplier;
                    }
                    ctx.SaveChanges();
                    return new DetailResponse { ResultCode = 1, Message = "绑定成功" };
                }

            }
        }

        /// <summary>
        ///     获取供应商订单列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TokenAuthorize]
        [Route("GetOrders"), HttpPost]
        public PageResponse GetOrders(PageRequest req)
        {

            int id = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            IQueryable<TicketOrder> query = ctx.TicketOrders.Where(s => s.Supplier.MemberId == id);
            switch (req.id)
            {
                case 1:
                    query = query.Where(s => s.Status == OrderStatus.Paied || s.Status == OrderStatus.Success);
                    break;
                case 2:
                    query = query.Where(s => s.Status==OrderStatus.Handled);
                    break;
                case 3:
                    query = query.Where(s => s.Status == OrderStatus.CheckOut);
                    break;
                case 4:
                    query = query.Where(s => s.Status == OrderStatus.TurnBack||s.HasRequest);
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(req.keyword))
            {
                query = query.Where(s => s.RealName.Contains(req.keyword) || s.Mobile.Contains(req.keyword)||s.Address.Contains(req.keyword)||s.PackageCode.Contains(req.keyword));
            }
            if (req.date.HasValue)
            {
                query = query.Where(s => s.UseDate == req.date.Value);
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
                    s.Mobile,
                    s.CreateTime,
                    s.Price,
                    s.UseDate,
                    s.TicetPackage.Ticket.Pic,
                    s.Address,
                    s.Request,
                    s.HasRequest,
                    s.TicetPackage.Ticket.ServiceType,
                    s.Id,
                    s.PackageCode
                }).ToList()
            };
        }

        /// <summary>
        ///     核销订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("CheckoutOrder"), HttpPost]
        [TokenAuthorize]
        public DetailResponse CheckoutOrder(CheckOutOrderRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var member = GetEnabledMember(memberId);
            if (member != null)
            {
                if (IsEnabledSupplier(member))
                {
                    var order = ctx.TicketOrders.FirstOrDefault(s => s.Supplier.MemberId == memberId && s.Id == req.Id &&( s.Status == OrderStatus.Success||s.Status==OrderStatus.Handled));
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
                                return new DetailResponse { ResultCode = 1, Message = "核销成功" };
                            }
                            catch (Exception)
                            {
                                return new DetailResponse { ResultCode =0, Message = "系统忙请重试" };
                            }
                           
                        }
                        else
                        {
                            return new DetailResponse { ResultCode = 0, Message = "请先处理该订单的退款申请！" };

                        }
                    }
                    else
                    {
                        return new DetailResponse { ResultCode = -1, Message = "没有找到订单或订单不在可核销状态" };
                    }

                }
                else
                {
                    return new DetailResponse { ResultCode = -1, Message = "账户不可用" };
                }
            }
            else
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
        }
        /// <summary>
        ///     标志订单为已处理
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("HandleOrder"), HttpPost]
        [TokenAuthorize]
        public DetailResponse HandleOrder(CheckOutOrderRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var member = GetEnabledMember(memberId);
            if (member != null)
            {
                if (IsEnabledSupplier(member))
                {
                    var order = ctx.TicketOrders.FirstOrDefault(s => s.Supplier.MemberId == memberId && s.Id == req.Id && (s.Status == OrderStatus.Success));
                    if (order != null)
                    {
                        if (!order.HasRequest)
                        {
                            try
                            {
                                order.Status = OrderStatus.Handled;
                                ctx.SaveChanges();
                                return new DetailResponse { ResultCode = 1, Message = "处理成功" };
                            }
                            catch (Exception)
                            {
                                return new DetailResponse { ResultCode = 0, Message = "系统忙请重试" };
                            }
                        }
                        else
                        {
                            return new DetailResponse { ResultCode = 0, Message = "请先处理该订单的退款申请！" };

                        }
                    }
                    else
                    {
                        return new DetailResponse { ResultCode = -1, Message = "没有找到订单或订单不在可处理状态" };
                    }

                }
                else
                {
                    return new DetailResponse { ResultCode = -1, Message = "账户不可用" };
                }
            }
            else
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
        }

        /// <summary>
        ///     拒绝对货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Deny"), HttpPost]
        [TokenAuthorize]
        public DetailResponse Deny(CheckOutOrderRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var member = GetEnabledMember(memberId);
            if (member != null)
            {
                if (IsEnabledSupplier(member))
                {
                    var order = ctx.TicketOrders.FirstOrDefault(s => s.Supplier.MemberId == memberId && s.Id == req.Id && (s.Status == OrderStatus.Success||s.Status==OrderStatus.Handled));
                    if (order != null)
                    {
                        if (order.HasRequest)
                        {
                            try
                            {
                                order.Request = null;
                                order.HasRequest = false;
                                ctx.SaveChanges();
                                return new DetailResponse { ResultCode = 1, Message = "已拒绝退款申请" };
                            }
                            catch (Exception)
                            {
                                return new DetailResponse { ResultCode = 0, Message = "系统忙请重试" };
                            }
                        }
                        else
                        {
                            return new DetailResponse { ResultCode = 0, Message = "无退款申请" };

                        }
                    }
                    else
                    {
                        return new DetailResponse { ResultCode = -1, Message = "没有找到订单或订单不在可处理状态" };
                    }

                }
                else
                {
                    return new DetailResponse { ResultCode = -1, Message = "账户不可用" };
                }
            }
            else
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
        }

        /// <summary>
        ///     允许对货
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Allow"), HttpPost]
        [TokenAuthorize]
        public DetailResponse Allow(CheckOutOrderRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var member = GetEnabledMember(memberId);
            if (member != null)
            {
                if (IsEnabledSupplier(member))
                {
                    var order = ctx.TicketOrders.FirstOrDefault(s => s.Supplier.MemberId == memberId && s.Id == req.Id && (s.Status == OrderStatus.Success || s.Status == OrderStatus.Handled));
                    if (order != null)
                    {
                        if (order.HasRequest)
                        {
                            try
                            {
                                PayHandler.TicketTurnback(order, ctx);
                                return new DetailResponse { ResultCode = 1, Message = "已同意退款" };
                            }
                            catch (Exception)
                            {
                                return new DetailResponse { ResultCode = 0, Message = "系统忙请重试" };
                            }
                        }
                        else
                        {
                            return new DetailResponse { ResultCode = 0, Message = "无退款申请" };

                        }
                    }
                    else
                    {
                        return new DetailResponse { ResultCode = -1, Message = "没有找到订单或订单不在可处理状态" };
                    }

                }
                else
                {
                    return new DetailResponse { ResultCode = -1, Message = "账户不可用" };
                }
            }
            else
            {
                return new DetailResponse { ResultCode = -1, Message = "用户不可用" };
            }
        }
    
    
    }
}
