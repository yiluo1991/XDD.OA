using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
namespace XDD.Web.Infrastructure
{
    public static class WeChat
    {
        private static String appId;

        public static String AppId
        {
            get {
                if (appId == null)
                {
                  appId=  WebConfigurationManager.AppSettings["AppId"];
                }
                return appId;
            
            }
        }
        private static String appSecret;

        public static String AppSecret
        {
            get
            {
                if (appSecret == null)
                {
                    appSecret = WebConfigurationManager.AppSettings["AppSecret"];
                }
                return appSecret;
            }
        }
    }
}