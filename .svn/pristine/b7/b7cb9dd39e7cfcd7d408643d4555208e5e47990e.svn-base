//---------------------------------------------
// 版权信息：版权所有(C) 2016，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2016/07/04        创建
//---------------------------------------------

using System;
using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace CWI.MCP.Models
{
    /// <summary>
    /// McpPrinterStatusInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_printer_status")]
	public class McpPrinterStatusInfo : Model
    {
		/// <summary>
		/// 记录ID
		/// </summary>
		[TableMapping(FieldName = "id", PrimaryKey = true)]
		public long Id { set; get; }

		/// <summary>
		/// 打印设备唯标识
		/// </summary>
		[TableMapping(FieldName = "printer_code")]
		public string PrinterCode { set; get; }

		/// <summary>
		/// 状态编码：1-正常,2-缺纸,3-其他异常
		/// </summary>
		[TableMapping(FieldName = "status_code")]
		public  int StatusCode { set; get; }

		/// <summary>
		/// 异常开始时间
		/// </summary>
		[TableMapping(FieldName = "begin_time")]
		public DateTime BeginTime { set; get; }

		/// <summary>
		/// 异常结束时间
		/// </summary>
		[TableMapping(FieldName = "end_time")]
		public DateTime EndTime { set; get; }

	}
}
