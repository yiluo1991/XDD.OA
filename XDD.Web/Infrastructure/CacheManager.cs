using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using XDD.Web.Models;
using System.Web.Configuration;
using System.Text;
using XDD.Web.Models.WeChat;
using XDD.Payment;
namespace XDD.Web.Infrastructure
{
    public static class CacheManager
    {
        /// <summary>
        ///     设置手机验证码缓存
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="mobilecode"></param>
        /// <returns></returns>
        public static bool SetMobileCode(int MemberId, MobileCode mobilecode)
        {
            if (HttpRuntime.Cache["m" + MemberId] == null || (HttpRuntime.Cache["m" + MemberId] != null && (mobilecode.SendTime - (HttpRuntime.Cache["m" + MemberId] as MobileCode).SendTime).TotalSeconds >= 120))
            {
                HttpRuntime.Cache.Insert("m" + MemberId, mobilecode, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        ///     从缓存验证手机验证码
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckMobileCode(int MemberId, string mobile, string code)
        {
            if (HttpRuntime.Cache["m" + MemberId] == null)
            {
                return false;
            }
            else
            {
                MobileCode mobilecode = HttpRuntime.Cache["m" + MemberId] as MobileCode;
                return mobilecode.Code.ToLower() == code.ToLower() && mobile == mobilecode.Mobile;
            }
        }

        public static void ClearMobileCode(int MemberId)
        {
            if (HttpRuntime.Cache["m" + MemberId] != null)
            {
                HttpRuntime.Cache.Remove("m" + MemberId);
            }

        }



        /// <summary>
        ///     设置供应商识别码
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="mobilecode"></param>
        /// <returns></returns>
        public static void SetSupplierCode(string code, int supplierId)
        {

            HttpRuntime.Cache.Insert("s" + code.ToLower(), supplierId, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);


        }

        /// <summary>
        ///     从缓存验证供应商识别码
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int CheckSupplierCode(string code)
        {
            if (HttpRuntime.Cache["s" + code.ToLower()] == null)
            {
                return 0;
            }
            else
            {

                return Convert.ToInt32(HttpRuntime.Cache["s" + code.ToLower()]);
            }
        }

        public static void ClearSupplierCode(string code)
        {
            if (HttpRuntime.Cache["s" + code.ToLower()] != null)
            {
                HttpRuntime.Cache.Remove("s" + code.ToLower());
            }

        }



        public static void SetMemberPublisherFlag(int MemberId)
        {
            CacheDependency mydep = new CacheDependency(HttpContext.Current.Server.MapPath("~/Views/config.json"));
            HttpRuntime.Cache.Insert("p" + MemberId, true, mydep, DateTime.Now.AddSeconds(Setting.MySetting.MemberPublisherTime), TimeSpan.Zero);
        }

        public static bool CanMemberPublisher(int MemberId)
        {
            return HttpRuntime.Cache["p" + MemberId] == null;
        }


        public static void SetPrepay_id(int memberId, string prepay_id)
        {
            HttpRuntime.Cache.Insert("t" + memberId, prepay_id, null, DateTime.Now.AddHours(2), TimeSpan.Zero);
        }
        public static string GetPrepay_id(int memberId)
        {
            if (HttpRuntime.Cache["t" + memberId] != null)
            {
                return HttpRuntime.Cache["t" + memberId].ToString();
            }
            else
            {
                return null;
            }
        }



        public static void SetAppToken(string token, int second)
        {
            HttpRuntime.Cache.Insert("accesstoken", token, null, DateTime.Now.AddSeconds((second - 100) > 0 ? (second - 100) : 0), TimeSpan.Zero);
        }

        public static string GetAppToken()
        {
            if (HttpRuntime.Cache["accesstoken"] != null)
            {
                return HttpRuntime.Cache["accesstoken"].ToString();
            }
            else
            {
                WebClient web = new WebClient();
                try
                {
                    string str = Encoding.UTF8.GetString(web.DownloadData("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WebConfigurationManager.AppSettings["AppId"] + "&secret=" + WebConfigurationManager.AppSettings["AppSecret"]));
                   
                    var s = (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<WXTokenResponse>(str);
                    if (!string.IsNullOrEmpty(s.access_token)) {
                        SetAppToken(s.access_token, s.expires_in);
                        return s.access_token;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }
}