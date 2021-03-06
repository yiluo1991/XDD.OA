using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace XDD.Core.Model
{
    /// <summary>
    ///     票券套餐
    /// </summary>
    public class TicketPackage
    {
        public int Id { get; set; }

        /// <summary>
        ///     票券Id
        /// </summary>
        public int TicketId { get; set; }

        /// <summary>
        ///     套餐名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     库存余量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        ///     最大能预定到几天后的票
        /// </summary>
        public int LastDay { get; set; }

        /// <summary>
        ///     一级代理佣金
        /// </summary>
        public decimal L1AgentCharges { get; set; }

        /// <summary>
        ///     二级代理佣金
        /// </summary>
        public decimal L2AgentCharges { get; set; }

        /// <summary>
        ///     一级代理佣金百分比
        /// </summary>
        public decimal L1AgentChargesPercent { get; set; }

        /// <summary>
        ///     二级代理佣金百分比
        /// </summary>
        public decimal L2AgentChargesPercent { get; set; }
        /// <summary>
        ///     一级代理佣金提成
        /// </summary>
        public decimal L1AgentMoreCharges { get; set; }

        /// <summary>
        ///     一级代理佣金提成百分比
        /// </summary>
        public decimal L1AgentMoreChargesPercent { get; set; }

        /// <summary>
        ///     提货价
        /// </summary>
        public decimal DeliveryPrice { get; set; }
        /// <summary>
        ///     价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     最后更新人Id
        /// </summary>
        public int LastUpdateId { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }


        /// <summary>
        ///  排序号
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        ///  启用，禁用，禁用时自动下架
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///  上架
        /// </summary>
        public bool OnSale { get; set; }


        /// <summary>
        ///     供票商Id
        /// </summary>
        public int SupplierId{get;set;}


        /// <summary>
        ///     导航：最后更新人
        /// </summary>
        public virtual Employee LastUpdator { get; set; }

        /// <summary>
        ///     导航：供票商
        /// </summary>
        public virtual Supplier Supplier { get; set; }

        /// <summary>
        ///     票券
        /// </summary>
        public virtual Ticket Ticket { get; set; }

        public virtual ICollection<TicketOrder>  Orders{get;set;}

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
