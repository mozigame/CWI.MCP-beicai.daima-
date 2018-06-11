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
    /// McpPrinterInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_printer")]
	public class McpPrinterInfo : Model
    {
		/// <summary>
		/// 打印设备ID
		/// </summary>
		[TableMapping(FieldName = "printer_id", PrimaryKey = true)]
		public  int PrinterId { set; get; }

		/// <summary>
		/// 打印设备唯一标识
		/// </summary>
		[TableMapping(FieldName = "printer_code")]
		public string PrinterCode { set; get; }

        /// <summary>
		/// 打印设备左纠偏量，单位毫米，范围(-20至20)
		/// </summary>
		[TableMapping(FieldName = "offsets_left")]
		public decimal OffSetLeft { set; get; }

		/// <summary>
		/// 打印设备上纠偏量，单位毫米，范围(-20至20)
		/// </summary>
		[TableMapping(FieldName = "offsets_top")]
        public decimal OffSetTop { set; get; }

		/// <summary>
		/// 连接ID
		/// </summary>
		[TableMapping(FieldName = "connection_id")]
		public long ConnectionId { set; get; }

		/// <summary>
		/// 打印设备IP地址及端口
		/// </summary>
		[TableMapping(FieldName = "ip_port")]
		public string IpPort { set; get; }

		/// <summary>
		/// 打印设备状态:1-启用,2-停用,3-删除
		/// </summary>
		[TableMapping(FieldName = "status_code")]
		public  int StatusCode { set; get; }

		/// <summary>
		/// 创建人
		/// </summary>
		[TableMapping(FieldName = "created_by")]
		public string CreatedBy { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

		/// <summary>
		/// 更新人
		/// </summary>
		[TableMapping(FieldName = "modified_by")]
		public string ModifiedBy { set; get; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[TableMapping(FieldName = "modified_on")]
		public DateTime ModifiedOn { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "memo")]
		public string Memo { set; get; }

        #region 扩展字段

        /// <summary>
        /// 命令缓存Key【扩展字段】
        /// </summary>
        public string CacheKey { set; get; }

        /// <summary>
        /// 打印任务号【扩展字段】
        /// </summary>
        public string OrderId { set; get; }

        /// <summary>
        /// 打印任务Key【扩展字段】
        /// </summary>
        public string OrderKey { set; get; }

        /// <summary>
        /// 打印份数【扩展字段】
        /// </summary>
        public int Copies { set; get; }

        /// <summary>
        /// 小票类型【扩展字段】
        /// </summary>
        public int BillType { set; get; }

        /// <summary>
        /// 需要打印的内容【扩展字段】
        /// </summary>
        public string OrderContent { set; get; }

        #endregion

    }
}
