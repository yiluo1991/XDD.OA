using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDD.Core.Model
{
   public class BBSCategory
    {
       public int Id { get; set; }

       public string Name { get; set; }

       public string Icon { get; set; }

       public bool Enable { get; set; }

       public int SN { get; set; }

       public TimeSpan? TimeAreaOneStart { get; set; }
       public TimeSpan? TimeAreaOneEnd { get; set; }
       public TimeSpan? TimeAreaTwoStart { get; set; }
       public TimeSpan? TimeAreaTwoEnd { get; set; }
       /// <summary>
       /// 选填项
       /// </summary>
       public CheckableItems Option { get; set; }
       /// <summary>
       /// 必填项
       /// </summary>
       public CheckableItems Required { get; set; }

       public virtual ICollection<BBSArticle> BBSArticles { get; set; }
    }

    [Flags]
   public enum CheckableItems
   {
        /// <summary>
        /// 约会时间
        /// </summary>
       DateTime=0x1,
        /// <summary>
        /// 地点
        /// </summary>
        Address=0x2,
        /// <summary>
        /// 人数限制
        /// </summary>
        PeopleLimit=0x4,
        /// <summary>
        /// 平均支付
        /// </summary>
        Payment=0x8,
        /// <summary>
        /// 主题
        /// </summary>
        Subject=0x10
   }
}
