using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class WordComment
    {
        public Int32 Id { get; set; }
        public Int32 WordId { get; set; }
        public String Content { get; set; }
        public int MemberId { get; set; }

        public DateTime CreateTime { get; set; }
        public virtual Word Word { get; set; }
        public virtual Member Member { get; set; }
    }
}
