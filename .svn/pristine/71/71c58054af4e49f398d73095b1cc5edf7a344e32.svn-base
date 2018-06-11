// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2014/06/08       创建
// --------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace  CWI.MCP.Common.Attributes
{
    /// <summary>
    /// 定义Enum的描述
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 锁变量
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// 缓存数据
        /// </summary>
        private static Dictionary<string, string> enumDescriptions = new Dictionary<string, string>();

        /// <summary>
        /// 描述
        /// </summary>
        /// <param name="des">描述内容</param>
        public EnumDescriptionAttribute(string des)
        {
            this.Description = des;
        }

        /// <summary>
        /// 根据动作得到描述
        /// </summary>
        /// <param name="a">枚举</param>
        /// <returns>返回枚举描述</returns>
        public static string GetDescription(Enum a)
        {
            Type type = a.GetType();
            return GetDescription(type, a);
        }

        /// <summary>
        /// 获取枚举描述信息
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDescription(Type enumType, Enum enumValue)
        {
            Type type = enumType;
            string key = type.ToString() + "." + enumValue.ToString();
            if (enumDescriptions.ContainsKey(key))
                return enumDescriptions[key];

            FieldInfo f = type.GetField(enumValue.ToString());
            string result = string.Empty;
            if (f != null)
            {
                EnumDescriptionAttribute attr = Attribute.GetCustomAttribute(f, typeof(EnumDescriptionAttribute)) as EnumDescriptionAttribute;
                if (attr != null)
                    result = attr.Description;
                lock (locker)
                {
                    enumDescriptions.Add(key, result);
                }
                return result;
            }

            return result;
        }

        /// <summary>
        /// 枚举值转换为键值对列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetDictionary(Enum enumType)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(enumType.GetType()))
            {
                result.Add(item.GetHashCode(), item.ToString());
            }
            return result;
        }
    }
}
