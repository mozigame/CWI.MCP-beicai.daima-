using Evt.Framework.Mvc;

namespace CWI.MCP.Models.ViewModels.MCP.OPEN
{
    /// <summary>
    /// 内容摘要：模板参数视图Model
    /// 编码作者：ZLP
    /// 编码时间：2016-7-14
    /// </summary>
    public class DevExpressParamsViewModel : ViewModel
    {
        /// <summary>
        /// 参数代码
        /// </summary>
        public string ParamCode { set; get; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { set; get; }

        /// <summary>
        /// 参数类型:1-字符型,2-数字型,3-布尔型
        /// </summary>
        public int ParamType { set; get; }

        /// <summary>
        /// 参数来源【模板ID】
        /// </summary>
        public string ParamSource { set; get; }
    }
}
