using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using CWI.MCP.WinServ.Busy;
using CWI.MCP.Common;

namespace CWI.MCP.WinServ
{
    /// <summary>
    /// 业务处理主Service
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
        /// <summary>
        /// 是否已启动
        /// </summary>
        private static bool _isRun = false;
        private static object locker = new object();
        /// <summary>
        /// 用于检查数据库版本的timer
        /// </summary>
        private Timer _timerCheckDb;

        /// <summary>
        /// 数据库定时检查时间间隔(秒)
        /// </summary>
        private const int CHECK_DB_TIMER_SPAN = 2;

        private void InitCheckDbTimer()
        {
            //初始化检查db版本定时器
            _timerCheckDb = new Timer();
            _timerCheckDb.Enabled = true;
            _timerCheckDb.Interval = CHECK_DB_TIMER_SPAN * 1000;
            _timerCheckDb.Elapsed += new ElapsedEventHandler(_timerCheckDb_Elapsed);
        }

        /// <summary>
        /// 检查数据库连接是否正常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerCheckDb_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                LogUtil.Info("正在初始化:尝试连接到指定的数据库服务器...");
                CommonUtil.GetDBDateTime();
                LogUtil.Info("数据库连接成功...");
                if (!_isRun)
                {
                    StartService();
                }
                _timerCheckDb.Stop();
            }
            catch
            {
                LogUtil.Info("初始化失败:不能连接到指定的数据库服务器...");
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        private void StartService()
        {
            lock (locker)
            {
                try
                {
                    BusyMain.Instance.Start();

                    _isRun = true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error("服务启动失败", ex);
                }
            }
        }


        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
                InitCheckDbTimer();
                _timerCheckDb_Elapsed(null, null);
                LogUtil.Info("服务启动");
            }
            catch (Exception ex)
            {
                LogUtil.Error("服务启动失败", ex);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            BusyMain.Instance.Pause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            BusyMain.Instance.Continue();
        }

        protected override void OnStop()
        {
            base.OnStop();
            LogUtil.Info("服务停止");
            BusyMain.Instance.Stop();
        }
    }
}
