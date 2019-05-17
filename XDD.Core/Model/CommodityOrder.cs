using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class CommodityOrder
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
        ///     收货人
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        ///     省份
        /// </summary>
        public String Province { get; set; }

        /// <summary>
        ///     城市
        /// </summary>
        public String City { get; set; }

        /// <summary>
        ///     区
        /// </summary>
        public String Area { get; set; }

        /// <summary>
        ///     地址
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        ///     退货省份
        /// </summary>
        public String BackProvince { get; set; }

        /// <summary>
        ///     退货城市
        /// </summary>
        public String BackCity { get; set; }

        /// <summary>
        ///     退货区
        /// </summary>
        public String BackArea { get; set; }

        /// <summary>
        ///     退货地址
        /// </summary>
        public string BackAddress { get; set; }

        public string BackRealName { get; set; }

        public string BackMobile { get; set; }



        /// <summary>
        ///     商品ID
        /// </summary>
        public int CommodityId { get; set; }

        /// <summary>
        ///     购买人
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     订单记录：时间：事件|时间：事件
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     收货时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }

        /// <summary>
        ///     退货签收时间
        /// </summary>
        public DateTime? BackDeliveryTime { get; set; }

        public String ExpressName { get; set; }

        public String ExpressNo { get; set; }
        public String BackExpressName { get; set; }
        public String BackExpressNo { get; set; }

        public String BackQequest { get; set; }

        public String BackPicthres { get; set; }

        public String BackFeedback { get; set; }

        public CommodityOrderStatus CommodityOrderStatus{get;set;}

        /// <summary>
        ///     冻结订单
        /// </summary>
        public bool Freeze { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual Member Member { get; set; }

        public virtual Commodity Commodity { get; set; }



    }
    public enum CommodityOrderStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        None,
        /// <summary>
        ///     已付款
        /// </summary>
        Paied,
        /// <summary>
        ///     付款超时
        /// </summary>
        Fail,
        /// <summary>
        ///     已发货
        /// </summary>
        Express,
        /// <summary>
        ///     已收货
        /// </summary>
        Delivery,
        /// <summary>
        ///     申请退货
        /// </summary>
        BackRequest,
        /// <summary>
        ///     允许退货
        /// </summary>
        BackAllow,
        /// <summary>
        ///     拒绝退货
        /// </summary>
        BackDeny,
        /// <summary>
        ///     退货发货
        /// </summary>
        BackExpress,
        /// <summary>
        ///     退货收货
        /// </summary>
        BackDelivery,
        /// <summary>
        ///     交易完成
        /// </summary>
        TotalSuccess,
        /// <summary>
        ///     交易取消
        /// </summary>
        TotalCancel

    }
}
