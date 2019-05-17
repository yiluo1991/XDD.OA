using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XDD.ConsoleApp
{
    class Program
    {
        public static void WriteLogs(string content)
        {
            try
            {
                string path = Environment.CurrentDirectory;
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

        static void Main(string[] args)
        {
            WebClient client = new WebClient();

            WriteLogs( Encoding.UTF8.GetString(client.DownloadData("http://www.xyxiaodingdang.com/timer")));
          
        }
    }
}
