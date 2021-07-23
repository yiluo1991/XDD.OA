using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Windows.Forms;
using XDD.SMS;

namespace SMSTest
{

    class Program
    {
        public static byte[] PhotoImageInsert(System.Drawing.Image imgPhoto)
        {
            //将Image转换成流数据，并保存为byte[] 
            MemoryStream mstream = new MemoryStream();
            imgPhoto.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] byData = new Byte[mstream.Length];
            mstream.Position = 0;
            mstream.Read(byData, 0, byData.Length);
            mstream.Close();
            return byData;
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        public static int num = 0;


        static void Main(string[] args)
        {
            Console.WriteLine(SMSManager.SendMemberSMS("18559819573", "测试"));
            //new Thread(() =>
            //{
            //    var myImageCodecInfo = GetEncoderInfo("image/jpeg");

            //    EncoderParameters ps = new EncoderParameters(1);
            //    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            //    ps.Param[0] = new EncoderParameter(myEncoder, (long)40);
            //    while (true)
            //    {
            //        //   Console.WriteLine(SMSManager.SendMemberSMS("18559819573", "测试"));
            //        // 新建一个和屏幕大小相同的图片
            //        Bitmap catchBmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            //        // 创建一个画板，让我们可以在画板上画图
            //        // 这个画板也就是和屏幕大小一样大的图片
            //        // 我们可以通过Graphics这个类在这个空白图片上画图
            //        Graphics g = Graphics.FromImage(catchBmp);
            //        // 把屏幕图片拷贝到我们创建的空白图片 CatchBmp中
            //        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height));
            //        g.Dispose();
            //        //    catchBmp.Save(@"C:\Users\Syne\Desktop\jietu\" + DateTime.Now.ToString("HHmmssfff") + ".jpg", myImageCodecInfo, ps);
            //        new Task(() =>
            //        {
            //            System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
            //            var data = PhotoImageInsert(catchBmp);
            //            var buffer = calculator.ComputeHash(data);
            //            //将字节数组转换成十六进制的字符串形式
            //            StringBuilder stringBuilder = new StringBuilder();
            //            for (int i = 0; i < buffer.Length; i++)
            //            {
            //                stringBuilder.Append(buffer[i].ToString("x2"));
            //            }
            //            // Console.WriteLine(stringBuilder.ToString());
            //            stringBuilder.Clear();
            //            catchBmp.Dispose();
            //        }).Start();
            //        num++;
            //    }
            //}).Start();



            //while (true)
            //{
            //    Thread.Sleep(1000);
            //    // Console.Clear();
            //    Console.WriteLine(num);
            //    num = 0;
            //}
            //Console.ReadKey();
        }
    }
}
