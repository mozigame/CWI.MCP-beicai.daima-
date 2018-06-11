using System;
using System.Collections.Generic;
using System.Reflection;

namespace CWI.MCP.Models.Common
{
    /// <summary>
    /// 内容摘要：数据模型相互转换（这样子查询的时候就不需要各种手写列去做两个数据模型赋值的操作）
    /// 编码作者：ZLP
    /// 编码时间：2016-7-12
    /// </summary>
    public class ModelConvertHelper
    {
        /// <summary>
        /// 泛型集合转换
        /// </summary>
        /// <typeparam name="T">目标获取的类型</typeparam>
        /// <typeparam name="U">原类型</typeparam>
        /// <param name="modellist">需要转换的泛型集合</param>
        /// <returns></returns>
        public static IList<T> ToConvertModelS<T, U>(IList<U> modellist)
        {
            List<T> list = new List<T>();
            if (modellist == null)
            {
                return list;
            }
            foreach (U obj in modellist)
            {
                Type newt = typeof(T);
                Type oldt = obj.GetType();
                object newobj = Activator.CreateInstance(newt);
                PropertyInfo[] newproer = newt.GetProperties();
                PropertyInfo[] oldproer = oldt.GetProperties();
                foreach (PropertyInfo newProperty in newproer)
                {
                    foreach (PropertyInfo oldProperty in oldproer)
                    {
                        if (oldProperty.Name.ToUpper() == newProperty.Name.ToUpper())
                        {
                            if (newProperty.PropertyType.Name == "Nullable`1")
                            {
                                if (oldProperty.GetValue(obj, null) != null)
                                {
                                    newProperty.SetValue(newobj, Convert.ChangeType(oldProperty.GetValue(obj, null), newProperty.PropertyType.GetGenericArguments()[0]), null);
                                }
                            }
                            else
                            {
                                if (oldProperty.GetValue(obj, null) != null)
                                {
                                    newProperty.SetValue(newobj, Convert.ChangeType(oldProperty.GetValue(obj, null), newProperty.PropertyType), null);
                                }
                            }
                            break;
                        }

                    }
                }
                list.Add((T)newobj);
            }
            return list;
        }


        /// <summary>   
        /// Model类型互转
        /// </summary>   
        /// <param name="model"> 要转化的Model</param> 
        /// <returns></returns>   
        public static T ToConvertModel<T>(object model)
        {
            if (model == null)
            {
                return default(T);
            }
            Type newt = typeof(T);
            Type oldt = model.GetType();
            object newobj = Activator.CreateInstance(newt);
            PropertyInfo[] newproer = newt.GetProperties();
            PropertyInfo[] oldproer = oldt.GetProperties();
            foreach (PropertyInfo newProperty in newproer)
            {
                foreach (PropertyInfo oldProperty in oldproer)
                {
                    if (oldProperty.Name.ToUpper() == newProperty.Name.ToUpper())
                    {
                        //如果是可空类型处理
                        if (newProperty.PropertyType.Name == "Nullable`1")
                        {
                            if (oldProperty.GetValue(model, null) != null)
                            {
                                newProperty.SetValue(newobj, Convert.ChangeType(oldProperty.GetValue(model, null), newProperty.PropertyType.GetGenericArguments()[0]), null);
                            }
                        }
                        else
                        {
                            if (oldProperty.GetValue(model, null) != null)
                            {
                                newProperty.SetValue(newobj, Convert.ChangeType(oldProperty.GetValue(model, null), newProperty.PropertyType), null);
                            }
                        }
                        break;
                    }

                }
            }
            return (T)newobj;
        }  
     
    }
}
