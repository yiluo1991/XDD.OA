using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Payment
{
    class CompanyPaymentQueryResult
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string return_code
        {
            get;
            set;
        }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string return_msg { get; set; }

        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }

        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }

        /// <summary>
        /// 商户单号
        /// </summary>
        public string partner_trade_no { get; set; }

        /// <summary>
        /// Appid
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 微信内部付款单号
        /// </summary>
        public string detail_id { get; set; }

        /// <summary>
        /// 转账状态
        /// SUCCESS:转账成功
        /// FAILED:转账失败
        /// PROCESSING:处理中
        /// </summary>
        public string status { get; set; }

        /// <summary>
        ///     失败理由
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 收款用户openid
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public string payment_amount { get; set; }

        /// <summary>
        /// 转账时间
        /// </summary>
        public string transfer_time { get; set; }

        /// <summary>
        /// 付款成功时间
        /// </summary>
        public string payment_time { get; set; }

        /// <summary>
        /// 企业付款备注
        /// </summary>
        public string desc { get; set; }

    }
}
