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
    /// McpSysSmsInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_sys_sms")]
	public class McpSysSmsInfo : Model
    {
		/// <summary>
		/// 系统短信ID
		/// </summary>
		[TableMapping(FieldName = "sys_sms_id", PrimaryKey = true)]
		public long SysSmsId { set; get; }

		/// <summary>
		/// APP应用标识
		/// </summary>
		[TableMapping(FieldName = "app_sign")]
		public string AppSign { set; get; }

		/// <summary>
		/// 发送方号码
		/// </summary>
		[TableMapping(FieldName = "sender_no")]
		public string SenderNo { set; get; }

		/// <summary>
		/// 接收方号码
		/// </summary>
		[TableMapping(FieldName = "receiver_no")]
		public string ReceiverNo { set; get; }

		/// <summary>
		/// 短信标题
		/// </summary>
		[TableMapping(FieldName = "title")]
		public string Title { set; get; }

		/// <summary>
		/// 短信内容
		/// </summary>
		[TableMapping(FieldName = "content")]
		public string Content { set; get; }

		/// <summary>
		/// 短信类型: 1-绑定手机号,2-解除绑定,3-找回密码,4-失败反馈,5-成功反馈,6-用户反馈
		/// </summary>
		[TableMapping(FieldName = "sms_type")]
		public  int SmsType { set; get; }

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
