//---------------------------------------------
// 版权信息：版权所有(C) 2014，PAIDUI.CN
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/12/21         创建
//---------------------------------------------

using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;

using CWI.MCP.Common;

namespace CWI.MCP.API.Handels
{
    /// <summary>
    /// 异步委托
    /// </summary>
    public delegate void AsyncCallWebApi();

    /// <summary>
    /// HttpServer宿主管理
    /// </summary>
    public class HttpServerHost
    {
        /// <summary>
        /// HttpSelfHostServer实例
        /// </summary>
        private HttpSelfHostServer _server;

        /// <summary>
        /// 启动HTTP服务器
        /// </summary>
        public void Start()
        {
            StartHttpServer();
            AsyncCallWebApi asyncCallWebApi = new AsyncCallWebApi(CallWebApiFirst);
            asyncCallWebApi.BeginInvoke(null, null);
        }

        /// <summary>
        /// 服务启动后尝试异步第一次调用一下WebApi接口
        /// </summary>
        public void CallWebApiFirst()
        {
            HttpRequestUtility.Instance.GetUrl(string.Format("http://localhost:{0}", Port), null, false);
        }

        /// <summary>
        /// 停止HTTP服务器
        /// </summary>
        public void Stop()
        {
            _server.CloseAsync().Wait();
        }

        /// <summary>
        /// 端口号
        /// </summary>
        private string Port
        {
            get
            {
                string port = ConfigUtil.GetConfig("ServicePort");
                if (string.IsNullOrWhiteSpace(port))
                {
                    port = "8686";
                }
                return port;
            }
        }

        /// <summary>
        /// 启动HTTP服务器
        /// </summary>
        private void StartHttpServer()
        {
            var config = new HttpSelfHostConfiguration(string.Format("http://localhost:{0}", Port));

            config.Routes.MapHttpRoute("WebAPI", "{app_sign}/{controller}/{action}/{id}", new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute("API Default", "{controller}/{action}/{id}", new { controller = "Base", action = "index", id = RouteParameter.Optional });

            //设置最大接收消息大小
            config.MaxReceivedMessageSize = int.MaxValue;

            //将默认缓冲形式的数据传输模式改为流模式, 缓冲模式需要将所有数据接收完成后才会写入, 流模式可以一边接收一边写入一般用于大数据量文件传输
            config.TransferMode = TransferMode.Buffered;

            config.Services.Replace(typeof(IHttpControllerSelector), new AppSignHttpControllerSelector(config));

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();
        }
    }
}
