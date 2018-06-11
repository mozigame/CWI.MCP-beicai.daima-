//---------------------------------------------
// 版权信息：版权所有(C) 2017，Yingmei.me
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2017/12/22         创建
//---------------------------------------------
using System;
using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace CWI.MCP.Models
{
    /// <summary>
    /// McpTemplateInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_template")]
	public class McpTemplateInfo : Model
    {
		/// <summary>
		/// 模版类型ID
		/// </summary>
		[TableMapping(FieldName = "template_type_id")]
		public string TemplateTypeId { set; get; }

		/// <summary>
		/// 模版类型ID
		/// </summary>
		[TableMapping(FieldName = "template_id", PrimaryKey = true)]
		public string TemplateId { set; get; }

		/// <summary>
		/// 模版名称
		/// </summary>
		[TableMapping(FieldName = "template_name")]
		public string TemplateName { set; get; }

		/// <summary>
		/// 模板版本
		/// </summary>
		[TableMapping(FieldName = "template_ver")]
		public string TemplateVer { set; get; }

		/// <summary>
		/// 模板样例
		/// </summary>
		[TableMapping(FieldName = "template_example_path")]
		public string TemplateExamplePath { set; get; }

		/// <summary>
		/// 排序值
		/// </summary>
		[TableMapping(FieldName = "sort_index")]
		public decimal SortIndex { set; get; }

		/// <summary>
		/// 状态:1-启用,2-停用,3-删除
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

	}
}
