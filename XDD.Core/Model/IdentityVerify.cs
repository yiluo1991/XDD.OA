using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class IdentityVerify
    {
        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     真实姓名
        /// </summary>
        public  string RealName{get;set;}

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
        ///     性别
        /// </summary>
        public Sex Sex{get;set;}

        /// <summary>
        ///     上传的认证图片路径，多张用|分隔
        /// </summary>
        public String ImagePaths { get; set; }
                                    
        /// <summary>
        ///     申请人Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     审核状态
        /// </summary>
        public VerifyStatus Status { get; set; }

        /// <summary>
        ///     审核人Id
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        ///     申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     审核时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }

        /// <summary>
        ///     审核回馈信息
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        ///     导航属性：审核人
        /// </summary>
        public virtual Employee Employee { get; set; }

        /// <summary>
        ///     导航属性：申请人
        /// </summary>
        public virtual Member Member { get; set; }

    }

    public enum VerifyStatus{
        /// <summary>
        ///     待审核
        /// </summary>
        None,
        /// <summary>
        ///     允许
        /// </summary>
        Allow,
        /// <summary>
        ///     拒绝
        /// </summary>
        Deny
    }
}
