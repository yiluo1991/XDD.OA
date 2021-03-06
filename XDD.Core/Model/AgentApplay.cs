using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    /// <summary>
    ///     代理申请
    /// </summary>
    public class AgentApply
    {
        public int Id { get; set; }
        /// <summary>
        ///     申请人Id
        /// </summary>

        public int MemberId { get; set; }

        /// <summary>
        ///     队长id，有队长id的申请，表示申请的是二级代理
        /// </summary>

        public int? CaptainId { get; set; }

        /// <summary>
        ///     身份证正面、背面、手持身份证照片
        /// </summary>
        public string Paths { get; set; }

        /// <summary>
        ///     身份证号
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        ///     真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public Sex Sex { get; set; }


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

        public VerifyStatus Status { get; set; }

        /// <summary>
        ///     导航属性：审核人
        /// </summary>
        public virtual Employee Employee { get; set; }

        /// <summary>
        ///     导航属性：申请人
        /// </summary>
        public virtual Member Member { get; set; }

    }
}
