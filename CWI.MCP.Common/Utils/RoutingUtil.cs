// 版权信息：版权所有(C) 2011，Evervictory Tech
// 变更历史：
// 姓名         日期          说明
// --------------------------------------------------------
//    王军锋    2011/06/10       创建
// --------------------------------------------------------
//    王军锋     2011/10/16       添加，GetPagerUrlFormat方法
// --------------------------------------------------------
//    王军锋     2011/10/17       添加, GetRouteUrl方法

using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Web.Routing;
using System.Collections.Generic;
using System.Web.Mvc;
using Evt.Framework.Common;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 封装与路由相关的操作。
    /// </summary>
    public static class RoutingUtil
    {
        private static string _companySpellingShortName = String.Empty;
        private static string _linkCode = String.Empty;
        private static string _linkType = String.Empty;

        /// <summary>
        /// 获取公司的拼音缩写。
        /// </summary>
        public static string CompanySpellingShortName
        {
            get
            {
                AnalyzeCompanyTag();
                return _companySpellingShortName;
            }
        }

        /// <summary>
        /// 获取连接代码。
        /// </summary>
        public static string LinkCode
        {
            get
            {
                AnalyzeCompanyTag();
                return _linkCode;
            }
        }

        /// <summary>
        /// 获取连接类型。
        /// </summary>
        public static string LinkType
        {
            get
            {
                AnalyzeCompanyTag();
                return _linkType;
            }
        }

        /// <summary>
        /// 获取Url中的公司标签（公司拼音缩写+连接代码+连接类型）。
        /// </summary>
        public static string CompanyTag
        {
            get
            {
                return HttpContext.Current.Request.RequestContext.RouteData.Values["company"] as String;
            }
        }

        /// <summary>
        /// 获取Url中的SessionId和公司标签部分。
        /// </summary>
        public static string SessionIdUrlComponent
        {
            get
            {
                return String.Format("/(S({0}))/{1}", HttpContext.Current.Session.SessionID, CompanyTag);
            }
        }

        /// <summary>
        /// 分析Url中的公司标签，将其分解为公司拼音缩写，连接代码和连接类型。
        /// </summary>
        private static void AnalyzeCompanyTag()
        {
            string companyTag = CompanyTag;
            if (Regex.IsMatch(companyTag, @"^[a-zA-z]+\d+[a|f|c]$"))
            {
                Match match = Regex.Match(companyTag, @"(?<spellingShortName>[a-zA-Z]+)(?<linkCode>[\d]+)(?<linkType>[\w]{1})", RegexOptions.ExplicitCapture);
                _companySpellingShortName = match.Groups["spellingShortName"].Value;
                _linkCode = match.Groups["linkCode"].Value;
                _linkType = match.Groups["linkType"].Value;
            }
            else
            {
                _companySpellingShortName = String.Empty;
                _linkCode = String.Empty;
                _linkType = String.Empty;
            }
        }

        /// <summary>
        /// 从路由URL和路由数据中获得含有分页参数{0}的URL格式化字符串
        /// 形式如：/List/0001-0002-1-2-1-{0}
        /// </summary>
        /// <param name="routeData">封装了当前路由数据的对象</param>
        /// <param name="pageKey">原URL中表示分页的KEY</param>
        /// <param name="optionalKey">原URL中表示可选的参数KEY</param>
        /// <returns>含有分页参数{0}的URL格式化字符串</returns>
        public static string GetPagerUrlFormat(RouteData routeData, string pageKey, string optionalKey)
        {
            string pagerUrlFormat = (routeData.Route as Route).Url;
            foreach (KeyValuePair<string, object> kvp in routeData.Values)
            {
                if (kvp.Key.Equals(pageKey))
                    pagerUrlFormat = pagerUrlFormat.Replace("{" + kvp.Key + "}", "{0}");
                else
                    pagerUrlFormat = pagerUrlFormat.Replace("{" + kvp.Key + "}", kvp.Value.ToString());
            }
            if (!String.IsNullOrEmpty(optionalKey))
                pagerUrlFormat = pagerUrlFormat.Replace("/{" + optionalKey + "}", String.Empty);

            if (!pagerUrlFormat.StartsWith("/"))
                pagerUrlFormat = "/" + pagerUrlFormat;

            return pagerUrlFormat;
        }

        /// <summary>
        /// 使用当前的UrlHelper和RouteValueDictionary对象，返回给某个参数设置新值后的URL
        /// </summary>
        /// <param name="paramName">需要设置新值的参数名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>给某个参数设置新值后的URL</returns>
        public static string GetRouteUrl(string paramName, object newValue)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            RouteValueDictionary routeValue = HttpContext.Current.Request.RequestContext.RouteData.Values;
            object oldValue = routeValue[paramName];
            routeValue[paramName] = newValue;
            string routeUrl = url.RouteUrl(routeValue);
            routeValue[paramName] = oldValue;

            return routeUrl;
        }

        /// <summary>
        /// 使用当前的UrlHelper和RouteValueDictionary对象，返回给某些参数设置新值后的URL
        /// </summary>
        /// <param name="paramNameArray">需要设置新值的参数名称组成的数组</param>
        /// <param name="newValueArray">新值数组</param>
        /// <returns>返回给某些参数设置新值后的URL</returns>
        public static string GetRouteUrl(string[] paramNameArray, object[] newValueArray)
        {
            if (paramNameArray == null || newValueArray == null || paramNameArray.Length != newValueArray.Length || paramNameArray.Length == 0)
                throw new MessageException("GetRouteUrl参数值错误");

            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            RouteValueDictionary routeValue = HttpContext.Current.Request.RequestContext.RouteData.Values;
            object[] oldValueArray = new object[newValueArray.Length];

            // 设置新的routeValue对象，并且保存原有值
            for (int i = 0; i < paramNameArray.Length; i++)
            {
                string paramName = paramNameArray[i];
                object newValue = newValueArray[i];

                oldValueArray[i] = routeValue[paramName];
                routeValue[paramName] = newValue;
            }
            string routeUrl = url.RouteUrl(routeValue);

            // 重新写回原来的routeValue
            for (int i = 0; i < paramNameArray.Length; i++)
            {
                routeValue[paramNameArray[i]] = oldValueArray[i];
            }

            return routeUrl;
        }

        /// <summary>
        /// version
        /// </summary>
        public static string Version
        {
            get
            {
                string version = ConfigurationManager.AppSettings["Version"];
                version = string.IsNullOrEmpty(version) ? string.Empty : version;

                return version;
            }
        }

        /// <summary>
        /// 获取Css的完整路径。
        /// </summary>
        /// <param name="css">Css文件名</param>
        /// <returns>Css的完整路径。</returns>
        public static string CssUrlByFile(string css)
        {
            string cssUrlConfig = ConfigurationManager.AppSettings["CssUrl"];
            cssUrlConfig = string.IsNullOrEmpty(cssUrlConfig) ? string.Empty : cssUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + cssUrlConfig + HttpContext.Current.Request.Url.Authority + (CommonUtil.IsDebug() ? ConfigUtil.DebugCssPath : ConfigUtil.ReleaseCssPath) + "/" + css + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取Css的完整路径
        /// </summary>
        /// <param name="debugPath">debug路径</param>
        /// <param name="releasePath">release路径</param>
        /// <returns>cs</returns>
        public static string CssUrlByPath(string releasePath)
        {
            string cssUrlConfig = ConfigurationManager.AppSettings["CssUrl"];
            cssUrlConfig = string.IsNullOrEmpty(cssUrlConfig) ? string.Empty : cssUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + cssUrlConfig + HttpContext.Current.Request.Url.Authority + releasePath + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取Css的完整路径
        /// </summary>
        /// <param name="debugPath">debug路径</param>
        /// <param name="releasePath">release路径</param>
        /// <returns>cs</returns>
        public static string CssUrlByPath(string debugPath, string releasePath)
        {
            string cssUrlConfig = ConfigurationManager.AppSettings["CssUrl"];
            cssUrlConfig = string.IsNullOrEmpty(cssUrlConfig) ? string.Empty : cssUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + cssUrlConfig + HttpContext.Current.Request.Url.Authority + (CommonUtil.IsDebug() ? debugPath : releasePath) + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取Js的完整路径。
        /// </summary>
        /// <param name="js">Js文件名</param>
        /// <returns>Js的完整路径。</returns>
        public static string JsUrlByFile(string js)
        {
            string jsUrlConfig = ConfigurationManager.AppSettings["JsUrl"];
            jsUrlConfig = string.IsNullOrEmpty(jsUrlConfig) ? string.Empty : jsUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + jsUrlConfig + HttpContext.Current.Request.Url.Authority + (CommonUtil.IsDebug() ? ConfigUtil.DebugJsPath : ConfigUtil.ReleaseJsPath) + "/" + js + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取完整Js路径
        /// </summary>
        /// <param name="releasePath">Release路径</param>
        /// <returns>完整Js路径</returns>
        public static string JsUrlByPath(string releasePath)
        {
            string jsUrlConfig = ConfigurationManager.AppSettings["JsUrl"];
            jsUrlConfig = string.IsNullOrEmpty(jsUrlConfig) ? string.Empty : jsUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + jsUrlConfig + HttpContext.Current.Request.Url.Authority + releasePath + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取完整Js路径
        /// </summary>
        /// <param name="debugPath">Debug路径</param>
        /// <param name="releasePath">Release路径</param>
        /// <returns>完整Js路径</returns>
        public static string JsUrlByPath(string debugPath, string releasePath)
        {
            string jsUrlConfig = ConfigurationManager.AppSettings["JsUrl"];
            jsUrlConfig = string.IsNullOrEmpty(jsUrlConfig) ? string.Empty : jsUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + jsUrlConfig + HttpContext.Current.Request.Url.Authority + (CommonUtil.IsDebug() ? debugPath : releasePath) + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取Css的完整路径。
        /// </summary>
        /// <param name="relativrPath">Css的相对路径。</param>
        /// <returns>Css的完整路径。</returns>
        public static string CssUrl(string relativrPath)
        {
            string cssUrlConfig = ConfigurationManager.AppSettings["CssUrl"];
            cssUrlConfig = string.IsNullOrEmpty(cssUrlConfig) ? string.Empty : cssUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + cssUrlConfig + HttpContext.Current.Request.Url.Authority + relativrPath + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取Js的完整路径。
        /// </summary>
        /// <param name="relativePath">Js的相对路径。</param>
        /// <returns>Js的完整路径。</returns>
        public static string JsUrl(string relativePath)
        {
            string jsUrlConfig = ConfigurationManager.AppSettings["JsUrl"];
            jsUrlConfig = string.IsNullOrEmpty(jsUrlConfig) ? string.Empty : jsUrlConfig;
            string result = HttpContext.Current.Request.Url.Scheme + "://" + jsUrlConfig + HttpContext.Current.Request.Url.Authority + relativePath + "?" + Version;

            return result;
        }

        /// <summary>
        /// 获取img的完整路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>图片的完整路径</returns>
        public static string ImgUrl(object relativePath)
        {
            if (relativePath == null) return String.Empty;

            string imgUrlConfig = ConfigurationManager.AppSettings["ImageUrl"];
            if (!String.IsNullOrEmpty(imgUrlConfig))
            {
                if (!imgUrlConfig.ToLower().StartsWith("http://") || !imgUrlConfig.ToLower().StartsWith("https://"))
                {
                    imgUrlConfig = HttpContext.Current.Request.Url.Scheme + "://" + imgUrlConfig;
                }
                return imgUrlConfig + relativePath.ToString() + "?" + Version;
            }
            else
            {
                return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + relativePath.ToString() + "?" + Version;
            }
        }

        /// <summary>
        /// 获取img的完整路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>图片的完整路径</returns>
        public static string GetURL(object relativePath)
        {
            if (relativePath == null) return String.Empty;

            string imgUrlConfig = ConfigurationManager.AppSettings["ImageUrl"];
            if (!String.IsNullOrEmpty(imgUrlConfig))
            {
                if (!imgUrlConfig.ToLower().StartsWith("http://") || !imgUrlConfig.ToLower().StartsWith("https://"))
                {
                    imgUrlConfig = HttpContext.Current.Request.Url.Scheme + "://" + imgUrlConfig;
                }
                return imgUrlConfig + relativePath.ToString() + "?" + Version;
            }
            else
            {
                return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + relativePath.ToString() + "?" + Version;
            }
        }

        /// <summary>
        /// 获取可以直接下载的附件的完整路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>附件的完整路径</returns>
        public static string FileUrl(object relativePath)
        {
            if (relativePath == null) return String.Empty;
            string path = ConvertUtil.ToString(relativePath);
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                return path;
            }

            string fileUrlConfig = ConfigurationManager.AppSettings["FileUrl"];
            if (!String.IsNullOrEmpty(fileUrlConfig))
            {
                if (!fileUrlConfig.ToLower().StartsWith("http://") || !fileUrlConfig.ToLower().StartsWith("https://"))
                {
                    fileUrlConfig = HttpContext.Current.Request.Url.Scheme + "://" + fileUrlConfig;
                }
                return fileUrlConfig + path + "?" + Version;
            }
            else
            {
                return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + relativePath.ToString() + "?" + Version;
            }
        }

        /// <summary>
        /// 根据网页的相对路径获得绝对URL
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>网页的完整路径</returns>
        public static string WebUrl(object relativePath)
        {
            if (relativePath == null) return String.Empty;
            return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + relativePath.ToString();
        }

        /// <summary>
        /// 为Url添加版本号
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Url</returns>
        public static string AddVersion(string url)
        {
            if (url.Contains("?"))
            {
                if (url.EndsWith("&"))
                {
                    url += Version;
                }
                else
                {
                    url += "&" + Version;
                }
            }
            else
            {
                url += "?" + Version;
            }
            return url;
        }

        /// <summary>
        /// 获取第三方网页Url(包括Http)
        /// </summary>
        /// <param name="webUrl">第三方网页路径</param>
        /// <returns>第三方网页Url</returns>
        public static string ThirdWebUrl(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
            {
                if (!webUrl.StartsWith("http://") || !webUrl.StartsWith("https://"))
                {
                    return HttpContext.Current.Request.Url.Scheme + "://" + webUrl;
                }
                else
                {
                    return webUrl;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}