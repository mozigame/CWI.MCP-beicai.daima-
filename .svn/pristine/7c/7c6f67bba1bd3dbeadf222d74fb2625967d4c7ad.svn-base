//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/09/22        创建
//---------------------------------------------

using System;
using System.Threading;

using CWI.MCP.Common;

namespace CWI.MCP.WinServ.Busy.TimerCtrl
{
    /// <summary>
    /// 定时器基类
    /// </summary>
    public abstract class BaseTimer : IDisposable
    {
        #region 字段

        private Thread _timer = null;
        private bool _isStarted = false;

        //默认间隔【单位：ms】
        private int _interval = 1000;
        //最小间隔【单位：ms】
        private int _minInterval = 100;
        //超时时间【单位：ms】
        private int _timeOut = 200;

        #endregion

        #region 变量

        //间隔为1秒
        public const int sencondsInterval = 1000;
        //间隔为1小时
        public const int hourInterval = 1000 * 60 * 60;
        //间隔为半小时
        public const int halfhourInterval = 1000 * 60 * 30;
        //间隔为1分钟
        public const int minInterval = 1000 * 60;
        //间隔为时的开始时间
        public static DateTime beginDateTime = DateTime.MaxValue;

        #endregion

        #region 构造

        public BaseTimer()
        {
            _timer = new Thread(new ThreadStart(ThreadRunning));
            _timer.IsBackground = true;
            _timer.Start();
        }

        #endregion

        #region 间隔

        /// <summary>
        /// 轮循间隔【单位：ms】）
        /// </summary>
        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value < _minInterval ? _minInterval : value;
            }
        }

        #endregion

        #region 启动

        public void Start()
        {
            _isStarted = true;
        }

        #endregion

        #region 停止

        public void Stop()
        {
            _isStarted = false;
        }

        #endregion

        #region 定时执行线程

        private void ThreadRunning()
        {
            while (true)
            {
                try
                {
                    //状态为启动时再执行定时事件
                    if (_isStarted)
                    {
                        Timer_Elapsed();
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex.Message, ex);
                }
                finally
                {
                    //不符合数值时重置间隔
                    if (_interval < _minInterval)
                    {
                        _interval = _minInterval;
                    }

                    //休眠
                    Thread.Sleep(_interval);
                }
            }
        }

        public abstract void Timer_Elapsed();

        #endregion

        #region 释放

        public void Dispose()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Abort();
                    _timer.Join(_timeOut);
                    _timer = null;
                }
            }
            catch
            {

            }
        }

        #endregion
    }
}