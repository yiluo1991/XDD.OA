using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Word
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int? RefferId { get; set; }

        public int MemberId { get; set; }
        public String Tags { get; set; }

        public DateTime CreateTime { get; set; }

        public int SN { get; set; }
        /// <summary>
        ///     置顶
        /// </summary>
        public bool Top { get; set; }

        public int LinksCount { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }
        public bool RefferHasDeleted { get; set; }

        public virtual Word Reffer { get; set; }

        public virtual ICollection<Word> Links { get; set; }

        public virtual ICollection<WordComment> Comments { get; set; }

        public virtual Member Member { get; set; }

        public virtual ICollection<WordLike> Likes { get; set; }

    }
}
