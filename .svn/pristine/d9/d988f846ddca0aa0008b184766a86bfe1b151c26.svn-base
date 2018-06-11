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
    /// McpMerchantPrinterInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_merchant_printer")]
	public class McpMerchantPrinterInfo : Model
    {
		/// <summary>
		/// 商户打印设备映射ID
		/// </summary>
		[TableMapping(FieldName = "merchant_printer_id", PrimaryKey = true)]
		public  int MerchantPrinterId { set; get; }

		/// <summary>
		/// 应用ID
		/// </summary>
		[TableMapping(FieldName = "app_id")]
		public string AppId { set; get; }

		/// <summary>
		/// 商户编码，app内唯一
		/// </summary>
		[TableMapping(FieldName = "merchant_code")]
		public string MerchantCode { set; get; }

		/// <summary>
		/// 打印设备唯一标识
		/// </summary>
		[TableMapping(FieldName = "printer_code")]
		public string PrinterCode { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

	}
}
