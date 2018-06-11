//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 图片实用工具类
    /// </summary>
    public class ImageUtil
    {
        /// <summary>
        /// 指定缩放类型
        /// </summary>
        public enum ImgThumbnailType
        {
            /// <summary>
            /// 指定高宽缩放（可能变形）
            /// </summary>
            WH = 0,
            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            W = 1,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            H = 2,
            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            Cut = 3
        }

        /// <summary>
        /// 获取图片尺寸大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetImageSize(string filePath)
        {
            Image img = Image.FromFile(filePath);
            if (img.Height < img.Width)
            {
                img = KiRotate(img, RotateFlipType.Rotate90FlipNone);
            }
            return Tuple.Create<int, int>(img.Width, img.Height);
        }

        /// <summary>
        /// 获取旋转后的图片
        /// </summary>
        /// <param name="img"></param>
        /// <param name="type">
        /// 1.顺时针旋转90度:RotateFlipType.Rotate90FlipNone,
        /// 2.逆时针旋转90度:RotateFlipType.Rotate270FlipNone,
        /// 3.水平翻转:RotateFlipType.Rotate180FlipY
        /// 4.垂直翻转:RotateFlipType.Rotate180FlipX
        /// </param>
        /// <returns>旋转后的图片</returns>
        public static System.Drawing.Image KiRotate(System.Drawing.Image img, RotateFlipType type)
        {
            try
            {
                img.RotateFlip(type);
                return img;
            }
            catch
            {
                return null;
            }
        }

        #region 压缩图片

        /// <summary>
        /// 获取无损压缩图片
        /// </summary>
        /// <param name="sourceFilePath">原图片</param>
        /// <param name="thumFilePath">压缩后保存位置</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>压缩后的新路径</returns>
        public static string GetThumbnailImg(string sourceFilePath, string thumFilePath, int width, int height)
        {
            int flag = 100;
            Image soureImg = Image.FromFile(sourceFilePath);
            if (soureImg.Height < soureImg.Width)
            {
                soureImg = KiRotate(soureImg, RotateFlipType.Rotate90FlipNone);
            }

            ImgThumbnailType type = soureImg.Width > soureImg.Height ? ImgThumbnailType.W : ImgThumbnailType.H;
            string imgPath = GetThumbnailImg(soureImg, thumFilePath, width, height, flag, type, ImageFormat.Png);
            if (!string.IsNullOrWhiteSpace(imgPath))
            {
                return imgPath;
            }
            else
            {
                return GetThumbnailImg(sourceFilePath, thumFilePath, width, height, ImageFormat.Png);
            }
        }

        /// <summary>
        /// 获取无损压缩图片
        /// </summary>
        /// <param name="sourceFilePath">原图片</param>
        /// <param name="thumFilePath">压缩后保存位置</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns>压缩后的新路径</returns>
        public static string GetThumbnailImg(string sourceFilePath, string thumFilePath, int width, int height, int flag, ImgThumbnailType type, ImageFormat imageFormat)
        {
            Image soureImg = Image.FromFile(sourceFilePath);
            if (soureImg.Height < soureImg.Width)
            {
                soureImg = KiRotate(soureImg, RotateFlipType.Rotate90FlipNone);
            }

            return GetThumbnailImg(soureImg, thumFilePath, width, height, flag, type, imageFormat);
        }

        /// <summary>
        /// 获取无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="thumFilePath">压缩后保存位置</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns>压缩后的新路径</returns>
        private static string GetThumbnailImg(Image sourceImg, string thumFilePath, int width, int height, int flag, ImgThumbnailType type, ImageFormat imageFormat)
        {
            //缩放后的宽度和高度
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = sourceImg.Width;
            int oh = sourceImg.Height;

            switch (type)
            {
                case ImgThumbnailType.WH://指定高宽缩放（可能变形）           
                    {
                        break;
                    }
                case ImgThumbnailType.W://指定宽，高按比例     
                    {
                        toheight = sourceImg.Height * width / sourceImg.Width;
                        break;
                    }
                case ImgThumbnailType.H://指定高，宽按比例
                    {
                        towidth = sourceImg.Width * height / sourceImg.Height;
                        break;
                    }
                case ImgThumbnailType.Cut://指定高宽裁减（不变形）     
                    {
                        if ((double)sourceImg.Width / (double)sourceImg.Height > (double)towidth / (double)toheight)
                        {
                            oh = sourceImg.Height;
                            ow = sourceImg.Height * towidth / toheight;
                            y = 0;
                            x = (sourceImg.Width - ow) / 2;
                        }
                        else
                        {
                            ow = sourceImg.Width;
                            oh = sourceImg.Width * height / towidth;
                            x = 0;
                            y = (sourceImg.Height - oh) / 2;
                        }
                        break;
                    }
                default:
                    break;
            }

            Bitmap ob = new Bitmap(towidth, toheight);
            try
            {
                using (Graphics g = Graphics.FromImage(ob))
                {
                    g.Clear(System.Drawing.Color.WhiteSmoke);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(sourceImg
                      , new Rectangle(x, y, towidth, toheight)
                      , new Rectangle(0, 0, sourceImg.Width, sourceImg.Height)
                      , GraphicsUnit.Pixel);
                    g.Dispose();
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Error(string.Format(@"图片文件压缩失败，参考信息：{0}。", ex.Message.ToString()));
                sourceImg.Dispose();
                ob.Dispose();
                return string.Empty;
            }

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int i = 0; i < arrayICI.Length; i++)
                {
                    if (arrayICI[i].FormatDescription.Equals(imageFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        jpegICIinfo = arrayICI[i];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(thumFilePath, jpegICIinfo, ep);
                }
                else
                {
                    ob.Save(thumFilePath, imageFormat);
                }
                return thumFilePath;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                sourceImg.Dispose();
                ob.Dispose();
            }
        }

        /// <summary>
        /// 获取无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="thumFilePath">压缩后保存位置</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns>压缩后的新路径</returns>
        private static string GetThumbnailImg(string sourceImgPath, string thumFilePath, int width, int height, ImageFormat imageFormat)
        {
            int flag = 100;
            string fileName = Path.GetFileNameWithoutExtension(thumFilePath);
            thumFilePath = thumFilePath.Replace(fileName, string.Format("{0}_temp", fileName));

            Image sourceImg = Image.FromFile(sourceImgPath);
            if (sourceImg.Height < sourceImg.Width)
            {
                sourceImg = KiRotate(sourceImg, RotateFlipType.Rotate270FlipNone);
            }

            ImgThumbnailType type = sourceImg.Width > sourceImg.Height ? ImgThumbnailType.W : ImgThumbnailType.H;
            sourceImg.Save(thumFilePath);
            sourceImg = Image.FromFile(thumFilePath);

            //缩放后的宽度和高度
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = sourceImg.Width;
            int oh = sourceImg.Height;

            switch (type)
            {
                case ImgThumbnailType.WH://指定高宽缩放（可能变形）           
                    {
                        break;
                    }
                case ImgThumbnailType.W://指定宽，高按比例     
                    {
                        toheight = sourceImg.Height * width / sourceImg.Width;
                        break;
                    }
                case ImgThumbnailType.H://指定高，宽按比例
                    {
                        towidth = sourceImg.Width * height / sourceImg.Height;
                        break;
                    }
                case ImgThumbnailType.Cut://指定高宽裁减（不变形）     
                    {
                        if ((double)sourceImg.Width / (double)sourceImg.Height > (double)towidth / (double)toheight)
                        {
                            oh = sourceImg.Height;
                            ow = sourceImg.Height * towidth / toheight;
                            y = 0;
                            x = (sourceImg.Width - ow) / 2;
                        }
                        else
                        {
                            ow = sourceImg.Width;
                            oh = sourceImg.Width * height / towidth;
                            x = 0;
                            y = (sourceImg.Height - oh) / 2;
                        }
                        break;
                    }
                default:
                    break;
            }

            Bitmap ob = new Bitmap(towidth, toheight);
            try
            {
                using (Graphics g = Graphics.FromImage(ob))
                {
                    g.Clear(System.Drawing.Color.WhiteSmoke);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(sourceImg
                      , new Rectangle(x, y, towidth, toheight)
                      , new Rectangle(0, 0, sourceImg.Width, sourceImg.Height)
                      , GraphicsUnit.Pixel);
                    g.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format(@"图片文件压缩失败，参考信息：{0}。", ex.Message.ToString()));

                sourceImg.Dispose();
                ob.Dispose();
                return string.Empty;
            }

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int i = 0; i < arrayICI.Length; i++)
                {
                    if (arrayICI[i].FormatDescription.Equals(imageFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        jpegICIinfo = arrayICI[i];
                        break;
                    }
                }

                thumFilePath = thumFilePath.Replace(string.Format("{0}_temp", fileName), fileName);
                if (jpegICIinfo != null)
                {
                    ob.Save(thumFilePath, jpegICIinfo, ep);
                }
                else
                {
                    ob.Save(thumFilePath, imageFormat);
                }
                return thumFilePath;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                sourceImg.Dispose();
                ob.Dispose();
            }
        }

        /// <summary>
        /// 获取转换后的图片路径
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="outPutFilePath"></param>
        /// <returns></returns>
        public static string GetImagePath(string sourceFilePath, string outPutFilePath, ImageFormat imageFormat, RotateFlipType rotateType = RotateFlipType.Rotate90FlipNone)
        {
            Image soureImg = Image.FromFile(sourceFilePath);
            if (soureImg.Height < soureImg.Width)
            {
                soureImg = KiRotate(soureImg, rotateType);
            }

            try
            {
                soureImg.Save(outPutFilePath, imageFormat);
                return outPutFilePath;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                soureImg.Dispose();
            }
        }

        #endregion
    }
}
