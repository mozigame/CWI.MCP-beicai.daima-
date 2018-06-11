//---------------------------------------------
// 版权信息：版权所有(C) 2017，Yingmei.me
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2017/04/19        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWI.MCP.Common;
using Evt.Framework.Common;

namespace CWI.MCP.WinServ.Common
{
    public class ConfigHelper
    {

        #region 打印任务清理配置

        /// <summary>
        /// 自动清理打印任务频率[单位：分钟]
        /// </summary>
        public static int ClearPrintTaskInterval
        {
            get
            {
                //任务启动的频率，单位为秒
                return TryConvertUtil.ToInt(ConfigUtil.GetConfig("ClearPrintTaskInterval"), 1) * 1000 * 60;
            }
        }

        /// 打印任务保留最大分钟数 [单位：分钟]
        /// </summary>
        public static int PrintTaskExistsMaxMins
        {
            get
            {
                return TryConvertUtil.ToInt(ConfigUtil.GetConfig("PrintTaskExistsMaxMins"), 1);
            }
        }

        #endregion
    }
}
