using System.Collections.Generic;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class GetDeviceRetViewModel : ViewModel
    {
        /// <summary>
        /// 返回消息状态
        /// </summary>
        public ApiRetViewModel base_resp { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceid { get; set; }

        /// <summary>
        /// 设备二维码
        /// </summary>
        public string qrticket { get; set; }

        /// <summary>
        /// 设备证书
        /// </summary>
        public string devicelicence { get; set; }

        /// <summary>
        /// 设备Mac码【扩展属性】
        /// </summary>
        public string devicemac { get; set; }

        /// <summary>
        /// 设备类型【扩展属性】
        /// </summary>
        public string devicetype { get; set; }
    }
}

