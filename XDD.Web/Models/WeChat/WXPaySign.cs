using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{


    public class WXPaySign
    {
        public string appId { get; set; }
        public int timeStamp { get; set; }
        public string nonceStr { get; set; }
        public string package { get; set; }
        public string signType { get; set; }
        public string paySign { get; set; }
    }


}