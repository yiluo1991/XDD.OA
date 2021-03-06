using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    /// <summary>
    ///     论坛文章
    /// </summary>
    public class BBSArticle
    {
        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     分类Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     约会时间
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        ///     约会地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     起始人数
        /// </summary>
        public int? PeopleStart { get; set; }

        /// <summary>
        ///     截止人数
        /// </summary>
        public int? PeopleEnd { get; set; }

        /// <summary>
        ///     人均支出
        /// </summary>
        public decimal? Payment { get; set; }

        /// <summary>
        ///     主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     排序号
        /// </summary>
        public int SN { get; set; }
        /// <summary>
        ///     首页显示
        /// </summary>
        public bool HomeShow { get; set; }
        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     文章内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     是否后台文章,后台文章支持html格式，小程序需要使用插件加载
        /// </summary>
        public bool IsBackgroundArticle { get; set; }

        /// <summary>
        ///     图片路径，多张使用|分割
        /// </summary>
        public string Paths { get; set; }

        /// <summary>
        ///     阅读数
        /// </summary>
        public int ReadCount { get; set; }

        /// <summary>
        ///     评论数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        ///     可评论
        /// </summary>
        public bool AllowComment { get; set; }

        /// <summary>
        ///     显示评论
        /// </summary>
        public bool ShowComment { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     创建人
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     楼层计数器
        /// </summary>
        public int Counter { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual BBSCategory BBSCategory { get; set; }

        public virtual Member Member { get; set; }

        public virtual ICollection<BBSComment> BBSComments { get; set; }


    }
}
