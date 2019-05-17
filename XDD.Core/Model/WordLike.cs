using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class WordLike
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int WordId { get; set; }

        public virtual Word Word { get; set; }
        public virtual Member Member { get; set; }
    }
}
