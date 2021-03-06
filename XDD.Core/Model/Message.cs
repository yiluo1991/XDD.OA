using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDD.Core.Model
{
    public class Message
    {
        public int Id { get; set; }
        
        /// <summary>
        ///     谁发的，NULL为系统消息
        /// </summary>
        public int? FromId { get; set; }

        /// <summary>
        ///     发送给谁
        /// </summary>
        public int ToId { get; set; }

        /// <summary>
        ///     消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     已读
        /// </summary>
        public bool HasRead { get; set; }

        /// <summary>
        ///     发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        public virtual Member From { get; set; }

        public virtual Member To { get; set; }
    }
}
