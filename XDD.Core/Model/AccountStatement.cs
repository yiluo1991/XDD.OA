using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class AccountStatement
    {
        public int Id { get; set; }

        /// <summary>
        ///     用户
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     交易前账户余额
        /// </summary>
        public decimal BeforeBalance { get; set; }

         /// <summary>
        ///     交易金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        ///     时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     交易类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     相关记录Id
        /// </summary>
        public int? RefferId{get;set;}

        public virtual Member Member { get; set; }
    }
}
