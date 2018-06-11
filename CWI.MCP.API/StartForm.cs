//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/16        创建
//---------------------------------------------
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using CWI.MCP.API.Handels;
using CWI.MCP.Common;
using MySql.Data.MySqlClient;

namespace CWI.MCP.API
{
    /// <summary>
    /// 启动WinForm
    /// </summary>
    public partial class StartForm : Form
    {
        HttpServerHost server = null;
        private Thread _checkDbThread;        //后台线程,用来检查数据库连接成功与否

        /// <summary>
        /// 构造函数
        /// </summary>
        public StartForm()
        {
            InitializeComponent();

            _checkDbThread = new Thread(() =>
            {
                CheckDbAndStartServer();
            });
            _checkDbThread.IsBackground = true;
            _checkDbThread.Start();

            button1.Visible = false;
        }


        /// <summary>
        /// 定时检查数据库连接,连接成功后在处理启动服务
        /// </summary>
        private void CheckDbAndStartServer()
        {
            while (true)
            {
                try
                {
                    var connecStr = Settings.Intance.MCPDBConnectionString;
                    var connection = new MySqlConnection(connecStr);
                    connection.Open();
                    connection.Close();
                    connection.Dispose();
                    SysDateTime.InitDateTime(CommonUtil.GetDBDateTime());
                    LogUtil.Info("数据库已成功连接,正在启动服务");
                    break;
                }
                catch
                {
                    LogUtil.Info("数据库尝试连接失败,稍后重试");
                    Thread.Sleep(1000);
                }
            }

            StartHttpServer();
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">事件参数</param>
        private void Button1_Click_1(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 启动HTTP服务器
        /// </summary>
        private void StartHttpServer()
        {
            try
            {
                server = new HttpServerHost();
                server.Start();
            }
            catch (Exception ex)
            {
                LogUtil.Error("服务启动失败：" + ex.ToString());
            }

            this.Invoke(new Action(() => { label1.Text = "HttpServer已启动"; }));
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                server.Stop();
                Process.GetCurrentProcess().Kill();
            }
            catch { }
        }
    }
}
