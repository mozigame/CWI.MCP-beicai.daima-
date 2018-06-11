//---------------------------------------------
// 版权信息：版权所有(C) 2015，Coolwi
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2013/06/26        创建
//---------------------------------------------
using System;
using CWI.MCP.Common;
using Evt.Framework.Common;
using System.Data;

namespace CWI.MCP.Common
{
    public class SingleInstance<T> where T : new()
    {
        SingleInstance()
        {

        }

        private static readonly object _syncObject = new object();          //互斥锁
        private static T _instance;                                         //当前实例

        /// <summary>
        /// 获取当前实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            try
                            {
                                _instance = (T)Activator.CreateInstance(typeof(T), true);
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null)
                                {
                                    LogUtil.Error("构建对象" + typeof(T) + "发生异常", ex.InnerException);
                                }
                                else
                                {
                                    LogUtil.Error("构建对象" + typeof(T) + "发生异常", ex);
                                }
                                throw;
                            }
                        }
                    }
                }
                return _instance;
            }
        }
    }
}