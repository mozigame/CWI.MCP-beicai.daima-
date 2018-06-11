using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class RetDeviceInfoViewModel : ViewModel
    {
        public BaseInfoRetViewModel base_info { get; set; }

         /// <summary>
        /// 错误码,0代表成功
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误消息文本描述
        /// </summary>
        public string errmsg { get; set; }

    }
}
