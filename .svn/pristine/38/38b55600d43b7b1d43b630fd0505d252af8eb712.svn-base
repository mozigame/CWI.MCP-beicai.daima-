//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2013/02/26        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Evt.Framework.Common;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// Cookie Util
    /// </summary>
    public class CookieUtil
    {
        /// <summary> 
        /// 保存一个Cookie 
        /// </summary> 
        /// <param name="cookieName">Cookie名称</param> 
        /// <param name="cookieValue">Cookie值</param> 
        /// <param name="cookieTime">Cookie过期时间(小时),0为关闭页面失效</param> 
        public static void SaveCookie(string cookieName, string cookieValue, double cookieTime)
        {
            DateTime? expireDate = null;

            if (cookieTime != 0)
            {
                expireDate = DateTime.Now.AddHours(cookieTime);
            }

            SaveCookie(cookieName, cookieValue, expireDate);
        }

        /// <summary>
        /// 保存一个Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="cookieValue">Cookie值</param>
        /// <param name="expireDate">过期日期</param>
        public static void SaveCookie(string cookieName, string cookieValue, DateTime? expireDate)
        {
            cookieValue = HttpUtility.UrlEncode(cookieValue);

            HttpCookie myCookie = new HttpCookie(cookieName);
            myCookie.Value = cookieValue;

            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName];

            if (expireDate.HasValue)
            {
                //有两种方法，第一方法设置Cookie时间的话，关闭浏览器不会自动清除Cookie 
                //第二方法不设置Cookie时间的话，关闭浏览器会自动清除Cookie ,但是有效期 
                //多久还未得到证实。 
                myCookie.Expires = expireDate.Value;
                if (HttpContext.Current.Response.Cookies[cookieName] != null)
                    HttpContext.Current.Response.Cookies.Remove(cookieName);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            else
            {
                if (cookie != null)
                {
                    cookie.Value = cookieValue;
                }
                else
                {
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
        }

        /// <summary> 
        /// 取得CookieValue 
        /// </summary> 
        /// <param name="cookieName">Cookie名称</param> 
        /// <returns>Cookie的值</returns> 
        public static string GetCookie(string cookieName)
        {
            HttpCookie myCookie = new HttpCookie(cookieName);
            myCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (myCookie != null)
                return HttpUtility.UrlDecode(myCookie.Value);
            else
                return null;
        }

        /// <summary> 
        /// 清除CookieValue 
        /// </summary> 
        /// <param name="cookieName">Cookie名称</param> 
        public static void ClearCookie(string cookieName)
        {
            HttpCookie myCookie = new HttpCookie(cookieName);
            DateTime now = DateTime.Now;
            myCookie.Expires = now.AddYears(-2);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
    }
}
