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
    /// 打印设备校验参数
    /// </summary>
    public class PrinterCheckViewModel : ViewModel
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [Required(ErrorMessage = "访问令牌不能为空！")]
        public string access_token { get; set; }

        /// <summary>
        /// 制造编号串（以逗号隔开）
        /// </summary>
        [Required(ErrorMessage = "制造编号串不能为空！")]
        public string printer_codes { get; set; }
    }
}
