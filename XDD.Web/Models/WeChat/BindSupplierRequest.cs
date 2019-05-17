using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using XDD.Core.Model;

namespace XDD.Web.Models.WeChat
{
    public class BindSupplierRequest
    {
        /// <summary>
        ///     真实姓名
        /// </summary>
        [MaxLength(64)]
        public string RealName { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string Mobile { get; set; }
    

        /// <summary>
        ///     验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     绑定码
        /// </summary>
        public string BindCode { get; set; }
    }
}