using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDD.Core.Model;

namespace XDD.Web.Models.WeChat
{
    public class AddAgentVerifyRequest
    {
        /// <summary>
        ///     真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        ///     上传的认证图片路径，多张用|分隔
        /// </summary>
        public String ImagePaths { get; set; }

        public string Mobile { get; set; }

        public string IDCard { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        public string Code { get; set; }

        public string RefferCode { get; set; }
    }
}