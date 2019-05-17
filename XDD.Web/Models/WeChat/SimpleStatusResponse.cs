using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class SimpleStatusResponse
    {
        public int ResultCode { get; set; }
        public int Id { get; set; }
        public int Num { get; set; }
        public string Message { get; set; }
    }
}