using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDD.Web.Models.WeChat
{
    public class EditUserInfoRequest
    {
        public string Name { get; set; }
        public string Province { get; set; }

        public string City { get; set; }

        public string AvatarUrl { get; set; }
    }
}