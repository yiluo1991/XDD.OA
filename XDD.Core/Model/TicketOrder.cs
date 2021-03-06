using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class TicketOrder
    {
        public int Id { get; set; }

        /// <summary>
        /// 支付接口提交的订单号
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 支付接口提交的价格
        /// </summary>
        public decimal OrderPrice { get; set; }

        /// <summary>
        ///     核销验证码
        /// </summary>
        public string VCode { get; set; }

        public string RealName { get; set; }


        public string Mobile { get; set; }

        public DateTime UseDate { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        /// <summary>
        /// 商品名（微信接口）
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 商品描述（微信接口）
        /// </summary>
        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public OrderStatus Status { get; set; }

        /// <summary>
        ///     票券套餐Id
        /// </summary>
        public int TicketPackageId { get; set; }

        /// <summary>
        ///  供货商Id
        /// </summary>
        public int SupplierId { get; set; }

        public int MemberId { get; set; }

        /// <summary>
        ///     一级代理佣金
        /// </summary>
        public decimal L1AgentCharges { get; set; }

        /// <summary>
        ///     二级代理佣金
        /// </summary>
        public decimal L2AgentCharges { get; set; }
        /// <summary>
        ///     一级代理佣金提成
        /// </summary>
        public decimal L1AgentMoreCharges { get; set; }

        /// <summary>
        ///     一级代理佣金百分比
        /// </summary>
        public decimal L1AgentChargesPercent { get; set; }

        /// <summary>
        ///     二级代理佣金百分比
        /// </summary>
        public decimal L2AgentChargesPercent { get; set; }

        /// <summary>
        ///     一级代理佣金提成百分比
        /// </summary>
        public decimal L1AgentMoreChargesPercent { get; set; }

        /// <summary>
        ///      一级代理实际佣金
        /// </summary>
        public decimal L1BalanceCharges { get; set; }

        /// <summary>
        ///      二级代理实际佣金
        /// </summary>
        public decimal L2BalanceCharges { get; set; }



        /// <summary>
        ///     提货价
        /// </summary>
        public decimal DeliveryPrice { get; set; }

        /// <summary>
        ///     推荐的代理用户Id
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        ///     退款理由
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        ///     申请退款中
        /// </summary>
        public bool HasRequest { get; set; }

        /// <summary>
        ///     宿舍地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     取件码
        /// </summary>
        public string PackageCode { get; set; }



        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual TicketPackage TicetPackage { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual Member Member { get; set; }

        public virtual Member Agent { get; set; }
       
     
    }


    public enum ServiceType
    {
        /// <summary>
        /// 门票
        /// </summary>
        Ticket,
        /// <summary>
        /// 包裹
        /// </summary>
        Package
    }
    public enum OrderStatus
    {
        /// <summary>
        ///     未付款：0
        /// </summary>
        None ,
        /// <summary> 
        ///     已付款：1
        /// </summary>
        Paied ,
        /// <summary>
        ///     付款超时：2
        /// </summary>
        Fail,
        /// <summary>
        ///     已出票：3
        /// </summary>
        Success,
        /// <summary>
        ///     已处理：4
        /// </summary>
        Handled,
        /// <summary>
        ///     已核销：5
        /// </summary>
        CheckOut,
     
        /// <summary>
        ///     退票：6
        /// </summary>
        TurnBack

    }
}
