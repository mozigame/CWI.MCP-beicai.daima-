using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using CWI.MCP.Common;
using McpTcpServer.Properties;
using System.Diagnostics;

namespace McpTcpServer
{
    public partial class SysMainForm : Form
    {
        #region 变量

        //处理关机指令
        private static int WM_QUERYENDSESSION = 0x11;
        private static bool systemShutdown = false;

        //提示窗是否关闭
        private static bool _isClose = true;

        #endregion

        #region 构造函数

        /// <summary>
        /// 登录窗体对象
        /// </summary>
        private static SysMainForm _instance = null;

        /// <summary>
        /// 主窗体构造函数
        /// </summary>
        private SysMainForm()
        {
            InitializeComponent();
            McpServer.Instance.Start();
            this.Text = ConfigUtil.GetConfig("servName");
        }

        /// <summary>
        /// 获取主窗体实例
        /// </summary>
        public static SysMainForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SysMainForm();
                }
                return _instance;
            }
        }

        #endregion

        #region 窗体事件

        /// <summary>
        /// 菜单显示
        /// </summary>
        internal void InitMenu()
        {
            this.Visible = true;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysMainForm_Load(object sender, EventArgs e)
        {
            //设置通知栏图标
            this.notifyIcon1.Icon = Resources._16X16;
            this.notifyIcon1.Visible = true;

            //初始化右键菜单
            InitMenu();

            //初始化连接列表
            LoadList();
        }

        /// <summary>
        /// 窗体尺寸切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysMainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关机指令直接关机
            if (systemShutdown)
            {
                systemShutdown = false;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                e.Cancel = true;
            }
        }


        /// <summary>
        /// 状态栏图标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            NotifyIconClick();
        }

        /// <summary>
        /// 状态栏图标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            NotifyIconClick();
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            var dr = MessageBox.Show("您确定要退出吗？", "退出系统", messButton);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                notifyIcon1.Visible = false;
                Application.ExitThread();
                Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 托盘图标双单击事件
        /// </summary>
        private void NotifyIconClick()
        {
            //加载列表
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                LoadList();

                Rectangle rect = Screen.GetWorkingArea(this);
                this.Location = new Point((rect.Width - 760) / 2, (rect.Height - 366) / 2);
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        /// <summary>
        /// 退出程序事件
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //关机指令
            if (m.Msg == WM_QUERYENDSESSION)
            {
                this.notifyIcon1.Visible = false;
                systemShutdown = true;
            }

            base.WndProc(ref m);
        }

        private void LoadList()
        {
            dataGridView1.DataSource = McpServer.Instance.GetConnetionList();
            dataGridView1.Update();
        }

        #endregion
    }
}
