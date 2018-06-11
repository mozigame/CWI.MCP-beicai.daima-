//---------------------------------------------
// 版权信息：版权所有(C) 2014，PAIDUI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/11/15         创建
//---------------------------------------------

using System;
using System.IO;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 配置文件
    /// </summary>
    [Serializable]
    public class Settings
    {
        #region 定义配置文件

        /// <summary>
        /// 微云打数据库连接字符串
        /// </summary>
        public string MCPDBConnectionString { set; get; }

        /// <summary>
        /// 业务打印数据库连接字符串
        /// </summary>
        public string BUDDBConnectionString { set; get; }

        #endregion

        #region 获取配置实体
        /// <summary>
        /// 文件路径
        /// </summary>
        private const string SETTING_FILE_NAME = "Settings.config";

        /// <summary>
        /// 文件名称
        /// </summary>
        private const string SETTING_FILE_DIR = "Data";

        private static Settings _intance = null;

        /// <summary>
        /// 配置信息
        /// </summary>
        public static Settings Intance { get { return _intance; } }

        static Settings()
        {
            LoadSetting();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void LoadSetting()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTING_FILE_DIR, SETTING_FILE_NAME);
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("未找到配置文件！");
            }

            _intance = SerializerUtil.XmlDeserialize<Settings>(file);

            if (_intance == null)
            {
                throw new BusinessException("读取配置文件出错，请检查！");
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static bool Save()
        {
            
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTING_FILE_DIR, SETTING_FILE_NAME);
            bool flag = SerializerUtil.XmlSerialize(Intance, file);
            return flag;
        }
        #endregion
    }
}
