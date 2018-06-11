using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using CWI.MCP.Models;
using CWI.MCP.Models.Common;
using CWI.MCP.Models.ViewModels.MCP.OPEN;
using Evt.Framework.Common;

namespace CWI.MCP.Services.MCP
{
    /// <summary>
    /// 内容摘要：用户中心数据服务类
    /// 编码作者：ZLP
    /// 编码时间：2016-7-12
    /// </summary>
    public class UserAccountService : BaseService
    {
        #region 方法区

        #region 快递单

        /// <summary>
        /// 获取快递商集合
        /// </summary>
        /// <returns></returns>
        public IList<DevExpressViewModel> GetExpressList()
        {
            var cc = new ConditionCollection()
            {
                new Condition("template_business_id", "TP001"),
                new Condition("status_code",1)
            };
            var expressInfo = GetRepository<McpTemplateTypeInfo>().ListModel(cc);
            return ModelConvertHelper.ToConvertModelS<DevExpressViewModel, McpTemplateTypeInfo>(expressInfo);
        }

        /// <summary>
        /// 获取指定快递商模板参数
        /// </summary>
        /// <param name="expressId">快递商Id</param>
        /// <returns></returns>
        public IList<DevExpressTemplateViewModel> GetExpressTemplateList(string typeId)
        {
            var cc = new ConditionCollection()
            {
                new Condition("template_type_id", typeId),
                new Condition("status_code",1)
            };
            var expressTemplate = GetRepository<McpTemplateInfo>().ListModel(cc);
            return
                ModelConvertHelper.ToConvertModelS<DevExpressTemplateViewModel, McpTemplateInfo>(expressTemplate);
        }

        /// <summary>
        /// 根据模板Id获取模板实体
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <returns></returns>
        public DevExpressTemplateViewModel GetExpressTemplate(string templateId)
        {
            var cc = new ConditionCollection()
            {
                new Condition("template_id", templateId)
            };
            var expressTemplate = GetRepository<McpTemplateInfo>().GetModel(cc);
            return ModelConvertHelper.ToConvertModel<DevExpressTemplateViewModel>(expressTemplate);
        }

        /// <summary>
        /// 获取指定模板的参数集合
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns></returns>
        public IList<DevExpressTemplateParamsViewModel> GetExpressTemplateParamsList(string templateId)
        {
            var cc = new ConditionCollection()
            {
                new Condition("template_id",templateId)
            };
            var expressTemplateParams = GetRepository<McpTemplateParamInfo>().ListModel(cc);
            return
                ModelConvertHelper.ToConvertModelS<DevExpressTemplateParamsViewModel, McpTemplateParamInfo>(
                    expressTemplateParams);
        }

        #endregion

        #region API说明

        #endregion

        #region 接入指南

        #endregion

        #region 应用管理

        /// <summary>
        /// 新增应用
        /// </summary>
        /// <param name="model">视图数据模型</param>
        public ProcessResult AddApplyInfo(DevApplyViewModel model)
        {
            var result = new ProcessResult();
            var url = "";
            // 数据校验
            result = CheckApplyData(model);
            if (model.LogoFile != null)
            {
                result = LogFileUpload(model.LogoFile);
                if (result.Result)
                {
                    url =result.Data; 
                }
                else
                {
                    return result;
                }
            }

            if (result.Result)
            {
                // 判断应用Id是否为空
                if (!string.IsNullOrEmpty(model.AppId))
                {
                    var cc = new ConditionCollection()
                    {
                        new Condition("app_id", model.AppId)
                    };

                    var dataMode = GetRepository<McpApplicationInfo>().GetModel(cc);

                    dataMode.AppName = model.AppName;
                    dataMode.SignKey = model.SignKey;
                    dataMode.UpdateCallbackUrl = model.UpdateCallbackUrl;
                    dataMode.ModifiedBy = model.ModifiedBy;
                    dataMode.ModifiedOn = CommonUtil.GetDBDateTime();
                    dataMode.DeveloperId = model.DeveloperId;
                    dataMode.Memo = model.Memo;
                    if (string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(model.LogoPath))
                    {
                        dataMode.LogoPath = model.LogoPath;
                    }
                    else
                    {
                        dataMode.LogoPath = url;
                    }


                    var count = this.GetRepository<McpApplicationInfo>().Update(dataMode);

                    if (count <= 0)
                    {
                        result.Result = false;
                        result.Msg = "修改应用失败，请与管理员联系";
                        return result;
                    }
                    result.Msg = "修改应用成功";
                }
                else
                {
                    var cc = new ConditionCollection()
                    {
                        new Condition("app_name", model.AppName)
                    };

                    if (GetRepository<McpApplicationInfo>().IsExists(cc))
                    {
                        result.Result = false;
                        result.Msg = "应用名称已经在系统中存在，请更换应用名称";
                        return result;
                    }

                    var dataMode = new McpApplicationInfo()
                    {
                        AppId = CommonUtil.GetGuidNoSeparator(),
                        AppKey = VerifyCodeUtil.Intance.GenCode(16).ToLower(),
                        AppName = model.AppName,
                        SignKey = model.SignKey,
                        UpdateCallbackUrl = model.UpdateCallbackUrl,
                        LogoPath = url,
                        CreatedBy = model.ModifiedBy,
                        CreatedOn = CommonUtil.GetDBDateTime(),
                        DeveloperId = model.DeveloperId,
                        Memo = model.Memo,
                        StatusCode = Convert.ToInt32(ApplyStatuCode.Operative)
                    };

                    var appId = this.GetRepository<McpApplicationInfo>().Create(dataMode);
                    if (string.IsNullOrEmpty(appId.ToString()))
                    {
                        result.Result = false;
                        result.Msg = "新增应用失败，请与管理员联系";
                        return result;
                    }
                    result.Msg = "新增应用成功";
                }
                result.Result = true;
                return result;
            }
            return result;
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appId">应用ID</param>
        public void DropApplyInfo(string appId)
        {
            //校验当前APP是否绑定打印设备
            var isBind = CheckIsExistsAppIsBinded(appId);
            if (isBind)
            {
                throw new MessageException("当前应用已有打印设备关联，请先解除关联再删除");
            }

            var cc = new ConditionCollection
            {
                new Condition("app_id", appId)
            };
            var count = this.GetRepository<McpApplicationInfo>().Delete(cc);
            if (count <= 0)
            {
                throw new MessageException("删除应用失败，请与管理员联系");
            }
        }

        /// <summary>
        /// 校验APP是否绑定
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool CheckIsExistsAppIsBinded(string appId)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("app_id", appId));
            return this.GetRepository<McpMerchantPrinterInfo>().IsExists(cc);
        }

