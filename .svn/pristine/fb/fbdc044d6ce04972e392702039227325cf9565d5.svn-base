//-------------------------------------------------
//版本信息:版权所有(C) 2014,PAIDUI.CN
//变更历史:
//    姓名            日期                  说明
//-------------------------------------------------
//   王军锋     2014/12/11 13:46:11           创建
//-------------------------------------------------
using System;
using System.ServiceProcess;
using CWI.MCP.Common;
using CWI.MCP.API.Handels;

namespace CWI.MCP.API
{
    public partial class HostService : ServiceBase
    {
        /// <summary>
        /// HttpSelfHostServer
        /// </summary>
        private HttpServerHost _httpServer;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HostService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动事件
        /// </summary>
        /// <param name="args">启动参数</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                _httpServer = new HttpServerHost();
                _httpServer.Start();
            }
            catch (Exception ex)
            {
                LogUtil.Error("服务启动失败：" + ex.ToString());
            }
        }

        /// <summary>
        /// 结束事件
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                _httpServer.Stop();
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.ToString());
            }
        }
    }
}
