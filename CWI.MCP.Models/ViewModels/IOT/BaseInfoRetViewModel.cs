using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class BaseInfoRetViewModel : ViewModel
    {
        /// <summary>
        /// 错误码,0代表成功
        /// </summary>
        public string device_type { get; set; }

        /// <summary>
        /// 错误消息文本描述
        /// </summary>
        public string device_id { get; set; }
    }
}
