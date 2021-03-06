using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;

namespace XDD.Web.Infrastructure
{
    public static class Setting
    {

        private static SettingRoot _MySetting;
        public static SettingRoot MySetting { get{
            if (HttpRuntime.Cache["config"] == null)
            {
                var path = HttpContext.Current.Server.MapPath("~/Views/config.json");
                CacheDependency mydep = new CacheDependency(path);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string text = File.ReadAllText(path);
                SettingRoot config = jss.Deserialize<SettingRoot>(text);
                _MySetting = config;
                HttpRuntime.Cache.Insert("config", _MySetting, mydep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                return _MySetting;
            }
            else
            {
                return HttpRuntime.Cache["config"] as SettingRoot;
            }
        }
            set {
                _MySetting = value;
            }
        }
    }

    public class SettingRoot
    {
        public string VersionLimie { get; set; }
        public bool EnableLoginLimit { get; set; }
        public int LoginLimitTimes { get; set; }
        public int LoginLimitTime { get; set; }
        public int MemberPublisherTime { get; set; }
        public string Block1Src { get; set; }
        public string Block2Src { get; set; }
        public string Block3Src { get; set; }
        public string Block4Src { get; set; }
        public string Block1Url { get; set; }
        public string Block2Url { get; set; }
        public string Block3Url { get; set; }
        public string Block4Url { get; set; }
    }

}