using System.Collections.Generic;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class ConfigDeviceViewModel : ViewModel
    {
        /// <summary>
        /// 设备所属的公众号的原始ID
        /// </summary>
        public int device_num { get; set; }

        /// <summary>
        /// ⽤用户的openid
        /// </summary>
        public int op_type { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public List<DeviceInfoViewModel> device_list { get; set; }
    }
}

