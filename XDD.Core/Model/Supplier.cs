using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Supplier
    {
        /// <summary>
        ///     供应商Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     创建人
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     绑定的用户Id
        /// </summary>
        public int? MemberId { get; set; }

        public bool Enable { get; set; }
        /// <summary>
        ///     导航属性：用户
        /// </summary>
        public virtual Member Member { get; set; }

        /// <summary>
        ///     导航属性：创建人
        /// </summary>
        public virtual Employee Employee { get; set; }

        /// <summary>
        ///     导航属性：提供的票券套餐
        /// </summary>
        public virtual ICollection<TicketPackage> TicketPackages { get; set; }

        public virtual ICollection<TicketOrder> Orders { get; set; }
    }
}
