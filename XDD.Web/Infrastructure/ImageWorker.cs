using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace XDD.Web.Infrastructure
{
    public static class ImageWorker
    {
        #region 图片保存相关
        public static bool IsValidImage(System.Web.HttpPostedFileBase postedFile)
        {
            string sMimeType = postedFile.ContentType.ToLower();
            string ext = null;
            if (postedFile.FileName.IndexOf('.') > 0)
            {
                string[] fs = postedFile.FileName.Split('.');
                ext = fs[fs.Length - 1];
            }
            if (!new List<string>() { "jpg", "jpeg", "png", "gif" }.Contains(ext.ToLower()))
            {
                return false;
            }
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(postedFile.InputStream);
                if (img.Width * img.Height < 1)
                    return false;

                img.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string saveRemoteImg(string sUrl)
        {

            int maxAttachSize = 2097152;        // 最大上传大小，默认是2M
            byte[] fileContent;
            string upExt = ",jpg,png,gif,jpeg,";      //上传扩展名
            string sExt;
            string sFile;
            if (sUrl.StartsWith("data:image"))
            {
                // base64编码的图片，可能出现在firefox粘贴，或者某些网站上，例如google图片
                int pstart = sUrl.IndexOf('/') + 1;
                sExt = sUrl.Substring(pstart, sUrl.IndexOf(';') - pstart).ToLower();
                if (upExt.IndexOf("," + sExt + ",") == -1) return "";
                fileContent = Convert.FromBase64String(sUrl.Substring(sUrl.IndexOf("base64,") + 7));
                if (fileContent.Length > maxAttachSize) return "";//超过最大上传大小忽略
                sFile = getLocalPath(sExt);
                //有效图片保存
                MakeThumbnail(fileContent, HttpContext.Current.Server.MapPath(sFile), 2048, 2048, "EQU", System.Drawing.Imaging.ImageFormat.Jpeg);
                return string.Format("http://{0}{1}{2}", HttpContext.Current.Request.Url.Host, (HttpContext.Current.Request.Url.Port == 80 ? "" : (":" + HttpContext.Current.Request.Url.Port)), sFile);
            }
            else
            {
                return "";
            }
        }

        public static string getLocalPath(string extension)
        {
            int dirType = 1;                    // 1:按天存入目录 2:按月存入目录 3:按扩展名存目录  建议使用按天存
            string attachDir = "/Images/uploads";        //上传文件保存路径，结尾不要带/
            string attach_dir, attach_subdir, filename;
            switch (dirType)
            {
                case 1:
                    attach_subdir = "" + DateTime.Now.ToString("yyyyMMdd");
                    break;
                case 2:
                    attach_subdir = "" + DateTime.Now.ToString("yyyyMM");
                    break;
                default:
                    attach_subdir = "ext_" + extension;
                    break;
            }
            attach_dir = attachDir + "/" + attach_subdir + "/";
            var dir = HttpContext.Current.Server.MapPath(attach_dir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            filename = Guid.NewGuid() + "." + extension;
            return attach_dir + filename;
        }
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="postedFile">图片文件</param>
        /// <param name="thumbnailPath">生成缩略图图片路径，如：c:\\images\\2.gif</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="mode">EQU：指定最大高宽等比例缩放；HW：//指定高宽缩放（可能变形）；W:指定宽，高按比例；H:指定高，宽按比例；Cut：指定高宽裁减（不变形）</param>
        public static void MakeThumbnail(Byte[] byteData, string thumbnailPath, int width, int height, string mode, System.Drawing.Imaging.ImageFormat imageFormat)
        {

            System.IO.Stream oStream = new MemoryStream(byteData);
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(oStream);
            int towidth = originalImage.Width;
            int toheight = originalImage.Height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            if (ow > width || oh > height)
            {
                towidth = width;
                toheight = height;
                if (mode == "EQU")//指定最大高宽，等比例缩放
                {
                    //if(height/oh>width/ow),如果高比例多，按照宽来缩放；如果宽的比例多，按照高来缩放
                    if (height * ow > width * oh)
                    {
                        mode = "W";
                    }
                    else
                    {
                        mode = "H";
                    }
                }
                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                   
                        break;
                    case "W"://指定宽，高按比例                       
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例   
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                   
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }
            }

            //新建一个bmp图片   
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板   
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充   
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分   
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            try
            {

                bitmap.Save(thumbnailPath, imageFormat);

            }
            catch (System.Exception e)
            {

            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

        }
        #region 图片缩放，多种指定方式生成图片
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="postedFile">图片文件</param>
        /// <param name="thumbnailPath">生成缩略图图片路径，如：c:\\images\\2.gif</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="mode">EQU：指定最大高宽等比例缩放；HW：//指定高宽缩放（可能变形）；W:指定宽，高按比例；H:指定高，宽按比例；Cut：指定高宽裁减（不变形）</param>
        public static void MakeThumbnail(HttpPostedFileBase postedFile, string thumbnailPath, int width, int height, string mode, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            Byte[] oFileByte = new byte[postedFile.ContentLength];
            System.IO.Stream oStream = postedFile.InputStream;
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(oStream);
            int towidth = originalImage.Width;
            int toheight = originalImage.Height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            if (ow > width || oh > height)
            {
                towidth = width;
                toheight = height;
                if (mode == "EQU")//指定最大高宽，等比例缩放
                {
                    //if(height/oh>width/ow),如果高比例多，按照宽来缩放；如果宽的比例多，按照高来缩放
                    if (height * ow > width * oh)
                    {
                        mode = "W";
                    }
                    else
                    {
                        mode = "H";
                    }
                }
                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                   
                        break;
                    case "W"://指定宽，高按比例                       
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例   
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                   
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }
            }

            //新建一个bmp图片   
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板   
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            //清空画布并以透明背景色填充   
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分   
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            try
            {
                // ImageCodecInfo tiffEncoder = GetEncoder(ImageFormat.Tiff);
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
                ImageCodecInfo encoder = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == imageFormat.Guid)
                    {
                        encoder = codec;
                    }
                }

                var encoderParams = new EncoderParameters(1);
                // encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)88);
                //encoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
               // encoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 1);
                //encoderParams.Param[4] = new EncoderParameter(System.Drawing.Imaging.Encoder.Version, (long)EncoderValue.VersionGif87);
                bitmap.Save(thumbnailPath, encoder, encoderParams);

            }
            catch (System.Exception e)
            {

            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

        }
        #endregion

        #endregion
    }

}