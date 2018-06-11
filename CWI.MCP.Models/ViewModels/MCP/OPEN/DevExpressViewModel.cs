using System;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models.ViewModels.MCP.OPEN
{
    /// <summary>
    /// 编码内容：快递单模板视图Model
    /// 编码作者：ZLP
    /// 编码时间：2016-7-14
    /// </summary>
    public class DevExpressViewModel : ViewModel
    {
        /// <summary>
        /// 模版类型ID
        /// </summary>
        public string TemplateTypeId { set; get; }

        /// <summary>
        /// 模版类型名称
        /// </summary>
        public string TemplateTypeName { set; get; }

        /// <summary>
        /// 默认模板ID
        /// </summary>
        public string DefaultTemplateId { set; get; }

        /// <summary>
        /// 排序值
        /// </summary>
        public decimal SortIndex { set; get; }

        /// <summary>
        /// 状态:1-启用,2-停用,3-删除
        /// </summary>
        public int StatusCode { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { set; get; }

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
