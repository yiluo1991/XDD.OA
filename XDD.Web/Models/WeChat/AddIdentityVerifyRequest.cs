using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using XDD.Core.Model;

namespace XDD.Web.Models.WeChat
{
    public class AddIdentityVerifyRequest
    {
        /// <summary>
        ///     真实姓名
        /// </summary>
        [MaxLength(64)]
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

        /// <summary>
        ///     学号
        /// </summary>
        public string SchoolSN { get; set; }

        /// <summary>
        ///     学院/学校
        /// </summary>
        public string Institute { get; set; }

        /// <summary>
        ///     专业
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        ///     验证码
        /// </summary>
        public string Code { get; set; }
    }
}