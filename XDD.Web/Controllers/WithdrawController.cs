using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDD.Core.DataAccess;
using XDD.Core.Model;
using System.Transactions;

namespace XDD.Web.Controllers
{

    [Authorize(Roles = "管理员")]
    public class WithdrawController : Controller
    {
        // GET: Withdraw

        XDDDbContext ctx = new XDDDbContext();
        [Authorize(Roles = "管理员")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取提现列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="paymentTypeIdentifier"></param>
        /// <returns></returns>
        public JsonResult WithDrawals(string keyword, WithdrawStatus? status, DateTime? start, DateTime? end)
        {

            int pageIndex = Convert.ToInt32(Request.Params["page"] ?? "1");
            int pageSize = Convert.ToInt32(Request.Params["rows"] ?? "15");
            decimal? totalAmount;
            return Json(new
            {
                total = GetWithDrawalsCount(keyword, status, start,
                    end, out totalAmount),
                rows = GetWithDrawalsByPage(keyword, pageIndex, pageSize, status, start, end),
                totalAmount,
            }, JsonRequestBehavior.AllowGet);
        }


        #region 分页获取提现列表

        private int GetWithDrawalsCount(string keyword, WithdrawStatus? status, DateTime? start, DateTime? end, out decimal? totalAmount)
        {
            Int32 accountId;
            bool isInt32 = Int32.TryParse(keyword, out accountId);
            if (end != null) end = end.Value.Date.AddDays(1);
            var query = ctx.Withdraws.Where(p => (start == null ? true : p.CreateTime >= start.Value) && (end == null ? true : p.CreateTime < end.Value) && (status == null ? true : p.Status == status) && ((keyword != null && keyword != "") ? (p.Member.RealName.Contains(keyword) || p.Member.NickName.Contains(keyword) || p.BankCard.Contains(keyword) || (isInt32 ? p.Member.Id == accountId : false) || p.OrderSN.Contains(keyword) || p.Remark.Contains(keyword)) : true));
            totalAmount = query.Sum(s => (decimal?)s.Money);
            return query.Count();
        }
        private object GetWithDrawalsByPage(string keyword, int pageIndex, int pageSize, WithdrawStatus? status, DateTime? start, DateTime? end)
        {
            Int32 accountId;
            bool isInt32 = Int32.TryParse(keyword, out accountId);
            if (end != null) end = end.Value.Date.AddDays(1);
            return ctx.Withdraws.Where(p => (start == null ? true : p.CreateTime >= start.Value) && (end == null ? true : p.CreateTime < end.Value) && (status == null ? true : p.Status == status) && ((keyword != null && keyword != "") ? (p.Member.RealName.Contains(keyword) || p.Member.NickName.Contains(keyword) || p.BankCard.Contains(keyword) || (isInt32 ? p.Member.Id == accountId : false) || p.OrderSN.Contains(keyword) || p.Remark.Contains(keyword)) : true)).OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(p => new
            {
                p.Money,//提现金额  
                AccountId = p.MemberId,//用户ID
                p.BankName,//提现银行   
                p.BankOfDeposit,//提现银行地址  
                p.BankCard,//提现卡
                p.OrderSN,//提现订单号
                p.Status,//提现状态
                p.Remark,//提现备注
                p.Member.Account,//账户余额
                p.CreateTime,
                p.CheckTime,
                p.PayTime,
                p.EmployeeId,//审核人
                p.Id,
                MemberName = p.Member.RealName,
                MemberLoginName = p.Member.NickName,
            });
        }

        #endregion
        /// <summary>
        ///     获取提现审核信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetWithdrawAuditInfo(Int32 id)
        {
            var withDraw = ctx.Withdraws.FirstOrDefault(s => s.Id == id);
            if (withDraw != null)
            {
                if (withDraw.Status == WithdrawStatus.None)
                {
                    var account = withDraw.Member;
                    return Json(new
                    {
                        resultCode = 1,
                        data = new
                        {
                            AllowedDepositFrzByWithdraw = true,
                            withDraw.Member.RealName,
                            withDraw.Member.NickName,
                            withDraw.CreateTime,
                            withDraw.Member.WeChatBindPhone,
                            withDraw.Member.PlatformBindPhone,
                            withDraw.BankName,
                            withDraw.BankCard,
                            withDraw.BankOfDeposit,
                            withDraw.Money,
                            withDraw.Remark,
                            Withdrawable = account.Account,
                            TotalAmount = account.Account,
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        resultCode = 0,
                        message = "该记录无法审核"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    resultCode = 0,
                    message = "没有找到相应提现记录"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     审核提现申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AuditWithDrawal(Int32 id, WithdrawStatus status, string remark)
        {
            JsonResult result;
            if (ModelState.IsValid && (status == WithdrawStatus.Allow || status == WithdrawStatus.Deny))
            {
                var now = DateTime.Now;
                int user = int.Parse(User.Identity.Name);
                Employee employee = ctx.Employees.First(s => s.Id == user);
                using (TransactionScope scope = new TransactionScope())
                {
                    var withdrawal = ctx.Withdraws.SingleOrDefault(p => p.Id == id);
                    if (withdrawal != null)
                    {
                        if (withdrawal.Status == WithdrawStatus.None)
                        {
                            try
                            {
                                var account = withdrawal.Member;
                                if (status == WithdrawStatus.Deny)
                                {
                                    //拒绝
                                    account.Account += withdrawal.Money;
                                    withdrawal.Status = status;
                                    withdrawal.Remark = "姓名/卡号：" + account.RealName + "/" + withdrawal.BankCard + "<br/>审核人:" + employee.RoleName + "，审核时间：" + now.ToString("yyyy-MM-dd HH:mm:ss") + "<br />备注：" + remark ?? "无";
                                    AddAccountStatement(withdrawal.Money, account, account.Account, "提现审核不通过",withdrawal.Id);
                                    ctx.SaveChanges();
                                    result = Json(new
                                    {
                                        resultCode = 1,
                                        row = new
                                        {
                                            AccountId = withdrawal.MemberId,
                                            MemberLoginName = withdrawal.Member.NickName,
                                            MemberName = withdrawal.Member.RealName,
                                            withdrawal.CreateTime,
                                            withdrawal.Id,
                                            Account = withdrawal.BankCard,
                                            withdrawal.BankName,
                                            withdrawal.BankOfDeposit,
                                            withdrawal.OrderSN,
                                            withdrawal.Status,
                                            withdrawal.Money,
                                            withdrawal.Remark
                                        }

                                    });
                                }
                                else
                                {
                                    withdrawal.Status = status;
                                    withdrawal.Remark = "姓名/卡号：" + account.RealName + "/" + withdrawal.BankCard + "<br/>审核人:" + employee.RoleName + "，审核时间：" + now.ToString("yyyy-MM-dd HH:mm:ss") + "<br />备注：" + remark ?? "无";
                                    ctx.SaveChanges();
                                    result = Json(new
                                    {
                                        resultCode = 1,
                                        row = new
                                        {
                                            AccountId = withdrawal.MemberId,
                                            MemberLoginName = withdrawal.Member.NickName,
                                            MemberName = withdrawal.Member.RealName,
                                            withdrawal.CreateTime,
                                            withdrawal.Id,
                                            Account = withdrawal.BankCard,
                                            withdrawal.BankName,
                                            withdrawal.BankOfDeposit,
                                            withdrawal.OrderSN,
                                            withdrawal.Status,
                                            withdrawal.Money,
                                            withdrawal.Remark
                                        }
                                    });
                                }
                                scope.Complete();
                            }
                            catch (Exception e)
                            {

                                result = Json(new { resultCode = 0, message = "事务冲突，请重试" });

                            }
                        }
                        else
                        {
                            result = Json(new { resultCode = 0, message = "当前记录无法进行审核" });
                        }
                    }
                    else
                    {
                        result = Json(new { resultCode = 0, message = "未找到指定的提现记录" });
                    }
                }
            }

            else
            {
                result = Json(new { resultCode = 0, message = "参数有误" });
            }
            return result;
        }

        /// <summary>
        ///    代付
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Pay(Int32 id)
        {
            var now = DateTime.Now;
            int user = int.Parse(User.Identity.Name);
            Employee employee = ctx.Employees.First(s => s.Id == user);
            var target=ctx.Withdraws.FirstOrDefault(s => s.Id == id&&(s.Status==WithdrawStatus.Allow));
          
            if(null!=target){
                  var ordersn=now.ToString("WyyyyMMddHHmmssfff") + (Guid.NewGuid().ToString().Split('-')[0]);
                target.OrderSN=ordersn;
                target.Status=WithdrawStatus.Paying;

                target.PayTime = now;
                ctx.SaveChanges();
               var result= Payment.CompanyPaymentProvider.Pay(ordersn, target.Money, target.Member.AppOpenId);
               if (result.Success)
               {
                   target.Status = WithdrawStatus.Success;
                   ctx.SaveChanges();
                   return Json(new { ResultCode = 1, Message = "提现成功" });
               }
               else
               {
                   return Json(new { ResultCode = 0, Message = result.Message });
               }
            }
            else
            {
                return Json(new { ResultCode = 0, Message = "没有找到要处理的提现信息" });
            }
        }
        [HttpPost]
        public JsonResult Qyert(Int32 id)
        {
            var now = DateTime.Now;
            int user = int.Parse(User.Identity.Name);
            Employee employee = ctx.Employees.First(s => s.Id == user);
            var target = ctx.Withdraws.FirstOrDefault(s => s.Id == id && (s.Status == WithdrawStatus.Paying));

            if (null != target)
            {
              
                var result = Payment.CompanyPaymentProvider.Query(target.OrderSN, target.Member.AppOpenId);
                if (result.Success)
                {
                    target.Status = WithdrawStatus.Success;
                    ctx.SaveChanges();
                    return Json(new { ResultCode = 1, Message = "提现成功" });
                }
                else
                {
                    return Json(new { ResultCode = 0, Message = result.Message });
                }
            }
            else
            {
                return Json(new { ResultCode = 0, Message = "没有找到要处理的提现信息或不在支付中状态" });
            }
        }

        /// <summary>
        ///     将客户提现审核设置为失败
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeWithdrawalStatusToFailture(Int32 id)
        {
            try
            {
                int user = int.Parse(User.Identity.Name);
                Employee employee = ctx.Employees.First(s => s.Id == user);
                var withdrawal = ctx.Withdraws.Where(p => p.Id == id).FirstOrDefault();
                if (withdrawal != null)
                {
                    if (withdrawal.Status == WithdrawStatus.Allow || withdrawal.Status == WithdrawStatus.Paying)
                    {
                        string status = "";
                        switch (withdrawal.Status)
                        {
                            case WithdrawStatus.Allow:
                                status = "已通过审核";
                                break;
                            case WithdrawStatus.Paying:
                                status = "处理中";
                                break;
                        }
                        var account = withdrawal.Member;
                        withdrawal.Status = WithdrawStatus.Cancel;
                        account.Account += withdrawal.Money;
                        DateTime now = DateTime.Now;
                        AddAccountStatement(withdrawal.Money, account, account.Account, "提现失败",withdrawal.Id);
                        withdrawal.Remark += "<br/>" + now.ToString("yyyy/MM/dd hh:mm:ss") + " " + employee.RoleName + "由（" + status + "）变更为（提现失败）";
                        ctx.SaveChanges();
                        return Json(new { resultCode = 1, message = "处理成功" });
                    }
                    else
                    {
                        return Json(new { resultCode = 0, message = "所选提现申请不在可变更为提现失败的状态" });
                    }
                }
                else
                {
                    return Json(new { resultCode = 0, message = "未找到要操作的提现申请" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultCode = 0, message = "事务冲突，请重新提交" });
            }
        }
        /// <summary>
        ///     添加账户明细
        /// </summary>
        private void AddAccountStatement(decimal amount, Member RealName, decimal Account, string type,int id)
        {
            AccountStatement accountSmt = new AccountStatement
            {
                Money = amount,
                CreateTime = DateTime.Now,
                BeforeBalance = Account,
                Type = type,
                Member = RealName,
                RefferId=id//真实姓名,

            };
            ctx.AccountStatements.Add(accountSmt);
        }

        /// <summary>
        ///     人工处理打款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfirmRemittance(Int32 id, string orderSN)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (!string.IsNullOrEmpty(orderSN))
                {
                    orderSN = orderSN.Trim();
                    try
                    {
                        if (ctx.Withdraws.Count(s => s.OrderSN == orderSN) == 0)
                        {
                            var withdrawal = ctx.Withdraws.SingleOrDefault(p => p.Id == id);
                            if (withdrawal != null)
                            {
                                if (withdrawal.Status == WithdrawStatus.Allow)
                                {
                                    if (withdrawal.OrderSN == null)
                                    {
                                        var account = withdrawal.Member;
                                        withdrawal.OrderSN = orderSN;
                                        withdrawal.Status = WithdrawStatus.Success;
                                        withdrawal.PayTime = DateTime.Now;
                                        ctx.SaveChanges();
                                        scope.Complete();
                                        return Json(new { resultCode = 1, message = "打款成功" });
                                    }
                                    else
                                    {
                                        return Json(new { resultCode = 0, message = "已有单号，不可修改" });
                                    }
                                }
                                else
                                {
                                    return Json(new { resultCode = 0, message = "该记录无法打款" });
                                }

                            }
                            else
                            {
                                return Json(new { resultCode = 0, message = "未找到指定的提现记录" });
                            }
                        }
                        else
                        {
                            return Json(new { resultCode = 0, message = "检测到重复的交易单号" });

                        }
                    }
                    catch (Exception)
                    {
                        return Json(new { resultCode = 0, message = "事务冲突，请重试" });

                    }
                }
                else
                {
                    return Json(new { resultCode = 0, message = "参数有误" });
                }
            }
        }
        /// <summary>
        ///     获取凭条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSlip(Int32 id)
        {

            var withdrawal = ctx.Withdraws.FirstOrDefault(s => s.Id == id);
            if (withdrawal != null)
            {

                var account = withdrawal.Member;
                return Json(new
                {
                    resultCode = 1,
                    data = new
                    {
                        AllowedDepositFrzByWithdraw = true,
                        AccountId = withdrawal.MemberId,
                        MemberLoginName = withdrawal.Member.NickName,
                        MemberName = withdrawal.Member.RealName,
                        withdrawal.CreateTime,
                        withdrawal.Id,
                        Account = withdrawal.BankCard,
                        withdrawal.BankName,
                        withdrawal.BankOfDeposit,
                        withdrawal.OrderSN,
                        withdrawal.Status,
                        withdrawal.Money,
                        withdrawal.Remark,
                        withdrawal.Member.PlatformBindPhone,
                        withdrawal.PayTime
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    resultCode = 0,
                    message = "没有找到相应提现记录"
                }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}

