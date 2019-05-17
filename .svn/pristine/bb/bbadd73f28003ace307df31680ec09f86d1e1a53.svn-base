using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
   public class Ticket
    {

       public int Id { get; set; }

       /// <summary>
       ///  显示的售价
       /// </summary>
       public decimal Price { get; set; }
       /// <summary>
       ///  显示的原价
       /// </summary>
       public decimal OrginPrice { get; set; }
       /// <summary>
       ///  提前预定天数
       /// </summary>
       public int EarlyDay { get; set; }
       /// <summary>
       ///  票券名
       /// </summary>
       public string Name { get; set; }

       /// <summary>
       ///  消费地址    
       /// </summary>
       public string Address { get; set; }

       /// <summary>
       ///  消费商家名
       /// </summary>
       public string ShopName { get; set; }

       /// <summary>
       ///  已销售数
       /// </summary>
       public int SaleNum { get; set; }

       /// <summary>
       ///  票券分类Id
       /// </summary>
       public int TicketCategoryId { get; set; }

       /// <summary>
       ///  票券封面图片
       /// </summary>
       public string Pic { get; set; }

       /// <summary>
       ///  描述内容
       /// </summary>
       public string Content { get; set; }

       /// <summary>
       ///  创建人Id
       /// </summary>
       public int EmployeeId { get; set; }

       /// <summary>
       ///  外部文章地址
       /// </summary>
       public string MoreUrl { get; set; }

       /// <summary>
       ///  创建时间
       /// </summary>
       public DateTime CreateTime { get; set; }

       /// <summary>
       ///  启用，禁用，禁用时自动下架
       /// </summary>
       public bool Enable { get; set; }

       /// <summary>
       ///  上架
       /// </summary>
       public bool OnSale { get; set; }

       /// <summary>
       ///  地图经度
       /// </summary>
       public decimal Lng { get; set; }

       /// <summary>
       ///  地图纬度
       /// </summary>
       public decimal Lat { get; set; }

       /// <summary>
       ///  排序号
       /// </summary>
       public int SN { get; set; }

       /// <summary>
       ///     服务类型
       /// </summary>
       public ServiceType ServiceType { get; set; }

       /// <summary>
       ///  标签
       /// </summary>
       public String Tags { get; set; }

       /// <summary>
       ///  导航属性，创建者
       /// </summary>
       public virtual Employee Employee
       {
           get;
           set;
       }


       /// <summary>
       ///  导航属性，票券分类
       /// </summary>
       public virtual TicketCategory TicketCategory { get; set; }


       /// <summary>
       ///  导航属性，票券套餐
       /// </summary>
       public virtual ICollection<TicketPackage> TicketPackages { get; set; }

    }
}
