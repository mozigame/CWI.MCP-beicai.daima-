//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/11/14         创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// DataTable扩展
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static class DataTableExtensions
    {
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            if (dt == null)
            {
                return null;
            }

            //创建一个属性的列表   
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口   
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表    
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合   
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例   
                TResult ob = new TResult();
                //找到对应的数据  并赋值   
                prlist.ForEach(p =>
                {
                    if (row[p.Name] != DBNull.Value)
                    {
                        Type dataType = row[p.Name].GetType();
                        switch (dataType.Name)
                        {
                            case "MySqlDateTime":
                                p.SetValue(ob, TryConvertUtil.ToDateTime(row[p.Name]), null);
                                break;
                            case "Int64":
                            case "Int32":
                                {
                                    if (p.PropertyType == typeof(bool))
                                    {
                                        p.SetValue(ob, TryConvertUtil.ToInt(row[p.Name]) == 1, null);
                                    }
                                    else
                                    {
                                        p.SetValue(ob, TryConvertUtil.ToInt(row[p.Name]), null);
                                    }
                                    break;
                                }
                            default:
                                p.SetValue(ob, row[p.Name], null);
                                break;
                        }
                    }
                });
                //放入到返回的集合中.   
                oblist.Add(ob);
            }
            return oblist;
        }

        public static TResult ToModel<TResult>(this DataRow row) where TResult : class, new()
        {
            if (row == null)
            {
                return null;
            }

            DataTable dt = row.Table;
            //创建一个属性的列表   
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口   
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表    
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合   
            List<TResult> oblist = new List<TResult>();

            //创建TResult的实例   
            TResult ob = new TResult();
            //找到对应的数据  并赋值   
            prlist.ForEach(p =>
            {
                if (row[p.Name] != DBNull.Value)
                {
                    Type dataType = row[p.Name].GetType();
                    switch (dataType.Name)
                    {
                        case "MySqlDateTime":
                            p.SetValue(ob, TryConvertUtil.ToDateTime(row[p.Name]), null);
                            break;
                        default:
                            p.SetValue(ob, row[p.Name], null);
                            break;
                    }
                }
            });

            //放入到返回的集合中.   
            return ob;

        }

        public static DataTable OrderBy(this DataTable dt, string orderBy)
        {
            dt.DefaultView.Sort = orderBy;
            return dt.DefaultView.ToTable();
        }

        public static DataTable Where(this DataTable dt, string where)
        {
            DataTable resultDt = dt.Clone();
            DataRow[] resultRows = dt.Select(where);
            foreach (DataRow dr in resultRows) resultDt.Rows.Add(dr.ItemArray);
            return resultDt;
        }

        public static List<TResult> OrderBy<TResult>(this DataTable dt, string orderBy) where TResult : class, new()
        {
            return dt.OrderBy(orderBy).ToList<TResult>();
        }

        public static List<TResult> Where<TResult>(this DataTable dt, string where) where TResult : class, new()
        {
            return dt.Where(where).ToList<TResult>();
        }

        public static List<TResult> ToPage<TResult>(this DataTable dt, int pageIndex, int pageSize, out int totalRecords) where TResult : class, new()
        {
            totalRecords = dt.Rows.Count;
            int startRow = (pageIndex - 1) * pageSize;
            int endRow = startRow + pageSize;
            if (startRow > totalRecords || startRow < 0) { startRow = 0; endRow = pageSize; }
            if (endRow > totalRecords + pageSize) { startRow = totalRecords - pageSize; endRow = totalRecords; }

            DataTable dt2 = dt.Clone();
            for (int i = startRow; i < endRow; i++) { if (i >= totalRecords) break; dt2.Rows.Add(dt.Rows[i].ItemArray); }

            return dt2.ToList<TResult>();
        }
    }
}
