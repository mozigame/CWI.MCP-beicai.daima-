//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名       日期                 说明
// --------------------------------------------
//      王军锋     2014/12/13 10:35:00  创建
//---------------------------------------------

using CWI.MCP.Models;
using CWI.MCP.Common;
using System.Collections.Generic;
using CWI.MCP.Common.ORM;
using Evt.Framework.Common;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.Caching;

namespace CWI.MCP.Services
{
    /// <summary>
    /// 模版服务
    /// </summary>
    public class TemplateService : BaseService
    {
        private ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// 校验模板是否存在
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public bool CheckTemplateIsExist(string templateId)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("template_id", templateId));
            cc.Add(new Condition("status_code", StatusCodeType.Valid.GetHashCode()));
            return this.GetRepository<McpTemplateInfo>().IsExists(cc);
        }

        /// <summary>
        /// 获取快递面点票据内容
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="billContent"></param>
        /// <param name="paramCode"></param>
        /// <returns></returns>
        public object GetExpressBillJson(string templateId, string billContent, string paramCode)
        {
            var billDic = new Dictionary<string, object>();
            try
            {
                var billcont = GetExpressBillContent(templateId, billContent);
                var printer = SingleInstance<PrinterService>.Instance.GetPrinter(paramCode);
                billDic.Add("left", (printer != null ? printer.OffSetLeft : 0.00m));
                billDic.Add("top", (printer != null ? printer.OffSetTop : 0.00m));
                billDic.Add("template_id", templateId);
                billDic.Add("content", billcont);
            }
            catch (Exception ex)
            {
                throw new MessageException(ex.Message);
            }

            return JsonConvert.SerializeObject(billDic);
        }

        /// <summary>
        /// 获取快递单内容
        /// </summary>
        /// <param name="templateId">快递模板ID</param>
        /// <param name="billContent">快递单打印参数</param>
        /// <returns></returns>
        private List<ExpressContentViewModel> GetExpressBillContent(string templateId, string billContent)
        {
            var paramdic = new Dictionary<string, object>();
            var content = new List<ExpressContentViewModel>();
            var exParams = GetExpressParams(templateId);
            try
            {
                paramdic = JsonConvert.DeserializeObject<Dictionary<string, object>>(billContent.Trim());
                foreach (var param in paramdic)
                {
                    var exPart = exParams.Find(d => d.param_code.Equals(param.Key, StringComparison.CurrentCultureIgnoreCase));
                    if (exPart != null)
                    {
                        var pv = param.Value.ToString().Trim();
                        if (exPart.is_need && string.IsNullOrWhiteSpace(pv))
                        {
                            throw new MessageException(string.Format("参数：{0}为必填项。", param.Key));
                        }
                        else
                        {
                            content.Add(new ExpressContentViewModel
                            {
                                v = (exPart.param_type == ParamType.Bool.GetHashCode() && pv == "1") ? Consts.AFFIRM : pv,
                                x = exPart.loc_x,
                                y = exPart.loc_y
                            });
                        }
                    }
                }
            }
            catch
            {
                LogUtil.Error(string.Format("错误的数据格式，billContent：{0}", billContent.Trim()));
                throw new MessageException("参数billContent格式应为Json格式。");
            }

            return content;
        }

        /// <summary>
        /// 获取参数坐标
        /// </summary>
        /// <param name="templateId">获取模板参数</param>
        /// <returns>模板参数列表</returns>
        private List<ExpressParamViewModel> GetExpressParams(string templateId)
        {
            var exParams = new List<ExpressParamViewModel>();
            if (cache.Contains(templateId))
            {
                exParams = cache.Get(templateId) as List<ExpressParamViewModel>;
            }
            else
            {
                var cc = new ConditionCollection();
                cc.Add(new Condition("template_id", templateId));
                var list = this.GetRepository<McpTemplateParamInfo>().ListModel(cc);
                if (list != null)
                {
                    foreach (var param in list)
                    {
                        exParams.Add(new ExpressParamViewModel()
                        {
                            param_code = TryConvertUtil.ToString(param.ParamCode),
                            loc_x = TryConvertUtil.ToDecimal(param.LocX),
                            loc_y = TryConvertUtil.ToDecimal(param.LocY),
                            param_type = param.ParamType,
                            is_need = TryConvertUtil.ToBool(param.IsNeed)
                        });
                    }
                }

                cache.Add(templateId, exParams, DateTimeOffset.Now.AddHours(24));
            }

            return exParams;
        }
    }
}
