using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDD.Core.Model
{
    /// <summary>
    ///     票券分类
    /// </summary>
    public class TicketCategory
    {
        public int Id { get; set; }
        /// <summary>
        ///     分类名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        ///     排序号
        /// </summary>
        public int SN { get; set; }
        /// <summary>
        ///     是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///     导航属性：票券
        /// </summary>
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
