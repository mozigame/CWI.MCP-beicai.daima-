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
using System.Transactions;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Caching;
using System.Threading;

namespace CWI.MCP.Services
{
    public class PrintOrderService : BaseService
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
        /// 获取小票地址
        /// </summary>
        private static string billUrl = string.Format(@"http://{0}/mcp/sys/getPrintBill", ConfigUtil.GetConfig("httpApiUrl"));

        #region 打印

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="bill">小票信息</param>
        public void Print(BillViewModel bill)
        {
            //1.生成打印订单
            string orderId = string.Empty;
            try
            {
                orderId = CreatePrintTasks(bill);
            }
            catch (Exception ex)
            {
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.AppCallBack(ResultCodeType.UnKnow.GetHashCode(), orderId, (bill != null ? bill.printer_codes : string.Empty));
                });
                ac.BeginInvoke(null, null);

                LogUtil.Error(string.Format("创建打印任务失败,请求参数：{0},错误信息：{1}", JsonConvert.SerializeObject(bill), ex.Message));
                throw ex;
            }

            //2.发送TCP消息至设备
            int tryNum = 1;
            try
            {
                SendTcpMsg(orderId, bill.printer_codes, tryNum);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("发送打印任务失败,请求参数：{0},错误信息：{1}", JsonConvert.SerializeObject(bill), ex.Message));

                System.Threading.Thread.Sleep(delayTryInterval);
                if (tryNum <= tryMax)
                {
                    SendTcpMsg(orderId, bill.printer_codes, ++tryNum);
                }
                else
                {
                    Action ac = new Action(() =>
                    {
                        SingleInstance<AppService>.Instance.AppCallBack(ResultCodeType.NetworkFault.GetHashCode(), orderId, bill.printer_codes);
                    });
                    ac.BeginInvoke(null, null);

                    throw new MessageException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="bill">小票信息</param>
        public void PrintDemonstration(BillViewModel bill)
        {
            //1.生成打印订单
            string orderId = string.Empty;
            try
            {
                if (bill.sign != null && bill.sign != "")
                {
                    CheckPrintSign(bill);
                }
                orderId = CreatePrintTasksDemonstration(bill);
            }
            catch (Exception ex)
            {
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.AppCallBack(ResultCodeType.UnKnow.GetHashCode(), orderId, (bill != null ? bill.printer_codes : string.Empty));
                });
                ac.BeginInvoke(null, null);

                LogUtil.Error(string.Format("创建打印任务失败,请求参数：{0},错误信息：{1}", JsonConvert.SerializeObject(bill), ex.Message));
                throw new MessageException(ex.Message);
            }

            //2.发送TCP消息至设备
            int tryNum = 1;
            try
            {
                SendTcpMsg(orderId, bill.printer_codes, tryNum);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("发送打印任务失败,请求参数：{0},错误信息：{1}", JsonConvert.SerializeObject(bill), ex.Message));

                System.Threading.Thread.Sleep(delayTryInterval);
                if (tryNum <= tryMax)
                {
                    SendTcpMsg(orderId, bill.printer_codes, ++tryNum);
                }
                else
                {
                    Action ac = new Action(() =>
                    {
                        SingleInstance<AppService>.Instance.AppCallBack(ResultCodeType.NetworkFault.GetHashCode(), orderId, bill.printer_codes);
                    });
                    ac.BeginInvoke(null, null);

                    throw new MessageException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 打印小票返回打印订单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object TcpPrint(TcpQueryModel query, ObjectCache cache, out McpPrinterInfo deviceConn)
        {
            //2018年5月4日11:21:37  原本检索超时放在这里，但是判断了打印机状态正常才会走到这一步，所以要把该操作提前到判断打印机状态之前
            
            if (string.IsNullOrWhiteSpace(query.Id))
            {
                query.Tp = TcpOperateType.Print.GetHashCode();
                //要做防重打功能，比如北彩服务器，过滤已经推送的任务；不是，则不过滤
                var NotRePrintServiceType = ConfigUtil.GetConfig("NotRePrintServiceType");
                var ServiceType = ConfigUtil.GetConfig("ServiceType");
                //bool isfilter = NotRePrintServiceType.Contains(ServiceType) ? true : false;
                bool isfilter = false;
                object obj = QueryPrintTask(query, cache, out deviceConn, isfilter);
                return obj;
            }
            else
            {
                return SendPrintTask(query, cache, out deviceConn);
            }
        }
        

        /// <summary>
        /// 生成打印任务命令
        /// </summary>
        /// <param name="query"></param>
        /// <param name="deviceConn"></param>m>
        /// <returns></returns>
        private object GenPrintTaskCmd(int tp, McpPrinterInfo deviceConn)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("tp", tp);
            dic.Add("err", 0);
            dic.Add("id", deviceConn.OrderId);
            dic.Add("key", deviceConn.OrderKey);
            dic.Add("copies", deviceConn.Copies);
            dic.Add("type", deviceConn.BillType);
            //2018年4月16日16:22:12  原本直接推打印内容  现在修改成按原方式  http下载
            //2018年4月20日14:05:10  打印内容为html链接或者图片链接时-直接将链接赋值给ordercontent参数，打印内容为，url参数保持不变
            if (deviceConn.BillType == BillType.Html.GetHashCode() || deviceConn.BillType == BillType.HtmPict.GetHashCode())
            {
                dic.Add("ordercontent", deviceConn.OrderContent);
            }
            else
            {
                dic.Add("ordercontent", billUrl);
            }
            dic.Add("url", billUrl);
            LogUtil.Info(string.Format("生成打印任务命令:{0}", deviceConn.CacheKey));

            return dic;
        }      

        /// <summary>
        /// 【102】发送打印任务
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cache"></param>
        /// <param name="deviceConn"></param>
        /// <returns></returns>
        public object SendPrintTask(TcpQueryModel query, ObjectCache cache, out McpPrinterInfo deviceConn)
        {
            LogUtil.Info(string.Format("【102】设备号：{0},本次新推打印任务号：{1}", query.Did, query.Id));

            deviceConn = new McpPrinterInfo();
            McpOrderInfo order = null;
            McpOrderPrinterInfo oldOrder = null;
            var newOrder = GetPrintOrder(query.Id);
            if (newOrder != null)
            {
                var device = SingleInstance<PrinterService>.Instance.GetPrinter(query.Did);
                if (device != null)
                {
                    //校验打印设备状态是否OK
                    oldOrder = null;
                    device.CacheKey = string.Empty;
                    if (device.ConnectionId == 0)
                    {
                        LogUtil.Info(string.Format("【102】设备号：{0}, 长连接已断开，connection_id为0。", device.PrinterCode));
                        LogUtil.Info(string.Format("【102】发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},状态: {2}", query.Did, query.Id, PrintOrderStatusType.OutConnection.GetHashCode()));
                        TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did, Code = PrintOrderStatusType.OutConnection.GetHashCode() };
                        SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);

                        return TcpConsts.PrinterDisconneted;
                    }

                    int statusCode = SingleInstance<PrinterService>.Instance.GetPrinterStatusCode(device.PrinterCode);
                    //if (statusCode == ResultCodeType.Success.GetHashCode())
                    //判断打印机状态
                    if (statusCode == PrintStatusType.Normal.GetHashCode())
                    {
                        //校验前面是否有未打印的订单
                        oldOrder = CheckIsExistUnPrintOrder(device.PrinterCode, newOrder.OrderId);
                        if (oldOrder != null)
                        {
                            //1.前面存在未打印任务暂不打印加入待打印队列
                            LogUtil.Info(string.Format("设备号：{0}, 打印队列中存在未打印任务,本次打印任务：{1}加入打印队列,等候下发打印命令。", device.PrinterCode, newOrder.OrderId));

                            //2.打印队列前面有任务未打印，触发再次下发打印命令
                            device.CacheKey = string.Format("{0}-{1}-{2}", oldOrder.PrinterCode, oldOrder.OrderId, device.ConnectionId.ToString());
                            //不能通过数据库的推送状态来判断任务是否已经推送，因为除了北彩以外，防丢单功能就算推送过了，但是打印失败，这里还是要推送
                            if (cache == null || !cache.Contains(device.CacheKey))
                            {
                                order = null;
                                order = GetPrintOrder(oldOrder.OrderId);
                                device.OrderId = order.OrderId;
                                device.OrderKey = order.OrderKey;
                                device.Copies = order.Copies;
                                device.BillType = order.BillType;
                                device.OrderContent = order.OrderContent;
                                deviceConn = device;
                                LogUtil.Info(string.Format("设备号：{0}, 打印队列前面存在未打印任务,触发添加实际待打印命令：{1}。", oldOrder.PrinterCode, device.CacheKey));

                                //生成打印任务命令
                                return GenPrintTaskCmd(query.Tp, deviceConn);
                            }
                            else
                            {
                                LogUtil.Info(string.Format("设备号：{0}, 打印队列前面存在未打印任务,但前面待打印任务：{1} 已推送,本次不再推送。", oldOrder.PrinterCode, oldOrder.OrderId));

                                //打印设备前面有未打印任务但已推送
                                return TcpConsts.PrinterHasPreTaskAndSend;
                            }
                        }
                        else
                        {
                            //添加发送打印命令的打印设备
                            device.CacheKey = string.Format("{0}-{1}-{2}", device.PrinterCode, newOrder.OrderId, device.ConnectionId.ToString());
                            device.OrderId = newOrder.OrderId;
                            device.OrderKey = newOrder.OrderKey;
                            device.Copies = newOrder.Copies;
                            device.BillType = newOrder.BillType;
                            device.OrderContent = order.OrderContent;
                            deviceConn = device;
                            LogUtil.Info(string.Format("设备号：{0},添加实际待发送命令打印任务：{1}", device.PrinterCode, device.CacheKey));

                            //生成打印任务命令
                            return GenPrintTaskCmd(query.Tp, deviceConn);
                        }
                    }
                    else
                    {
                        LogUtil.Info(string.Format("设备号：{0}, 状态码：{1}【异常】,本次打印任务：{2}加入打印队列。", device.PrinterCode, statusCode, newOrder.OrderId));
                        TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did };
                        newQue.Code = GetOrderStatusByPrintStatus(statusCode);
                        LogUtil.Info(string.Format("【102】发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},设置订单状态: {2},设备状态{3}", query.Did, query.Id, newQue.Code, statusCode));
                        SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);

                        //打印设备状态异常
                        return TcpConsts.PrinterStatusException;
                    }
                }
                else
                {
                    LogUtil.Info(string.Format("【102】设备号：{0},执行打印任务号:{1},通讯超时异常，没有联网信息", query.Did, query.Id));
                    LogUtil.Info(string.Format("【102】发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},状态: {2}", query.Did, query.Id, PrintOrderStatusType.NoConnect.GetHashCode()));
                    TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did, Code = PrintOrderStatusType.NoConnect.GetHashCode() };
                    SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);

                    //打印设备不存在
                    return TcpConsts.PrinterNotFind;
                }
            }
            else
            {
                //打印任务不存在
                return TcpConsts.PrintOrderNotFind;
            }
        }

        /// <summary>
        /// 【105】查询打印任务
        /// </summary>
        /// <param name="deviceConn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public object QueryPrintTask(TcpQueryModel query, ObjectCache cache, out McpPrinterInfo deviceConn, bool isFliter = true)
        {
            LogUtil.Info(string.Format("【100-105-{0}】设备号：{1},查询打印任务。", query.Tp, query.Did));

            McpOrderInfo order = null;
            deviceConn = new McpPrinterInfo();
            var statusCode = ResultCodeType.Success.GetHashCode();
            //查询下一个未打印订单
            var orderPrinter = GetNextUnPrintOrder(query.Did);
            if (orderPrinter != null)
            {
                LogUtil.Info($"查询出未打任务{orderPrinter.OrderId}");
                var device = SingleInstance<PrinterService>.Instance.GetPrinter(query.Did);
                if (device != null)
                {
                    if (device.ConnectionId == 0)
                    {
                        LogUtil.Info(string.Format("设备号：{0}, 长连接已断开，connection_id为0。", device.PrinterCode));
                        LogUtil.Info(string.Format("发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},状态: {2}", query.Did, query.Id, PrintOrderStatusType.OutConnection.GetHashCode()));
                        TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did, Code = PrintOrderStatusType.OutConnection.GetHashCode() };
                        SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);
                        return TcpConsts.PrinterDisconneted;
                    }
                    statusCode = SingleInstance<PrinterService>.Instance.GetPrinterStatusCode(device.PrinterCode);
                    order = GetPrintOrder(orderPrinter.OrderId);
                    if (statusCode == PrintStatusType.Normal.GetHashCode())
                    {
                        device.CacheKey = string.Format("{0}-{1}-{2}", orderPrinter.PrinterCode, orderPrinter.OrderId, device.ConnectionId.ToString());
                        if (isFliter)
                        {
                            //不能通过数据库的推送状态来判断任务是否已经推送，因为除了北彩以外，防丢单功能就算推送过了，但是打印失败，这里还是要推送
                            if (cache == null || !cache.Contains(device.CacheKey))
                            {
                                device.OrderId = order.OrderId;
                                device.OrderKey = order.OrderKey;
                                device.Copies = order.Copies;
                                device.BillType = order.BillType;
                                device.OrderContent = order.OrderContent;
                                deviceConn = device;
                                LogUtil.Info(string.Format("设备号：{0}, 打印队列前面存在未打印任务,触发添加实际待打印命令：{1}。", orderPrinter.PrinterCode, device.CacheKey));

                                //生成打印任务命令
                                return GenPrintTaskCmd(query.Tp, deviceConn);
                            }
                            else
                            {
                                LogUtil.Info(string.Format("设备号：{0}, 待打印任务：{1} 已推送,本次不再推送。", orderPrinter.PrinterCode, orderPrinter.OrderId));
                                LogUtil.Info($"检测到打印设备前面有未打印任务但已推送，本次不再推送，打印队列前面存在未打印任务，出发添加实际打印实际命令，{1}");
                                //打印设备前面有未打印任务但已推送
                                return TcpConsts.PrinteTaskSended;
                            }
                        }
                        else
                        {
                            //不过滤重复直接添加发送
                            device.OrderId = order.OrderId;
                            device.OrderKey = order.OrderKey;
                            device.Copies = order.Copies;
                            device.BillType = order.BillType;
                            device.OrderContent = order.OrderContent;
                            deviceConn = device;
                            LogUtil.Info(string.Format("设备号：{0}, 打印队列前面存在未打印任务,触发添加实际待打印命令：{1}。", orderPrinter.PrinterCode, device.CacheKey));

                            //生成打印任务命令
                            return GenPrintTaskCmd(query.Tp, deviceConn);
                        }
                    }
                    else
                    {
                        LogUtil.Info(string.Format("设备号：{0}, 状态码：{1}【异常】。", device.PrinterCode, statusCode));
                        TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did };
                        newQue.Code = GetOrderStatusByPrintStatus(statusCode);
                        //if (statusCode == PrintStatusType.Fault.GetHashCode())
                        //{
                        //    newQue.Code = PrintOrderStatusType.OpenCoverPushed.GetHashCode();
                        //}
                        //else if (statusCode == PrintStatusType.OutPaper.GetHashCode())
                        //{
                        //    newQue.Code = PrintOrderStatusType.OutPaperPushed.GetHashCode();
                        //}
                        //else if (statusCode == PrintStatusType.NoFind.GetHashCode())
                        //{
                        //    newQue.Code = PrintOrderStatusType.NoFindPrint.GetHashCode();
                        //}
                        //else
                        //{
                        //    newQue.Code = PrintOrderStatusType.Other.GetHashCode();
                        //}
                        LogUtil.Info(string.Format("发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},设置订单状态: {2},设备状态{3}", query.Did, query.Id, newQue.Code, statusCode));
                        SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);

                        //打印设备状态异常
                        return TcpConsts.PrinterStatusException;
                    }
                }
                else
                {
                    LogUtil.Info(string.Format("设备号：{0}, 通讯超时异常，没有联网信息。", device.PrinterCode));
                    LogUtil.Info(string.Format("发送打印任务时，更新订单打印状态请求，设备号：{0},订单号：{1},状态: {2}", query.Did, query.Id, PrintOrderStatusType.NoConnect.GetHashCode()));
                    TcpQueryModel newQue = new TcpQueryModel { Id = query.Id, Did = query.Did, Code = PrintOrderStatusType.NoConnect.GetHashCode() };
                    SingleInstance<PrintOrderService>.Instance.UpdateStatus(newQue);
                    return TcpConsts.PrinterNotFind;
                }
            }
            else
            {
                //打印任务不存在
                LogUtil.Info(string.Format("设备号：{0}, 暂无打印任务。", query.Did));

                return 0;
            }
        }

        /// <summary>
        /// 根据打印机的状态判断打印任务的状态
        /// </summary>
        /// <param name="printStatusType"></param>
        /// <returns></returns>
        public int GetOrderStatusByPrintStatus(int printStatusType)
        {
            int orderStatus;
            if (printStatusType == PrintStatusType.Fault.GetHashCode())
            {
                orderStatus = PrintOrderStatusType.OpenCoverPushed.GetHashCode();
            }
            else if (printStatusType == PrintStatusType.OutPaper.GetHashCode())
            {
                orderStatus = PrintOrderStatusType.OutPaperPushed.GetHashCode();
            }
            else if (printStatusType == PrintStatusType.NoFind.GetHashCode())
            {
                orderStatus = PrintOrderStatusType.NoFindPrint.GetHashCode();
            }
            else
            {
                orderStatus = PrintOrderStatusType.Other.GetHashCode();
            }
            return orderStatus;
        }

        /// <summary>
        /// 获取打印小票
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderKey">订单Key</param>
        /// <returns>打印小票</returns>
        public object GetPrintBill(GetBillViewModel getModel)
        {
            if (string.IsNullOrWhiteSpace(getModel.id.Trim()))
            {
                return TcpConsts.PrintOrderNotFind;
            }
            if (string.IsNullOrWhiteSpace(getModel.key.Trim()))
            {
                return TcpConsts.OrderKeyNotFind;
            }

            var order = GetPrintOrder(getModel.id.Trim(), getModel.key.Trim());
            if (order != null)
            {
                object result = null;
                UTF8Encoding encode = new UTF8Encoding();
                if (getModel.ptype.Equals("1"))  //打印机传过来需要加密
                {
                    order.OrderContent = RC4CryptoUtil.EncryptRC4(order.OrderContent.Trim(), Consts.RC4_KEY);
                }
                byte[] datas = encode.GetBytes(order.OrderContent);
                var data = encode.GetString(datas);
                if (order.BillType == BillType.Json.GetHashCode() ||
                    order.BillType == BillType.Template.GetHashCode() ||
                    order.BillType == BillType.Express.GetHashCode())
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                }
                else
                {
                    result = data;
                }

                return result;
            }
            else
            {
                return TcpConsts.PrintOrderNotFind;
            }
        }

        /// <summary>
        /// 获取打印小票
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderKey">订单Key</param>
        /// <returns>打印小票</returns>
        public object GetPrintBillXml(string orderId, string orderKey)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return TcpConsts.PrintOrderNotFind;
            }
            if (string.IsNullOrWhiteSpace(orderKey))
            {
                return TcpConsts.OrderKeyNotFind;
            }

            var order = GetPrintOrder(orderId, orderKey);
            if (order != null)
            {
                return order.OrderContent;
            }
            else
            {
                return TcpConsts.PrintOrderNotFind;
            }
        }

        /// <summary>
        /// 更新打印状态
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object UpdateStatus(TcpQueryModel query)
        {

            var task = GetOrderPrinter(query.Id, query.Did);
            if (task != null)
            {
                task.OrderStatus = query.Code;
                task.ModifiedOn = CommonUtil.GetDBDateTime();
                int cnt = this.GetRepository<McpOrderPrinterInfo>().Update(task, "order_status,modified_on");
                LogUtil.Info(string.Format("执行更新订单打印状态，设备号：{0},订单号：{1},状态: {2}", query.Did, query.Id, query.Code));

                //执行打印任务状态回调通知
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.AppCallBack(query.Code, query.Id, query.Did);
                });
                ac.BeginInvoke(null, null);
                if (query.Code == 1)
                {

                    //更新成功时判断是否还有因异常而未打印任务
                    SingleInstance<PrintOrderService>.Instance.SendUnPrintOrder(query.Did, "更新打印任务状态时");
                    return 0;
                }
                else
                {
                    LogUtil.Info(string.Format("设备号：{0},打印状态: {1},打印任务号：{2}打印失败", query.Did, query.Code, query.Id));
                    return cnt > 0 ? 0 : TcpConsts.UpStatusFailed;
                }
            }
            else
            {
                return TcpConsts.PrintOrderNotFind;
            }
        }

        /// <summary>
        /// 获取打印设备异常时段待打印的订单
        /// </summary>
        /// <param name="printerCode">打印设备地址</param>
        /// <param name="beginTime">异常开始时间</param>
        /// <param name="endTime">异常结束时间</param>
        /// <returns></returns>
        public List<McpOrderPrinterInfo> GetUnPrintOrders(string printerCode, DateTime beginTime, DateTime endTime)
        {
            ConditionCollection c1 = new ConditionCollection();
            c1.Add(new Condition("printer_code", TryConvertUtil.ToString(printerCode)));
            c1.Add(new Condition("order_status", 0));
            c1.Add(new Condition("created_on", "btime", beginTime.AddSeconds(-30), OperationType.GreaterThanOrEqual));
            c1.Add(new Condition("created_on", "etime", endTime.AddSeconds(30), OperationType.LessThanOrEqual));
            OrderBy order = new OrderBy();
            order.FieldName = "order_printer_id";
            order.SortType = SortTypeEnum.Asc;
            return this.GetRepository<McpOrderPrinterInfo>().ListModel(c1, "*", order);
        }

        /// <summary>
        /// 获取本打印设备未打印的下一待打印订单
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public McpOrderPrinterInfo GetNextUnPrintOrder(string printerCode)
        {
            string checkPushed = string.Empty;
            string orderStatus = "0";
            var NotRePrintServiceType = ConfigUtil.GetConfig("NotRePrintServiceType");
            var ServiceType = ConfigUtil.GetConfig("ServiceType");
            if (NotRePrintServiceType.Contains(ServiceType))
            {
                orderStatus = ConfigUtil.GetConfig("NotRePrintOrderStatus");
                //checkPushed = " AND push_status="+ PrintOrderPushStatusType.UnPushed.GetHashCode()+ " ";
            }
            else
            {
                orderStatus = ConfigUtil.GetConfig("RePrintOrderStatus");
            }
            McpOrderPrinterInfo order = null;
            string sql = "select * from mcp_order_printer where printer_code=$printer_code$ and order_status in (" + orderStatus + ") " + checkPushed + " order by order_printer_id limit 1";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("printer_code", TryConvertUtil.ToString(printerCode));
            //pc.Add("order_status", 0);
            var orders = this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql, pc);
            //order = orders != null ? orders.Find(d => d.OrderStatus == 0) : null;
            order = orders != null ? orders.FirstOrDefault() : null;

            LogUtil.Info(string.Format("设备号：{0},获取下一待打印任务号：{1}", printerCode, order != null ? order.OrderId : string.Empty));

            return order;
        }

        /// <summary>
        /// 校验是前面是否有未打印的订单
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public McpOrderPrinterInfo CheckIsExistUnPrintOrder(string printerCode, string orderId)
        {
            string checkPushed = string.Empty;
            string orderStatus = "0";
            var NotRePrintServiceType = ConfigUtil.GetConfig("NotRePrintServiceType");
            var ServiceType = ConfigUtil.GetConfig("ServiceType");
            if (NotRePrintServiceType.Contains(ServiceType))
            {
                orderStatus = ConfigUtil.GetConfig("NotRePrintOrderStatus");
                //checkPushed = " AND pre.push_status="+ PrintOrderPushStatusType.UnPushed.GetHashCode()+ " ";
            }
            else
            {
                orderStatus = ConfigUtil.GetConfig("RePrintOrderStatus");
            }
            McpOrderPrinterInfo order = null;
            var sql = @"SELECT 
                              pre.* 
                        FROM mcp_order_printer AS pre
                             INNER JOIN
                             (SELECT order_printer_id,printer_code FROM mcp_order_printer WHERE printer_code =@printer_code AND order_id = @order_id) AS cur
                             ON pre.printer_code = cur.printer_code
                        WHERE pre.order_status in (" + orderStatus + ") " + checkPushed + " AND pre.order_printer_id < cur.order_printer_id ORDER BY pre.order_printer_id LIMIT 1";

            var pc = new ParameterCollection();
            pc.Add("printer_code", TryConvertUtil.ToString(printerCode));
            pc.Add("order_id", TryConvertUtil.ToString(orderId));
            var orders = this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql, pc);
            //order = orders != null ? orders.Find(d => d.OrderStatus == 0) : null;
            order = orders != null ? orders.FirstOrDefault() : null;

            LogUtil.Info(string.Format("校验打印设备：{0},在本打印任务：{1} 前面未打印任务：{2}。", printerCode, orderId, (order != null ? order.OrderId : string.Empty)));

            return order;
        }

        #endregion

        /// <summary>
        /// 获取打印订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>打印订单信息</returns>
        public McpOrderInfo GetPrintOrder(string orderId, string orderKey = "")
        {
            try
            {
                ConditionCollection cc = new ConditionCollection();
                cc.Add(new Condition("order_id", TryConvertUtil.ToString(orderId)));
                if (!string.IsNullOrWhiteSpace(orderKey))
                {
                    cc.Add(new Condition("order_key", TryConvertUtil.ToString(orderKey)));
                }
                return this.GetRepository<McpOrderInfo>().GetModel(cc);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("获取打印订单错误，参考信息：{0},order_id:{1},orderKey:{2}", ex.Message, orderId, orderKey));
                return null;
            }
        }

        /// <summary>
        /// 获取打印订单
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        private McpOrderInfo GetOrder(string appId, string billNo)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("app_id", appId));
            cc.Add(new Condition("bill_no", billNo));
            return this.GetRepository<McpOrderInfo>().GetModel(cc);

        }

        /// <summary>
        /// 删除订单对饮的打印设备列表
        /// </summary>
        /// <param name="orderId"></param>
        private void DeleteOrderPrinters(string orderId)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("order_Id", TryConvertUtil.ToString(orderId)));
            //cc.Add(new Condition("order_status", PrintOrderStatusType.UnPrinted.GetHashCode()));
            this.GetRepository<McpOrderPrinterInfo>().Delete(cc);
        }

        /// <summary>
        /// 生成打印任务（订单及对应打印设备）
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        private string CreatePrintTasks(BillViewModel bill)
        {
            var orderId = string.Empty;

            //1.校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(bill.app_id);
            if (!appIsOk)
                return orderId;

            //2.校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(bill.app_id.Trim(), bill.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException("无效访问令牌！");
            }

            //3.校验打印设备
            var printCodes = GetPrinterCodes(bill);
            if (printCodes.Count <= 0)
            {
                return orderId;
            }

            //检验打印机状态：缺纸或开盖则创建任务失败
            var printstatue=SingleInstance<PrinterService>.Instance.GetPrinterStatus(bill.printer_codes);
            if (printstatue==null||printstatue.StatusCode!=PrinterFaultType.Enable.GetHashCode())
            {
                throw new MessageException("打印机故障，开盖或缺纸，或者无可用状态可用状态，请重启打印机试试，或者联系管理员");
            }

            //检验打印机联网状态，断网则不创建
            bool isConnect = SingleInstance<PrinterService>.Instance.IsPrintConnet(bill.printer_codes);
            if (!isConnect)
            {
                throw new MessageException("打印机断网或者网络不稳定！");
            }

            //检验前面是否有未完成任务
            for (int i = 0; i < printCodes.Count; i++)
            {
                var orderPrinter = GetNextUnPrintOrder(printCodes[i]);
                if (orderPrinter != null)
                {
                    LogUtil.Info($"未打订单信息-{JsonUtil.Serialize(orderPrinter)}，订单参数-{JsonUtil.Serialize(bill)}");
                    throw new InnerException($"打印机{printCodes[i]}前面存在未打印任务，创建任务失败");
                }
            }


            //4.获取票据参数
            var billParsms = GetBillParams(bill, printCodes[0]);
            int billType = billParsms.Item1;
            int billCopies = billParsms.Item2;
            var templateId = billParsms.Item3;
            var billContent = billParsms.Item4;
            if (billType == 0
                || string.IsNullOrWhiteSpace(billContent)
                || (billType == BillType.Template.GetHashCode() && string.IsNullOrWhiteSpace(templateId))
                || (billType == BillType.Express.GetHashCode() && string.IsNullOrWhiteSpace(templateId)))
            {
                return orderId;
            }

            //5.创建及更新打印订单
            var printOrder = GetOrder(bill.app_id.Trim(), bill.bill_no.Trim());
            var orderKey = VerifyCodeUtil.Intance.GenCode(12);
            DateTime dbNow = DateTime.Now;

            //5.1.新增打印订单
            var order = new McpOrderInfo()
            {
                OrderId = GetNewPrintOrderId(bill.app_id.Trim()),
                AppId = bill.app_id.Trim(),
                BillNo = bill.bill_no.Trim(),
                BillType = billType,
                OrderContent = billContent,
                OrderDate = dbNow,
                OrderKey = orderKey,
                Copies = billCopies
            };
            orderId = order.OrderId;

            #region 5.1.1 此处在创建订单前增加Html打印为图片的功能

            //Thread thread = new Thread(new ThreadStart(delegate() {
            //    //文件名
            //    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            //    //保存地址
            //    string saveUrl = $"{ConfigUtil.ImageAddress}{fileName}";

            //    //转换为图片
            //    bool isSuccess = HtmlConvertUtil.HtmlConvertImage(order.OrderContent, saveUrl);

            //    //转换成功修改订单指向的Html改为图片
            //    if (isSuccess)
            //    {
            //        LogUtil.Info($"订单号：【{order.OrderId}】的Html转换为图片，原Html为：【{order.OrderContent}】");
            //        order.OrderContent = $"{ConfigUtil.HttpAddress}{fileName}";
            //    }
            //}));
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
            //thread.Join();

            #endregion

            this.GetRepository<McpOrderInfo>().Create(order);

            //5.2.新增订单对应打印设备
            var successPrintMacs = GetSuccessPrinterCodes(orderId);
            var printerList = new List<McpOrderPrinterInfo>();
            if (printCodes != null && printCodes.Count > 0)
            {
                //若成功打印过的打印设备不再分发任务
                foreach (var printer in successPrintMacs)
                {
                    printCodes.Remove(printer.PrinterCode.ToUpper());
                }

                foreach (var printer in printCodes)
                {
                    printerList.Add(new McpOrderPrinterInfo()
                    {
                        OrderId = orderId,
                        PrinterCode = printer.ToUpper(),
                        CreatedOn = dbNow,
                        OrderStatus = PrintOrderStatusType.UnPrinted.GetHashCode()
                    });
                }
                this.GetRepository<McpOrderPrinterInfo>().Create(printerList);
            }
            else
            {
                throw new MessageException("没有可用的打印设备！");
            }

            return orderId;
        }

        /// <summary>
        /// 生成打印任务（订单及对应打印设备）
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        private string CreatePrintTasksDemonstration(BillViewModel bill)
        {
            var orderId = string.Empty;

            #region 2018年4月17日12:25:12  演示打印彩票，不用绑定应用，所以不用做这些验证
            ////1.校验应用信息
            bool appIsOk = SingleInstance<AppService>.Instance.CheckApp(bill.app_id);
            if (!appIsOk)
                return orderId;

            //2.校验身份安全
            bool tokenIsOk = SingleInstance<AppService>.Instance.CheckAccessToken(bill.app_id.Trim(), bill.access_token.Trim());
            if (!tokenIsOk)
            {
                throw new MessageException("无效访问令牌！");
            }

            ////3.校验打印设备
            var printCodes = GetPrinterCodesDemonstration(bill);
            //if (printCodes.Count <= 0)
            //{
            //    return orderId;
            //}
            #endregion
            //4.获取票据参数
            var billParsms = GetBillParams(bill, bill.printer_codes.Trim().ToUpper());
            int billType = billParsms.Item1;
            int billCopies = billParsms.Item2;
            var templateId = billParsms.Item3;
            var billContent = billParsms.Item4;
            if (billType == 0
                || string.IsNullOrWhiteSpace(billContent)
                || (billType == BillType.Template.GetHashCode() && string.IsNullOrWhiteSpace(templateId))
                || (billType == BillType.Express.GetHashCode() && string.IsNullOrWhiteSpace(templateId)))
            {
                return orderId;
            }

            //5.创建及更新打印订单
            var printOrder = GetOrder(bill.app_id.Trim(), bill.bill_no.Trim());
            var orderKey = VerifyCodeUtil.Intance.GenCode(12);
            DateTime dbNow = DateTime.Now;

            //5.1.新增打印订单
            var order = new McpOrderInfo()
            {
                OrderId = GetNewPrintOrderId(bill.app_id.Trim()),
                AppId = bill.app_id.Trim(),
                BillNo = bill.bill_no.Trim(),
                BillType = billType,
                OrderContent = billContent,
                OrderDate = dbNow,
                OrderKey = orderKey,
                Copies = billCopies
            };
            orderId = order.OrderId;

            #region 5.1.1 此处在创建订单前增加Html打印为图片的功能

            //if (billType == BillType.HtmPict.GetHashCode())
            //{
            //    Thread thread = null;
            //    thread = new Thread(new ThreadStart(delegate ()
            //    {
            //        //获取打印机的纸张设置
            //        string paperType = string.Empty;
            //        McpEquipmentInfo mcpDevice = null;
            //        if (!string.IsNullOrWhiteSpace(bill.printer_codes))
            //        {
            //            mcpDevice = GetPrintEmpInfo(bill.printer_codes);
            //        }
            //        if (mcpDevice != null)
            //        {
            //            paperType = mcpDevice.PaperType;
            //        }

            //        string saveUrl = string.Empty;

            //        DateTime dtStart = DateTime.Now;
            //        bool flag = HtmlConvertUtil.HtmlConvertImage(order.OrderContent, paperType, out saveUrl);
            //        DateTime dtEnd = DateTime.Now;
            //        TimeSpan span = dtEnd.Subtract(dtStart);
            //        LogUtil.Info("999：转图片完成,用时：" + span.Milliseconds.ToString() + "ms");
            //        if (flag)
            //        {
            //            LogUtil.Info($"订单号：【{order.OrderId}】的Html转换为图片，原Html为：【{order.OrderContent}】");
            //            order.OrderContent = saveUrl;
            //            ProcessOrderInfo(order, orderId, printCodes, dbNow);
            //        }
            //        else
            //        {
            //            throw new MessageException("打印文件出错！");
            //        }

            //    }));

            //    thread.SetApartmentState(ApartmentState.STA);
            //    thread.Start();

            //    DateTime dtStart2 = DateTime.Now;
            //    while (thread.IsAlive) Thread.Sleep(1);
            //    thread.Abort();
            //    //thread.Join();
            //    DateTime dtEnd2 = DateTime.Now;
            //    TimeSpan span2 = dtEnd2.Subtract(dtStart2);
            //    LogUtil.Info("9999：线程完成,用时：" + span2.Milliseconds.ToString() + "ms");
            //    return orderId;
            //}
            //if (billType == BillType.DesignTemplate.GetHashCode() || billType == BillType.HtmPict.GetHashCode())
            //{
            //    //获取打印机的纸张设置
            //    string paperType = string.Empty;
            //    McpEquipmentInfo mcpDevice = null;
            //    if (!string.IsNullOrWhiteSpace(bill.printer_codes))
            //    {
            //        mcpDevice = GetPrintEmpInfo(bill.printer_codes);
            //    }
            //    if (mcpDevice != null)
            //    {
            //        paperType = mcpDevice.PaperType;
            //    }

            //    string saveUrl = string.Empty;
            //    bool isReturn = false;
            //    DateTime dtStart = DateTime.Now;
            //    if (billType == BillType.HtmPict.GetHashCode())
            //    {
            //        isReturn = WebSnapshotsHelper.GetWebSiteThumbnail(order.OrderContent, paperType, 1, out saveUrl);
            //    }
            //    else
            //    {
            //        isReturn = WebSnapshotsHelper.GetWebSiteThumbnail(order.OrderContent, paperType, 0, out saveUrl);
            //    }
            //    DateTime dtEnd = DateTime.Now;
            //    TimeSpan span = dtEnd.Subtract(dtStart);
            //    LogUtil.Info("999：转图片完成,用时：" + span.Milliseconds.ToString() + "ms");
            //    if (isReturn)
            //    {
            //        LogUtil.Info($"订单号：【{order.OrderId}】的Html转换为图片，原Html为：【{order.OrderContent}】");
            //        order.OrderContent = saveUrl;
            //    }
            //    else
            //    {
            //        throw new MessageException("生成网页快照出错，请重试！");
            //    }
            //    ProcessOrderInfo(order, orderId, printCodes, dbNow);
            //    return orderId;
            //}
            //else
            //{
            //    return ProcessOrderInfo(order, orderId, printCodes, dbNow);
            //}
            #endregion

            this.GetRepository<McpOrderInfo>().Create(order);

            //5.2.新增订单对应打印设备
            var successPrintMacs = GetSuccessPrinterCodes(orderId);
            var printerList = new List<McpOrderPrinterInfo>();
            if (printCodes != null && printCodes.Count > 0)
            {
                //若成功打印过的打印设备不再分发任务
                foreach (var printer in successPrintMacs)
                {
                    printCodes.Remove(printer.PrinterCode.ToUpper());
                }

                foreach (var printer in printCodes)
                {
                    printerList.Add(new McpOrderPrinterInfo()
                    {
                        OrderId = orderId,
                        PrinterCode = printer.ToUpper(),
                        CreatedOn = dbNow,
                        OrderStatus = PrintOrderStatusType.UnPrinted.GetHashCode()
                    });
                }
                this.GetRepository<McpOrderPrinterInfo>().Create(printerList);
            }
            else
            {
                throw new MessageException("没有可用的打印设备！");
            }

            return orderId;
        }

        /// <summary>
        /// 获取票据打印设备列表
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        private List<string> GetPrinterCodesDemonstration(BillViewModel bill)
        {
            List<string> printerCodes = new List<string>();
            var codes = string.Format("'{0}'", bill.printer_codes.Trim().ToUpper().Replace(",", "','"));
            var cc = new ConditionCollection();
            //2018年4月17日12:37:04 演示  不用验证app_id和merchant_code
            //cc.Add(new Condition("app_id", bill.app_id.Trim()));
            //cc.Add(new Condition("merchant_code", bill.merchant_code.Trim()));
            //cc.Add(new Condition("printer_code", codes, OperationType.In));
            var printers = this.GetRepository<McpMerchantPrinterInfo>().ListModel(cc);
            var prters = bill.printer_codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < prters.Length; i++)
            //{
            //    if (!printers.Exists(d => d.PrinterCode.Equals(prters[i].Trim(), StringComparison.CurrentCultureIgnoreCase)))
            //    {
            //        throw new MessageException(string.Format("未找到打印设备：{0}", prters[i].Trim()));
            //    }
            //}
            printerCodes = prters.ToList();
            return printerCodes;
        }


        /// <summary>
        /// 获取票据打印设备列表
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        private List<string> GetPrinterCodes(BillViewModel bill)
        {
            List<string> printerCodes = new List<string>();
            var codes = string.Format("'{0}'", bill.printer_codes.Trim().ToUpper().Replace(",", "','"));
            var cc = new ConditionCollection();
            cc.Add(new Condition("app_id", bill.app_id.Trim()));
            cc.Add(new Condition("merchant_code", bill.merchant_code.Trim()));
            cc.Add(new Condition("printer_code", codes, OperationType.In));
            var printers = this.GetRepository<McpMerchantPrinterInfo>().ListModel(cc);
            var prters = bill.printer_codes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < prters.Length; i++)
            {
                if (!printers.Exists(d => d.PrinterCode.Equals(prters[i].Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new MessageException(string.Format("未找到打印设备：{0}，请绑定", prters[i].Trim()));
                }
            }
            printerCodes = prters.ToList();
            return printerCodes;
        }

        /// <summary>
        /// 获取票据参数
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="paramCode"></param>
        /// <returns></returns>
        private Tuple<int, int, string, string> GetBillParams(BillViewModel bill, string paramCode)
        {
            var templateId = string.IsNullOrWhiteSpace(bill.template_id) ? string.Empty : bill.template_id.Trim();
            var billContent = string.Empty;
            int billType = TryConvertUtil.ToInt(bill.bill_type, 1);
            int billCopies = TryConvertUtil.ToInt(bill.copies, 1);
            switch (billType)
            {
                case (int)BillType.Html:
                    {
                        //html格式小票，url地址
                        billContent = bill.bill_content.Trim();
                        if (!RegexUtil.IsMatch(ref billContent, RegexConsts.URL_PATTERN))
                        {
                            throw new MessageException("小票打印内容格式不正确！");
                        }
                        break;
                    }
                case (int)BillType.Esc:
                    {
                        //打印机Esc指令，内容透传
                        billContent = bill.bill_content.Trim();
                        break;
                    }
                case (int)BillType.Template:
                    {
                        //制定模版，json数据内容透传
                        if (string.IsNullOrWhiteSpace(templateId))
                        {
                            throw new MessageException("快递单打印模板ID不能为空！");
                        }
                        else
                        {
                            //校验模版是否存在
                            var templateIsOk = SingleInstance<TemplateService>.Instance.CheckTemplateIsExist(templateId);
                            if (!templateIsOk)
                            {
                                templateId = string.Empty;
                                throw new MessageException("无效模板ID！");
                            }
                        }
                        billContent = bill.bill_content.Trim();
                        break;
                    }
                case (int)BillType.Json:
                    {
                        //json格式小票，内容透传
                        billContent = bill.bill_content.Trim();
                        break;
                    }
                case (int)BillType.Express:
                    {
                        //快递面单小票，json数据，内容组合后透传
                        if (string.IsNullOrWhiteSpace(templateId))
                        {
                            throw new MessageException("快递单打印模板ID不能为空！");
                        }
                        else
                        {
                            var templateIsOk = SingleInstance<TemplateService>.Instance.CheckTemplateIsExist(templateId);
                            if (!templateIsOk)
                            {
                                templateId = string.Empty;
                                throw new MessageException("无效模板ID！");
                            }

                            //快递面单处理
                            billCopies = 1;
                            billContent = bill.bill_content.Trim();
                            billContent = SingleInstance<TemplateService>.Instance.GetExpressBillJson(templateId, billContent, paramCode).ToString();
                        }
                        break;
                    }
                default:
                    {
                        billType = 0;
                        throw new MessageException("无效的票据类型！");
                    }
            }

            return Tuple.Create<int, int, string, string>(billType, billCopies, templateId, billContent);
        }

        /// <summary>
        /// 获取订单已成功打印的打印设备列表
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <returns></returns>
        private List<McpOrderPrinterInfo> GetSuccessPrinterCodes(string orderId)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("order_Id", TryConvertUtil.ToString(orderId)));
            cc.Add(new Condition("order_status", PrintOrderStatusType.PrintSuccess.GetHashCode()));
            return this.GetRepository<McpOrderPrinterInfo>().ListModel(cc);
        }

        /// <summary>
        /// 获取打印设备打印任务
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="printCode">打印设备Mac</param>
        /// <returns></returns>
        private McpOrderPrinterInfo GetOrderPrinter(string orderId, string printCode)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("order_Id", TryConvertUtil.ToString(orderId)));
            cc.Add(new Condition("printer_code", TryConvertUtil.ToString(printCode)));
            //cc.Add(new Condition("order_status", PrintOrderStatusType.UnPrinted.GetHashCode()));
            return this.GetRepository<McpOrderPrinterInfo>().GetModel(cc);
        }

        /// <summary>
        /// 获取新打印订单号
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

        /// <summary>
        /// 发送未处理的订单至打印设备
        /// </summary>
        /// <param name="printCode">打印设备编码</param>
        /// <returns>处理结果</returns>
        public void SendUnPrintOrder(string printCode, string operation = "")
        {
            //1.设备心跳且状态正常2.打印任务状态更新触发下一打印任务
            LogUtil.Info(string.Format("设备号:{0},{1}主动查询发送打印任务命令。", printCode, operation));

            var cmd = "{tp:" + TcpOperateType.Print.GetHashCode() + ",did:\"" + printCode + "\"}";
            SingleInstance<TcpService>.Instance.SendTcpMsg(cmd);
        }

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="tryNum">尝试次数</param>
        public void SendTcpMsg(string orderId, string printerCodes, int tryNum, bool isUnPrint = false)
        {
            //发送TCP消息至设备
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                var ps = printerCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (ps.Count > 0)
                {
                    foreach (var code in ps)
                    {
                        string msg = "{tp:" + TcpOperateType.Print.GetHashCode() + ",did:\"" + code + "\", id:\"" + orderId + "\"}";
                        SingleInstance<TcpService>.Instance.SendTcpMsg(msg);

                        LogUtil.Info(string.Format("第{0}次尝试发送TCP消息至设备：{1}。", tryNum, code));
                    }
                }
            }
        }

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="tryNum">尝试次数</param>
        public void SendTcpMsg(string orderId, string printerCodes, int tryNum, TcpOperateType tcptype, bool isUnPrint = false)
        {
            //发送TCP消息至设备
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                var ps = printerCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (ps.Count > 0)
                {
                    foreach (var code in ps)
                    {
                        string msg = "{tp:" + tcptype.GetHashCode() + ",did:\"" + code + "\", id:\"" + orderId + "\"}";
                        SingleInstance<TcpService>.Instance.SendTcpMsg(msg);

                        LogUtil.Info(string.Format("第{0}次尝试发送TCP消息至设备：{1}。", tryNum, code));
                    }
                }
            }
        }

        /// <summary>
        /// 推送任务成功时，更改状态为已经推送
        /// </summary>
        public void UpdatePushStatus(string orderId, string printerCode)
        {
            try
            {
                int pushStatus = PrintOrderPushStatusType.Pushed.GetHashCode();
                //LogUtil.Info(string.Format("打印任务已通过TCP发送给打印机，准备更新数据库推送状态，设备号：{0},订单号：{1},状态: {2}", printerCode, orderId, pushStatus));
                //获取打印任务
                var task = GetOrderPrinter(orderId, printerCode);
                if (task != null)
                {
                    task.PushStatus = pushStatus;
                    //task.OrderStatus = PrintOrderStatusType.PushedNoResult.GetHashCode();
                    task.ModifiedOn = DateTime.Now;
                    //int cnt = this.GetRepository<McpOrderPrinterInfo>().Update(task, "order_status,push_status,modified_on");
                    int cnt = this.GetRepository<McpOrderPrinterInfo>().Update(task, "push_status,modified_on");
                    if (cnt > 0)
                    {
                        LogUtil.Info($"更新数据库任务推送状态成功，设备号：{printerCode},订单号：{orderId},任务状态：{PrintOrderStatusType.PushedNoResult.GetHashCode()},推送状态: {pushStatus}");
                    }
                    else
                    {
                        LogUtil.Info(string.Format("更新数据库任务推送状态失败，设备号：{0},订单号：{1},状态: {2}", printerCode, orderId, pushStatus));
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("更新数据库任务推送状态异常，设备号：{0},订单号：{1}", printerCode, orderId));
                LogUtil.Error(string.Format("更新数据库任务推送状态异常，设备号：{0},订单号：{1}", printerCode, orderId),ex);
            }
        }

        /// <summary>
        /// 签名参数数据校验
        /// </summary>
        /// <param name="billModel"></param>
        /// <returns></returns>
        public void CheckPrintSign(BillViewModel billModel)
        {
            if (!string.IsNullOrEmpty(billModel.sign))
            {
                string str = $"app_id={billModel.app_id}&access_token={billModel.access_token}&merchant_code={billModel.merchant_code}&printer_codes={billModel.printer_codes}&template_id={billModel.template_id}&copies={billModel.copies}&bill_no={billModel.bill_no}&bill_type={billModel.bill_type}&bill_content={billModel.bill_content}";
                bool result = MD5CryptionUtil.Verify(str, billModel.sign);
                if (!result)
                {
                    LogUtil.Info($"交易号{billModel.bill_no}签名验证失败");
                    throw new MessageException("签名验证失败！");
                } 
            }
        }

        /// <summary>
        /// 修改超时的任务状态为超时，并且返回这些数据
        /// </summary>
        /// <param name="printerCode"></param>
        /// <returns></returns>
        public List<McpOrderPrinterInfo> GetOverTimePrintOrders(string printerCode)
        {
            string date = DateTime.Now.ToString();
            int StatusUnPrinted = PrintOrderStatusType.UnPrinted.GetHashCode();
            string orderStatus = "0";
            var NotRePrintServiceType = ConfigUtil.GetConfig("NotRePrintServiceType");
            var ServiceType = ConfigUtil.GetConfig("ServiceType");
            if (NotRePrintServiceType.Contains(ServiceType))
            {
                orderStatus = ConfigUtil.GetConfig("NotRePrintOrderStatus");
            }
            else
            {
                orderStatus = ConfigUtil.GetConfig("RePrintOrderStatus");
            }
            int StatusOverTime = PrintOrderStatusType.OverTime.GetHashCode();
            int OverTime = int.Parse(ConfigUtil.GetConfig("PrintOrderOutTime"));
            string sql = $"SELECT * FROM `mcp_order_printer` WHERE printer_code='{printerCode}' AND order_status in ({orderStatus}) AND TO_SECONDS('{date}')-TO_SECONDS(created_on)>{OverTime};UPDATE `mcp_order_printer` SET order_status={StatusOverTime} WHERE printer_code='{printerCode}' AND order_status in ({orderStatus}) AND TO_SECONDS('{date}')-TO_SECONDS(created_on)>{OverTime}";
            LogUtil.Debug($"GetOverTimePrintOrders方法查询超时：{sql}");            
            LogUtil.Info(string.Format("准备执行超时清理，设备号{0},设定超时时间为{1}秒,执行的sql:{2}", printerCode, OverTime, sql));
            var listPrintOrder = this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql);
            if (listPrintOrder.Count <= 0)
            {
                LogUtil.Info(string.Format("未发现超时任务，设备号：{0}", printerCode));
            }
            foreach (var task in listPrintOrder)
            {
                LogUtil.Info(string.Format("已清理超时任务,即将执行状态回调，设备号：{0},订单号：{1}", printerCode, task.OrderId));
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.AppCallBack(PrintOrderStatusType.OverTime.GetHashCode(), task.OrderId, task.PrinterCode);
                });
                ac.BeginInvoke(null, null);
            }
            return listPrintOrder;
        }

        /// <summary>
        /// 是否需要通过sql清理超时任务
        /// </summary>
        /// <returns></returns>
        public bool IsClearOutTimeOrder()
        {
            string OutTimeBySqlServiceType = ConfigUtil.GetConfig("OutTimeBySqlServiceType");
            string ServiceType = ConfigUtil.GetConfig("ServiceType");
            return OutTimeBySqlServiceType.Contains(ServiceType);
        }

        /// <summary>
        /// 打印任务结果状态，由内部状态转化为外部状态
        /// </summary>
        /// <param name="printOrderStatusType"></param>
        /// <returns></returns>
        public int PrintOrderStatusConvert(int printOrderStatusType)
        {
            try
            {
                switch (printOrderStatusType)
                {
                    case 1:
                        return ResultCodeType.Success.GetHashCode();
                    case 2:
                        return ResultCodeType.Fail.GetHashCode();
                    case 3:
                        return ResultCodeType.TimeOutUnPrint.GetHashCode();
                    case 4:
                        return ResultCodeType.UnBindClear.GetHashCode();
                    case 6:
                        return ResultCodeType.OpenCoverPrinted.GetHashCode();
                    case 7:
                        return ResultCodeType.Fault.GetHashCode();
                    case 8:
                        return ResultCodeType.NetworkFault.GetHashCode();
                    case 9:
                        return ResultCodeType.OpenCoverPushed.GetHashCode();
                    case 10:
                        return ResultCodeType.OutPaperPushed.GetHashCode();
                    case 11:
                        return ResultCodeType.NoConnect.GetHashCode();
                    case 12:
                        return ResultCodeType.NoFindPrint.GetHashCode();
                    default:
                        break;
                }
                return 10000;
            }
            catch (Exception ex)
            {
                LogUtil.Info(string.Format("将内部状态转成外部状态异常,状态值{0}", printOrderStatusType));
                LogUtil.Error(string.Format("将内部状态转成外部状态异常,状态值{0}", printOrderStatusType), ex);
                return 10000;
            }
        }

        /// <summary>
        /// 设置超时订单，自动判断是防丢单还是防重大模式
        /// </summary>
        /// <param name="printCode"></param>
        public void ClearOutTimeOrder(string printCode)
        {
            //设置超时并且获取超时订单;张友辉
            bool IsClearOutTimeOrder = SingleInstance<PrintOrderService>.Instance.IsClearOutTimeOrder();
            if (IsClearOutTimeOrder)
            {
                SingleInstance<PrintOrderService>.Instance.GetOverTimePrintOrders(printCode);
            }
        }

        /// <summary>
        /// 查询打印任务状态
        /// </summary>
        /// <param name="verModel"></param>
        /// <returns></returns>
        public object GetPrintOrderStatue(PrtintViewQueryModel queryModel)
        {

            var data = GetPrintOrders(queryModel);
            if (string.IsNullOrEmpty(queryModel.bill_no))
            {
                LogUtil.Info($"查询打印任务订单状态失败：订单号不能为空");
                throw new MessageException("订单号不能为空");
            }
            var result = data.Select(x => { return new { bill_no = queryModel.bill_no, order_id = x.OrderId, order_status = x.OrderStatus }; });

            return result;
        }

        /// <summary>
        /// 根据商户订单号查询任务状态
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public List<McpOrderPrinterInfo> GetPrintOrders(PrtintViewQueryModel queryModel)
        {
            var sql = @"SELECT * FROM `mcp_order_printer`
                        WHERE order_id IN
                        (SELECT `order_id` FROM `mcp_order` WHERE bill_no =@bill_no);";
            var pc = new ParameterCollection();
            pc.Add("bill_no", TryConvertUtil.ToString(queryModel.bill_no));
            var orders = this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sql, pc);
            if (orders == null || orders.Count <= 0)
            {
                LogUtil.Info($"没有查询到相关订单，请检查bii_no-{queryModel.bill_no}是否填错");
                throw new MessageException("没有查询到相关订单，请检查bii_no是否填错");
            }
            return orders;
        }

        /// <summary>
        /// 记录回调信息
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="OrderId"></param>
        /// <param name="orderStatu"></param>
        public void RecordCallBack(string billno,string orderid,string printercode, int orderstatus)
        {
            var call = new McpCallbackRecord
            {
                BillNo = billno,
                OrderId = orderid,
                PrinterCode = printercode,
                OrderStatus = orderstatus,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            //将信息写进数据库
            this.GetRepository<McpCallbackRecord>().Create(call);

        }

        /// <summary>
        /// 如果用户传入的交易号已经存在，那么更新一下时间
        /// </summary>
        /// <param name="billModel"></param>
        /// <returns></returns>
        public List<McpOrderInfo> UpdateOrderCreatTimeIfSameBill(BillViewModel billModel)
        {
            //检验打印机状态：缺纸或开盖则创建任务失败
            var printstatue = SingleInstance<PrinterService>.Instance.GetPrinterStatus(billModel.printer_codes);
            if (printstatue == null || printstatue.StatusCode != PrinterFaultType.Enable.GetHashCode())
            {
                throw new MessageException("打印机故障，开盖或缺纸，或者无可用状态可用状态，请重启打印机试试，或者联系管理员");
            }

            //检验打印机联网状态，断网则不创建
            bool isConnect = SingleInstance<PrinterService>.Instance.IsPrintConnet(billModel.printer_codes);
            if (!isConnect)
            {
                throw new MessageException("打印机断网或者网络不稳定！");
            }

            //根据这个交易号查询订单
            var sqlorder = @"SELECT * FROM `mcp_order`
                        WHERE bill_no=@bill_no";
            var pc = new ParameterCollection();
            pc.Add("bill_no", TryConvertUtil.ToString(billModel.bill_no));
            var orders = this.GetRepository<McpOrderInfo>().ListModelBySql(sqlorder, pc);

            var sqlorderInfo = @"SELECT * FROM `mcp_order_printer`
                        WHERE order_id IN
                        (SELECT `order_id` FROM `mcp_order` WHERE bill_no =@bill_no);";
            var orderInfos = this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sqlorderInfo, pc);
            string sqlUpdateStatu = " ";
            if (orderInfos!=null&&orderInfos.Count>0)
            {
                if (orderInfos.FirstOrDefault().OrderStatus== PrintOrderStatusType.OverTime.GetHashCode())
                {
                    LogUtil.Info($"到交易号{billModel.bill_no}状态为超时，改为未打印状态");
                    sqlUpdateStatu = $" ,order_status={PrintOrderStatusType.UnPrinted.GetHashCode()} ";
                }
            }
            if (orders==null||orders.Count<=0)
            {
                LogUtil.Info($"没有找到交易号为{billModel.bill_no}的订单，创建新任务");
                return null;
            }
            else
            {
                LogUtil.Info($"存在交易号为{billModel.bill_no}的订单，更新订单时间");
                var sqlOrderInfo = $@"UPDATE  `mcp_order` SET order_date=NOW() WHERE bill_no=@bill_no;UPDATE `mcp_order_printer` SET created_on=NOW() {sqlUpdateStatu}
                        WHERE order_id IN
                        (SELECT `order_id` FROM `mcp_order` WHERE bill_no =@bill_no);";
                LogUtil.Debug($"当交易号{billModel.bill_no}相同时，修改时间和状态SQL语句-{sqlOrderInfo}");
                this.GetRepository<McpOrderPrinterInfo>().ListModelBySql(sqlOrderInfo, pc);
                return orders;
            }
        }

    }
}