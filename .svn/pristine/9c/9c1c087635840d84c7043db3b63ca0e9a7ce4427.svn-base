//-------------------------------------------------
//版本信息:版权所有(C) 2014，COOLWI.COM
//变更历史:
//    姓名            日期             说明
//-------------------------------------------------
//   王军锋     2014/11/12 16:31:15           创建
//-------------------------------------------------

using System;

namespace CWI.MCP.Models.WebApi
{
    /// <summary>
    /// Service上下文 
    /// </summary>
    public sealed class ServiceContext
    {
        private ServiceContext()
        {
            RequestTerminal = new DefaultRequestTerminal();
        }

        /// <summary>
        /// 上下文
        /// </summary>
        [ThreadStatic]
        private static ServiceContext _current ;

        static ServiceContext()
        {
            _current = new ServiceContext();
        }

        /// <summary>
        /// 获取或设置一个接口,该接口指示如何获取或设置当前会话的用户信息
        /// </summary>
        public IServiceContextUser ContextUser { get; set; }

        /// <summary>
        /// 请求的设备信息
        /// </summary>
        public IRequestTerminal RequestTerminal { get; set; }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public LoginInfo currUser
        {
            get
            {
                if (ContextUser == null || ContextUser.CurrUser == null)
                {
                    throw new Exception("未初始化ServiceContext的Current");
                }
                return ContextUser.CurrUser;
            }
            set
            {
                if (ContextUser == null)
                {
                    ContextUser = new DefaultServiceContextUser();
                }
                ContextUser.CurrUser = value;
            }
        }

        /// <summary>
        /// Service上下文
        /// </summary>       
        public static ServiceContext Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new ServiceContext();
                }
                return _current;
            }
            set
            {
                _current = value;
            }
        }
    }

    public class TempThreadContext<T>
    {
        public T Data { get; set; }
        public ServiceContext Current { get; set; }

        public TempThreadContext()
        {
            Current = ServiceContext.Current;
        }
        public TempThreadContext(T data)
            : this()
        {
            Data = data;
        }
    }
}
