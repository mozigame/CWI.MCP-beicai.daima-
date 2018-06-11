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
    /// McpSysEmailInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_sys_email")]
	public class McpSysEmailInfo : Model
    {
		/// <summary>
		/// 系统邮件ID
		/// </summary>
		[TableMapping(FieldName = "sys_mail_id", PrimaryKey = true)]
		public  int SysMailId { set; get; }

		/// <summary>
		/// APP应用标识
		/// </summary>
		[TableMapping(FieldName = "app_sign")]
		public string AppSign { set; get; }

		/// <summary>
		/// 发件人名称
		/// </summary>
		[TableMapping(FieldName = "sender_name")]
		public string SenderName { set; get; }

		/// <summary>
		/// 发件人邮箱
		/// </summary>
		[TableMapping(FieldName = "sender_email")]
		public string SenderEmail { set; get; }

		/// <summary>
		/// 收件人名称
		/// </summary>
		[TableMapping(FieldName = "receiver_name")]
		public string ReceiverName { set; get; }

		/// <summary>
		/// 收件人邮箱
		/// </summary>
		[TableMapping(FieldName = "receiver_email")]
		public string ReceiverEmail { set; get; }

		/// <summary>
		/// 邮件标题
		/// </summary>
		[TableMapping(FieldName = "title")]
		public string Title { set; get; }

		/// <summary>
		/// 邮件内容
		/// </summary>
		[TableMapping(FieldName = "content")]
		public string Content { set; get; }

		/// <summary>
		/// 优先级: 1-高,2-中,3-低
		/// </summary>
		[TableMapping(FieldName = "priority")]
		public  int Priority { set; get; }

		/// <summary>
		/// 邮件类型: 1-绑定邮箱,2-解除绑定,3-找回密码
		/// </summary>
		[TableMapping(FieldName = "mail_type")]
		public  int MailType { set; get; }

		/// <summary>
		/// 创建人ID
		/// </summary>
		[TableMapping(FieldName = "created_by")]
		public string CreatedBy { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

		/// <summary>
		/// 状态码:1-有效,2-无效
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
