using System.Collections.Generic;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class DeviceViewModel : ViewModel
    {
        /// <summary>
        /// 设备所属的公众号的原始ID
        /// </summary>
        public string device_type { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string device_id { get; set; }

        /// <summary>
        /// ⽤用户的openid
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// ⾃自⾏行填写任意内容，微信平台将进⾏行透传
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 欲查询的能⼒力项及属性
        /// </summary>
        public Dictionary<string, object> services { get; set; }
    }
}

