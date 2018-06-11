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
    /// McpSysVerifycodeInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_sys_verifycode")]
	public class McpSysVerifycodeInfo : Model
    {
		/// <summary>
		/// 验证码ID
		/// </summary>
		[TableMapping(FieldName = "verify_code_id", PrimaryKey = true)]
		public long VerifyCodeId { set; get; }

		/// <summary>
		/// APP应用标识
		/// </summary>
		[TableMapping(FieldName = "app_sign")]
		public string AppSign { set; get; }

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
		/// 商家帐号
		/// </summary>
		[TableMapping(FieldName = "merchant_code")]
		public string MerchantCode { set; get; }

		/// <summary>
		/// 验证码
		/// </summary>
		[TableMapping(FieldName = "verify_code")]
		public string VerifyCode { set; get; }

		/// <summary>
		/// 验证失效时间
		/// </summary>
		[TableMapping(FieldName = "expire_date")]
		public DateTime ExpireDate { set; get; }

		/// <summary>
		/// 是否已验证:0-未验证,1-已验证
		/// </summary>
		[TableMapping(FieldName = "verified")]
		public  int Verified { set; get; }

		/// <summary>
		/// 验证日期
		/// </summary>
		[TableMapping(FieldName = "verified_date")]
		public DateTime VerifiedDate { set; get; }

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
		/// 修改人ID
		/// </summary>
		[TableMapping(FieldName = "modified_by")]
		public string ModifiedBy { set; get; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[TableMapping(FieldName = "modified_on")]
		public DateTime ModifiedOn { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "remark")]
		public string Remark { set; get; }

		/// <summary>
		/// 终端标识
		/// </summary>
		[TableMapping(FieldName = "terminal_code")]
		public string TerminalCode { set; get; }

	}
}
