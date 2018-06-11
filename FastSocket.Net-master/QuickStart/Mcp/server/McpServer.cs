using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.SocketBase;
using Sodao.FastSocket.Server.Command;
using System.Threading;

namespace Server
{
    public class McpServer
    {
        public void Start()
        {
            SocketServerManager.Init();
            SocketServerManager.Start();

            Timer time = new Timer(CheckSocketAlive, null, 10000, 60000);

        }

        public void CheckSocketAlive(object obj)
        {
            IHost host = null;
            SocketServerManager.TryGetHost("mcp", out host);
            if (host != null)
            {
                host.ListAllConnection().Where(s => s.Active == false || (DateTime.UtcNow - s.LatestActiveTime).TotalSeconds > 60)
                    .ToList().ForEach(ss => ss.BeginDisconnect());
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
            Console.WriteLine(connection.RemoteEndPoint.ToString() + " connected");
        }
        /// <summary>
        /// 当连接断开时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(connection.RemoteEndPoint.ToString() + " disconnected");
            //Console.ForegroundColor = ConsoleColor.Gray;
        }
        /// <summary>
        /// 当发生错误时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnException(IConnection connection, Exception ex)
        {
            base.OnException(connection, ex);
            //Console.WriteLine("error: " + ex.ToString());
        }

        /// <summary>
        /// 处理未知的命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected override void HandleUnKnowCommand(IConnection connection, McpCommandInfo commandInfo)
        {
            Console.WriteLine("unknow command: " + commandInfo.CmdName);
        }
    }

    /// <summary>
    /// 
    /// Mcp Tcp业务处理
    /// </summary>
    public sealed class TcpHandelCmd : ICommand<McpCommandInfo>
    {
        /// <summary>
        /// 返回服务名称
        /// </summary>
        public string Name
        {
            get { return "TcpDataHandel"; }
        }
        /// <summary>
        /// 执行命令并返回结果
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        public void ExecuteCommand(IConnection connection, McpCommandInfo commandInfo)
        {
            commandInfo.Reply(connection, "now: " + DateTime.Now.ToLongTimeString());
            // connection.BeginDisconnect();
        }
    }

}