        /// <summary>
        /// 根据应用ID获取应用明细
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <returns>应用数据模型</returns>
        public DevApplyViewModel GetApplyViewModel(string appId)
        {
            var cc = new ConditionCollection
            {
                new Condition("app_id", appId)
            };
            var appInfo = this.GetRepository<McpApplicationInfo>().GetModel(cc);
            var viewModel = ModelConvertHelper.ToConvertModel<DevApplyViewModel>(appInfo);
            return viewModel;
        }

        /// <summary>
        /// 根据开发者Id获取应用列表
        /// </summary>
        /// <returns>应用列表集合</returns>
        public IList<DevApplyViewModel> GetApplyModelList()
        {
            var developerId = SessionUtil.Current.Account;
            var cc = new ConditionCollection()
            {
                new Condition("developer_id", developerId)
            };
            // ReSharper disable once JoinDeclarationAndInitializer
            IList<DevApplyViewModel> list;
            list = ModelConvertHelper.ToConvertModelS<DevApplyViewModel, McpApplicationInfo>(this.GetRepository<McpApplicationInfo>().ListModel(cc));
            return list;
        }

        /// <summary>
        /// 应用数据校验
        /// </summary>
        /// <param name="model"></param>
        private ProcessResult CheckApplyData(DevApplyViewModel model)
        {
            var result = new ProcessResult();

            var updateCallbackUrl = model.UpdateCallbackUrl;

            if (string.IsNullOrEmpty(model.AppName))
            {
                result.Result = false;
                result.Msg = "请输入应用名称";
                return result;
            }

            if (string.IsNullOrEmpty(model.Memo))
            {
                result.Result = false;
                result.Msg = "请输入应用描述";
                return result;
            }

            if (model.AppName.Length > 60)
            {
                result.Result = false;
                result.Msg = "应用名称不超过60个字符";
                return result;
            }

            if (model.Memo.Length > 255)
            {
                result.Result = false;
                result.Msg = "应用描述不超过255个字符";
                return result;
            }

            if (!string.IsNullOrEmpty(updateCallbackUrl))
            {
                if (!RegexUtil.IsMatch(ref updateCallbackUrl, RegexConsts.URL_PATTERN))
                {
                    throw new MessageException("回调地址格式不正确");
                }

                if (string.IsNullOrEmpty(model.SignKey))
                {
                    result.Result = false;
                    result.Msg = "您输入了回调地址，所以签名Key不能为空，请输入签名Key";
                    return result;
                }
            }
            result.Result = true;
            return result;
        }

        #endregion

        #region 上传图片（因为不是异步提交，所以没用公用的上传方法）

        /// <summary>
        /// 上传文件处理
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        private ProcessResult LogFileUpload(HttpPostedFileBase postedFile)
        {
            var result = new ProcessResult();
            result.Result = true;
            // 文件扩展名集合
            //string[] fileExtName = {".jpg",".jpeg",".png"};
            IList<string> fileExtName = new List<string>()
            {
                ".jpg",".jpeg",".png"
            };

            var extName = Path.GetExtension(postedFile.FileName);
            var fileSize = postedFile.ContentLength;
            var maxSize = ConvertUtil.ToInt(ConfigUtil.GetConfig("UploadFileMaxSize"));
            if (postedFile.ContentLength == 0)
            {
                result.Msg = "文件对象不能为空";
                result.Result = false;
                return result;
            }

            if (extName != null && !fileExtName.Contains(extName.ToLower()))
            {
                result.Msg = string.Format("{0}文件格式不支持，支持的文件格式有：'.jpg','.jpeg','.png'。", extName.ToLower());
                result.Result = false;
                return result;
            }

            if (fileSize > maxSize)
            {
                result.Msg = string.Format("您上传文件大小为{0}超过了单个文件最大限制{1}。", UploadFileUtil.GetFileSizeFormat(fileSize), UploadFileUtil.GetFileSizeFormat(maxSize));
                result.Result = false;
                return result;
            }

            string virtualPath = UploadFileUtil.GetVirtualPath(postedFile.FileName, "Upload");
            string physicalPath = UploadFileUtil.GetPhysicalPath(virtualPath);
            postedFile.SaveAs(physicalPath);
            result.Data = virtualPath;
            return result;
        }

        #endregion

        #endregion
    }
}
