using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    /// <summary>
    ///     取现申请
    /// </summary>
    public class Withdraw
    {
        public int Id { get; set; }

        /// <summary>
        ///     申请提现金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        ///     用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     发卡银行，前期线下打款使用，后期作废
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        ///     开户行，前期线下打款使用，后期作废
        /// </summary>
        public string BankOfDeposit { get; set; }

        /// <summary>
        ///     银行卡号，前期线下打款使用，后期作废
        /// </summary>
        public string BankCard { get; set; }

        /// <summary>
        ///     订单号，前期手动填写打款交易号，后期对接微信付款，自动填写订单号
        /// </summary>
        public string OrderSN { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        public WithdrawStatus Status { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     审核人
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        ///     审核时间
        /// </summary>
        public DateTime? CheckTime { get; set; }

        /// <summary>
        ///     打款时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual Member Member { get; set; }

        public virtual Employee Employee { get; set; }
    }
    public enum WithdrawStatus
    {
        /// <summary>
        ///     待审核
        /// </summary>
        None,
        /// <summary>
        ///     已通过
        /// </summary>
        Allow,
        /// <summary>
        ///     已拒绝
        /// </summary>
        Deny,
        /// <summary>
        ///     打款中
        /// </summary>
        Paying,
        /// <summary>
        ///     已完成
        /// </summary>
        Success,
        /// <summary>
        ///     打款失败
        /// </summary>
        Cancel
    }
}
