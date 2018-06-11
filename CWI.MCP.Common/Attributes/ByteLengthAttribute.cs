//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/08/14        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace  CWI.MCP.Common.Attributes
{
    /// <summary>
    /// 指定字符串允许的最大字节数与最小字节数
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ByteLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="maximumLength">最大长度</param>
        public ByteLengthAttribute(int maximumLength) 
        {
            this.MaximumLength = maximumLength;
        }

        /// <summary>
        /// 获取或设置字符串的最大长度
        /// </summary>
        public int MaximumLength { get; private set; }

        /// <summary>
        /// 获取或设置字符串的最小长度
        /// </summary>
        public int MinimumLength { get; set; }

        /// <summary>
        /// 确定指定的对象是否有效
        /// </summary>
        /// <param name="value">要验证的对象</param>
        /// <returns>如果指定的对象有效，则为 true；否则为 false</returns>
        public override bool IsValid(object value)
        {
            string sValue = value == null ? string.Empty : value.ToString();
            if (MaximumLength >= MinimumLength && MinimumLength >= 0)
            {
                int byteCount = Encoding.GetEncoding("GB2312").GetByteCount(sValue);
                if (byteCount >= MinimumLength && byteCount <= MaximumLength)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("MaximumLength小于MinimumLength 或 MinimumLength为负数");
            }
        }
    }
}
