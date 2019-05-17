using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;

namespace XDD.Payment
{
    public  class CompanyPaymentProvider
    {
        private static string key = "abcdabcdabcdabcd1234123412341234";
        private static string mch_id = "1528136791";

        public static PayOrderResult Pay(string orderSN, decimal price, string openid)
        {
            string totalfee = Convert.ToInt32(price * 100).ToString();
            string ip = WebConfigurationManager.AppSettings["ip"];
            string appid = WebConfigurationManager.AppSettings["AppId"];
            var dic = new Dictionary<string, string>(){
                {"mch_appid", appid},
                {"mchid", mch_id},
                {"nonce_str", Payment.PaymentProvider.GetRandomString(20)/*Random.Next().ToString()*/},
                {"partner_trade_no",orderSN},//商户自己的订单号码
                {"openid",openid},
                {"check_name","NO_CHECK"},
                {"amount",totalfee},
                {"desc","付款到零钱"},
                {"spbill_create_ip",ip}
            };
            //加入签名
            dic.Add("sign", GetSignString(dic));
            var sb = new StringBuilder();
            sb.Append("<xml>");


            foreach (var d in dic)
            {
                sb.Append("<" + d.Key + ">" + d.Value + "</" + d.Key + ">");
            }
            sb.Append("</xml>");
            var xml = new XmlDocument();

            CookieCollection coo = new CookieCollection();
            Encoding en = Encoding.GetEncoding("UTF-8");

            HttpWebResponse response = CreatePostHttpResponse("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers", sb.ToString(), en);
         
            //打印返回值
            Stream stream = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html

            DataSet ds = new DataSet();
            StringReader stram = new StringReader(html);
            XmlTextReader reader = new XmlTextReader(stram);
            ds.ReadXml(reader);
            
            var json = JsonConvert.SerializeObject(ds.Tables[0]);
            CompanyPaymentResult result=null;
            try
            {
               result= JsonConvert.DeserializeObject<List<CompanyPaymentResult>>(json)[0];
            }
            catch (Exception)
            {
            }
            
            if (result!=null&& result.return_code.ToUpper() == "SUCCESS")
            {
                //通信成功
              
                if (result.result_code.ToUpper() == "SUCCESS")
                {
                    return new PayOrderResult { Success = true, Message ="付款成功"};
                }
                else
                {
                    return new PayOrderResult { Success = false, Message = "错误码："+result.err_code+"。描述："+ result.err_code_des };
                }

            }
            else if (result != null && result.return_code.ToUpper() == "FAIL")
            {
                return new PayOrderResult { Success = false, Message = "错误码：" + result.err_code + "。描述：" + result.err_code_des };
            }
            else
            {
                return new PayOrderResult { Success = false, Message = "通信失败" };
            }
        }

        public static PayOrderResult Query(string orderSN, string openid)
        {
          
            string ip = WebConfigurationManager.AppSettings["ip"];
            string appid = WebConfigurationManager.AppSettings["AppId"];
            var dic = new Dictionary<string, string>(){
                {"appid", appid},
                {"mch_id", mch_id},
                {"nonce_str", Payment.PaymentProvider.GetRandomString(20)/*Random.Next().ToString()*/},
                {"partner_trade_no",orderSN}
              
            };
            //加入签名
            dic.Add("sign",GetSignString(dic));
            var sb = new StringBuilder();
            sb.Append("<xml>");


            foreach (var d in dic)
            {
                sb.Append("<" + d.Key + ">" + d.Value + "</" + d.Key + ">");
            }
            sb.Append("</xml>");
            var xml = new XmlDocument();

            CookieCollection coo = new CookieCollection();
            Encoding en = Encoding.GetEncoding("UTF-8");

            HttpWebResponse response = CreatePostHttpResponse("https://api.mch.weixin.qq.com/mmpaymkttransfers/gettransferinfo", sb.ToString(), en);

            //打印返回值
            Stream stream = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html

            DataSet ds = new DataSet();
            StringReader stram = new StringReader(html);
            XmlTextReader reader = new XmlTextReader(stram);
            ds.ReadXml(reader);

            var json = JsonConvert.SerializeObject(ds.Tables[0]);
            CompanyPaymentQueryResult result = null;
            try
            {
                result = JsonConvert.DeserializeObject<List<CompanyPaymentQueryResult>>(json)[0];
            }
            catch (Exception)
            {
            }

            if (result != null )
            {
                if (result.return_code.ToUpper() == "SUCCESS")
                {
                    //通信成功
                    if (result.result_code.ToUpper() == "SUCCESS")
                    {
                        switch (result.status.ToUpper())
                        {
                            case "SUCCESS":
                                return new PayOrderResult { Success = true, Message = "付款成功" };
                            case "FAILED":
                                return new PayOrderResult { Success = false, Message = "付款失败，原因："+result.reason };
                            case "PROCESSING":
                                return new PayOrderResult { Success = false, Message = "付款处理中，请稍后再查询" };
                            default:
                                return new PayOrderResult { Success = false, Message = "未知状态，请稍后再试"};
                        }
                    }
                    else
                    {
                        return new PayOrderResult { Success = false, Message = "错误码：" + result.err_code + "。描述：" + result.err_code_des };
                    }
                }
                else 
                {
                    return new PayOrderResult { Success = false, Message = "查询失败：" + result.return_msg  };
                }

            }
           
            else
            {
                return new PayOrderResult { Success = false, Message = "通信失败" };
            }
        }



        public static HttpWebResponse CreatePostHttpResponse(string url, string datas, Encoding charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat(datas);
            X509Certificate2 cer = new X509Certificate2(System.Web.HttpContext.Current.Server.MapPath("~/Views/sha.cshtml"), mch_id, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            request.ClientCertificates.Add(cer);//添加证书
            byte[] data = charset.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            return request.GetResponse() as HttpWebResponse;

        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
        public static string GetSignString(Dictionary<string, string> dic)
        {
            //排序
            dic = dic.OrderBy(d => d.Key).ToDictionary(d => d.Key, d => d.Value);
            //连接字段
            var sign = dic.Aggregate("", (current, d) => current + (d.Key + "=" + d.Value + "&"));
            sign += "key=" + key;
            //MD5
            // sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sign, "MD5").ToUpper();
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            sign = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sign))).Replace("-", null);
            return sign;
        }
    }
}
