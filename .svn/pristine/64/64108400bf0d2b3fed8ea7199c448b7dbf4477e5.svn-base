//---------------------------------------------
// 版权信息：版权所有(C) 2016，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      张友辉        2018/05/17        创建
//---------------------------------------------

using System;
using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace CWI.MCP.Models
{
    /// <summary>
    /// mcp_callbackrecord
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_callbackrecord")]
    public class McpCallbackRecord : Model
    {
        /// <summary>
		/// 应用ID
		/// </summary>
		[TableMapping(FieldName = "id", PrimaryKey = true)]
        public int Id { set; get; }

        /// <summary>
        /// 交易号
        /// </summary>
        [TableMapping(FieldName = "bill_no")]
        public string BillNo { set; get; }

        /// <summary>
        /// 订单号
        /// </summary>
        [TableMapping(FieldName = "order_id")]
        public string OrderId { set; get; }

        /// <summary>
        /// 订单号
        /// </summary>
        [TableMapping(FieldName = "printer_code")]
        public string PrinterCode { set; get; }

        /// <summary>
        /// 订单号
        /// </summary>
        [TableMapping(FieldName = "order_status")]
        public int OrderStatus { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [TableMapping(FieldName = "created_date")]
        public DateTime CreatedDate { set; get; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [TableMapping(FieldName = "modified_date")]
        public DateTime ModifiedDate { set; get; }
    }
}
