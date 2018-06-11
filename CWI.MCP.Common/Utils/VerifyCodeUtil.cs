//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//                 2012-7-4             创建
//---------------------------------------------

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 验证码生成公用类
    /// </summary>
    public class VerifyCodeUtil
    {
        /// <summary>
        /// 获取验证码实例
        /// </summary>
        public static VerifyCodeUtil Intance
        {
            get
            {
                return new VerifyCodeUtil();
            }
        }

        /// <summary>
        /// 产生纯数字随机字符串
        /// </summary>
        /// <param name="num">随机出几个字符</param>
        /// <returns>随机出的字符串</returns>
        public string GenNumCode(int num)
        {
            string[] source = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string numCode = string.Empty;
            Random rd = new Random(DateTime.Now.Millisecond);
            int i;
            for (i = 0; i < num; i++)
            {
                numCode += source[rd.Next(0, source.Length)];
            }
            return numCode;
        }

        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <param name="num">随机出几个字符</param>
        /// <returns>随机出的字符串</returns>
        public string GenCode(int num)
        {
            char[] source = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            string code = string.Empty;
            Random rd = new Random(DateTime.Now.Millisecond);
            int i;
            for (i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }

        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <param name="num">随机出几个字符</param>
        /// <returns>随机出的字符串</returns>
        public string GenAllCode(int num)
        {
            char[] source = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string code = string.Empty;
            Random rd = new Random(DateTime.Now.Millisecond);
            int i;
            for (i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }

        /// <summary>
        /// 产生随机字符串[0-9&A-F]
        /// </summary>
        /// <param name="num">随机出几个字符</param>
        /// <returns>随机出的字符串</returns>
        public string GenPrivateKey(int num)
        {
            string[] source = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            string code = string.Empty;
            Random rd = new Random();
            int i;
            for (i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public MemoryStream CreateCheckCodeImage(string checkCode)
        {
            int codeW = (int)(checkCode.Length * 21.5);
            int codeH = 28;
            int fontSize = 16;
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman" };
            Random rnd = new Random();
            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 5; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr, 2), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < checkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize, FontStyle.Bold);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(checkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18, (float)0);
            }
            //画图片的前景噪音点
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, bmp.Width - 1, bmp.Height - 1);

            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }
    }
}
