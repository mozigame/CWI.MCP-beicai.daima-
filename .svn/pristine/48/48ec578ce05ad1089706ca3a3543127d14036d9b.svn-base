//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2012/03/31       创建

using System;
using System.Configuration;
using CWI.MCP.Common.Extensions;
using Evt.Framework.Common;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 配置工具类。
    /// </summary>
    public static class ConfigUtil
    {
        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <param name="key">config文件key</param>
        /// <returns>返回Key对应的值</returns>
        public static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        #region 系统配置

        /// <summary>
        /// 获取是否记录公司底层框架的日志信息。
        /// </summary>
        public static bool TraceFramework
        {
            get
            {
                bool traceFramework;
                if (!Boolean.TryParse(GetConfig("TraceFramework"), out traceFramework))
                {
                    traceFramework = false;
                }

                return traceFramework;
            }
        }

        /// <summary>
        /// 获取跟踪日志是否只记录精简信息。
        /// </summary>
        public static bool TraceSimpleContent
        {
            get
            {
                bool traceSimpleContent;
                if (!Boolean.TryParse(GetConfig("TraceSimpleContent"), out traceSimpleContent))
                {
                    traceSimpleContent = false;
                }

                return traceSimpleContent;
            }
        }

        /// <summary>
        /// 获取Url统计开关值，1:开启，0关闭,默认为:0
        /// </summary>
        public static int UrlStatSwitch
        {
            get
            {
                int urlStatSwitch;
                if (!Int32.TryParse(GetConfig("UrlStatSwitch"), out urlStatSwitch))
                {
                    urlStatSwitch = 0;
                }

                return urlStatSwitch;
            }
        }

        /// <summary>
        /// 缓存处理警告时长，Memcached Set/Get数据超过设定时长时记录警告信息（单位为秒）。
        /// 在Web.config中配置“WarnDuration”AppSettings配置项
        /// 缺省值值为2秒
        /// </summary>
        public static int WarnDuration
        {
            get
            {
                int warnDuration;
                if (!Int32.TryParse(GetConfig("WarnDuration"), out warnDuration))
                {
                    warnDuration = 2;
                }

                return warnDuration;
            }
        }

        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public static string CacheKeySuffix
        {
            get
            {
                var cacheKeySuffix = GetConfig("CacheKeySuffix");
                if (string.IsNullOrEmpty(cacheKeySuffix))
                {
                    cacheKeySuffix = "_n";
                }
                return cacheKeySuffix;
            }
        }

        /// <summary>
        /// 获取缓存过期时间（单位为秒）。
        /// </summary>
        public static int CacheTimeout
        {
            get
            {
                int cacheTimeout;
                if (!Int32.TryParse(GetConfig("CacheTimeout"), out cacheTimeout))
                {
                    cacheTimeout = 3;
                }

                return cacheTimeout;
            }
        }

        /// <summary>
        /// 获取保存数据到Memcached的失败重试次数。
        /// </summary>
        public static int MemcachedTryCount
        {
            get
            {
                int tryCount;
                if (!Int32.TryParse(GetConfig("MemcachedTryCount"), out tryCount))
                {
                    tryCount = 3;
                }

                return tryCount;
            }
        }

        /// <summary>
        /// 获取CacheUtil开关值, 1为打开，0为关闭
        /// </summary>
        public static int CacheUtilSwitch
        {
            get
            {
                int CacheUtilSwitch;
                if (!Int32.TryParse(GetConfig("CacheUtilSwitch"), out CacheUtilSwitch))
                {
                    CacheUtilSwitch = 0;
                }

                return CacheUtilSwitch;
            }
        }

        /// <summary>
        /// 获取CacheUtil超时时间， 单位是秒
        /// </summary>
        public static int CacheUtilExpireTime
        {
            get
            {
                int CacheUtilExpireTime;
                if (!Int32.TryParse(GetConfig("CacheUtilExpireTime"), out CacheUtilExpireTime))
                {
                    CacheUtilExpireTime = 1 * 60 * 60;
                }

                return CacheUtilExpireTime;
            }
        }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public static bool EnableCache
        {
            get
            {
                bool enableCache;
                if (!Boolean.TryParse(GetConfig("EnableCache"), out enableCache))
                {
                    enableCache = true;
                }

                return enableCache;
            }
        }

        /// <summary>
        /// 延迟时间(单位：秒)
        /// </summary>
        public static int LazyTime
        {
            get
            {
                string config = GetConfig("LazyTime");
                return ConvertUtil.ToInt(config, 1);
            }
        }

        /// <summary>
        /// 是否使用压缩版JS和CSS。
        /// </summary>
        public static bool MinCssJs
        {
            get
            {
                bool minCssJs;
                if (!Boolean.TryParse(GetConfig("MinCssJs"), out minCssJs))
                {
                    minCssJs = true;
                }

                return minCssJs;
            }
        }

        /// <summary>
        /// 获取静态内容（JS，CSS等）的版本号。
        /// </summary>
        public static string Version
        {
            get
            {
                return GetConfig("Version");
            }
        }

        /// <summary>
        /// Js文件版本
        /// </summary>
        public static string JsVersion
        {
            get
            {
                return GetConfig("JsVersion");
            }
        }

        /// <summary>
        /// Css文件版本
        /// </summary>
        public static string CssVersion
        {
            get
            {
                return GetConfig("CssVersion");
            }
        }

        /// <summary>
        /// Image Version
        /// </summary>
        public static string ImgVersion
        {
            get
            {
                return GetConfig("ImgVersion");
            }
        }

        /// <summary>
        /// Css Debug Path
        /// </summary>
        public static string DebugCssPath
        {
            get
            {
                string path = GetConfig("DebugCssPath");
                if (string.IsNullOrEmpty(path))
                {
                    throw new MessageException("请配置Web.config中DebugCssPath节点。");
                }

                return path;
            }
        }

        /// <summary>
        /// Css Release Path
        /// </summary>
        public static string ReleaseCssPath
        {
            get
            {
                string path = GetConfig("ReleaseCssPath");
                if (string.IsNullOrEmpty(path))
                {
                    throw new MessageException("请配置Web.config中ReleaseCssPath节点。");
                }

                return path;
            }
        }

        /// <summary>
        /// Js Debug Path
        /// </summary>
        public static string DebugJsPath
        {
            get
            {
                string path = GetConfig("DebugJsPath");
                if (string.IsNullOrEmpty(path))
                {
                    throw new MessageException("请配置Web.config中DebugJsPath节点。");
                }

                return path;
            }
        }

        /// <summary>
        /// Js Release Path
        /// </summary>
        public static string ReleaseJsPath
        {
            get
            {
                string path = GetConfig("ReleaseJsPath");
                if (string.IsNullOrEmpty(path))
                {
                    throw new MessageException("请配置Web.config中ReleaseJsPath节点。");
                }

                return path;
            }
        }

        /// <summary>
        /// 验证码失败次数 DEBUG模式 3 ， Release 模式 10
        /// </summary>
        public static int VarifyCodeFailCount
        {
            get
            {
#if DEBUG
                int defaultCount = 3;
#else
                int defaultCount = 10;
#endif
                return TryConvertUtil.ToInt(GetConfig("VarifyCodeFailCount"), defaultCount);
            }
        }

        /// <summary>
        /// 验证码有效时间，单位：秒 DEBUG模式 3*60 ， Release 模式 10*60
        /// </summary>
        public static int ValidatecodeExpire
        {
            get
            {
#if DEBUG
                int defaultCount = 180;
#else
                int defaultCount = 600;
#endif
                return TryConvertUtil.ToInt(GetConfig("ValidatecodeExpire"), defaultCount);
            }
        }

        

        /// <summary>
        /// 是否是域名访问
        /// </summary>
        public static bool IsDomain
        {
            get
            {
                bool isDomain;
                if (!Boolean.TryParse(GetConfig("IsDomain"), out isDomain))
                {
                    isDomain = true;
                }

                return isDomain;
            }
        }

        /// <summary>
        /// 获取一页默认显示的数据条数
        /// </summary>
        public static int PageSize
        {
            get
            {
                int pageSize;
                if (!Int32.TryParse(GetConfig("PageSize"), out pageSize))
                {
                    pageSize = 15;
                }

                return pageSize;
            }
        }

        /// <summary>
        /// 页码容量 获取同时显示的最大页数
        /// </summary>
        public static int PageCapacity
        {
            get
            {
                int pageSize;
                if (!Int32.TryParse(GetConfig("PageCapacity"), out pageSize))
                {
                    pageSize = 8;
                }

                return pageSize;
            }
        }


        /// <summary>
        /// 令牌有效期，单位：秒
        /// </summary>
        public static int TokenExpireIn
        {
            get
            {
                int defaultVal = 7200;
                return TryConvertUtil.ToInt(GetConfig("TokenExpireIn"), defaultVal);
            }
        }

        /// <summary>
        /// 是否是域名访问
        /// </summary>
        public static bool IsTestModel
        {
            get
            {
                bool isTestModel = false;
                if (TryConvertUtil.ToInt(GetConfig("IsTestModel"), 0) == 1)
                {
                    isTestModel = true;
                }

                return isTestModel;
            }
        }

        /// <summary>
        /// 19位打印单号特殊AppId串
        /// </summary>
        public static string OneNineOrderIdAppIds
        {
            get
            {
                return GetConfig("OneNineOrderIdAppIds");
            }
        }

        /// <summary>
        /// 关联应用AppIds
        /// </summary>
        public static string AssociatedAppIds
        {
            get
            {
                return GetConfig("AssociatedAppIds");
            }
        }

        /// <summary>
        /// 系统应用标识
        /// </summary>
        public static string SystemAppSign
        {
            get
            {
                return GetConfig("SystemAppSign");
            }
        }

        /// <summary>
        /// 系统用户Session标识
        /// </summary>
        public static string SystemUserSessionKey
        {
            get
            {
                return string.Format("{0}_login_user", GetConfig("SystemAppSign"));
            }
        }

        /// <summary>
        /// 系统终端Session标识
        /// </summary>
        public static string SystemTerminalSessionKey
        {
            get
            {
                return string.Format("{0}_terminal_info", GetConfig("SystemAppSign"));
            }
        }

        /// <summary>
        /// 打印订单Html转图片保存地址
        /// </summary>
        public static string ImageAddress
        {
            get{ return GetConfig("ImageAddress"); }
        }

        /// <summary>
        /// 打印订单Html转图片Http保存地址
        /// </summary>
        public static string HttpAddress
        {
            get { return GetConfig("HttpAddress"); }
        }

        #region Memcached 配置文件Key

        /// <summary>
        /// Memcached的默认时间3分钟 单位为秒
        /// </summary>
        public static int MemcachedDefaultTime
        {
            get
            {
                int defaultTime;

                if (!int.TryParse(ConfigUtil.GetConfig("MemcachedDefaultTime"), out defaultTime))
                {
                    defaultTime = 180;
                }
                return defaultTime;
            }
        }

        /// <summary>
        /// Memcached的自定义时间 单位为秒
        /// </summary>
        public static int MemcachedCustomizeTime
        {
            get
            {
                int customizeTime;

                if (!int.TryParse(ConfigUtil.GetConfig("MemcachedCustomizeTime"), out customizeTime))
                {
                    customizeTime = 60;
                }
                return customizeTime;
            }
        }

        #endregion

        #endregion

        #region 清理配置

        /// <summary>
        /// 已打印文件保留时长 （单位：小时,默认48小时）
        /// </summary>
        public static int PrintedRePeriod
        {
            get
            {
                return TryConvertUtil.ToInt(GetConfig("PrintedRePeriod"), 48);
            }
        }

        /// <summary>
        /// 已下单未打印文件保留时长 （单位：小时,默认48小时）
        /// </summary>
        public static int OrderedUnPrintRePeriod
        {
            get
            {
                return TryConvertUtil.ToInt(GetConfig("OrderedUnPrintRePeriod"), 48);
            }
        }

        #endregion

        #region 发送邮件服务器配置

        /// <summary>
        /// 发送邮件帐号名称
        /// </summary>
        public static string SenderEmailName
        {
            get
            {
                return GetConfig("SenderEmailName");
            }
        }

        /// <summary>
        /// 发送邮件帐号
        /// </summary>
        public static string SenderEmailAccount
        {
            get
            {
                return GetConfig("SenderEmailAccount");
            }
        }

        /// <summary>
        /// 发送邮件帐号密码
        /// </summary>
        public static string SenderEmailAccountPwd
        {
            get
            {
                return GetConfig("SenderEmailAccountPwd");
            }
        }

        /// <summary>
        /// 发送邮件服务器域名
        /// </summary>
        public static string SenderServerHost
        {
            get
            {
                return GetConfig("SenderServerHost");
            }
        }

        /// <summary>
        /// 发送邮件服务器端口
        /// </summary>
        public static int SenderServerHostPort
        {
            get
            {
                return TryConvertUtil.ToInt(GetConfig("SenderServerHostPort"), 25);
            }
        }

        /// <summary>
        /// 发送邮件超时时间 
        /// </summary>
        public static int SenderEmailTimeOut
        {
            get
            {
                int emailTimeOut = 9999;
                return TryConvertUtil.ToInt(ConfigUtil.GetConfig("EmailTimeOut"), emailTimeOut);
            }
        }

        /// <summary>
        /// 是否启用ssl加密
        /// </summary>
        public static bool SenderServerEnableSSL
        {
            get
            {
                return TryConvertUtil.ToInt(GetConfig("SenderServerEnableSSL"), 0) > 0;
            }
        }

        /// <summary>
        /// 反馈邮件帐号名称
        /// </summary>
        public static string FeedbackEmailName
        {
            get
            {
                return GetConfig("FeedbackEmailName");
            }
        }

        /// <summary>
        /// 用户反馈邮件接收邮箱
        /// </summary>
        /// <returns></returns>
        public static string FeedbackReceiveEmailAccount
        {
            get
            {
                return GetConfig("FeedbackReceiveEmailAccount");
            }
        }

        /// <summary>
        /// 微云打用户报告接受邮箱
        /// </summary>
        public static string YmeReportEmailAccount
        {
            get
            {
                return GetConfig("YmeReportEmailAccount");
            }
        }

        /// <summary>
        /// 分配用户邮箱大小
        /// </summary>
        public static int AccountMaxSize
        {
            get
            {
                return TryConvertUtil.ToInt(GetConfig("AccountMaxSize"), 100);
            }
        }

        #endregion

        #region 短信服务配置

        /// <summary>
        /// 发送渠道
        /// </summary>
        public static string SmsWay
        {
            get
            {
                return GetConfig("SmsWay");
            }
        }

        /// <summary>
        /// 【阿里大鱼】短信平台正式环境地址
        /// </summary>
        public static string ServerUrl_ali
        {
            get
            {
                return GetConfig("ServerUrl_ali");
            }
        }

        /// <summary>
        ///  【阿里大鱼】应用的AppKey 
        /// </summary>
        public static string Appkey_ali
        {
            get
            {
                return GetConfig("AppKey_ali");
            }
        }

        /// <summary>
        ///  【阿里大鱼】短信APP Secret
        /// </summary>
        public static string AppSecret_ali
        {
            get
            {
                return GetConfig("AppSecret_ali");
            }
        }

        /// <summary>
        /// 【阿里大鱼】短信签名
        /// </summary>
        public static string SmsFreeSignName_ali
        {
            get
            {
                return GetConfig("SmsFreeSignName_ali");
            }
        }

        /// <summary>
        /// 【电信】短信AppId
        /// </summary>
        public static string SmsAppId_dx
        {
            get
            {
                return GetConfig("SmsAppId_dx");
            }
        }

        /// <summary>
        /// 【电信】短信AppSecret
        /// </summary>
        public static string SmsAppSecret_dx
        {
            get
            {
                return GetConfig("SmsAppSecret_dx");
            }
        }

        /// <summary>
        /// 【荣联云】短信AppId
        /// </summary>
        public static string SmsAppId_rly
        {
            get
            {
                return GetConfig("SmsAppId_rly");
            }
        }

        /// <summary>
        /// 【荣联云】短信AppSecret
        /// </summary>
        public static string SmsAppSecret_rly
        {
            get
            {
                return GetConfig("SmsAppSecret_rly");
            }
        }

        /// <summary>
        /// 【荣联云】短信帐号
        /// </summary>
        public static string SmsAccountSid_rly
        {
            get
            {
                return GetConfig("SmsAccountSid_rly");
            }
        }

        /// <summary>
        /// 【荣联云】短信帐号令牌
        /// </summary>
        public static string SmsAuthToken_rly
        {
            get
            {
                return GetConfig("SmsAuthToken_rly");
            }
        }

        #endregion

        #region WebAPI配置

        /// <summary>
        /// 请求的日志是否记录至数据库
        /// </summary>
        public static bool IsLogToDb
        {
            get
            {
                bool bl = false;
                string isLogToDbStr = GetConfig("IsLogToDb");
                if (isLogToDbStr == "1")
                {
                    bl = true;
                }
                return bl;
            }
        }

        /// <summary>
        ///  Debug版本是否使用服务启动
        /// </summary>
        public static bool IsDebugRunService
        {
            get
            {
                bool bl = false;
                string isDebugRunService = GetConfig("IsDebugRunService");
                if (isDebugRunService == "1")
                {
                    bl = true;
                }
                return bl;
            }
        }

        /// <summary>
        /// 是否开启订单过期机制【默认：关】
        /// </summary>
        public static bool EnableOrderExpired
        {
            get
            {
                var value = GetConfig("EnableOrderExpired");
                return value == null ? true : value.Equals("0");
            }
        }

        #endregion

        #region 微信配置

        public static string IotToken
        {
            get
            {
                var value = GetConfig("IotToken");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WechatSubscribeAutoMessage
        {
            get
            {
                var value = GetConfig("WechatSubscribeAutoMessage");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WechatAccount
        {
            get
            {
                var value = GetConfig("WechatAccount");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WechatToken
        {
            get
            {
                var value = GetConfig("WechatToken");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WechatAppIdForIot
        {
            get
            {
                var value = GetConfig("WechatAppIdForIot");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WechatAppSecretForIot
        {
            get
            {
                var value = GetConfig("WechatAppSecretForIot");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string ProductId
        {
            get
            {
                var value = GetConfig("ProductId");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string AuthKey
        {
            get
            {
                var value = GetConfig("AuthKey");
                return value == null ? string.Empty : value.ToString();
            }
        }

        public static string WiFiKey
        {
            get
            {
                var value = GetConfig("WiFiKey");
                return value == null ? string.Empty : value.ToString();
            }
        }
        
        #endregion
    }
}
