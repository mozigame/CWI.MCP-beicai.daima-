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
    /// McpSysVersionInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_sys_version")]
	public class McpSysVersionInfo : Model
    {
		/// <summary>
		/// 版本ID
		/// </summary>
		[TableMapping(FieldName = "sys_version_id", PrimaryKey = true)]
		public  int SysVersionId { set; get; }

		/// <summary>
		/// 版本对象类型:1-MCP【Printer】
		/// </summary>
		[TableMapping(FieldName = "objec_type")]
		public string ObjecType { set; get; }

		/// <summary>
		/// 版本号
		/// </summary>
		[TableMapping(FieldName = "object_version")]
		public string ObjectVersion { set; get; }

		/// <summary>
		/// 版本文件
		/// </summary>
		[TableMapping(FieldName = "version_file")]
		public string VersionFile { set; get; }

		/// <summary>
		/// 是否强制升级:0-否,1-是
		/// </summary>
		[TableMapping(FieldName = "is_force")]
		public  int IsForce { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

		/// <summary>
		/// 创建人
		/// </summary>
		[TableMapping(FieldName = "created_by")]
		public string CreatedBy { set; get; }

		/// <summary>
		/// 状态码:1-启用,2-停用,3-删除
		/// </summary>
		[TableMapping(FieldName = "status_code")]
		public  int StatusCode { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "remark")]
		public string Remark { set; get; }

	}
}
