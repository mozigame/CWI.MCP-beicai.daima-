using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.SocketBase;
using Sodao.FastSocket.Server.Command;
using Evt.Framework.Common;
using System.Timers;
using CWI.MCP.Common;
using CWI.MCP.Services;

namespace McpTcpServer
{
    public class McpServer
    {
        public static McpServer Instance
        {
            get { return _instance; }
        }

        private static McpServer _instance = new McpServer();

        private McpServer()
        {

        }

        public void Start()
        {
            SocketServerManager.Init();
            SocketServerManager.Start();
            Timer timer = new Timer(1000);

            //5秒检测一次
            timer.Interval = 1000 * 5; 
            timer.Elapsed += new ElapsedEventHandler(CheckSocketAlive);
            timer.Start();
        }

        public void CheckSocketAlive(object sender, ElapsedEventArgs e)
        {
            //N秒无心跳断开连接
            var connections = GetConnetionList();
            int timeOut = TryConvertUtil.ToInt(ConfigUtil.GetConfig("TcpDisconnectTime"), 0);
            if (connections != null && connections.Length > 0)
            {
                connections.Where(s => s.Active == false || (DateTime.Now - s.LatestActiveTime).TotalSeconds > timeOut) 
                    .ToList().ForEach(ss => ss.BeginDisconnect());
            }
        }

        public IConnection[] GetConnetionList()
        {
            IHost host = null;
            SocketServerManager.TryGetHost("mcp", out host);
            if (host != null)
            {
                return host.ListAllConnection();
            }
            else
            {
                return null;
            }
        }
    }


    /// <summary>
    /// 实现自定义服务
    /// </summary>
    public class McpTcpService : CommandSocketService<McpCommandInfo>
    {
        /// <summary>
        /// 当连接时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
            LogUtil.Debug(string.Format("{0}-{1}连接成功！", connection.RemoteEndPoint.ToString(), connection.ConnectionID));
        }

        /// <summary>
        /// 当连接断开时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);

            //断开时清空设备的链接信息
            SingleInstance<TcpService>.Instance.TcpDisConnect(connection.ConnectionID);
        }

        /// <summary>
        /// 当发生错误时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnException(IConnection connection, Exception ex)
        {
            base.OnException(connection, ex);
        }

        /// <summary>
        /// 处理未知的命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected override void HandleUnKnowCommand(IConnection connection, McpCommandInfo commandInfo)
        {
            commandInfo.Reply(connection, JsonUtil.Serialize(new { type = 0, errno = 401 }));
        }
    }
}
