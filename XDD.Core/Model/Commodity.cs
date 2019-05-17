using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Commodity
    {
        /// <summary>
        ///     商品Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     商品名
        /// </summary>
        public String Name { get; set; }

        public int SN { get; set; }

        /// <summary>
        ///     封面
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        ///    图片
        /// </summary>
        public string Paths { get; set; }

        /// <summary>
        ///     商品详情
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        ///     商品分类Id
        /// </summary>
        public int CommodityCategoryId{ get; set; }

        /// <summary>
        ///     几成新,1~10,10为99新
        /// </summary>
        public int NewLevel { get; set; }

        public int ViewCount { get; set; }

        /// <summary>
        ///     价格
        /// </summary>
        public decimal Price { get; set; }

        public TradeType Type
        {
            get;
            set;
        }

        /// <summary>
        ///     用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     上架
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        ///     启用，禁用相当于删除
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }




        public virtual Member Member { get; set; }

        public virtual CommodityCategory CommodityCategory { get; set; }

        public virtual ICollection<CommodityOrder> CommodityOrders { get; set; }
      
    }

    public enum TradeType
    {
        Sale,
        Recovery
    }

   
}
