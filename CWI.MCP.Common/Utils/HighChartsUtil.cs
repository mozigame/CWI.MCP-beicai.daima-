// 版权信息：版权所有(C) 2013，Evervictory Tech
// 变更历史：
//     姓名         日期              说明
// --------------------------------------------------------
//    王军锋     2013/08/22       创建
// --------------------------------------------------------

using System;
using System.Text;
using System.Net;
using System.IO;
using Evt.Framework.Common;
using System.Collections.Generic;
using System.Data;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// highcharts数据辅助类
    /// </summary>
    public class HighChartsUtil
    {
        /// <summary>
        ///  填充表格数据
        /// </summary>
        /// <param name="dt">需要填充表</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="chartTime">时间颗粒（hour、day、 month、year</param>
        public static void FillData(DataTable dt, DateTime startDate, DateTime endDate, string chartTime)
        {
            DateTime d = ConvertUtil.ToDateTime(startDate);
            DateTime end = ConvertUtil.ToDateTime(endDate);
            string chartX = string.Empty;
            switch (chartTime.ToLower())
            {
                case "hour":
                    for (int i = 0; i < 24; i++)
                    {
                        chartX = d.ToString("yyyy-MM-dd HH");
                        DataRow[] row = dt.Select("chartX='" + chartX + "'");
                        if (row.Length == 0)
                            dt.Rows.Add(new object[] { chartX, 0 });
                        d = d.AddHours(1);
                    }
                    break;
                case "day":
                    while (d <= end)
                    {
                        chartX = d.ToString("yyyy-MM-dd");
                        DataRow[] row = dt.Select("chartX='" + chartX + "'");
                        if (row.Length == 0)
                            dt.Rows.Add(new object[] { chartX, 0 });
                        d = d.AddDays(1);
                    }
                    break;
                case "month":
                    d = new DateTime(d.Year, d.Month, 1);
                    end = new DateTime(end.Year, end.Month, 1);
                    while (d <= end)
                    {
                        chartX = d.ToString("yyyy-MM");
                        DataRow[] row = dt.Select("chartX='" + chartX + "'");
                        if (row.Length == 0)
                            dt.Rows.Add(new object[] { chartX, 0 });
                        d = d.AddMonths(1);
                    }
                    break;
                case "year":
                    d = new DateTime(d.Year, 1, 1);
                    end = new DateTime(end.Year, 1, 1);
                    while (d <= end)
                    {
                        chartX = d.ToString("yyyy");
                        DataRow[] row = dt.Select("chartX='" + chartX + "'");
                        if (row.Length == 0)
                            dt.Rows.Add(new object[] { chartX, 0 });
                        d = d.AddYears(1);
                    }
                    break;
            }
        }

        /// <summary>
        /// 将表格数据转化list,为图表Y轴数据所用
        /// </summary>
        /// <param name="dt">需转化表格</param>
        public static List<string> CovertToChartY(DataTable dt)
        {
            List<string> list = new List<string>();
            DataView dv = dt.DefaultView;
            dv.Sort = "chartX asc";
            DataTable newDt = dv.ToTable();
            foreach (DataRow dr in newDt.Rows)
            {
                list.Add(dr["num"].ToString());
            }
            return list;
        }
    }
}