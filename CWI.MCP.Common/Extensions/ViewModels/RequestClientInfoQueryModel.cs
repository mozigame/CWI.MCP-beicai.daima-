//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名         日期                说明
// --------------------------------------------
//      王军锋     2013/8/6              创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common.Extensions.ViewModel
{
    public class RequestClientInfoQueryModel
    {
        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppSign { get; set; }

        /// <summary>
        /// 应用的版本号
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 客户端推送Token
        /// </summary>
        public string ClientToken { get; set; }
    }
}
