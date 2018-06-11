using CWI.MCP.Common;
using CWI.MCP.WinServ.Busy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWI.MCP.WinServ
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            DateTime dbNow = CommonUtil.GetDBDateTime();
            SysDateTime.InitDateTime(dbNow);

            BusyMain.Instance.Start();

            label1.Text = "服务已启动";
        }

        private void HandleError(Exception ex)
        {
            LogUtil.Error(ex.Message, ex);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            BusyMain.Instance.Start();
            label1.Text = "服务已启动";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            BusyMain.Instance.Stop();
            label1.Text = "服务已停止";
        }
    }
}
