using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class WXSessionResponse
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
    }

}