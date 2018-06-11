//---------------------------------------------
// 版权信息：版权所有(C) 2017，Yingmei.me
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2017/04/19        创建
//---------------------------------------------
using System;
using CWI.MCP.Common;
using CWI.MCP.WinServ.Common;
using CWI.MCP.Services;

namespace CWI.MCP.WinServ.Busy.TimerCtrl
{
    /// <summary>
    /// 清理打印任务定时服务
    /// </summary>
    public class ClearPrintTaskTimer : BaseTimer
    {
        /// <summary>
        /// 清理打印任务
        /// </summary>
        public ClearPrintTaskTimer()
        {
            this.Interval = ConfigHelper.ClearPrintTaskInterval;
        }

        /// <summary>
        /// 执行服务
        /// </summary>
        public override void Timer_Elapsed()
        {
            LogUtil.Info("开始清理打印任务...");

            SingleInstance<SysService>.Instance.ClearPrintTasks(ConfigHelper.PrintTaskExistsMaxMins);
        }
    }
}