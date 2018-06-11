
namespace  CWI.MCP.Common
{
    /// <summary>
    /// 微信常量
    /// </summary>
    public static class WeChatConsts
    {
        #region 基础接口地址

        /// <summary>
        ///  获取基础接口AccessToken（包括JS-SDK）【7200s有效】
        /// </summary>
        public const string WECHAT_TOKEN = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

        /// <summary>
        ///  获取网页授权AccessToken【AccessToken 及 OPENID，每次获取】
        /// </summary>
        public const string WECHAT_GET_OPENID = @"https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        /// <summary>
        ///  校验获取基础接口AccessToken
        /// </summary>
        public const string WECHAT_TOKEN_ISEXPIRED = @"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}";

        /// <summary>
        ///  校验网页授权AccessToken
        /// </summary>
        public const string AUTH_WECHAT_TOKEN_ISEXPIRED = @"https://api.weixin.qq.com/sns/auth?access_token={0}&openid={1}";

        /// <summary>
        ///  回调地址
        /// </summary>
        public const string WECHAT_AUTHORIZE = @"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect";

        /// <summary>
        /// 获取JS-SDK时所需的票据
        /// </summary>
        public const string WECHAT_JSSDK_TICKET = @"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";

        /// <summary>
        /// 创建菜单
        /// </summary>
        public const string WECHAT_MENU_ADD = @"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";

        /// <summary>
        /// 发送模版消息
        /// </summary>
        public const string WECHAT_SEND_MSG = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";

        #endregion

        #region 硬件平台接口地址

        /// <summary>
        ///【硬件平台】获取设备ID及二维码
        /// </summary>
        public const string IOT_GET_DEVICE_INFO = @"https://api.weixin.qq.com/device/getqrcode?access_token={0}";

        /// <summary>
        ///【硬件平台】设备授权  
        /// </summary>
        public const string IOT_AUTH_DEVICE = @"https://api.weixin.qq.com/device/authorize_device?access_token={0}";

        /// <summary>
        ///【硬件平台】设备控制  
        /// </summary>
        public const string IOT_SET_DEVICE = @"https://api.weixin.qq.com/hardware/mydevice/platform/ctrl_device?access_token={0}";

        /// <summary>
        ///【硬件平台】设备查询  
        /// </summary>
        public const string IOT_GET_DEVICE = @"https://api.weixin.qq.com/hardware/mydevice/platform/get_device_status?access_token={0}";

        #endregion

        #region 消息模版ID

        /// <summary>
        /// 上传文件成功微信模版
        /// </summary>
        public const string ClientUpSuccWxTempId = "uqeiexCub3bEZu1ettbnkiSI47PbWS8ZNyTsxHuzra4";

        /// <summary>
        /// 支付成功微信模版
        /// </summary>
        public const string ClientPaySuccWxTempId = "rOoypd6bwzTKVHcIraqeL_6seNPRoBja8wrZzmy-ITU";

        /// <summary>
        /// 退款成功微信模版
        /// </summary>
        public const string ClientRefundSuccWxTempId = "w1MvCCw81223-6Px0ab-QVP12lUQaxyC4MIQetwxSUQ";

        /// <summary>
        /// 打印成功微信模版
        /// </summary>
        public const string ClientPrintSuccWxTempId = "JgSMwX54UvSePoY028Tt6JIRkisR-tRoJC3NDPUigCk";

        #endregion
    }
}
