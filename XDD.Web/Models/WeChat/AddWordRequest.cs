using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class AddWordRequest
    {
        [MaxLength(512)]
        public string Content { get; set; }
        public int? RefferId { get; set; }
    }
}