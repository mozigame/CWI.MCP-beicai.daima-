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
    /// McpSysBlacklistInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_sys_blacklist")]
	public class McpSysBlacklistInfo : Model
    {
		/// <summary>
		/// 黑名单ID
		/// </summary>
		[TableMapping(FieldName = "black_id", PrimaryKey = true)]
		public long BlackId { set; get; }

		/// <summary>
		/// APP应用标识
		/// </summary>
		[TableMapping(FieldName = "app_sign")]
		public string AppSign { set; get; }

		/// <summary>
		/// 设备标示
		/// </summary>
		[TableMapping(FieldName = "terminal_code")]
		public string TerminalCode { set; get; }

		/// <summary>
		/// 计数
		/// </summary>
		[TableMapping(FieldName = "counter")]
		public int Counter { set; get; }

		/// <summary>
		/// 锁定日期
		/// </summary>
		[TableMapping(FieldName = "lock_expire_date")]
		public DateTime LockExpireDate { set; get; }

		/// <summary>
		/// 最后核查时间
		/// </summary>
		[TableMapping(FieldName = "last_check_time")]
		public DateTime LastCheckTime { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[TableMapping(FieldName = "remark")]
		public string Remark { set; get; }

	}
}
