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
using System.Transactions;

namespace XDD.Web.Controllers.API
{
    /// <summary>
    /// 账户接口
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        XDDDbContext ctx = new XDDDbContext();


        /// <summary>
        ///     获取账单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetStatements"), HttpPost]
        [TokenAuthorize]
        public PageResponse GetStatements(PageRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var query = ctx.AccountStatements.Where(s => s.MemberId == memberId && !s.Type.Contains("充值"));
            return new PageResponse
            {
                Rows = query.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 15).Take(15).Select(s => new
                {
                    s.CreateTime,
                    s.Id,
                    s.Money,
                    s.Type
                }).ToList(),
                Total = query.Count()
            };
        }

        /// <summary>
        ///     获取佣金记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetCharges"), HttpPost]
        [TokenAuthorize]
        public PageResponse GetCharges(PageRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var query = ctx.AccountStatements.Where(s => s.MemberId == memberId && s.Type.Contains("佣金"));
            return new PageResponse
            {
                Rows = query.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 15).Take(15).Select(s => new
                {
                    s.CreateTime,
                    s.Id,
                    s.Money,
                    s.Type
                }).ToList(),
                Total = query.Count()
            };
        }

        /// <summary>
        ///     获取佣金总额
        /// </summary>
        /// <returns></returns>
        [Route("GetChargeTotal"), HttpPost]
        [TokenAuthorize]
        public DetailResponse GetChargeTotal()
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var query = ctx.AccountStatements.Where(s => s.MemberId == memberId && s.Type.Contains("佣金"));
            return new DetailResponse
            {
                Message = "获取成功",
                ResultCode = 1,
                Detail = query.Sum(s => (decimal?)s.Money)??0
            };
        }

        /// <summary>
        ///     获取队员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetTeamMembers"), HttpPost]
        [TokenAuthorize]
        public PageResponse GetTeamMembers(PageRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var query = ctx.Members.Where(s => s.CaptainId==memberId&&s.Status.HasFlag(MemberStatus.Agant)&&!s.Status.HasFlag(MemberStatus.Freeze));
            return new PageResponse
            {
                Rows = query.OrderByDescending(s => s.Id).Skip((req.page - 1) * 15).Take(15).Select(s => new
                {
                   s.Id,
                   s.AvatarUrl,
                   OrderCount = s.AgentOrders.Where(t => t.Status != OrderStatus.None && t.Status != OrderStatus.Fail && t.Status != OrderStatus.TurnBack).Count(),
                   s.NickName
                }).ToList(),
                Total = query.Count()
            };
        }


        /// <summary>
        ///     获取提现记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("GetWithdraws"), HttpPost]
        [TokenAuthorize]
        public PageResponse GetWithdraws(PageRequest req)
        {
            int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
            var query = ctx.Withdraws.Where(s => s.MemberId == memberId);
            return new PageResponse
            {
                Rows = query.OrderByDescending(s => s.CreateTime).Skip((req.page - 1) * 15).Take(15).Select(s => new
                {
                    s.CreateTime,
                    s.Money,
                    s.Status,
                    s.Remark,
                    s.Id
                }).ToList(),
                Total = query.Count()
            };
        }

        /// <summary>
        ///     提现
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("WithdrawMoney"), HttpPost]
        [TokenAuthorize]
        public SimpleStatusResponse WithdrawMoney(WithdrawRequest req)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                req.Money = Decimal.Round(req.Money, 2);
                int memberId = Convert.ToInt32(TicketWoker.GetTicket(Request.Headers.GetValues("token").First()).Name);
                var member = ctx.Members.FirstOrDefault(s => s.Id == memberId&&!s.Status.HasFlag(MemberStatus.Freeze));
                if (member != null)
                {
                    if (req.Money <= member.Account && req.Money > 0)
                    {
                        var orginAccount = member.Account;
                        Withdraw w = new Withdraw { Money = req.Money, BankCard = req.BankCard, BankName = req.BankName, BankOfDeposit = req.BankOfDeposit, Status = WithdrawStatus.None, MemberId = memberId, CreateTime = DateTime.Now };
                        member.Account -= req.Money;
                        ctx.Withdraws.Add(w);


                        try
                        {
                            ctx.SaveChanges();
                            ctx.AccountStatements.Add(new AccountStatement
                            {
                                CreateTime = DateTime.Now,
                                BeforeBalance = orginAccount,
                                Money = -req.Money,
                                RefferId = w.Id,
                                Type = "提现",
                                MemberId = member.Id

                            });
                            ctx.SaveChanges();
                            scope.Complete();
                            return new SimpleStatusResponse { ResultCode = 1, Message = "提现申请成功" };
                        }
                        catch (Exception)
                        {

                            return new SimpleStatusResponse { ResultCode = 0, Message = "请重试" };
                        }

                    }
                    else
                    {

                        return new SimpleStatusResponse { ResultCode = 0, Message = "交易金额超出限制" };
                    }

                }
                else
                {

                    return new SimpleStatusResponse { ResultCode = 0, Message = "用户不存在" };
                }

            }

        }
    }
}
