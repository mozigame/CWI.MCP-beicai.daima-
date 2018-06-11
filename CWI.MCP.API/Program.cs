using System;
using System.Windows.Forms;
using System.ServiceProcess;

using CWI.MCP.Common;

namespace CWI.MCP.API
{
    /// <summary>
    /// 项目入口类
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            LogUtil.SystemName = "MCP";

#if DEBUG
            if (ConfigUtil.IsDebugRunService)
            {
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] 
                { 
                  new HostService()
                };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StartForm());
            }

#else
            string[] argument = Environment.GetCommandLineArgs();
            if (argument != null && argument.Length > 1 && argument[1].Equals("1", StringComparison.CurrentCultureIgnoreCase))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StartForm());
            }
            else
            {
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] 
                { 
                  new HostService()
                };
                ServiceBase.Run(servicesToRun);
            }
#endif
        }
    }
}
