using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using XDD.Core.Model;
using XDD.Payment;

namespace XDD.Web.Infrastructure
{
  
    public static class PayHandler
    {
        private static object locko = new object();
        public static void PaySuccess(DateTime now, PayOrderResult result, TicketOrder order, XDD.Core.DataAccess.XDDDbContext ctx)
        {
            order.TicetPackage.Ticket.SaleNum++;
            #region 处理客户账户
            var ms1 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now, RefferId = order.Id, Type = "微信支付充值", Money = result.Money, MemberId = order.MemberId };
            order.Member.Account += result.Money;
            var ms2 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now.AddMilliseconds(4), RefferId = order.Id, Type = "门票付款", Money = -result.Money, MemberId = order.MemberId };
            order.Member.Account -= result.Money;
            ctx.AccountStatements.Add(ms1);
            ctx.AccountStatements.Add(ms2);
            #endregion

           //转移到核销处理 DealAgentCharges(now, order, ctx);

            #region 供应商付款
            var ss1 = new AccountStatement { BeforeBalance = order.Supplier.Member.Account, CreateTime = now.AddMilliseconds(15), RefferId = order.Id, Type = "门票收入", Money = order.DeliveryPrice, MemberId = order.Supplier.Member.Id };
            order.Supplier.Member.Account += order.DeliveryPrice;
            ctx.AccountStatements.Add(ss1);
            #endregion
            //处理门票状态
            order.Status = OrderStatus.Success;
            string str = "1234567890";
            string y = "";
            Random ran = new Random();
            for (int x = 0; x < 6; x++)
            {
                int r = ran.Next(0, str.Length);
                string a = str.Substring(r, 1);
                y = y + a;
            }
            order.VCode = y;
            ctx.SaveChanges();
            string token = "";
            lock (locko)
            {
                token = CacheManager.GetAppToken();
            }
            if (!string.IsNullOrEmpty(token)) {
                var pid = CacheManager.GetPrepay_id(order.MemberId);
                if (pid != null)
                {
                    var tid = "pDh-peuHBNCptLwxQldR-rUVMkDntWltOx09lAiKw5Y";
                    WebClient client = new WebClient();
                    var obj = new
                    {
                        touser = order.Member.AppOpenId,
                        template_id = tid,
                        page = "pages/ticketorder/list?tab=2",
                        form_id = pid,
                        data = new
                        {
                            keyword1 = new
                            {
                                value = order.TicetPackage.Ticket.Name + "-" + order.TicetPackage.Name
                            },
                            keyword2 = new
                            {
                                value = order.OrderPrice.ToString("C")
                            },
                            keyword3 = new
                            {
                                value = order.OrderNum
                            },
                        }
                    };
                    string json = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(obj);

                    Encoding.UTF8.GetString(client.UploadData("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token=" + token, Encoding.UTF8.GetBytes(json)));

                }
            }
            try
            {
                if (!string.IsNullOrEmpty(order.PackageCode) && !string.IsNullOrEmpty(order.Supplier.Member.PlatformBindPhone))
                {

                    SMS.SMSManager.SendServiceSMS(order.Supplier.Member.PlatformBindPhone, HttpUtility.UrlEncode(order.TicetPackage.Ticket.Name + "-" + order.TicetPackage.Name), HttpUtility.UrlEncode(order.RealName), HttpUtility.UrlEncode(order.Mobile),HttpUtility.UrlEncode(order.Address));
                    SMS.SMSManager.SendMemberSMS(order.Mobile, HttpUtility.UrlEncode("13074861113"));
                }
            }
            catch 
            {
                
            }
           
            
        }

        public static void TicketTurnback(TicketOrder order, XDD.Core.DataAccess.XDDDbContext ctx)
        {
            DateTime now = DateTime.Now;
            #region 处理客户账户
            var ms1 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now, RefferId = order.Id, Type = "门票退款", Money = order.OrderPrice, MemberId = order.MemberId };
            order.Member.Account += order.OrderPrice;
            ctx.AccountStatements.Add(ms1);
            #endregion


            #region 供应商付款
            var ss1 = new AccountStatement { BeforeBalance = order.Supplier.Member.Account, CreateTime = now.AddMilliseconds(15), RefferId = order.Id, Type = "供应门票退票扣费", Money =- order.DeliveryPrice, MemberId = order.Supplier.Member.Id };
            order.Supplier.Member.Account -= order.DeliveryPrice;
            ctx.AccountStatements.Add(ss1);
            #endregion
            //处理门票状态
            order.Status = OrderStatus.TurnBack;
            ctx.SaveChanges();
        }


        public static void DealAgentCharges(DateTime now, TicketOrder order, XDD.Core.DataAccess.XDDDbContext ctx)
        {
            if (order.L1BalanceCharges > 0 || order.L2BalanceCharges > 0)
            {
                #region 处理代理佣金
                if (order.AgentId != null && !order.Agent.Status.HasFlag(MemberStatus.Freeze) && order.Agent.Status.HasFlag(MemberStatus.Agant))
                {
                    if (order.Agent.CaptainId == null)
                    {
                        if (order.L1BalanceCharges > 0)
                        {
                            var s1 = new AccountStatement { BeforeBalance = order.Agent.Account, CreateTime = now.AddMilliseconds(10), MemberId = order.Agent.Id, Money = order.L1BalanceCharges, RefferId = order.Id, Type = "一级代理佣金" };
                            ctx.AccountStatements.Add(s1);
                            order.Agent.Account += order.L1BalanceCharges;
                        }

                    }
                    else
                    {
                        if (order.L2BalanceCharges > 0)
                        {
                            var s1 = new AccountStatement { BeforeBalance = order.Agent.Account, CreateTime = now.AddMilliseconds(10), MemberId = order.Agent.Id, Money = order.L2BalanceCharges, RefferId = order.Id, Type = "二级代理佣金" };
                            ctx.AccountStatements.Add(s1);
                            order.Agent.Account += order.L2BalanceCharges;
                        }
                        if (order.L1BalanceCharges > 0 && !order.Agent.Captain.Status.HasFlag(MemberStatus.Freeze) && order.Agent.Captain.Status.HasFlag(MemberStatus.Agant))
                        {
                            var s2 = new AccountStatement { BeforeBalance = order.Agent.Captain.Account, CreateTime = now.AddMilliseconds(10), MemberId = order.Agent.Captain.Id, Money = order.L1BalanceCharges, RefferId = order.Id, Type = "队员代理佣金" };
                            ctx.AccountStatements.Add(s2);
                            order.Agent.Captain.Account += order.L1BalanceCharges;
                        }
                    }
                }
                #endregion
            }
        }


        internal static void PayCommoditySuccess(DateTime now, PayOrderResult result, CommodityOrder order, Core.DataAccess.XDDDbContext ctx)
        {
            #region 处理客户账户
            var ms1 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now, RefferId = order.Id, Type = "微信支付充值", Money = result.Money, MemberId = order.MemberId };
            order.Member.Account += result.Money;
            var ms2 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now.AddMilliseconds(4), RefferId = order.Id, Type = "二手交易付款", Money = -result.Money, MemberId = order.MemberId };
            order.Member.Account -= result.Money;
            ctx.AccountStatements.Add(ms1);
            ctx.AccountStatements.Add(ms2);
            #endregion
            
            order.Remark +="@|@"+ now.ToString("yyyy-MM-dd HH:mm:ss") + "：支付完成";
            order.Commodity.OnSale = false;
           
            order.CommodityOrderStatus = CommodityOrderStatus.Paied;
            
            ctx.SaveChanges();
            if (!string.IsNullOrEmpty(order.Commodity.Member.PlatformBindPhone))
            {
                SMS.SMSManager.SendNoticeSMS(new List<string> {
                    "尊敬的用户，","一件二手商品成功售出",order.Mobile,"-我卖出的宝贝"
                }, order.Commodity.Member.PlatformBindPhone);
            }
           
        }





 

        internal static void CommodityOrderCancelWidthoutSaveChange(CommodityOrder order, DateTime now, Core.DataAccess.XDDDbContext ctx)
        {
            var ms1 = new AccountStatement { BeforeBalance = order.Member.Account, CreateTime = now, RefferId = order.Id, Type = "二手交易退款", Money = order.OrderPrice, MemberId = order.MemberId };
            order.Member.Account += order.OrderPrice;
            ctx.AccountStatements.Add(ms1);
        }
        internal static void CommodityOrderSuccessWidthoutSaveChange(CommodityOrder order, DateTime now, Core.DataAccess.XDDDbContext ctx)
        {
            var ms1 = new AccountStatement { BeforeBalance = order.Commodity.Member.Account, CreateTime = now, RefferId = order.Id, Type = "二手交易入账", Money = order.OrderPrice, MemberId = order.Commodity.MemberId };
            order.Commodity.Member.Account += order.OrderPrice;
            ctx.AccountStatements.Add(ms1);
        }
    }
}