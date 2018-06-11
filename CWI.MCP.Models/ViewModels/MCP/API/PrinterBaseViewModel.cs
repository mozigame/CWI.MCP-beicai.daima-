using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;
using Evt.Framework.Mvc;
using CWI.MCP.Common.Attributes;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 打印设备基础参数
    /// </summary>
    public class PrinterBaseViewModel : ViewModel
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [Required(ErrorMessage = "访问令牌不能为空！")]
        public string access_token { get; set; }

        /// <summary>
        /// 商家编码
        /// </summary>
        [Required(ErrorMessage = "商家编码不能为空！")]
        public string merchant_code { get; set; }

        /// <summary>
        /// 打印设备编码串（以逗号隔开）
        /// </summary>
        [Required(ErrorMessage = "打印设备编码不能为空！")]
        public string printer_codes { get; set; }
    }
}
