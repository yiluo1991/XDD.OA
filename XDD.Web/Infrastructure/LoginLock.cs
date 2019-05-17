using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace XDD.Web.Infrastructure
{
    public static class LoginLock
    {
        public static void MemberPasswordError(string loginName) {
            var name="lock"+loginName.ToLower();
            CacheDependency mydep = new CacheDependency(HttpContext.Current.Server.MapPath("~/Views/config.json"));
            if (HttpRuntime.Cache[name] != null)
            {
                var newV=(int)HttpRuntime.Cache[name]+1;
                HttpRuntime.Cache.Insert(name, newV, mydep, DateTime.Now.AddSeconds(Setting.MySetting.LoginLimitTime), Cache.NoSlidingExpiration);
            }else{
                HttpRuntime.Cache.Insert(name, 1, mydep, DateTime.Now.AddSeconds(Setting.MySetting.LoginLimitTime), Cache.NoSlidingExpiration);
            }
        }

        public static void MemberLoginSuccess(string loginName) {
            var name = "lock" + loginName.ToLower();
            if (HttpRuntime.Cache[name] != null)
            {
                HttpRuntime.Cache.Remove(name);
            }
        }

        public static bool CanLogin(string loginName)
        {
            var name = "lock" + loginName.ToLower();
            if (HttpRuntime.Cache[name] != null)
            {
                if ((int)HttpRuntime.Cache[name] >= Setting.MySetting.LoginLimitTimes)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return true;
        }
    }
}