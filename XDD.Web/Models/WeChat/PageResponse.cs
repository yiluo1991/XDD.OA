using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class PageResponse
    {
        public int Total { get; set; }
        public object Rows { get; set; }

        public object More { get; set; }
    }
}