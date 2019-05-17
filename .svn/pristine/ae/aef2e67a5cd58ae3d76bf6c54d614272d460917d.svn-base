using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    /// <summary>
    ///     论坛评论
    /// </summary>
    public class BBSComment
    {
        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     文章Id
        /// </summary>
        public int BBSArticleId { get; set; }

        /// <summary>
        ///     回复的评论Id
        /// </summary>
        public int? RefferId { get; set; }

        /// <summary>
        ///     用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     评论内容
        /// </summary>
        public String Comment { get; set; }

        /// <summary>
        ///     图片路径，多张使用|分割
        /// </summary>
        public string Paths { get; set; }

        /// <summary>
        ///     评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     楼层
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        ///     回复的楼层已被删除
        /// </summary>
        public bool RefferHasDeleted { get; set; }

        /// <summary>
        ///     评论所属BBS文章
        /// </summary>
        public virtual BBSArticle BBSArticle { get; set; }
        /// <summary>
        ///     评论人
        /// </summary>
        public virtual Member Member { get; set; }

        /// <summary>
        ///     指定要回复的评论
        /// </summary>
        public virtual BBSComment Reffer { get; set; }

        public virtual ICollection<BBSComment> Links { get; set; }
        

    }
}
