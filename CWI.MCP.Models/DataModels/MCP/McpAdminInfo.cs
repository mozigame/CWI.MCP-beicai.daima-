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
    /// McpAdminInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_admin")]
	public class McpAdminInfo : Model
    {
		/// <summary>
		/// 管理员ID
		/// </summary>
		[TableMapping(FieldName = "admin_id", PrimaryKey = true)]
		public  int AdminId { set; get; }

		/// <summary>
		/// 管理员帐号
		/// </summary>
		[TableMapping(FieldName = "admin_account")]
		public string AdminAccount { set; get; }

		/// <summary>
		/// 管理员密码
		/// </summary>
		[TableMapping(FieldName = "admin_pwd")]
		public string AdminPwd { set; get; }

		/// <summary>
		/// 是否为超级管理员:1-是,0-否
		/// </summary>
		[TableMapping(FieldName = "issupper")]
		public  int Issupper { set; get; }

		/// <summary>
		/// 管理员姓名
		/// </summary>
		[TableMapping(FieldName = "admin_name")]
		public string AdminName { set; get; }

		/// <summary>
		/// 手机号码
		/// </summary>
		[TableMapping(FieldName = "mobile")]
		public string Mobile { set; get; }

		/// <summary>
		/// 邮箱地址
		/// </summary>
		[TableMapping(FieldName = "email")]
		public string Email { set; get; }

		/// <summary>
		/// 最后一次登录IP
		/// </summary>
		[TableMapping(FieldName = "last_login_ip")]
		public string LastLoginIp { set; get; }

		/// <summary>
		/// 最后一次登录时间
		/// </summary>
		[TableMapping(FieldName = "last_login_date")]
		public DateTime LastLoginDate { set; get; }

		/// <summary>
		/// 状态码:1-启用,2-停用,3-删除
		/// </summary>
		[TableMapping(FieldName = "status_code")]
		public  int StatusCode { set; get; }

		/// <summary>
		/// 创建人ID
		/// </summary>
		[TableMapping(FieldName = "created_by")]
		public string CreatedBy { set; get; }

		/// <summary>
		/// 创建日期
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

		/// <summary>
		/// 最后修改人ID
		/// </summary>
		[TableMapping(FieldName = "modified_by")]
		public string ModifiedBy { set; get; }

		/// <summary>
		/// 最后变更日期
		/// </summary>
		[TableMapping(FieldName = "modified_on")]
		public DateTime ModifiedOn { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "memo")]
		public string Memo { set; get; }

	}
}
