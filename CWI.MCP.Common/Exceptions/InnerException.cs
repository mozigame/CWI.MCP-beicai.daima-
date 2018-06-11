// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2014/04/09       创建
// --------------------------------------------

using System;
using System.Runtime.Serialization;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 系统内部异常
    /// </summary>
    public class InnerException : Exception
    {
        /// <summary>
        /// 初始化  CWI.MCP.Infrastructure.InnerException 类的新实例。
        /// </summary>
        public InnerException()
            : base()
        {
        }

        /// <summary>
        /// 初始化  CWI.MCP.Infrastructure.InnerException 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public InnerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化  CWI.MCP.Infrastructure.InnerException 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="innerException">内部异常。</param>
        public InnerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 用序列化数据初始化  CWI.MCP.Infrastructure.InnerException 类的新实例。
        /// </summary>
        /// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context">它包含有关源或目标的上下文信息。</param>
        protected InnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

