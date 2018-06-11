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
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace CWI.MCP.Services
{
    public class TcpService : BaseService
    {
        #region 私有变量

        /// <summary>
        /// TCP通讯心跳监控器
        /// </summary>
        private System.Threading.Timer timerCheck;

        /// <summary>
        /// TCP通讯套接字
        /// </summary>
        private static Socket _clientSocket = null;

        /// <summary>
        /// 静态锁
        /// </summary>
        private static object syncLock = new object();

        //定时器时间间隔：单位毫秒
        private const int dueTime = 1000;
        private const int checkInterval = 10000;

        #endregion

        #region TCP连接&断开

        /// <summary>
        /// 创建链接及注册
        /// </summary>
        private void TcpConnect()
        {
            try
            {
                lock (syncLock)
                {
                    //1.断开现有TCP服务
                    DisTcpConnect();

                    //2.设定服务器IP地址
                    string tcpip = ConfigUtil.GetConfig("TCPServerIP");
                    int port = TryConvertUtil.ToInt(ConfigUtil.GetConfig("TCPServerPort"), 0);
                    if (port <= 0)
                    {
                        LogUtil.Info("TCP服务端口为空！");
                        throw new MessageException("TCP服务端口不能为空！");
                    }
                    else
                    {
                        IPAddress ip = IPAddress.Parse(tcpip);
                        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _clientSocket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
                        LogUtil.Info("连接TCP服务器成功！");
                    }

                    //开始心跳检测
                    ChangeTimer(true);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("连接TCP服务器失败,参考信息：{0}！", ex));
            }
        }

        /// <summary>
        /// 断开打印设备TCP链接信息
        /// </summary>
        /// <param name="connId"></param>
        public void TcpDisConnect(long connId)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("connection_id", connId));
            var device = this.GetRepository<McpPrinterInfo>().GetModel(cc);
            if (device != null)
            {
                device.IpPort = string.Empty;
                device.ConnectionId = 0;
                device.ModifiedOn = DateTime.Now;
                this.GetRepository<McpPrinterInfo>().Update(device, "ip_port,connection_id,modified_on");
                //打印设备回调
                LogUtil.Info(string.Format("断开打印设备TCP链接信息并执行打印设备回调,设备号：{0}, 状态: {1}", device.PrinterCode, PrinterFaultType.NetworkFault.GetHashCode()));
                var dbNow = CommonUtil.GetDBDateTime();
                //打印设备状态更改回调
                Action ac = new Action(() =>
                {
                    SingleInstance<AppService>.Instance.DeviceCallBack(PrinterFaultType.NetworkFault.GetHashCode(), device.PrinterCode, dbNow);
                });
                ac.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// 断开TCP链接
        /// </summary>
        public void DisTcpConnect()
        {
            try
            {
                ChangeTimer(false);

                if (_clientSocket != null)
                {
                    if (_clientSocket.Connected)
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                        _clientSocket.Close();
                    }
                    _clientSocket.Dispose();
                    _clientSocket = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.Message);
            }
        }

        #endregion

        #region 发送消息

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        public void SendTcpMsg(string msg)
        {
            byte[] result = new byte[4096];

            try
            {
                if (_clientSocket == null || !_clientSocket.Connected)
                {
                    //1.重新链接
                    TcpConnect();
                }

                //2.发送消息
                SendMsg(msg, result);
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("连接TCP服务器失败,参考信息：{0}！", ex.Message);
                LogUtil.Error(errMsg);
                throw new MessageException(errMsg);
            }
        }

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="result"></param>
        private void SendMsg(string msg, byte[] result)
        {
            try
            {
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    _clientSocket.Send(Encoding.GetEncoding("gb2312").GetBytes(msg));
                    LogUtil.Info(string.Format("向服务器发送消息：{0}", msg));
                }
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("发送TCP消息失败,参考信息：{0}！", ex.Message);
                LogUtil.Error(errMsg);
                throw new MessageException(errMsg);
            }
        }

        #endregion

        #region 心跳检测

        /// <summary>
        /// 修改心跳监控器
        /// </summary>
        /// <param name="isStart">是否启动</param>
        private void ChangeTimer(bool isStart)
        {
            //心跳监控器
            if (timerCheck != null)
            {
                timerCheck.Change(0, isStart ? checkInterval : int.MaxValue);
            }
            else
            {
                if (isStart)
                {
                    StarCheckMonitor();
                }
            }
        }

        /// <summary>
        /// 启动心跳检测监听
        /// </summary>
        public void StarCheckMonitor()
        {
            if (timerCheck == null)
            {
                timerCheck = new System.Threading.Timer(CheckSocketAlive, null, dueTime, checkInterval);
            }
        }

        /// <summary>
        /// TCP通讯心跳检测
        /// </summary>
        /// <param name="obj"></param>
        public void CheckSocketAlive(object obj)
        {
            try
            {
                LogUtil.Info("执行心跳检测...");

                //监控TCP连接
                var did = "Server";
                var sendMessage = "{tp:" + TcpOperateType.ChkAlive.GetHashCode() + ",did:\"" + did + "\"}";
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    SendTcpMsg(sendMessage);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("心跳检测异常，参考信息：{0}", ex));
            }
        }

        #endregion
    }
}
