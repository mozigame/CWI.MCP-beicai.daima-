using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class DeviceInfoViewModel : ViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mac { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string connect_protocol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string auth_key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string close_strategy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string conn_strategy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string crypt_method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string auth_ver { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string manu_mac_pos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ser_mac_pos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ble_simple_protocol { get; set; }
    }
}