using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace CWI.MCP.WinServ
{
    static class Program
    {
            /// <summary>
            /// 应用程序的主入口点。
            /// </summary>
        static void Main()
        {
#if DEBUG
            //LogUtil.SystemName = "WinServ";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());

#else
            #region 增加服务进程管理

            //如果传递参数是1以则以 exe 方式启动
            string[] argument = Environment.GetCommandLineArgs();
            if (argument != null && argument.Length > 1 && argument[1].Equals("1", StringComparison.CurrentCultureIgnoreCase))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StartForm());
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				        new ServiceMain() 
			     };
                ServiceBase.Run(ServicesToRun);
            }
            #endregion
#endif
        }
    }
}
