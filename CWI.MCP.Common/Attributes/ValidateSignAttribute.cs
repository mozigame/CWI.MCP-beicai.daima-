using System;

namespace  CWI.MCP.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateSignAttribute : Attribute
    {
        /// <summary>
        /// 是否需要验证签名
        /// </summary>
        public bool IsValidate { get; set; }

        /// <summary>
        /// 签名名称
        /// </summary>
        public string SignName { get; set; }

        /// <summary>
        /// 签名构造函数
        /// </summary>
        /// <param name="isValidate">是否需要验证签名</param>
        /// <param name="signName">签名</param>
        public ValidateSignAttribute(bool isValidate, string signName)
        {
            this.IsValidate = isValidate;
            this.SignName = signName;
        }
    }
}
