using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class WithdrawRequest
    {
        public decimal Money { get; set; }
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
    }
}