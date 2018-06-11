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
    /// McpApplicationInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_application")]
	public class McpApplicationInfo : Model
    {
		/// <summary>
		/// 应用ID
		/// </summary>
		[TableMapping(FieldName = "app_id", PrimaryKey = true)]
		public string AppId { set; get; }

		/// <summary>
		/// 应用Key
		/// </summary>
		[TableMapping(FieldName = "app_key")]
		public string AppKey { set; get; }

		/// <summary>
		/// 应用类型：1-APP,2-WX,3-Web,4-Winform,5-Other
		/// </summary>
		[TableMapping(FieldName = "app_type")]
		public  int AppType { set; get; }

		/// <summary>
		/// 应用名称
		/// </summary>
		[TableMapping(FieldName = "app_name")]
		public string AppName { set; get; }

		/// <summary>
		/// 签名密钥
		/// </summary>
		[TableMapping(FieldName = "sign_key")]
		public string SignKey { set; get; }

		/// <summary>
		/// 授权认证回调地址
		/// </summary>
		[TableMapping(FieldName = "auth_callback_url")]
		public string AuthCallbackUrl { set; get; }

		/// <summary>
		/// 更新状态回调地址
		/// </summary>
		[TableMapping(FieldName = "update_callback_url")]
		public string UpdateCallbackUrl { set; get; }

		/// <summary>
		/// logoUrl地址
		/// </summary>
		[TableMapping(FieldName = "logo_path")]
		public string LogoPath { set; get; }

		/// <summary>
		/// 营业执照地址
		/// </summary>
		[TableMapping(FieldName = "business_license_path")]
		public string BusinessLicensePath { set; get; }

		/// <summary>
		/// 开发者ID
		/// </summary>
		[TableMapping(FieldName = "developer_id")]
		public string DeveloperId { set; get; }

		/// <summary>
		/// 应用状态：0-新建,1-运营,2-停用,3-删除
		/// </summary>
		[TableMapping(FieldName = "status_code")]
		public  int StatusCode { set; get; }

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
		/// 更新时间
		/// </summary>
		[TableMapping(FieldName = "modified_on")]
		public DateTime ModifiedOn { set; get; }

		/// <summary>
		/// 更新人
		/// </summary>
		[TableMapping(FieldName = "modified_by")]
		public string ModifiedBy { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "memo")]
		public string Memo { set; get; }

	}
}
