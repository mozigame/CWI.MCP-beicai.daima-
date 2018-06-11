using System;
using System.Web;
using CWI.MCP.Models.Common;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models.ViewModels.MCP.OPEN
{
    /// <summary>
    /// 内容摘要：应用管理视图Model
    /// 编码作者：ZLP
    /// 编码时间：2016-7-12
    /// </summary>
    public class DevApplyViewModel : ViewModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { set; get; }

        /// <summary>
        /// 应用Key
        /// </summary>
        public string AppKey { set; get; }

        /// <summary>
        /// 应用类型：1-APP,2-WX,3-Web,4-Winform,5-Other
        /// </summary>
        public int AppType { set; get; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { set; get; }

        /// <summary>
        /// 签名密钥
        /// </summary>
        public string SignKey { set; get; }

        /// <summary>
        /// 授权认证回调地址
        /// </summary>
        public string AuthCallbackUrl { set; get; }

        /// <summary>
        /// 更新状态回调地址
        /// </summary>
        public string UpdateCallbackUrl { set; get; }

        /// <summary>
        /// logoUrl地址
        /// </summary>
        public string LogoPath { set; get; }

        /// <summary>
        /// 营业执照地址
        /// </summary>
        public string BusinessLicensePath { set; get; }

        /// <summary>
        /// 开发者ID
        /// </summary>
        public string DeveloperId { set; get; }

        /// <summary>
        /// 应用状态：0-新建,1-运营,2-停用,3-删除
        /// </summary>
        public int StatusCode { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { set; get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ModifiedOn { set; get; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedBy { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { set; get; }

        /// <summary>
        /// logo上传
        /// </summary>
        public HttpPostedFileBase LogoFile { set; get; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public ProcessResult Result { set; get; }
    }
}
