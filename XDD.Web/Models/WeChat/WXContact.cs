using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDD.Core.Model;

namespace XDD.Web.Models.WeChat
{
    public class WXContact
    {
        public int Id { get; set; }
        public DateTime LastTime { get; set; }
        public string LastContent { get; set; }
        public string AvatarUrl { get; set; }
        public string NickName { get; set; }
        public MemberStatus Status { get; set; }
    }
    


}