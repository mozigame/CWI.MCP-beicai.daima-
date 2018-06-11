//版权信息：版权所有(C) 2014，PAIDUI.COM
//变更历史：
//     姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2014/03/28        创建
//--------------------------------------------------------

using System.Collections.Generic;
using System;
using System.Web;

using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 数据访问助手
    /// </summary>
    public static class DbUtil
    {
        private static DataManagerCollection _dataManagerCollection = new DataManagerCollection();

        public static DataManagerCollection DataManager
        {
            get
            {
                return _dataManagerCollection;
            }
        }

        public static TransactionManager GetTransactionManager()
        {
            TransactionManager tm = new TransactionManager();
            tm.AddDataManager(_dataManagerCollection.DataManagerMcp);
            return tm;
        }
    }

    /// <summary>
    /// 微云打数据管理器。
    /// </summary>
    public class DataManagerMcp : DataManager
    {
        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                return Settings.Intance.MCPDBConnectionString;
            }
        }

        /// <summary>
        /// 获取数据库类型。
        /// </summary>
        protected override DatabaseTypeEnum DatabaseType
        {
            get
            {
                return DatabaseTypeEnum.MySql;
            }
        }

        /// <summary>
        /// 获取数据库上下文ID。
        /// </summary>
        protected override string ContextID
        {
            get
            {
                return "DataManagerMCP";
            }
        }
    }

    /// <summary>
    /// 业务数据管理器。
    /// </summary>
    public class DataManagerBud : DataManager
    {
        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                return Settings.Intance.BUDDBConnectionString;
            }
        }

        /// <summary>
        /// 获取数据库类型。
        /// </summary>
        protected override DatabaseTypeEnum DatabaseType
        {
            get
            {
                return DatabaseTypeEnum.MySql;
            }
        }

        /// <summary>
        /// 获取数据库上下文ID。
        /// </summary>
        protected override string ContextID
        {
            get
            {
                return "DataManagerBUD";
            }
        }
    }

    /// <summary>
    /// 数据管理器集合
    /// </summary>
    public class DataManagerCollection
    {
        private DataManagerMcp _dataManagerMcp = new DataManagerMcp();
        private DataManagerBud _dataManagerBud = new DataManagerBud();

        public DataManager Current
        {
            get
            {
                return _dataManagerMcp;
            }
        }

        public DataManager DataManagerMcp
        {
            get
            {
                return _dataManagerMcp;
            }
        }

        public DataManager DataManagerBud
        {
            get
            {
                return _dataManagerBud;
            }
        }
    }
}

