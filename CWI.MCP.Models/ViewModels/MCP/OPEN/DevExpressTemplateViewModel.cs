using System;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models.ViewModels.MCP.OPEN
{
    /// <summary>
    /// 内容摘要：模板视图Model
    /// 编码作者：ZLP
    /// 编码时间：2016-7-4
    /// </summary>
    public class DevExpressTemplateViewModel : ViewModel
    {
        /// <summary>
        /// 模版ID
        /// </summary>
        public string TemplateTypeId { set; get; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { set; get; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { set; get; }

        /// <summary>
        /// 模板样例
        /// </summary>
        public string TemplateExamplePath { set; get; }

        /// <summary>
        /// 状态:1-启用,2-停用,3-删除
        /// </summary>
        public int StatusCode { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { set; get; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedBy { set; get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ModifiedOn { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { set; get; }

    }
}
