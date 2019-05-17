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
    public static class PaymentProvider
    {

        private static string key = "dnij83t9ubshgfkl152l3i5h2u3mc092";
        private static string mch_id = "1511773541";

        public static void WriteLogs(string content)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(path))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "log";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (!File.Exists(path))
                    {
                        FileStream fs = File.Create(path);
                        fs.Close();
                    }
                    if (File.Exists(path))
                    {
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + content);
                        //  sw.WriteLine("----------------------------------------");
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {
                
             
            }
          
        }

        /// <summary>
        ///     发起支付
        /// </summary>
        /// <param name="now"></param>
        /// <param name="openid"></param>
        /// <param name="orderSN"></param>
        /// <param name="Price"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Pay(DateTime now, string openid, string orderSN, decimal Price, string body)
        {


            string appid = WebConfigurationManager.AppSettings["AppId"];
            string secret = WebConfigurationManager.AppSettings["AppSecret"];
            string ip = WebConfigurationManager.AppSettings["ip"];
            string PayResulturl = WebConfigurationManager.AppSettings["PayResulturl"];


            string totalfee = Convert.ToInt32(Price * 100).ToString();

            System.Random Random = new System.Random();
            var dic = new Dictionary<string, string>(){
                {"appid", appid},
                {"mch_id", mch_id},
                {"nonce_str", GetRandomString(20)/*Random.Next().ToString()*/},
                {"body",body},
                {"out_trade_no",orderSN},//商户自己的订单号码
                {"total_fee",totalfee},
                {"spbill_create_ip",ip},//服务器的IP地址
                {"notify_url",PayResulturl},//异步通知的地址，不能带参数
                {"trade_type","JSAPI" },
                {"openid",openid}
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

            HttpWebResponse response = CreatePostHttpResponse("https://api.mch.weixin.qq.com/pay/unifiedorder", sb.ToString(), en);
            //打印返回值
            Stream stream = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html

            xml.LoadXml(html);


            var root = xml.DocumentElement;
            DataSet ds = new DataSet();
            StringReader stram = new StringReader(html);
            XmlTextReader reader = new XmlTextReader(stram);
            ds.ReadXml(reader);
            string return_code = ds.Tables[0].Rows[0]["return_code"].ToString();
            if (return_code.ToUpper() == "SUCCESS")
            {
                //通信成功
                string result_code = ds.Tables[0].Rows[0]["result_code"].ToString();//业务结果
                if (result_code.ToUpper() == "SUCCESS")
                {
                    var res = new Dictionary<string, string>
                    {
                        {"appId", appid},
                        {"timeStamp", GetTimeStamp()},
                        {"nonceStr", dic["nonce_str"]},
                        {"package",  "prepay_id="+ds.Tables[0].Rows[0]["prepay_id"].ToString()},
                        {"signType", "MD5"}
                    };
                    //在服务器上签名
                    res.Add("paySign", GetSignString(res));
                    return res;
                }
                else
                {
                    WriteLogs(ds.Tables[0].Rows[0]["return_msg"].ToString());
                }

            }
            else if (return_code.ToUpper() == "FAIL")
            {
                WriteLogs(ds.Tables[0].Rows[0]["return_msg"].ToString());
            }
            return null;
        }


        /// <summary>
        ///     接收通知
        /// </summary>
        /// <returns></returns>
        public static PayOrderResult ReviceNotice()
        {
            String xmlData = getPostStr();//获取请求数据
            if (xmlData == "")
            {
                return null;
            }
            else
            {

                DataSet ds = new DataSet();
                StringReader stram = new StringReader(xmlData);
                XmlTextReader datareader = new XmlTextReader(stram);
               
                ds.ReadXml(datareader);
                if (ds.Tables[0].Rows[0]["return_code"].ToString() == "SUCCESS")
                {


                    string wx_appid = "";//微信开放平台审核通过的应用APPID
                    string wx_mch_id = "";//微信支付分配的商户号

                    string wx_nonce_str = "";// 	随机字符串，不长于32位
                    string wx_sign = "";//签名，详见签名算法
                    string wx_result_code = "";//SUCCESS/FAIL

                    string wx_return_code = "";
                    string wx_openid = "";//用户在商户appid下的唯一标识
                    string wx_is_subscribe = "";//用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
                    string wx_trade_type = "";// 	APP
                    string wx_bank_type = "";// 	银行类型，采用字符串类型的银行标识，银行类型见银行列表
                    string wx_fee_type = "";// 	货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型


                    string wx_transaction_id = "";//微信支付订单号
                    string wx_out_trade_no = "";//商户系统的订单号，与请求一致。
                    string wx_time_end = "";// 	支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
                    int wx_total_fee = -1;// 	订单总金额，单位为分
                    int wx_cash_fee = -1;//现金支付金额订单现金支付金额，详见支付金额


                    #region  数据解析
                    //列 是否存在
                    string signstr = "";//需要前面的字符串
                    //wx_appid
                    if (ds.Tables[0].Columns.Contains("appid"))
                    {
                        wx_appid = ds.Tables[0].Rows[0]["appid"].ToString();
                        if (!string.IsNullOrEmpty(wx_appid))
                        {
                            signstr += "appid=" + wx_appid;
                        }
                    }

                    //wx_bank_type
                    if (ds.Tables[0].Columns.Contains("bank_type"))
                    {
                        wx_bank_type = ds.Tables[0].Rows[0]["bank_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_bank_type))
                        {
                            signstr += "&bank_type=" + wx_bank_type;
                        }
                    }
                    //wx_cash_fee
                    if (ds.Tables[0].Columns.Contains("cash_fee"))
                    {
                        wx_cash_fee = Convert.ToInt32(ds.Tables[0].Rows[0]["cash_fee"].ToString());

                        signstr += "&cash_fee=" + wx_cash_fee;
                    }

                    //wx_fee_type
                    if (ds.Tables[0].Columns.Contains("fee_type"))
                    {
                        wx_fee_type = ds.Tables[0].Rows[0]["fee_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_fee_type))
                        {
                            signstr += "&fee_type=" + wx_fee_type;
                        }
                    }

                    //wx_is_subscribe
                    if (ds.Tables[0].Columns.Contains("is_subscribe"))
                    {
                        wx_is_subscribe = ds.Tables[0].Rows[0]["is_subscribe"].ToString();
                        if (!string.IsNullOrEmpty(wx_is_subscribe))
                        {
                            signstr += "&is_subscribe=" + wx_is_subscribe;
                        }
                    }

                    //wx_mch_id
                    if (ds.Tables[0].Columns.Contains("mch_id"))
                    {
                        wx_mch_id = ds.Tables[0].Rows[0]["mch_id"].ToString();
                        if (!string.IsNullOrEmpty(wx_mch_id))
                        {
                            signstr += "&mch_id=" + wx_mch_id;
                        }
                    }

                    //wx_nonce_str
                    if (ds.Tables[0].Columns.Contains("nonce_str"))
                    {
                        wx_nonce_str = ds.Tables[0].Rows[0]["nonce_str"].ToString();
                        if (!string.IsNullOrEmpty(wx_nonce_str))
                        {
                            signstr += "&nonce_str=" + wx_nonce_str;
                        }
                    }

                    //wx_openid
                    if (ds.Tables[0].Columns.Contains("openid"))
                    {
                        wx_openid = ds.Tables[0].Rows[0]["openid"].ToString();
                        if (!string.IsNullOrEmpty(wx_openid))
                        {
                            signstr += "&openid=" + wx_openid;
                        }
                    }

                    //wx_out_trade_no
                    if (ds.Tables[0].Columns.Contains("out_trade_no"))
                    {
                        wx_out_trade_no = ds.Tables[0].Rows[0]["out_trade_no"].ToString();
                        if (!string.IsNullOrEmpty(wx_out_trade_no))
                        {
                            signstr += "&out_trade_no=" + wx_out_trade_no;
                        }
                    }

                    //wx_result_code 
                    if (ds.Tables[0].Columns.Contains("result_code"))
                    {
                        wx_result_code = ds.Tables[0].Rows[0]["result_code"].ToString();
                        if (!string.IsNullOrEmpty(wx_result_code))
                        {
                            signstr += "&result_code=" + wx_result_code;
                        }
                    }

                    //wx_result_code 
                    if (ds.Tables[0].Columns.Contains("return_code"))
                    {
                        wx_return_code = ds.Tables[0].Rows[0]["return_code"].ToString();
                        if (!string.IsNullOrEmpty(wx_return_code))
                        {
                            signstr += "&return_code=" + wx_return_code;
                        }
                    }

                    //wx_sign 
                    if (ds.Tables[0].Columns.Contains("sign"))
                    {
                        wx_sign = ds.Tables[0].Rows[0]["sign"].ToString();
                        //if (!string.IsNullOrEmpty(wx_sign))
                        //{
                        //    signstr += "&sign=" + wx_sign;
                        //}
                    }

                    //wx_time_end
                    if (ds.Tables[0].Columns.Contains("time_end"))
                    {
                        wx_time_end = ds.Tables[0].Rows[0]["time_end"].ToString();
                        if (!string.IsNullOrEmpty(wx_time_end))
                        {
                            signstr += "&time_end=" + wx_time_end;
                        }
                    }

                    //wx_total_fee
                    if (ds.Tables[0].Columns.Contains("total_fee"))
                    {
                        wx_total_fee = Convert.ToInt32(ds.Tables[0].Rows[0]["total_fee"].ToString());

                        signstr += "&total_fee=" + wx_total_fee;
                    }

                    //wx_trade_type
                    if (ds.Tables[0].Columns.Contains("trade_type"))
                    {
                        wx_trade_type = ds.Tables[0].Rows[0]["trade_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_trade_type))
                        {
                            signstr += "&trade_type=" + wx_trade_type;
                        }
                    }

                    //wx_transaction_id
                    if (ds.Tables[0].Columns.Contains("transaction_id"))
                    {
                        wx_transaction_id = ds.Tables[0].Rows[0]["transaction_id"].ToString();
                        if (!string.IsNullOrEmpty(wx_transaction_id))
                        {
                            signstr += "&transaction_id=" + wx_transaction_id;
                        }
                    }

                    #endregion

                    //追加key 密钥
                    signstr += "&key=" + key;
                    //签名正确
                    string orderStrwhere = "ordernumber='" + wx_out_trade_no + "'";
                    if (wx_sign == System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(signstr, "MD5").ToUpper())
                    {
                        //签名正确   处理订单操作逻辑
                        
                        return new PayOrderResult { Success = true, Message = "接收成功", Money = Convert.ToDecimal(wx_total_fee) / 100, OrderSN = wx_out_trade_no, OpenId = wx_openid };
                    }
                    else
                    {
                        //追加备注信息
                        WriteLogs("签名验证有误");
                        return new PayOrderResult { Success = false, Message = "签名验证有误" };
                    }

                }
                else
                {
                    // 返回信息，如非空，为错误原因  签名失败 参数格式校验错误
                    WriteLogs("接收通知失败："+ds.Tables[0].Rows[0]["return_msg"].ToString());

                    return new PayOrderResult { Success = false, Message = ds.Tables[0].Rows[0]["return_msg"].ToString() };

                }

            }
        }

        public static PayOrderResult Query(string orderSN)
        {
            string appid = WebConfigurationManager.AppSettings["AppId"];
            var dic = new Dictionary<string, string>(){
                {"appid", appid},
                {"mch_id", mch_id},
                {"nonce_str", GetRandomString(20)/*Random.Next().ToString()*/},
                {"out_trade_no",orderSN},//商户自己的订单号码
               
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

            HttpWebResponse response = CreatePostHttpResponse("https://api.mch.weixin.qq.com/pay/orderquery", sb.ToString(), en);
            //打印返回值
            Stream stream = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html

            xml.LoadXml(html);


            var root = xml.DocumentElement;
            DataSet ds = new DataSet();
            StringReader stram = new StringReader(html);
            XmlTextReader reader = new XmlTextReader(stram);
            ds.ReadXml(reader);
            string return_code = ds.Tables[0].Rows[0]["return_code"].ToString();
            if (return_code.ToUpper() == "SUCCESS")
            {
                //通信成功
                string result_code = ds.Tables[0].Rows[0]["result_code"].ToString();//业务结果
                if (result_code.ToUpper() == "SUCCESS")
                {
                    string trade_state = ds.Tables[0].Rows[0]["trade_state"].ToString();
                    if (trade_state.ToUpper() == "SUCCESS")
                    {
                        var wx_out_trade_no = ds.Tables[0].Rows[0]["out_trade_no"].ToString();
                        var openid = ds.Tables[0].Rows[0]["openid"].ToString();
                        var total_fee = Convert.ToInt32(ds.Tables[0].Rows[0]["total_fee"].ToString());
                        return new PayOrderResult { Success = true, Message = "查询成功", OpenId = openid, OrderSN = wx_out_trade_no, Money = Convert.ToDecimal(total_fee) / 100 };
                    }
                    else
                    {
                        var wx_out_trade_no = ds.Tables[0].Rows[0]["out_trade_no"].ToString();
                        return new PayOrderResult { Success = false, Message = "查询成功", OrderSN = wx_out_trade_no };
                    }
                    
                }
                else
                {
                    WriteLogs(ds.Tables[0].Rows[0]["err_code_des"].ToString());
                    return new PayOrderResult { Success = false, Message = ds.Tables[0].Rows[0]["err_code_des"].ToString() };
                }
               
            }
            else if (return_code.ToUpper() == "FAIL")
            {
                WriteLogs(ds.Tables[0].Rows[0]["return_msg"].ToString());
                return new PayOrderResult { Success = false, Message = ds.Tables[0].Rows[0]["return_msg"].ToString() };
            }
            else
            {
                return new PayOrderResult { Success = false, Message ="通信失败" };
            }

        }






        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();

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
            byte[] data = charset.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            return request.GetResponse() as HttpWebResponse;

        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受   
        }

        /// <summary>
        /// 从字符串里随机得到，规定个数的字符串.
        /// </summary>
        /// <param name="allChar"></param>
        /// <param name="CodeCount"></param>
        /// <returns></returns>
        public static string GetRandomString(int CodeCount)
        {
            string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(allCharArray.Length - 1);
                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length - 1);
                }
                temp = t;
                RandomCode += allCharArray[t];
            }

            return RandomCode;

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

        public static string getPostStr()
        {
            Int32 intLen = Convert.ToInt32(System.Web.HttpContext.Current.Request.InputStream.Length);
            byte[] b = new byte[intLen];
            System.Web.HttpContext.Current.Request.InputStream.Read(b, 0, intLen);
            return System.Text.Encoding.UTF8.GetString(b);
        }

    }

}
