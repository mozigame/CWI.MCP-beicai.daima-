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
    /// McpDeveloperInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_developer")]
	public class McpDeveloperInfo : Model
    {
		/// <summary>
		/// 开发者ID
		/// </summary>
		[TableMapping(FieldName = "developer_id", PrimaryKey = true)]
		public string DeveloperId { set; get; }

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
		/// 开发者密码
		/// </summary>
		[TableMapping(FieldName = "user_pwd")]
		public string UserPwd { set; get; }

		/// <summary>
		/// 是否激活:0-未激活,1-已激活
		/// </summary>
		[TableMapping(FieldName = "is_actived")]
		public  int IsActived { set; get; }

		/// <summary>
		/// 激活日期
		/// </summary>
		[TableMapping(FieldName = "actived_datetime")]
		public DateTime ActivedDatetime { set; get; }

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
		/// 创建日期
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

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
