using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class StatusNotifyViewModel : ViewModel
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
        /// 由微信云平台⽣生成的全局唯⼀的消息序列号
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string msg_type { get; set; }

        /// <summary>
        /// 能⼒项键集合
        /// </summary>
        public List<operation_status> services { get; set; }

        /// <summary>
        /// 设备⾃自定义的数据块
        /// </summary>
        public string data { get; set; }
    }
}
