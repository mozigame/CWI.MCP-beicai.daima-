using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 微信接口参数
    /// </summary>
    public class WxJsApiParamsViewModel : ViewModel
    {
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 生成签名的时间戳
        /// </summary>
        public long timestamp { get; set; }

        /// <summary>
        /// 生成签名的随机串
        /// </summary>
        public string nonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string signature { get; set; }
    }

    /// <summary>
    /// 微信配置接口参数
    /// </summary>
    public class WxJsApiConfigViewModel : WxJsApiParamsViewModel
    {
        /// <summary>
        /// JS接口列表
        /// </summary>
        public string jsApiList { get; set; }
    }
}
