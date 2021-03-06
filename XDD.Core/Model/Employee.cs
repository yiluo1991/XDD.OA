using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Employee
    {
        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///     用户名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        ///     密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        ///     真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        ///     角色名
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        ///     是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///     创建者Id
        /// </summary>
        public int? CreatorId { get; set; }


        /// <summary>
        ///     导航属性，创建者
        /// </summary>
        public virtual Employee Creator { get; set; }

        /// <summary>
        ///     导航属性：审核的学生认证
        /// </summary>
        public virtual ICollection<IdentityVerify> IdentityVerifies { get; set; }

        public virtual ICollection<Withdraw> Withdraws { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<TicketPackage> TicketPackages { get; set; }

        public  virtual ICollection<AgentApply> AgentApplies { get; set; }
    }
}
