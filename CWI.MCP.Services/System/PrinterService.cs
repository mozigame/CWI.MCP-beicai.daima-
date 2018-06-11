﻿//---------------------------------------------
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
using System.Text;
using System.Linq;
using System.Transactions;
using System.Web;
using System.IO;
using System.Data;
using System.Runtime.Caching;
using CWI.Cache.Factory;
using CWI.Cache;
using Newtonsoft.Json;

namespace CWI.MCP.Services
{
    public class PrinterService : BaseService
    {
        /// <summary>
        /// 失败后尝试次数
        /// </summary>
        private const int tryMax = 5;

        /// <summary>
        /// 尝试延长执行间隔，单位：毫秒
        /// </summary>
        private const int delayTryInterval = 3000;

        /// <summary>
        /// Redis缓存
        /// </summary>
        private static ICache cache = CacheFactory.Cache();

        #region 获取打印设备

        /// <summary>
        /// 获取打印设备
        /// </summary>
        /// <param name="printerCode">打印设备Mac地址</param>
        /// <returns>打印设备</returns>
        public McpPrinterInfo GetPrinter(string printerCode)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpPrinterInfo>().GetModel(cc);
        }

        /// <summary>
        /// 获取应用关联打印设备列表
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public List<McpMerchantPrinterInfo> GetMerchantPrinter(string printerCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpMerchantPrinterInfo>().ListModel(cc);
        }

        /// <summary>
        /// 根据订单获取打印设备列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public List<McpPrinterInfo> GetOrderPrinterList(string orderId)
        {
            StringBuilder sb = new StringBuilder();
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("order_id", TryConvertUtil.ToString(orderId)));
            cc.Add(new Condition("order_status", PrintOrderStatusType.UnPrinted.GetHashCode()));
            var orderPrinters = this.GetRepository<McpOrderPrinterInfo>().ListModel(cc);
            foreach (var printer in orderPrinters)
            {
                sb.AppendFormat("'{0}',", printer.PrinterCode);
            }

