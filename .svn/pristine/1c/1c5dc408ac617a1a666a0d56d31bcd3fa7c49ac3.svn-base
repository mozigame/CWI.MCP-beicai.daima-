using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWI.MCP.Common
{
    /// <summary>
    /// Html 转换类，目前支持Html转图片
    /// </summary>
    public class HtmlConvertUtil
    {
        #region Html转图片
        /// <summary>
        /// Html转图片
        /// </summary>
        /// <param name="url">Html链接</param>
        /// <returns></returns>
        public static bool HtmlConvertImage(string htmlUrl,string saveUrl)
        {
            //处理结果
            bool isSuccess = true;

            try
            {
                WebBrowser webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.Navigate(htmlUrl);
                webBrowser.Tag = saveUrl;
                webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser_DocumentCompleted);
                while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();
                webBrowser.Dispose();
            }
            catch (Exception e)
            {
                //记日志
                isSuccess = false;
            }

            return isSuccess;
        }
        #endregion

        #region 文档加载完成触发的事件
        /// <summary>
        /// 文档加载完成触发的事件
        /// </summary>
        /// <param name="sender">文档源</param>
        /// <param name="e"></param>
        private static void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser webBrowser = (WebBrowser)sender;
            //页面实际宽高
            var size = webBrowser.Document.Body.ScrollRectangle;
            webBrowser.ClientSize = new Size(size.Width, size.Height);
            webBrowser.ScrollBarsEnabled = false;

            //图片文件内容
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            webBrowser.BringToFront();
            webBrowser.DrawToBitmap(bitmap, webBrowser.Bounds);
            bitmap = (Bitmap)bitmap.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
            bitmap.Save(webBrowser.Tag.ToString(), System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        #endregion
    }
}
