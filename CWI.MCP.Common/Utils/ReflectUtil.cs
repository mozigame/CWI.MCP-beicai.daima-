//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Specialized;
using CWI.MCP.Common.Attributes;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public class ReflectUtil
    {
        #region 原始

        /// <summary>
        /// 类型属性映射表
        /// </summary>
        private static Dictionary<Type, PropertyInfo[]> _typePropsMap = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 获取类型属性，此方法用来提升反射的效率
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProps(Type t)
        {
            if (!_typePropsMap.ContainsKey(t))
            {
                _typePropsMap[t] = t.GetProperties();
            }
            return _typePropsMap[t];
        }

        /// <summary>
        /// 用来比较DataModel,即只包含普通类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool AreEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null && obj2 == null) { return true; }
            if (obj1 == null && obj2 != null) { return false; }
            if (obj1 != null && obj2 == null) { return false; }

            PropertyInfo[] props1 = GetProps(obj1.GetType());
            PropertyInfo[] props2 = GetProps(obj2.GetType());

            for (int i = 0; i < props1.Length; i++)
            {
                var val1 = props1[i].GetValue(obj1, null);
                var val2 = props2[i].GetValue(obj2, null);
                val1 = val1 ?? string.Empty;
                val2 = val2 ?? string.Empty;
                if (val1.ToString() != val2.ToString())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 用obj2的值覆盖obj1的值，用来合并DataModel,即只包含普通类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="propList"></param>
        /// <returns></returns>
        public static T Merge<T>(T obj1, T obj2, string[] propList)
        {
            if (obj1 == null || obj2 == null) { return obj1; }
            if (propList == null) { throw new ArgumentNullException("请指定待赋值的字段列表"); }

            PropertyInfo[] props1 = GetProps(obj1.GetType());
            PropertyInfo[] props2 = GetProps(obj2.GetType());

            var map1 = new Dictionary<string, PropertyInfo>();
            foreach (var prop in props1)
            {
                map1[prop.Name] = prop;
            }

            var map2 = new Dictionary<string, PropertyInfo>();
            foreach (var prop in props2)
            {
                map2[prop.Name] = prop;
            }

            foreach (var name in propList)
            {
                if (!map1.ContainsKey(name))
                {
                    throw new ArgumentException(typeof(T).ToString() + "类中不存在属性" + name);
                }

                map1[name].SetValue(obj1, map2[name].GetValue(obj2, null), null);
            }

            return obj1;
        }

        /// <summary>
        /// 将obj2的所有属性赋值给obj1的属性。属性只能包含普通类型。
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void Assign(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) { return; }

            PropertyInfo[] props1 = GetProps(obj1.GetType());
            PropertyInfo[] props2 = GetProps(obj2.GetType());

            var map1 = new Dictionary<string, PropertyInfo>();
            foreach (var prop in props1)
            {
                map1[prop.Name] = prop;
            }

            foreach (var prop in props2)
            {
                if (!map1.ContainsKey(prop.Name)) { continue; }
                map1[prop.Name].SetValue(obj1, prop.GetValue(obj2, null), null);
            }
        }

        #endregion

        #region 新增

        public static SortedDictionary<string, string> GetObjPayDescriptionFormat<T>(T t) where T : new()
        {
            Type type = t.GetType();
            PropertyInfo[] pinfos = type.GetProperties();

            SortedDictionary<string, string> attrSD = new SortedDictionary<string, string>();
            foreach (PropertyInfo p in pinfos)
            {
                object[] attr = p.GetCustomAttributes(typeof(PayDescriptionAttribute), true);
                if (attr.Length > 0)
                {
                    PayDescriptionAttribute pda = attr[0] as PayDescriptionAttribute;
                    object vobj = p.GetValue(t, null);
                    string v = vobj == null ? string.Empty : vobj.ToString();
                    attrSD.Add(pda.Description, v);
                }
            }
            return attrSD;
        }

        /// <summary>
        /// 绑定Http请求对象到对象T中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="httpRequest">Http请求对象</param>
        /// <returns></returns>
        public static T GetObjPayDescriptionFormat<T>(NameValueCollection httpRequest) where T : new()
        {
            T t = System.Activator.CreateInstance<T>();
            Type type = t.GetType();
            PropertyInfo[] pinfos = type.GetProperties();

            SortedDictionary<string, string> attrSD = new SortedDictionary<string, string>();
            foreach (PropertyInfo p in pinfos)
            {
                object[] attr = p.GetCustomAttributes(typeof(PayDescriptionAttribute), true);
                if (attr.Length > 0)
                {
                    PayDescriptionAttribute pda = attr[0] as PayDescriptionAttribute;
                    string pvalue = httpRequest[pda.Description];
                    p.SetValue(t, pvalue, null); //设置属性新值
                }
            }

            return t;
        }

        /// <summary>
        /// XML 文件反序列化成T对象
        /// </summary>
        /// <typeparam name="T">T 对象</typeparam>
        /// <param name="xml">xml 字符串</param>
        /// <returns>T 实体对象</returns>
        public static T GetObjXMLFormat<T>(string xml) where T : class,new()
        {
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(reader) as T;
        }

        #endregion
    }
}
