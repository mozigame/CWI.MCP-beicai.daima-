//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋     2014/12/13 10:35:00         创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CWI.MCP.Models;
using CWI.MCP.Common;
using Evt.Framework.Common;
using CWI.MCP.Services.APICommon;
using System.Text.RegularExpressions;
using CWI.MCP.Common.ORM;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CWI.Cache;
using CWI.Cache.Factory;
using System.Runtime.Caching;

namespace CWI.MCP.Services
{
    /// <summary>
    /// 系统业务
    /// </summary>
    public class SysService : BaseService
    {
        /// <summary>
        /// Redis缓存
        /// </summary>
        private ObjectCache cache = MemoryCache.Default;

        #region 发送短信

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="appSign"></param>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <param name="smsType"></param>
        /// <returns></returns>
        public void SendSmsMessage(string appSign, string mobile, Dictionary<string, object> parms, string content, SMSType smsType, SmsFuncType tempType)
        {
            // 参数检查
            if (string.IsNullOrWhiteSpace(appSign))
            {
                throw new MessageException("应用标识不能为空！");
            }
            if (string.IsNullOrWhiteSpace(mobile))
            {
                throw new MessageException("手机号码不能为空！");
            }
            if (parms.Count <= 0)
            {
                throw new MessageException("短信内容参数不能为空！");
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new MessageException("短信内容不能为空！");
            }
            if (!RegexUtil.IsMobile(mobile))
            {
                throw new MessageException(string.Format("手机号码：{0},格式不正确。" + mobile));
            }

            var smsId = CreateSmsRecord(appSign, mobile, content, smsType);
            switch (smsType)
            {
                case SMSType.Bind:
                case SMSType.UnBind:
                case SMSType.GetPwd:
                case SMSType.ValidateCode:
                    SmsUtil.SendValidCode(mobile, parms["code"].ToString(), TryConvertUtil.ToDecimal(parms["code_expire"], 2), tempType);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 数据写入

        /// <summary>
        /// 添加发送短信记录
        /// </summary>
        /// <param name="appSign"></param>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <param name="smsType"></param>
        /// <returns></returns>
        private long CreateSmsRecord(string appSign, string mobile, string content, SMSType smsType)
        {
            // 写入数据
            McpSysSmsInfo sms = new McpSysSmsInfo();
            sms.AppSign = appSign;
            sms.ReceiverNo = mobile;
            sms.Content = content;
            sms.SmsType = (int)smsType;
            sms.CreatedBy = "1";
            sms.CreatedOn = CommonUtil.GetDBDateTime();
            sms.StatusCode = (int)StatusCodeType.Valid;
            return this.GetRepository<McpSysSmsInfo>().Create(sms);
        }

        #endregion

        #region 版本升级

        /// <summary>
        /// 检查版本信息
        /// </summary>
        /// <param name="verModel">版本信息参数</param>
        /// <returns>版本信息结果</returns>
        public object CheckVersion(VerViewModel verModel)
        {
            //校验设备表示
            bool isExist = SingleInstance<PrinterService>.Instance.CheckEquipmentIsExists(verModel.Mac);
            if (!isExist)
            {
                LogUtil.Info($"设备不存在-{JsonUtil.Serialize(verModel)}");
                throw new MessageException("设备不存在！");
            }

            //更改设备对应版本号
            bool result=SingleInstance<PrinterService>.Instance.UpdatePrintVersion(verModel);
            if (!result)
            {                
                throw new MessageException($"更新打印机{verModel.Mac}版本号失败，请重试");
            }

            //获取版本
            Dictionary<string, object> verDic = new Dictionary<string, object>();
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("objec_type", TryConvertUtil.ToString(verModel.Type)));
            cc.Add(new Condition("status_code", StatusCodeType.Valid.GetHashCode()));

            int isNeed = 0;
           
            string newVer = string.Empty;
            string filePath = string.Empty;
            var lastVer = this.GetRepository<McpSysVersionInfo>().GetModel(cc);
            if (lastVer != null)
            {
                var isUpdate = false;
                var serVer = 0;
                var curVer = 0;
                var serVers = lastVer.ObjectVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var curVers = verModel.Ver.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var maxLength = Math.Max(serVers.Length, curVers.Length);
               
                for (var i = 0; i < maxLength; i++)
                {
                    serVer = i < serVers.Length ? TryConvertUtil.ToInt(serVers[i]) : 0;
                    curVer = i < curVers.Length ? TryConvertUtil.ToInt(curVers[i]) : 0;
                    if (serVer != curVer)
                    {
                        isUpdate = serVer > curVer;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (isUpdate)
                {
                    isNeed = lastVer.IsForce > 0 ? 2 : 1;
                    newVer = lastVer.ObjectVersion;
                    filePath = lastVer.VersionFile;
                }
            }

            verDic.Add("IsNeedUpdate", isNeed);
            verDic.Add("NewVer", newVer);
            verDic.Add("UpdateFilePath", filePath);

            LogUtil.Info(string.Format("设备：{0},当前版本：{1}，获取新版本...", verModel.Mac, verModel.Ver));

            return verDic;
        }



        #endregion

        #region 统计年月

        /// <summary>
        /// 获取月份列表
        /// </summary>
        /// <param name="isCurrent">是否包括当前月份</param>
        /// <returns>月份列表</returns>
        public List<string> GetMonthList(bool isCurrent = true)
        {
            DateTime minDate = new DateTime(2015, 01, 01);
            DateTime dbNow = CommonUtil.GetDBDateTime().Date;
            if (!isCurrent)
            {
                dbNow = dbNow.AddMonths(-1);
            }
            DateTime beginDate = dbNow.AddMonths(-11).Date;
            beginDate = beginDate > minDate ? beginDate : minDate;

            List<string> dateList = new List<string>();
            for (int i = 0; i <= 11; i++)
            {
                DateTime date = beginDate.AddMonths(i);
                if (date <= dbNow)
                {
                    dateList.Add((date.Year * 100 + date.Month).ToString());
                }
            }

            return dateList;
        }

        /// <summary>
        /// 校验统计年月字符传
        /// </summary>
        /// <param name="month">统计年月</param>
        public void CheckMonthIsValid(string month)
        {
            month = TryConvertUtil.ToString(month);
            CheckMonth(month);

            try
            {
                string yy = month.Substring(0, 4);
                string mm = month.Substring(4, 2);
                DateTime dt = new DateTime(TryConvertUtil.ToInt(yy), TryConvertUtil.ToInt(mm), 1);
            }
            catch
            {
                throw new MessageException("参数错误！");
            }
        }

        /// <summary>
        /// 校验统计月
        /// </summary>
        /// <param name="month">统计月</param>
        public DateTime GetStatisMonth(string month)
        {
            DateTime statisDate = DateTime.MaxValue;
            CheckMonth(month);

            try
            {
                int yy = TryConvertUtil.ToInt(month.Substring(0, 4), DateTime.MaxValue.Year);
                int mm = TryConvertUtil.ToInt(month.Substring(4, 2), DateTime.MaxValue.Month);
                statisDate = new DateTime(yy, mm, 1);
            }
            catch (Exception ex)
            {
                LogUtil.Warn(string.Format("统计月份参数错误，参考信息：{0}。", ex.Message));
                throw new MessageException(string.Format("统计月份参数错误！"));
            }
            return statisDate;
        }

        /// <summary>
        /// 基本校验
        /// </summary>
        /// <param name="month"></param>
        private void CheckMonth(string month)
        {
            if (string.IsNullOrWhiteSpace(month))
            {
                throw new MessageException("参数不能空！");
            }

            if (month.Length != 6 && Regex.IsMatch(month, RegexConsts.ALL_NUM_PARTTERN))
            {
                throw new MessageException("参数格式错误！");
            }
        }

        #endregion

        #region 读写redis缓存

        /// <summary>
        /// 写缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetCache(string key, int value)
        {
            try
            {
                if (cache.Contains(key))
                {
                    cache.Remove(key);
                }

                cache.Add(key, value, DateTimeOffset.Now.AddHours(12));
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("写缓存，Key：{0},Value:{1},发生异常,参考信息：{2}", key, value, ex.Message));
            }
        }

        /// <summary>
        /// 读缓存
        /// </summary>
        /// <param name="did"></param>
        /// <returns></returns>
        public int GetCache(string key)
        {
            var scode = 0;
            try
            {
                if (cache.Contains(key))
                {
                    scode = TryConvertUtil.ToInt(cache.Get(key));
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("读取key：{0}value发生异常,参考信息：{1}", key, ex.Message));
            }
            return scode;
        }

        #endregion

        #region 清理打印任务

        /// <summary>
        /// 清理超时打印任务
        /// </summary>
        public int ClearPrintTasks(int mins)
        {
            ClearPrintTasksCallBack(mins);

            return UpdatPrintTaskStatus(mins);
        }

        /// <summary>
        /// 清理超时打印任务
        /// </summary>
        private int UpdatPrintTaskStatus(int mins)
        {
            string ClearStatus = ConfigUtil.GetConfig("ClearStatus");
            var sql = @"UPDATE mcp_order_printer SET order_status = 3, modified_on = NOW() WHERE order_status in ("+ ClearStatus + ") AND TIMESTAMPDIFF(MINUTE,created_on,NOW()) >= @mins ";
            var pc = new ParameterCollection();
            pc.Add("mins", mins);
            return DbUtil.DataManager.DataManagerMcp.IData.ExecuteNonQuery(sql, pc);
        }

        /// <summary>
        /// 获取待清理超时打印任务列表
        /// </summary>
        /// <param name="mins"></param>
        /// <returns></returns>
        private List<McpOrderPrinterInfo> GetWaitClearPrintTasks(int mins)
        {
            string ClearStatus = ConfigUtil.GetConfig("ClearStatus");
            var sql = @"SELECT * FROM mcp_order_printer WHERE order_status in "+ ClearStatus + " AND TIMESTAMPDIFF(MINUTE,created_on,NOW()) >= @mins ";
            var pc = new ParameterCollection();
            pc.Add("mins", mins);

            return this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql, pc);
        }

        /// <summary>
        /// 超时未打印通知回调
        /// </summary>
        /// <param name="mins"></param>
        private void ClearPrintTasksCallBack(int mins)
        {
            var taskList = GetWaitClearPrintTasks(mins);
            foreach (var task in taskList)
            {
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.AppCallBack(PrintOrderStatusType.OverTime.GetHashCode(), task.OrderId, task.PrinterCode);
                });
                ac.BeginInvoke(null, null);
            }
        }

        #endregion
    }
}
