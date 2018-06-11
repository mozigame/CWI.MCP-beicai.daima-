using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    public class PrinterTemplateViewModel : ViewModel
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// 是否选中 1.选中 0.未选中
        /// </summary>
        public int IsSelect { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
    }
}
