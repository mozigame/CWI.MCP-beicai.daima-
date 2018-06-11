// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//                                         创建
// --------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Evt.Framework.Common;
using System.IO;

namespace  CWI.MCP.Common
{
    /// <summary>
    ///  ResponseUtil
    /// </summary>
    public class ResponseUtil
    {
        /// <summary>
        /// ResponseTextHtml
        /// </summary>
        /// <param name="text">text</param>
        public static void ResponseTextHtml(string text)
        {
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Write(text);
        }

        /// <summary>
        /// ResponseTextXml
        /// </summary>
        /// <param name="text">text</param>
        public static void ResponseTextXml(string text)
        {
            HttpContext.Current.Response.ContentType = "application/xml";
            HttpContext.Current.Response.Write(text);
        }

        /// <summary>
        /// 显示PDF文件
        /// </summary>
        /// <param name="stream">PDF文件二进制</param>
        public static void ReponsePdfFile(MemoryStream stream)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Charset = string.Empty;
            HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format(@"inline; filename={0}.pdf", Guid.NewGuid().ToString()));

            HttpContext.Current.Response.OutputStream.Write(stream.GetBuffer(), 0, stream.GetBuffer().Length);
            HttpContext.Current.Response.OutputStream.Flush();
            HttpContext.Current.Response.OutputStream.Close();
        }
    }
}