using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace XDD.Web.Infrastructure
{
    public static class PublisherLock
    {
        public static void MemberPublish(int memberId) {
            var name = "_" + memberId;
            CacheDependency mydep = new CacheDependency(HttpContext.Current.Server.MapPath("~/Views/config.json"));
            if (HttpRuntime.Cache[name] != null)
            {
                var newV=(int)HttpRuntime.Cache[name]+1;
                HttpRuntime.Cache.Insert(name, newV, mydep, DateTime.Now.AddSeconds(Setting.MySetting.MemberPublisherTime), Cache.NoSlidingExpiration);
            }else{
                HttpRuntime.Cache.Insert(name, 1, mydep, DateTime.Now.AddSeconds(Setting.MySetting.MemberPublisherTime), Cache.NoSlidingExpiration);
            }
        }


        public static bool CanPublish(int memberId)
        {
            var name = "_" + memberId;
            if (HttpRuntime.Cache[name] != null)
            {
                return false;
            }
            else return true;
        }
    }
}