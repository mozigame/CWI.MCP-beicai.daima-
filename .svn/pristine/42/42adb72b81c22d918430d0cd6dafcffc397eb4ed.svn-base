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
    /// McpAppTokenInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_app_token")]
	public class McpAppTokenInfo : Model
    {
		/// <summary>
		/// 应用ID
		/// </summary>
		[TableMapping(FieldName = "app_id", PrimaryKey = true)]
		public string AppId { set; get; }

		/// <summary>
		/// 应用Key
		/// </summary>
		[TableMapping(FieldName = "app_key", PrimaryKey = true)]
		public string AppKey { set; get; }

		/// <summary>
		/// 登陆令牌
		/// </summary>
		[TableMapping(FieldName = "access_token")]
		public string AccessToken { set; get; }

		/// <summary>
		/// 令牌有效期（单位：秒）
		/// </summary>
		[TableMapping(FieldName = "expires_in")]
        public double ExpiresIn { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

	}
}
