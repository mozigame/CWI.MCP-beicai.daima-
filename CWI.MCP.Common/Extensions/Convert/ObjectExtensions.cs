//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/04/22          创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Evt.Framework.DataAccess;

namespace  CWI.MCP.Common.Extensions
{
    public static class ObjectExtensions
    {
        #region 比较实体属性值是否一致

        /// <summary>
        /// 比较属性值是否有改变
        /// </summary>
        /// <param name="thisModel"></param>
        /// <param name="model"></param>
        /// <param name="outComparePropertyNames"></param>
        /// <returns></returns>
        public static bool EqualsPropertyValue<TModel>(this TModel thisModel, TModel model, params string[] outComparePropertyNames)
        {
            if (model == null)
            {
                return true;
            }

            var type = thisModel.GetType();
            var infos = type.GetProperties();
            var outNames = new List<string>(outComparePropertyNames);
            
            foreach (PropertyInfo info in infos)
            {
                if (outNames.Contains(info.Name))
                {
                    continue;
                }

                var thisValue = info.GetValue(thisModel, null);
                var value = info.GetValue(model, null);

                if (thisValue == null && value == null)
                {
                    continue;
                }

                if (thisValue == null && value != null ||
                    thisValue != null && value == null)
                {
                    return false;
                }

                if (info.PropertyType.IsValueType || 
                    info.PropertyType.Name.StartsWith("String", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(thisValue.ToString(), value.ToString()))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!EqualsPropertyValueHelper(thisValue, value, outNames))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool EqualsPropertyValueHelper(object value1, object value2, List<string> outCompareNames)
        {
            var type = value1.GetType();
            var infos = type.GetProperties();

            foreach (PropertyInfo info in infos)
            {
                if (outCompareNames.Contains(info.Name))
                {
                    continue;
                }

                var pValue1 = info.GetValue(value1, null);
                var pValue2 = info.GetValue(value2, null);

                if (pValue1 == null && pValue2 == null)
                {
                    continue;
                }

                if (pValue1 == null && pValue2 != null ||
                    pValue1 != null && pValue2 == null)
                {
                    return false;
                }

                if (info.PropertyType.IsValueType ||
                    info.PropertyType.Name.StartsWith("String", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(pValue1.ToString(), pValue2.ToString()))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!EqualsPropertyValueHelper(pValue1, pValue2, outCompareNames))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
