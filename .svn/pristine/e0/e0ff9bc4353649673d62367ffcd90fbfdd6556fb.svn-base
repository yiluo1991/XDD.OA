using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace XDD.SMS
{
    public static class SMSManager
    {
        private static String auth = "APPCODE 0c46f76dc8e8447f9d14ac12e0bbef60";
        public static string SendVerifyCode(string mobile ,string code){
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", auth);
            var s = Encoding.UTF8.GetString(client.DownloadData("https://feginesms.market.alicloudapi.com/codeNotice?param=" + code + "&phone=" + mobile + "&sign=46795&skin=100018"));
            return "短信已发送";
        }

        public static string SendServiceSMS(string mobile, string order, string user,string contact,string address)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", auth);
            var s = Encoding.UTF8.GetString(client.DownloadData("https://feginesms.market.alicloudapi.com/codeNotice?param=" + order + "|" + user + "|" + contact +"|"+address+ "&phone=" + mobile + "&sign=46795&skin=44381"));
            return "短信已发送";
        }

        public static string SendMemberSMS(string mobile,string contact)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", auth);
            var s = Encoding.UTF8.GetString(client.DownloadData("https://feginesms.market.alicloudapi.com/codeNotice?param=" + contact + "&phone=" + mobile + "&sign=46795&skin=47864"));
            return "短信已发送";
        }

        /// <summary>
        /// @您有@，对方联系电话@，详情请进入小程序@查看
        /// </summary>
        /// <returns></returns>
        public static string SendNoticeSMS(List<string> list,string mobile) {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", auth);
            var s = Encoding.UTF8.GetString(client.DownloadData("https://feginesms.market.alicloudapi.com/codeNotice?param=" + string.Join("|", list) + "&phone=" + mobile + "&sign=46795&skin=900385"));
            return "短信已发送";
        }
    }
}
