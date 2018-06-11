//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2012/03/31       创建

using System;
using System.Web;
using System.Linq;
using Evt.Framework.Common;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// Session工具类。
    /// </summary>
    public static class SessionUtil
    {
        /// <summary>
        /// 获取HttpContext中Session对象的ID
        /// </summary>
        public static string SessionID
        {
            get
            {
                return HttpContext.Current.Session.SessionID;
            }
        }

        /// <summary>
        ///  微信OpenId
        /// </summary>
        public static string WechatOpenId
        {
            get
            {
                if (HttpContext.Current.Session[Consts.WECHAT_OPEN_ID] != null)
                {
                    return HttpContext.Current.Session[Consts.WECHAT_OPEN_ID].ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session[Consts.WECHAT_OPEN_ID] = value;
            }
        }

        /// <summary>
        /// 获取或设置前台登录用户相关信息的对象
        /// </summary>
        public static SessionData Current
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    return null;
                }
                else 
                {
                    return HttpContext.Current.Session[Consts.SESSION_KEY] as SessionData;
                }
            }
            set
            {
                HttpContext.Current.Session[Consts.SESSION_KEY] = value;
            }
        }

        /// <summary>
        /// 清除Session。
        /// </summary>
        public static void ClearSession()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
            }
        }

        /// <summary>
        /// 读写应用标识
        /// </summary>
        public static string AppSign
        {
            get
            {
                if (HttpContext.Current.Session[Consts.APP_SIGN] == null)
                    return "IOT";
                else
                    return HttpContext.Current.Session[Consts.APP_SIGN].ToString();
            }
            set
            {
                string appSign = value;
                if (!String.IsNullOrEmpty(appSign))
                {
                    if (!Regex.IsMatch(appSign, "MCP_OPEN|MCP_BPS|MCP_MPS|MCP_DEMO|MCP_IOT", RegexOptions.IgnoreCase))
                    {
                        throw new MessageException("应用标识错误");
                    }
                    appSign = appSign.ToUpper();
                }

                HttpContext.Current.Session[Consts.APP_SIGN] = appSign;
            }
        }

        /// <summary>
        /// 当前是否是新版（默认为新版）
        /// </summary>
        public static bool IsNewVersion
        {
            get
            {
                bool isNew = true;

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[Consts.WEB_VERSION_SESSION_KEY] != null)
                {
                    bool.TryParse(HttpContext.Current.Session[Consts.WEB_VERSION_SESSION_KEY].ToString(), out isNew);
                }

                return isNew;
            }
            set
            {
                HttpContext.Current.Session[Consts.WEB_VERSION_SESSION_KEY] = value;
            }
        }

        /// <summary>
        /// 验证码。注意：此属性不支持单元测试。
        /// </summary>
        public static string AuthenticationCode
        {
            get
            {
                return HttpContext.Current.Session[Consts.VERIFY_CODE_SESSION_KEY] as string;
            }
            set
            {
                HttpContext.Current.Session[Consts.VERIFY_CODE_SESSION_KEY] = value;
            }
        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="validateCode">验证码</param>
        public static void CheckValidateCode(string validateCode)
        {
            if (string.IsNullOrEmpty(validateCode))
            {
                throw new MessageException("请输入验证码");
            }

            if (string.IsNullOrEmpty(SessionUtil.AuthenticationCode))
            {
                throw new MessageException("验证码已过期");
            }

            if (SecurityUtil.ConvertToMD5(validateCode.ToLower()) != SessionUtil.AuthenticationCode)
            {
                throw new MessageException("验证码输入错误");
            }
        }
    }
}