using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class AppLoginRequest
    {
        public string encryptedData { get; set; }
        public string iv { get; set; }

        public int id { get; set; }
    }
}