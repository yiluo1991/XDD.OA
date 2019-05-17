using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class AddBBSCommentRequest
    {
        public int ArticleId { get; set; }

        [MaxLength(512)]
        public string Comment { get; set; }

        [MaxLength(1024)]
        public string Paths { get; set; }

        public int? RefferId { get; set; }
    }
}