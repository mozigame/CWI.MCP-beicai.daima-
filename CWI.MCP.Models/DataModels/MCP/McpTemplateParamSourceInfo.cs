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
    [TableMapping(TableName = "mcp_template_param_source")]
    public class McpTemplateParamSourceInfo : Model
    {
        /// <summary>
        /// 参数代码
        /// </summary>
        [TableMapping(FieldName = "param_code", PrimaryKey = true)]
        public string ParamCode { set; get; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [TableMapping(FieldName = "param_name")]
        public string ParamName { set; get; }

        /// <summary>
        /// 参数类型:1-字符型,2-数字型,3-布尔型
        /// </summary>
        [TableMapping(FieldName = "param_type")]
        public int ParamType { set; get; }

        /// <summary>
        /// 参数来源模板ID
        /// </summary>
        [TableMapping(FieldName = "param_source")]
        public string ParamSource { set; get; }

    }
}
