using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class AddWordCommentRequest
    {
        [MaxLength(512)]
        public string Content { get; set; }
        public int WordId { get; set; }
    }
}