            string codes = string.IsNullOrWhiteSpace(sb.ToString()) ? "''" : sb.ToString().TrimEnd(',');
            ConditionCollection c1 = new ConditionCollection();
            c1.Add(new Condition("printer_code", codes, OperationType.In));
            c1.Add(new Condition("status_code", StatusCodeType.Valid.GetHashCode()));
            return this.GetRepository<McpPrinterInfo>().ListModel(c1);
        }

        #endregion

        #region 打印设备操作

        /// <summary>
        /// 关联打印设备
        /// </summary>
        /// <param name="bindModel"></param>
        public List<string> CheckPrinterIsEnableBind(PrinterCheckViewModel checkModel)
        {
            //1.校验身份
            var appId = SingleInstance<AppService>.Instance.GetAppIdByAccessToken(checkModel.access_token);
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new MessageException("无效访问令牌！");
            }

            //2.构建参数
            var codes = new StringBuilder();
            var printerCodes = (checkModel.printer_codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).ToList();
            foreach (var printer in printerCodes)
            {
                codes.AppendFormat("'{0}',", printer.Trim());
            }

            //3.校验打印设备是否合法
            var c1 = new ConditionCollection();
            c1.Add(new Condition("equipment_code", codes.ToString().TrimEnd(','), OperationType.In));
            var list = this.GetRepository<McpEquipmentInfo>().ListModel(c1);
            var isAssApp = ConfigUtil.AssociatedAppIds.IndexOf(appId) >= 0;
            if (list != null)
            {
                foreach (var code in printerCodes)
                {
                    var printer = list.Find(d => d.EquipmentCode.Equals(code, StringComparison.CurrentCultureIgnoreCase));
                    if (printer == null)
                    {
                        throw new MessageException(string.Format("打印设备: {0}不存在。", code));
                    }
                    else
                    {
                        var appPrinters = GetMerchantPrinter(code);
                        if (appPrinters != null && appPrinters.Count > 0)
                        {
                            if (isAssApp)
                            {
                                //打印设备已关联，且当前应用为关联应用
                                var appPrinter = appPrinters.Find(d => d.AppId.Equals(appId, StringComparison.CurrentCultureIgnoreCase));
                                if (appPrinter != null)
                                {
                                    //已存在当前应用的关联信息
                                    throw new MessageException(string.Format("打印设备：{0}已关联！", code));
                                }
                                else
                                {
                                    appPrinter = appPrinters.FirstOrDefault();
                                    if (appPrinter != null && ConfigUtil.AssociatedAppIds.IndexOf(appPrinter.AppId) <= -1)
                                    {
                                        //已关联的为非关联应用
                                        throw new MessageException(string.Format("打印设备：{0}已关联！", code));
                                    }
                                }
                            }
                            else
                            {
                                //打印设备已关联，且当前应用为非关联应用
                                throw new MessageException(string.Format("打印设备：{0}已关联！", code));
                            }
                        }
                    }
                }
            }
            else
            {
                throw new MessageException("未找到任何打印设备。");
            }
            return printerCodes;
        }

        /// <summary>
        /// 关联打印设备
        /// </summary>
        /// <param name="bindModel"></param>
        public void BindPrinters(PrintViewModel bindModel)
        {
            //1.校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(bindModel.app_id);
            if (!appIsOk)
                return;

            //2.校验身份
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(bindModel.app_id, bindModel.access_token);
            if (!tokenIsOk)
            {
                throw new MessageException("无效访问令牌！");
            }

            //3.执行关联
            DateTime dbNow = CommonUtil.GetDBDateTime();
            var prtlist = GetPrinterList(bindModel.printer_codes.Trim(), bindModel.app_id.Trim(), bindModel.merchant_code.Trim());
            if (prtlist != null)
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        foreach (var prt in prtlist)
                        {
                            //2.1创建关联记录
                            var merchantPrinter = new McpMerchantPrinterInfo
                            {
                                AppId = bindModel.app_id,
                                MerchantCode = bindModel.merchant_code.Trim(),
                                PrinterCode = prt.ToUpper(),
                                CreatedOn = dbNow
                            };
                            this.GetRepository<McpMerchantPrinterInfo>().Create(merchantPrinter);
                        }

                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        var msg = string.Format("关联失败，{0}", ex.Message);
                        LogUtil.Error(msg);
                        throw new MessageException(msg);
                    }
                }
            }
        }

        /// <summary>
        /// 解邦打印设备
        /// </summary>
        /// <param name="unBindModel"></param>
        public void UnBindPrinters(PrinterBaseViewModel unBindModel)
        {
            //1.校验身份
            var appId = SingleInstance<AppService>.Instance.GetAppIdByAccessToken(unBindModel.access_token);
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new MessageException("无效访问令牌！");
            }

            //2.构建参数
            var codes = new StringBuilder();
            var printerCodes = unBindModel.printer_codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < printerCodes.Length; i++)
            {
                codes.AppendFormat("'{0}',", printerCodes[i].Trim());
            }

            //3.校验解除的打印设备必须先断网
            List<McpOrderPrinterInfo> taskList = null;
            var c1 = new ConditionCollection();
            c1.Add(new Condition("connection_id", 0, OperationType.GreaterThan));
            c1.Add(new Condition("printer_code", codes.ToString().TrimEnd(','), OperationType.In));
            bool isConned = this.GetRepository<McpPrinterInfo>().IsExists(c1);
            if (isConned)
            {
                throw new MessageException("请将打印机关机3分钟后再解绑！");
            }

            //4.0解绑校验
            var pCodes = codes.ToString().TrimEnd(',');
            var cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", pCodes, OperationType.In));
            var printer = this.GetRepository<McpMerchantPrinterInfo>().ListModel(cc);
            if (printer != null && printer.Count >= printerCodes.Length)
            {
                //有商家关联记录
                var appPrinter = printer.FindAll(d => d.AppId.Equals(appId, StringComparison.CurrentCultureIgnoreCase));
                if (appPrinter != null)
                {
                    //关联商家是否当前账号
                    var merchatPrinter = appPrinter.Find(d => d.MerchantCode.Equals(unBindModel.merchant_code, StringComparison.CurrentCultureIgnoreCase));
                    if (merchatPrinter == null)
                    {
                        throw new MessageException("该打印机与其他账号绑定，您无权解绑！");
                    }
                }
                else
                {
                    //与当前应用无关联记录
                    throw new MessageException("该打印机尚未与本应用帐号绑定！");
                }
            }
            else
            {
                //无商家关联记录
                throw new MessageException("该打印机尚未绑定或者绑定到了其他应用！");
            }

            try
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    //4.1执行删除
                    cc.Add(new Condition("app_id", appId));
                    cc.Add(new Condition("merchant_code", unBindModel.merchant_code));
                    int cnt = this.GetRepository<McpMerchantPrinterInfo>().Delete(cc);
                    if (cnt > 0)
                    {
                        //4.2查询解绑打印机待清理的任务
                        taskList = GetWaitClearPrintTasks(pCodes, appId);

                        //4.3删除打印机在线记录、打印机状态记录
                        var c2 = new ConditionCollection();
                        c2.Add(new Condition("printer_code", pCodes, OperationType.In));

                        this.GetRepository<McpPrinterInfo>().Delete(c2);
                        for (int i = 0; i < printerCodes.Length; i++)
                        {
                            try
                            {
                                //打印设备回调
                                LogUtil.Info(string.Format("解绑打印机,删除联网信息，并执行打印设备回调,设备号：{0}, 状态: {1}", printerCodes[i], PrinterFaultType.NetworkFault.GetHashCode()));
                                var dbNow = CommonUtil.GetDBDateTime();
                                //打印设备状态更改回调
                                Action ac = new Action(() =>
                                {
                                    SingleInstance<AppService>.Instance.DeviceCallBack(PrinterFaultType.NetworkFault.GetHashCode(), printerCodes[i], dbNow);
                                });
                                ac.BeginInvoke(null, null);
                            }
                            catch (Exception ex)
                            {
                                LogUtil.Error(string.Format("解绑打印机,删除联网信息，并执行打印设备回调出现异常,设备号：{0}, 状态: {1}", printerCodes[i], PrinterFaultType.NetworkFault.GetHashCode()),ex);
                            }
                        }
                        
                        this.GetRepository<McpPrinterStatusInfo>().Delete(c2);

                        //4.4更新目前未打印的任务状态为：解绑清理状态
                        var pc = new ParameterCollection();
                        pc.Add("app_id", appId);
                        var sql = string.Format("UPDATE mcp_order_printer SET order_status = 4, modified_on = NOW() WHERE printer_code IN ({0}) AND order_status = 0 AND order_id IN (SELECT order_id FROM mcp_order WHERE app_id =@app_id AND TIMESTAMPDIFF(HOUR,order_date,NOW())<=2); ", pCodes);
                        this.GetRepository<McpOrderPrinterInfo>().DBManager.IData.ExecuteNonQuery(sql, pc);
                    }

                    tran.Complete();
                }

                //5.回调更新打印任务状态
                ClearPrintTasksCallBack(taskList);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("解绑打印机失败，参考信息：{0}", ex.Message));
                throw new MessageException(ex.Message);
            }
        }

        /// <summary>
        /// 注册打印设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object RegisterPrinter(TcpQueryModel query)
        {
            LogUtil.Info(string.Format("【101】接入登记，设备号：{0}, 上次设备故障代码：{1}", query.Did, query.Code));

            int errno = 0;
            bool isOk = false;
            var device = GetPrinter(query.Did);
            if (device == null)
            {
                if (CheckEquipmentIsExists(query.Did))
                {
                    isOk = CeratePrinter(query);
                }
                else
                {
                    return TcpConsts.PrinterNotFind;
                }
            }
            else
            {
                device.ConnectionId = query.ConnId;
                device.IpPort = query.IpPort;
                isOk = UpdatePrinter(device);
            }

            //注册成功后请求未打印的订单下发
            //注册成功执行设备：张友辉，2018-03-09 18:03:26
            if (isOk)
            {
                errno = 0;
                //注册成功后，判断打印机状态并回调
                GetPrintStatusByCode(query.Did,"101");
                //注册时不再下发打印任务【2017/05/27调整】
                //SingleInstance<PrintOrderService>.Instance.SendUnPrintOrder(query.Did, "打印设备注册时");
            }
            else
            {
                errno = TcpConsts.RegDevFailed;
            }
            return errno;
        }

        /// <summary>
        /// 根据设备编码判断设备状态，包括联网状态,并回调
        /// </summary>
        /// <param name="printCode"></param>
        public void GetPrintStatusByCode(string printCode,string tp="00")
        {
            try
            {
                int statusCode = PrinterFaultType.NotFind.GetHashCode();
                //获取打印机联网信息
                var device = GetPrinter(printCode);
                var dbNow = CommonUtil.GetDBDateTime();
                if (device == null || device.ConnectionId == 0)
                {
                    statusCode = PrinterFaultType.NetworkFault.GetHashCode();
                }
                else
                {
                    //获取打印机状态
                    var status = GetPrinterStatus(printCode);
                    if (status != null)
                    {
                        statusCode = status.StatusCode;
                    }
                    else
                    {
                        statusCode = PrinterFaultType.NotFind.GetHashCode();
                    }
                }
                //打印设备状态更改回调
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.DeviceCallBack(statusCode, printCode, dbNow);
                });
                ac.BeginInvoke(null, null);
                LogUtil.Info(string.Format("【{0}】设备状态回调完成，设备号：{1},状态码：{2}", tp, printCode, statusCode));
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("【{0}】设备状态回调失败，设备号：{1}，详情请看错误日志", tp, printCode));
                LogUtil.Error(string.Format("【{0}】设备状态回调失败，设备号：{1}", tp, printCode),ex);
            }

        }

        /// <summary>
        /// 更新打印设备
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public bool UpdatePrinter(McpPrinterInfo printer)
        {
            printer.IpPort = printer.IpPort;
            printer.ConnectionId = printer.ConnectionId;
            printer.ModifiedBy = "printer";
            printer.ModifiedOn = CommonUtil.GetDBDateTime();
            bool result= this.GetRepository<McpPrinterInfo>().Update(printer, "ip_port,connection_id,modified_by,modified_on") > 0;
            if (result&&printer.ConnectionId == 0)
            {
                LogUtil.Info(string.Format("【101】注册打印设备:开始发送设备状态更改通知,设备号：{0}, 状态: {1}", printer.PrinterCode, PrinterFaultType.NetworkFault.GetHashCode()));
                var dbNow = CommonUtil.GetDBDateTime();
                //打印设备状态更改回调
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.DeviceCallBack(PrinterFaultType.NetworkFault.GetHashCode(), printer.PrinterCode, dbNow);
                });
                ac.BeginInvoke(null, null);
            }
            return result;
        }

        /// <summary>
        /// 注册打印设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool CeratePrinter(TcpQueryModel query)
        {
            var printer = new McpPrinterInfo
            {
                PrinterCode = query.Did,
                ConnectionId = query.ConnId,
                IpPort = query.IpPort,
                StatusCode = StatusCodeType.Valid.GetHashCode(),
                CreatedBy = "printer",
                CreatedOn = CommonUtil.GetDBDateTime()
            };
            bool result= this.GetRepository<McpPrinterInfo>().Create(printer) > 0;
            if (result&&query.ConnId==0)
            {
                LogUtil.Info(string.Format("【101】注册打印设备:开始发送设备状态更改通知,设备号：{0}, 状态: {1}", query.Did, PrinterFaultType.NetworkFault.GetHashCode()));
                var dbNow = CommonUtil.GetDBDateTime();
                //打印设备状态更改回调
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.DeviceCallBack(PrinterFaultType.NetworkFault.GetHashCode(), query.Did, dbNow);
                });
                ac.BeginInvoke(null, null);
            }

            return result;
        }

        /// <summary>
        /// 上报打印设备状态
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object UploadPrinterSatatus(TcpQueryModel query)
        {
            int errno = 0;
            LogUtil.Info(string.Format("【104】更新打印设备状态，设备号：{0},状态: {1}", query.Did, query.Code));
            if (query.Code == 0 || query.Code == 100)
            {
                LogUtil.Info(string.Format("设备号：{0},状态: {1},内部状态码不更新", query.Did, query.Code));
                return errno;
            }
            else
            {
                //校验打印设备是否注册
                bool isExist = CheckPrinterIsRegisted(query.Did);
                if (isExist)
                {
                    //已注册打印设备
                    var dbNow = CommonUtil.GetDBDateTime();
                    var status = GetPrinterStatus(query.Did);
                    //判断设备连接服务器的状态：张友辉
                    var device = GetPrinter(query.Did);
                    if (device==null||device.ConnectionId==0)
                    {
                        query.Code = PrinterFaultType.NetworkFault.GetHashCode();

                    }
                    if (status != null)
                    {
                        //打印设备已存在状态记录
                        if (status.StatusCode != query.Code)
                        {
                            //打印设备状态改变更新之
                            status.PrinterCode = query.Did;
                            status.StatusCode = query.Code;
                            if (query.Code == PrinterFaultType.Enable.GetHashCode())
                            {
                                status.EndTime = dbNow;
                            }
                            else
                            {
                                status.BeginTime = dbNow;
                                status.EndTime = DateTime.MaxValue;
                            }
                            errno = this.GetRepository<McpPrinterStatusInfo>().Update(status) > 0 ? 0 : TcpConsts.UpExceptionFailed;
                            LogUtil.Info(string.Format("更新打印设备:{0},状态为：{1}", query.Did, query.Code));

                            //打印设备状态由异常变更为正常
                            if (query.Code == 1)
                            {
                                errno = 0;

                                //更新打印设备状态时不再下发打印任务【2017/05/27调整】
                                //SingleInstance<PrintOrderService>.Instance.SendUnPrintOrder(query.Did, "更新打印设备状态时");
                            }
                        }
                    }
                    else
                    {
                        //新增打印设备状态记录
                        var newStatus = new McpPrinterStatusInfo()
                        {
                            PrinterCode = query.Did,
                            StatusCode = query.Code,
                            BeginTime = dbNow
                        };
                        errno = this.GetRepository<McpPrinterStatusInfo>().Create(newStatus) > 0 ? 0 : TcpConsts.UpExceptionFailed;
                        LogUtil.Info(string.Format("新增打印设备状态记录,设备号:{0},状态为：{1}", query.Did, query.Code));
                    }

                    //打印设备状态写入Redis缓存
                    SetDeviceStatCache(query.Did, query.Code);

                    LogUtil.Info(string.Format("【104】开始发送设备状态更改通知,设备号：{0}, 状态: {1}", query.Did, query.Code));
                    //打印设备状态更改回调
                    Action ac = new Action(() =>
                    {
                        SingleInstance<AppService>.Instance.DeviceCallBack(query.Code, query.Did, dbNow);
                    });
                    ac.BeginInvoke(null, null);
                }
                else
                {
                    //打印设备未注册
                    errno = TcpConsts.PrinterNotFind;
                }
                return errno;
            }
        }

        /// <summary>
        /// 设置设备状态缓存
        /// </summary>
        /// <param name="did"></param>
        /// <param name="code"></param>
        public void SetDeviceStatCache(string did, int code)
        {
            try
            {
                var cacheKey = string.Format(CacheKeyConsts.DEVICE_STAT_KEY, did);
                var stat = cache.GetCache<object>(cacheKey);
                if (stat != null)
                {
                    if (TryConvertUtil.ToInt(stat) != code)
                    {
                        cache.RemoveCache(cacheKey);
                        cache.WriteCache<object>(code, cacheKey, DateTime.Now.AddSeconds(60));
                    }
                }
                else
                {
                    cache.WriteCache<object>(code, cacheKey, DateTime.Now.AddSeconds(60));
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("设置设备：{0}状态发生异常,参考信息：{1}", did, ex.Message));
            }
        }

        /// <summary>
        /// 获取打印设备状态
        /// </summary>
        /// <param name="did"></param>
        /// <returns></returns>
        public int GetDeviceStatCache(string did)
        {
            var scode = PrintStatusType.NoFind.GetHashCode();
            try
            {
                var cacheKey = string.Format(CacheKeyConsts.DEVICE_STAT_KEY, did);
                object stat = null;
                try
                {
                    stat = cache.GetCache<object>(cacheKey);
                }
                catch (Exception ex)
                {
                    LogUtil.Info(string.Format("缓存中获取设备状态异常，设备号:{0}",did));
                    LogUtil.Error(string.Format("缓存中获取设备状态异常，设备号:{0},异常信息：{1}", did,ex.Message),ex);
                }
                if (stat != null)
                {
                    LogUtil.Info(string.Format("缓存中取设备：{0}的状态为{1}", did, stat));
                    scode = TryConvertUtil.ToInt(stat);
                }
                else
                {
                    //从数据库获取打印设备状态
                    var ps = GetPrinterStatus(did);
                    if (ps != null)
                    {
                        try
                        {
                            cache.WriteCache<object>(ps.StatusCode, cacheKey, DateTime.Now.AddSeconds(60));
                        }
                        catch (Exception ex)
                        {
                            LogUtil.Info(string.Format("缓存中设置设备状态异常，设备号:{0}", did));
                            LogUtil.Error(string.Format("缓存中设置设备状态异常，设备号:{0},异常信息：{1}", did,ex.Message));
                        }
                    }

                    scode = ps != null ? ps.StatusCode : PrintStatusType.NoFind.GetHashCode();
                    LogUtil.Info(string.Format("数据库中取设备：{0}的状态为{1}", did, scode));
                    return scode;
                }
            }
            catch(Exception ex)
            {
                var ps = GetPrinterStatus(did);
                scode = ps != null ? ps.StatusCode : PrintStatusType.NoFind.GetHashCode();
                LogUtil.Info(string.Format("读取设备：{0}状态发生异常,参考信息：{1}", did, ex.Message));
            }
            return scode;
        }

        /// <summary>
        /// 获取打印设备状态
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public McpPrinterStatusInfo GetPrinterStatus(string printerCode)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpPrinterStatusInfo>().GetModel(cc);
        }

        /// <summary>
        /// 获取打印设备状态【待完善】
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public int GetPrinterStatusCode(string printerCode)
        {
            var resultCode = PrintStatusType.NoFind.GetHashCode();
            var scode = GetDeviceStatCache(printerCode);
            if (scode != PrintStatusType.NoFind.GetHashCode())
            {
                //bool isBind = CheckPrinterIsExists(TryConvertUtil.ToString(printerCode));
                //2018年4月17日13:55:41 因为有测试demo 这里不判断是否绑定
                bool isBind = true;
                if (!isBind)
                {
                    //resultCode = ResultCodeType.NotBind.GetHashCode();
                    resultCode = PrintStatusType.NoBind.GetHashCode();
                }
                else
                {
                    resultCode = scode;
                    //switch (scode)
                    //{
                    //    case 1:
                    //        resultCode = 1;
                    //        //resultCode = ResultCodeType.Success.GetHashCode();
                    //        break;
                    //    case 2:
                    //        resultCode = 2;
                    //        //resultCode = ResultCodeType.NeedPaper.GetHashCode();
                    //        break;
                    //    case 3:
                    //        resultCode = 3;
                    //        //resultCode = ResultCodeType.Fault.GetHashCode();
                    //        break;
                    //    default:
                    //        resultCode = 99;
                    //        //resultCode = ResultCodeType.UnKnow.GetHashCode();
                    //        break;
                    //}
                }
            }
            else
            {
                resultCode = PrintStatusType.NoFind.GetHashCode();
            }

            LogUtil.Info(string.Format("校验打印设备:{0},当前状态：{1}", printerCode, resultCode.ToString()));
            return resultCode;
        }

        /// <summary>
        /// 校验设备是否存在
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public bool CheckEquipmentIsExists(string printerCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("equipment_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpEquipmentInfo>().IsExists(cc);
        }

        public McpEquipmentInfo GetPrinterEquipment(string printerCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("equipment_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpEquipmentInfo>().GetModel(cc);
        }

        /// <summary>
        /// 校验打印设备是否存在
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public bool CheckPrinterIsExists(string printerCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpMerchantPrinterInfo>().IsExists(cc);
        }

        /// <summary>
        /// 校验打印设备是否已注册
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public bool CheckPrinterIsRegisted(string printerCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            return this.GetRepository<McpPrinterInfo>().IsExists(cc);
        }

        /// <summary>
        /// 修改打印机版本号
        /// </summary>
        /// <param name="verModel"></param>
        /// <returns></returns>
        public bool UpdatePrintVersion(VerViewModel verModel)
        {
            int result = 0;
            McpEquipmentInfo ep = new McpEquipmentInfo();
            ep.EquipmentCode = verModel.Mac;
            ep.ModifiedOn = CommonUtil.GetDBDateTime();
            //判断是软件还是固件
            var isVer = ConfigUtil.GetConfig("IsVerOrfirmware");
            ConditionCollection cd = new ConditionCollection
            {
                new Condition("equipment_code",verModel.Mac)
            };
            if (verModel.Type.ToLower().Contains(isVer))
            {
                ep.Firmware_version = verModel.Ver;
                result=this.GetRepository<McpEquipmentInfo>().Update(ep, cd, "firmware_version,modified_on");
            }
            else
            {
                ep.Var_version = verModel.Ver;
                result=this.GetRepository<McpEquipmentInfo>().Update(ep, cd, "var_version,modified_on");
            }
            if (result<=0)
            {
                LogUtil.Info($"修改打印机{verModel.Mac}版本号失败");
                LogUtil.Error($"修改打印机{verModel.Mac}版本号失败");
                return false;
            }
            else
            {
                LogUtil.Info($"修改打印机{verModel.Mac}版本号成功，型号：{verModel.Type},版本号：{verModel.Ver}");
                return true;
            }
        }

        /// <summary>
        /// 修改打印机域名-domin
        /// </summary>
        /// <param name="verModel"></param>
        /// <returns></returns>
        public bool UpdatePrintDomain(string orderId,string printCode,string domain)
        {
            int result = 0;
            McpEquipmentInfo ep = new McpEquipmentInfo();
            ep.ModifiedOn = CommonUtil.GetDBDateTime();
            ConditionCollection cd = new ConditionCollection
            {
                new Condition("equipment_code",printCode)
            };
            ep.Domain =domain;
            result = this.GetRepository<McpEquipmentInfo>().Update(ep, cd, "domain,modified_on");
            if (result <= 0)
            {
                LogUtil.Info($"修改打印机{printCode}域名domin失败失败,订单号：{orderId}");
                LogUtil.Error($"修改打印机{printCode}域名domin失败,订单号：{orderId}");
                return false;
            }
            else
            {
                LogUtil.Info($"修改打印机{printCode}域名domin成功,订单号：{orderId}");
                return true;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取有效打印设备列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetPrinterList(string printerCodes, string appId, string merchantCode)
        {
            var prtlist = new List<string>();
            var equipList = this.GetRepository<McpEquipmentInfo>().ListModel();
            var prtinters = printerCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var isAssApp = ConfigUtil.AssociatedAppIds.IndexOf(appId) >= 0;
            var isOK = false;
            for (int i = 0; i < prtinters.Length; i++)
            {
                var codes = prtinters[i].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                if (codes.Length >= 2)
                {
                    var printerCode = codes[0].Trim();
                    var checkCode = codes[1].Trim();
                    var printer = equipList.Find(d => d.EquipmentCode.Equals(printerCode, StringComparison.CurrentCultureIgnoreCase));
                    if (printer != null)
                    {
                        var appPrinters = GetMerchantPrinter(printerCode);
                        if (appPrinters != null && appPrinters.Count > 0)
                        {
                            if (isAssApp)
                            {
                                //打印设备已关联，且当前应用为关联应用
                                var appPrinter = appPrinters.Find(d => d.AppId.Equals(appId, StringComparison.CurrentCultureIgnoreCase));
                                if (appPrinter != null)
                                {
                                    //已存在当前应用的关联信息
                                    if(merchantCode.Equals(appPrinter.MerchantCode, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        throw new MessageException(string.Format("您已绑定打印机'{0}'，若需重新绑定，请解绑后再操作！", printerCode));
                                    }
                                    else
                                    {
                                        throw new MessageException(string.Format("打印机'{0}'已绑定到别的账号，请联系客服！", printerCode));
                                    }
                                }
                                else
                                {
                                    appPrinter = appPrinters.FirstOrDefault();
                                    if (appPrinter != null && ConfigUtil.AssociatedAppIds.IndexOf(appPrinter.AppId) <= -1)
                                    {
                                        //已关联的为其他非关联应用
                                        throw new MessageException(string.Format("打印机'{0}'已绑定到别的账号，请联系客服！", printerCode));
                                    }
                                    else
                                    {
                                        isOK = true;
                                    }
                                }
                            }
                            else
                            {
                                //打印设备已关联，且当前应用为非关联应用
                                throw new MessageException(string.Format("打印机'{0}'已绑定到别的账号，请联系客服！", printerCode));
                            }
                        }
                        else
                        {
                            isOK = true;
                        }

                        if (isOK && printer.CheckCode.Equals(checkCode, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!prtlist.Contains(printer.EquipmentCode))
                            {
                                prtlist.Add(printer.EquipmentCode);
                            }
                            else
                            {
                                throw new MessageException(string.Format("打印机'{0}'不可绑定多次！", printerCode));
                            }
                        }
                        else
                        {
                            throw new MessageException(string.Format("打印机'{0}'校验码错误！", printerCode));
                        }
                    }
                    else
                    {
                        throw new MessageException(string.Format("打印机'{0}'不存在！", printerCode));
                    }
                }
                else
                {
                    if (codes.Length == 0)
                    {
                        throw new MessageException("制造编号不能为空！");
                    }
                    else if (codes.Length == 1)
                    {
                        throw new MessageException(string.Format("打印机'{0}'校验码不能为空！", codes[0].Trim()));
                    }
                }
            }

            return prtlist.Count == prtinters.Length ? prtlist : null;
        }

        /// <summary>
        /// 获取解绑待清理打印任务列表
        /// </summary>
        /// <param name="pCodes">打印机设备ID串</param>
        /// <param name="appId">打印机解绑应用ID</param>
        /// <returns></returns>
        private List<McpOrderPrinterInfo> GetWaitClearPrintTasks(string pCodes, string appId)
        {
            var pc = new ParameterCollection();
            pc.Add("app_id", appId);
            var sql = string.Format("SELECT * FROM mcp_order_printer WHERE printer_code IN ({0}) AND order_status = 0 AND order_id IN (SELECT order_id FROM mcp_order WHERE app_id =@app_id AND TIMESTAMPDIFF(HOUR,order_date,NOW())<=2); ", pCodes);
            return this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql, pc);
        }

        /// <summary>
        /// 清理待解绑打印机任务
        /// </summary>
        /// <param name="taskList">打印任务列表</param>
        private void ClearPrintTasksCallBack(List<McpOrderPrinterInfo> taskList)
        {
            if (taskList != null)
            {
                foreach (var task in taskList)
                {
                    Action ac = new Action(() =>
                    {
                        SingleInstance<AppService>.Instance.AppCallBack(PrintOrderStatusType.UnbundlingClear.GetHashCode(), task.OrderId, task.PrinterCode);
                    });
                    ac.BeginInvoke(null, null);
                }
            }
        }

        public void GetPrintConnectStatus(PrtintViewQueryModel queryModel)
        {
            var device = SingleInstance<PrinterService>.Instance.GetPrinter(queryModel.printer_codes);
            if (device == null||device.ConnectionId==0)
            {
                 
            }
            else
            {
                
            }

        }

        /// <summary>
        /// 查询打印设备状态
        /// </summary>
        /// <param name="verModel"></param>
        /// <returns></returns>
        public int GetPrintStatus(PrtintViewQueryModel queryModel)
        {
            if (string.IsNullOrEmpty(queryModel.printer_codes))
            {
                throw new MessageException("设备编号不能为空");
            }
            var printerStatue= SingleInstance<PrinterService>.Instance.GetPrinterStatusCode(queryModel.printer_codes);
            if (printerStatue!= PrintStatusType.Normal.GetHashCode())
            {
                return printerStatue;
            }
            var device = SingleInstance<PrinterService>.Instance.GetPrinter(queryModel.printer_codes);
            //如果打印机掉线，返回掉线状态4，没有掉线则继续往下判断是否缺纸等
            if (device == null || device.ConnectionId == 0)
            {
                return PrinterFaultType.NetworkFault.GetHashCode();
            }
            else
            {
                return PrinterFaultType.Enable.GetHashCode();
            }
        }

        /// <summary>
        /// 查看打印机版本
        /// </summary>
        /// <param name="QueryModel"></param>
        public object CheckSoftwareVersion(PrtintViewQueryModel queryModel)
        {
            //获取打印机信息
            var printerInfos= GetPrintInfos(queryModel);

            string sqlver = "SELECT * FROM `mcp_sys_version` ";
            var version = this.GetRepository<McpSysVersionInfo>().ListModelBySql(sqlver);
            var isVer = ConfigUtil.GetConfig("IsVerOrfirmware");
            var result = printerInfos.Select(x=> {
                var newVerinfo = version.Where(y => y.ObjecType == x.EquipmentType.ToString()).FirstOrDefault();
                var newVer = newVerinfo==null?"":newVerinfo.ObjectVersion;
                var newFirmwareInfo = version.Where(y => y.ObjecType == isVer + x.EquipmentType.ToString()).FirstOrDefault();
                var newFirmware= newFirmwareInfo==null?"":newFirmwareInfo.ObjectVersion;
                return new { printercodes = x.EquipmentCode, varversion = x.Var_version, firmwareversion = x.Firmware_version, lastvarversion=newVer, lastFirmwareVersion=newFirmware };
            });
            return result;
        }

        /// <summary>
        /// 检查打印机域名情况
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public object CheckDomain(PrtintViewQueryModel queryModel)
        {
            //获取打印机信息
            var printerInfos = GetPrintInfos(queryModel);
            var result = printerInfos.Select(x => {
                return new { printercodes = x.EquipmentCode, domain =x.Domain, newdomain =x.new_Domain};
            });
            return result;
        }

        /// <summary>
        /// 获取打印机基本信息
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public List<McpEquipmentInfo> GetPrintInfos(PrtintViewQueryModel queryModel)
        {
            //校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(queryModel.app_id);
            if (!appIsOk)
            {
                throw new MessageException($"无效应用:{queryModel.app_id}");
            }

            //校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(queryModel.app_id.Trim(), queryModel.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException($"无效访问令牌:{queryModel.access_token}");
            }

            string[] codes = queryModel.printer_codes.Trim().ToUpper().Replace("，", ",").Trim(',').Split(',');
            StringBuilder codesstr = new StringBuilder();
            foreach (var item in codes)
            {
                codesstr.Append($",'{item}'");
            }            
            string sqlCodes = $"SELECT * FROM `mcp_equipment` WHERE equipment_code IN ({codesstr.ToString().Trim(',').Replace("-", "")})";
            var printerInfos = this.GetRepository<McpEquipmentInfo>().ListModelBySql(sqlCodes);
            if (printerInfos == null || printerInfos.Count <= 0)
            {
                LogUtil.Info($"查询设备版本号失败：没有找到打印机{queryModel.printer_codes}");
                throw new MessageException($"没有找到打印机{queryModel.printer_codes}");
            }
            return printerInfos;
        }

        /// <summary>
        /// 修改打印机域名
        /// </summary>
        /// <param name="QueryModel"></param>
        /// <returns></returns>
        public object RenameDomain(PrtintViewQueryModel queryModel)
        {
            //验证域名
            if (string.IsNullOrEmpty(queryModel.domain))
            {
                throw new MessageException($"修改域名失败，域名不能为空");
            }

            //验证商户订单号
            if (string.IsNullOrEmpty(queryModel.bill_no))
            {
                throw new MessageException($"修改域名失败，任务号bill_no不能为空");
            }

            //验证打印机列表:也是这里判断操作全部还是部分
            List<string> printList = GetPrintList(queryModel);            
            if (printList.Count <= 0)
            {
                throw new MessageException("打印机编号不能为空，多个请使用英文逗号隔开");
            }
            queryModel.printer_codes = queryModel.printer_codes.ToUpper();

            //校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(queryModel.app_id);
            if (!appIsOk)
            {
                throw new MessageException($"修改域名{queryModel.domain}失败，无效应用:{queryModel.app_id}");
            }

            //校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(queryModel.app_id.Trim(), queryModel.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException($"修改域名{queryModel.domain}失败，无效访问令牌:{queryModel.access_token}");
            }

            //区分出已经联网的打印机和未联网的打印机
            List<string> conPrintCodes = GetPrintCodeIsConnected(printList);
            List<string> noconPrintCodes = printList.Select(x => x).Except(conPrintCodes).ToList();

            //创建任务
            queryModel.bill_type = PrintCmdStatue.RenameDomain.GetHashCode();
            queryModel.order_id = GetNewPrintOrderId(queryModel.app_id.Trim());
            var order=CreatCmdOrder(queryModel, conPrintCodes);

            //创建任务订单
            CreatCmdPrintOrder(conPrintCodes, queryModel.order_id);

            //将联网的打印机和未联网的打印机拼接成字符串返回
            string conPrintCodesStr = string.Join(",", conPrintCodes.ToArray());
            string noconPrintCodesStr = string.Join(",", noconPrintCodes.ToArray());

            //修改打印机域名
            UpdatePrintsNewDomin(conPrintCodesStr,queryModel.domain);

            var result = new { success = new { count = conPrintCodes.Count, printcodes = conPrintCodesStr }, fail = new { count = noconPrintCodes.Count, printcodes = noconPrintCodesStr } };

            //发送TCP消息至设备
            SendTcpMsgAssCommand(order.OrderId, conPrintCodesStr,queryModel);
            return result;
        }

        /// <summary>
        /// 发送tcp命令至打印机
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="printerCodes"></param>
        /// <param name="queryModel"></param>
        public void SendTcpMsgAssCommand(string orderId,string printerCodes,PrtintViewQueryModel queryModel)
        {
            int tryNum = 1;
            try
            {
                SingleInstance<PrintOrderService>.Instance.SendTcpMsg(orderId, printerCodes, tryNum, TcpOperateType.AssCommand);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("发送打印机辅助指令失败,请求参数：{0},错误信息：{1}", JsonConvert.SerializeObject(queryModel), ex.Message));

                System.Threading.Thread.Sleep(delayTryInterval);
                if (tryNum <= tryMax)
                {
                    SingleInstance<PrintOrderService>.Instance.SendTcpMsg(orderId, printerCodes, ++tryNum, TcpOperateType.AssCommand);
                }
                else
                {
                    Action ac = new Action(() =>
                    {
                        //SingleInstance<AppService>.Instance.AppCallBack(ResultCodeType.NetworkFault.GetHashCode(), orderId, bill.printer_codes);
                    });
                    ac.BeginInvoke(null, null);

                    throw new MessageException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 重启打印机
        /// </summary>
        /// <param name="QueryModel"></param>
        /// <returns></returns>
        public object RestartPrinter(PrtintViewQueryModel queryModel)
        {
            //验证商户订单号
            if (string.IsNullOrEmpty(queryModel.bill_no))
            {
                throw new MessageException($"重启失败，任务号bill_no不能为空");
            }

            //验证打印机列表:也是这里判断操作全部还是部分
            List<string> printList = GetPrintList(queryModel);
            if (printList.Count <= 0)
            {
                throw new MessageException("打印机编号不能为空，多个请使用英文逗号隔开");
            }
            queryModel.printer_codes = queryModel.printer_codes.ToUpper();

            //校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(queryModel.app_id);
            if (!appIsOk)
            {
                throw new MessageException($"重启失败，无效应用:{queryModel.app_id}");
            }

            //校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(queryModel.app_id.Trim(), queryModel.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException($"重启失败，无效访问令牌:{queryModel.access_token}");
            }

            //区分出已经联网的打印机和未联网的打印机
            List<string> conPrintCodes = GetPrintCodeIsConnected(printList);
            List<string> noconPrintCodes = printList.Select(x => x).Except(conPrintCodes).ToList();

            //创建任务
            queryModel.bill_type = PrintCmdStatue.Restart.GetHashCode();
            queryModel.order_id = GetNewPrintOrderId(queryModel.app_id.Trim());
            var order = CreatCmdOrder(queryModel, conPrintCodes);

            //创建任务订单
            CreatCmdPrintOrder(conPrintCodes, order.OrderId);

            //将联网的打印机和未联网的打印机拼接成字符串返回
            string conPrintCodesStr = string.Join(",", conPrintCodes.ToArray());
            string noconPrintCodesStr = string.Join(",", noconPrintCodes.ToArray());
            var result = new { success = new { count = conPrintCodes.Count, printcodes = conPrintCodesStr }, fail = new { count = noconPrintCodes.Count, printcodes = noconPrintCodesStr } };
            //发送TCP消息至设备
            SendTcpMsgAssCommand(order.OrderId, conPrintCodesStr, queryModel);
            return result;
        }

        /// <summary>
        /// 上传日志
        /// </summary>
        /// <param name="QueryModel"></param>
        /// <returns></returns>
        public object UploadLog(PrtintViewQueryModel queryModel)
        {
            //验证商户订单号
            if (string.IsNullOrEmpty(queryModel.bill_no))
            {
                throw new MessageException($"任务号bill_no不能为空");
            }

            //验证打印机列表:也是这里判断操作全部还是部分
            List<string> printList = GetPrintList(queryModel);
            if (printList.Count <= 0)
            {
                throw new MessageException("打印机编号不能为空，多个请使用英文逗号隔开");
            }

            //校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(queryModel.app_id);
            if (!appIsOk)
            {
                throw new MessageException($"无效应用:{queryModel.app_id}");
            }

            //校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(queryModel.app_id.Trim(), queryModel.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException($"无效访问令牌:{queryModel.access_token}");
            }

            //区分出已经联网的打印机和未联网的打印机
            List<string> conPrintCodes = GetPrintCodeIsConnected(printList);
            List<string> noconPrintCodes = printList.Select(x => x).Except(conPrintCodes).ToList();

            //创建任务
            queryModel.bill_type = PrintCmdStatue.UploadLog.GetHashCode();
            var order = CreatCmdOrder(queryModel, conPrintCodes);            
            //创建任务订单
            CreatCmdPrintOrder(conPrintCodes, order.OrderId);

            //将联网的打印机和未联网的打印机拼接成字符串返回
            string conPrintCodesStr = string.Join(",", conPrintCodes.ToArray());
            string noconPrintCodesStr = string.Join(",", noconPrintCodes.ToArray());
            var result = new { success = new { count = conPrintCodes.Count, printcodes = conPrintCodesStr }, fail = new { count = noconPrintCodes.Count, printcodes = noconPrintCodesStr } };
            //发送TCP消息至设备
            SendTcpMsgAssCommand(order.OrderId, conPrintCodesStr, queryModel);
            return result;
        }

        /// <summary>
        /// 区分多台打印机，会返回两个集合，第一个集合包含所有联网的打印机，第二个集合包含所有未联网的打印机
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> GetPrintCodeIsConnected(List<string> list)
        {
            StringBuilder codesBuil =new StringBuilder();
            foreach (var item in list)
            {
                codesBuil.Append($",'{item}'");
            }
            string codes = codesBuil.ToString().Trim(',');
            string sql = $"SELECT * FROM `mcp_printer` WHERE printer_code IN ({codes}) AND CONNECTION_ID!=0 AND CONNECTION_ID IS NOT NULL AND CONNECTION_ID!=''";
            LogUtil.Debug($"GetPrintCodeIsConnected方法：区分打印机是否联网，sql语句：{sql}");
            List<McpPrinterInfo> listPrint=this.GetRepository<McpPrinterInfo>().ListModelBySql(sql);
            var listCon = listPrint.Select(x => x.PrinterCode).ToList();
            return listCon;
        }

        /// <summary>
        /// 查询该打印机是否联网
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool IsPrintConnet(string printCode)
        {
            string sql = $"SELECT * FROM `mcp_printer` WHERE printer_code IN ('{printCode}') AND CONNECTION_ID!=0 AND CONNECTION_ID IS NOT NULL AND CONNECTION_ID!=''";
            LogUtil.Debug($"GetPrintCodeIsConnected方法：区分打印机是否联网，sql语句：{sql}");
            List<McpPrinterInfo> listPrint = this.GetRepository<McpPrinterInfo>().ListModelBySql(sql);
            if (listPrint.Count<=0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 创建命令任务详情
        /// </summary>
        /// <param name="printList"></param>
        /// <param name="orderid"></param>
        public void CreatCmdPrintOrder(List<string> printList,string orderid)
        {
            var cmdPrinterOrderList = new List<McpCmdOrderPrinterInfo>();
            foreach (var printer in printList)
            {
                cmdPrinterOrderList.Add(new McpCmdOrderPrinterInfo()
                {
                    OrderId = orderid,
                    PrinterCode = printer.ToUpper(),
                    CreatedOn = CommonUtil.GetDBDateTime(),
                    ModifiedOn = CommonUtil.GetDBDateTime(),
                    OrderStatus = PrintOrderStatusType.UnPrinted.GetHashCode(),
                    PushStatus = PrintOrderPushStatusType.UnPushed.GetHashCode()
                });
            }
            this.GetRepository<McpCmdOrderPrinterInfo>().Create(cmdPrinterOrderList);
        }

        /// <summary>
        /// 获取打印机列表
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public List<string> GetPrintList(PrtintViewQueryModel queryModel)
        {
            List<string> printList = new List<string>();
            //类型为1-修改部分，2-修改全部
            printList = queryModel.printer_codes.ToUpper().Trim().Replace("，", ",").Trim(',').Split(',').ToList();
            if (queryModel.Count_type == "1")
            {
                //printList = queryModel.printer_codes.Trim().Replace("，", ",").Trim(',').Split(',').ToList();
            }
            if (queryModel.Count_type == "2")
            {

            }
            printList=printList.Distinct().ToList();
            return printList;
        }

        /// <summary>
        /// 创建命令任务
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public McpCmdOrderInfo CreatCmdOrder(PrtintViewQueryModel queryModel, List<string> conPrintCodes)
        {
            if (conPrintCodes.Count<=0)
            {
                LogUtil.Info($"创建任务失败，打印机网络异常，任务号-{queryModel.bill_no}");
                throw new MessageException($"创建任务失败，打印机网络异常");
            }
            var orderKey = VerifyCodeUtil.Intance.GenCode(12);
            DateTime dbNow = CommonUtil.GetDBDateTime();
            var order = new McpCmdOrderInfo
            {
                OrderId = queryModel.order_id,
                AppId = queryModel.app_id.Trim(),
                BillNo = DateTime.Now.ToString("yyMMddHHmmssfff"),
                BillType = queryModel.bill_type,
                OrderContent = queryModel.domain,
                OrderDate = dbNow,
                OrderKey = orderKey,
                CallbackStatus = 0,
                ModifiedOn= dbNow
            };
            long result=this.GetRepository<McpCmdOrderInfo>().Create(order);
            if (result<=0)
            {
                LogUtil.Info($"创建任务失败,任务号{queryModel.bill_no}");
                throw new MessageException($"创建任务失败");                
            }
            else
            {
                return order;
            }
        }

        /// <summary>
        /// 获取新订单号
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        private string GetNewPrintOrderId(string appId)
        {
            if (ConfigUtil.OneNineOrderIdAppIds.IndexOf(appId) >= 0)
            {
                return string.Format("{0}{1}", DateTime.Now.ToString("yyMMddHHmmssfff"), VerifyCodeUtil.Intance.GenAllCode(4));
            }
            else
            {
                return string.Format("{0}{1}", DateTime.Now.ToString("MMdd"), CommonUtil.GetGuidNoSeparator());
            }
        }

        #region 上传打印机的日志文件

        /// <summary>
        /// 上传打印机的日志文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="sn">打印机编号</param>
        /// <param name="fileContent">文件内容</param>
        /// <returns></returns>
        public ProcessResult UploadPrinterLog(PrtintViewQueryModel queryModel)
        {
            ProcessResult result = new ProcessResult();

            try
            {
                //保存根路径
                string path = ConfigUtil.GetConfig("PrinterLogPath") + queryModel.printer_codes;

                //目录不存在，为此SN第一次创建打印日志
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = string.Format("{0}_{1}.txt", queryModel.printer_codes, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                //写入日志内容，并在[前增加换行
                File.WriteAllText(path + @"\" + fileName, queryModel.file_content.Replace("[", "\r\n["));

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Msg = "未知错误";
                LogUtil.Info($"SN：【{queryModel.printer_codes}】，错误信息：【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// 批量修改打印机新域名,2018年4月25日15:12:33  张友辉
        /// </summary>
        /// <param name="prints"></param>
        /// <param name="newDomin"></param>
        public void UpdatePrintsNewDomin(string prints,string newDomin)
        {
            string [] printsArr=prints.ToUpper().Trim().Replace("，", ",").Replace("-","").Trim(',').Split(',');
            StringBuilder buil = new StringBuilder();
            foreach (var item in printsArr)
            {
                buil.Append($",'{item}'");
            }
            string printCodes = buil.ToString().Trim(',');
            string sqlSele = $"SELECT * FROM `mcp_equipment` WHERE equipment_code IN ({printCodes})";
            var eqment = this.GetRepository<McpEquipmentInfo>().ListModelBySql(sqlSele);
            eqment.ForEach(x =>
            {
                x.new_Domain = newDomin;
            });
            this.GetRepository<McpEquipmentInfo>().Update(eqment, "new_domain");
        }

        #endregion

        #endregion
    }
}