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
    /// McpTemplateParamSourceInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_template_param")]
	public class McpTemplateParamInfo : Model
    {
		/// <summary>
		/// 模板ID
		/// </summary>
		[TableMapping(FieldName = "template_id", PrimaryKey = true)]
		public string TemplateId { set; get; }

		/// <summary>
		/// 参数序号
		/// </summary>
		[TableMapping(FieldName = "param_index")]
		public  int ParamIndex { set; get; }

		/// <summary>
		/// 参数代码
		/// </summary>
		[TableMapping(FieldName = "param_code", PrimaryKey = true)]
		public string ParamCode { set; get; }

		/// <summary>
		/// 参数描述
		/// </summary>
		[TableMapping(FieldName = "param_desc")]
		public string ParamDesc { set; get; }

		/// <summary>
		/// 参数值最大字符数:0-不受限制
		/// </summary>
		[TableMapping(FieldName = "param_max_len")]
		public  int ParamMaxLen { set; get; }

		/// <summary>
		/// 参数类型:1-字符型,2-数字型,3-布尔型
		/// </summary>
		[TableMapping(FieldName = "param_type")]
		public  int ParamType { set; get; }

		/// <summary>
		/// 参数X轴位置
		/// </summary>
		[TableMapping(FieldName = "loc_x")]
		public decimal LocX { set; get; }

		/// <summary>
		/// 参数Y轴位置
		/// </summary>
		[TableMapping(FieldName = "loc_y")]
		public decimal LocY { set; get; }

		/// <summary>
		/// 是否必填项:0-非必填,1-必填
		/// </summary>
		[TableMapping(FieldName = "is_need")]
		public  int IsNeed { set; get; }

	}
}
