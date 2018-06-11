//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/21        创建
//---------------------------------------------
using CWI.MCP.Common;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 快递内容
    /// </summary>
    public class ExpressContentViewModel : ViewModel
    {
        /// <summary>
        /// 参数值
        /// </summary>
        public string v { get; set; }

        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal x { get; set; }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal y { get; set; }
    }
}
