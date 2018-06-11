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
    /// McpOrderPrinterInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_order_printer")]
	public class McpOrderPrinterInfo : Model
    {
		/// <summary>
		/// ID
		/// </summary>
		[TableMapping(FieldName = "order_printer_id", PrimaryKey = true)]
		public long OrderPrinterId { set; get; }

		/// <summary>
		/// 订单ID
		/// </summary>
		[TableMapping(FieldName = "order_id")]
		public string OrderId { set; get; }

		/// <summary>
		/// 打印设备唯一标识
		/// </summary>
		[TableMapping(FieldName = "printer_code")]
		public string PrinterCode { set; get; }

		/// <summary>
		/// 订单状态:0-未打印,1-已打印
		/// </summary>
		[TableMapping(FieldName = "order_status")]
		public  int OrderStatus { set; get; }

        /// <summary>
		/// 订单推送状态:0-未推送,1-已推送
		/// </summary>
		[TableMapping(FieldName = "push_status")]
        public int PushStatus { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[TableMapping(FieldName = "modified_on")]
		public DateTime ModifiedOn { set; get; }

	}
}
