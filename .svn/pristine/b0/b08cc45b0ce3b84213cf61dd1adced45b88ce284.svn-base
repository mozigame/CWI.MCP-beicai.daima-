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
using System.Globalization;
using Microsoft.VisualBasic;

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateExtensions
    {
        private static readonly TimeSpan _OneMinute = new TimeSpan(0, 1, 0);
        private static readonly TimeSpan _TwoMinutes = new TimeSpan(0, 2, 0);
        private static readonly TimeSpan _OneHour = new TimeSpan(1, 0, 0);
        private static readonly TimeSpan _TwoHours = new TimeSpan(2, 0, 0);
        private static readonly TimeSpan _OneDay = new TimeSpan(1, 0, 0, 0);
        private static readonly TimeSpan _TwoDays = new TimeSpan(2, 0, 0, 0);
        private static readonly TimeSpan _OneWeek = new TimeSpan(7, 0, 0, 0);
        private static readonly TimeSpan _TwoWeeks = new TimeSpan(14, 0, 0, 0);
        private static readonly TimeSpan _OneMonth = new TimeSpan(31, 0, 0, 0);
        private static readonly TimeSpan _TwoMonths = new TimeSpan(62, 0, 0, 0);
        private static readonly TimeSpan _OneYear = new TimeSpan(365, 0, 0, 0);
        private static readonly TimeSpan _TwoYears = new TimeSpan(730, 0, 0, 0);

        public static TimeSpan GetTimeSpan(this DateTime startTime, DateTime endTime)
        {
            return endTime - startTime;
        }

        public static string ToDateTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToDateTimeF(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            return CalculateAge(dateOfBirth, DateTime.Today);
        }

        public static int CalculateAge(this DateTime dateOfBirth, DateTime referenceDate)
        {
            int years = referenceDate.Year - dateOfBirth.Year;
            if (referenceDate.Month < dateOfBirth.Month || (referenceDate.Month == dateOfBirth.Month && referenceDate.Day < dateOfBirth.Day)) --years;
            return years;
        }

        public static int GetCountDaysOfMonth(this DateTime date)
        {
            var nextMonth = date.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1).AddDays(-1).Day;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetFirstDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
            {
                dt = dt.AddDays(1);
            }
            return dt;
        }

        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, GetCountDaysOfMonth(date));
        }

        public static DateTime GetLastDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetLastDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
            {
                dt = dt.AddDays(-1);
            }
            return dt;
        }

        public static bool IsToday(this DateTime dt)
        {
            return (dt.Date == DateTime.Today);
        }

        public static bool IsToday(this DateTimeOffset dto)
        {
            return (dto.Date.IsToday());
        }

        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds)
        {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }

        public static DateTime SetTime(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }

        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime)
        {
            return localDateTime.ToDateTimeOffset(null);
        }

        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime, TimeZoneInfo localTimeZone)
        {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);

            if (localDateTime.Kind != DateTimeKind.Unspecified)
            {
                localDateTime = new DateTime(localDateTime.Ticks, DateTimeKind.Unspecified);
            }

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            return date.GetFirstDayOfWeek(null);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date, CultureInfo cultureInfo)
        {
            cultureInfo = (cultureInfo ?? CultureInfo.CurrentCulture);

            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            while (date.DayOfWeek != firstDayOfWeek) date = date.AddDays(-1);

            return date;
        }

        public static DateTime GetLastDayOfWeek(this DateTime date)
        {
            return date.GetLastDayOfWeek(null);
        }

        public static DateTime GetLastDayOfWeek(this DateTime date, CultureInfo cultureInfo)
        {
            return date.GetFirstDayOfWeek(cultureInfo).AddDays(6);
        }

        public static DateTime GetWeekday(this DateTime date, DayOfWeek weekday)
        {
            return date.GetWeekday(weekday, null);
        }

        public static DateTime GetWeekday(this DateTime date, DayOfWeek weekday, CultureInfo cultureInfo)
        {
            var firstDayOfWeek = date.GetFirstDayOfWeek(cultureInfo);
            return firstDayOfWeek.GetNextWeekday(weekday);
        }

        public static DateTime GetNextWeekday(this DateTime date, DayOfWeek weekday)
        {
            while (date.DayOfWeek != weekday) date = date.AddDays(1);
            return date;
        }

        public static DateTime GetPreviousWeekday(this DateTime date, DayOfWeek weekday)
        {
            while (date.DayOfWeek != weekday) date = date.AddDays(-1);
            return date;
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, int hours, int minutes, int seconds)
        {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time)
        {
            return date.SetTime(time, null);
        }

        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time, TimeZoneInfo localTimeZone)
        {
            var localDate = date.ToLocalDateTime(localTimeZone);
            localDate.SetTime(time);
            return localDate.ToDateTimeOffset(localTimeZone);
        }

        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc)
        {
            return dateTimeUtc.ToLocalDateTime(null);
        }

        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc, TimeZoneInfo localTimeZone)
        {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);
            return TimeZoneInfo.ConvertTime(dateTimeUtc, localTimeZone).DateTime;
        }

        public static string ToAgo(this DateTime date)
        {
            TimeSpan timeSpan = date.GetTimeSpan(DateTime.Now);
            if (timeSpan < TimeSpan.Zero) return "未来";
            if (timeSpan < _OneMinute) return "现在";
            if (timeSpan < _TwoMinutes) return "1 分钟前";
            if (timeSpan < _OneHour) return String.Format("{0} 分钟前", timeSpan.Minutes);
            if (timeSpan < _TwoHours) return "1 小时前";
            if (timeSpan < _OneDay) return String.Format("{0} 小时前", timeSpan.Hours);
            if (timeSpan < _TwoDays) return "昨天";
            if (timeSpan < _OneWeek) return String.Format("{0} 天前", timeSpan.Days);
            if (timeSpan < _TwoWeeks) return "1 周前";
            if (timeSpan < _OneMonth) return String.Format("{0} 周前", timeSpan.Days / 7);
            if (timeSpan < _TwoMonths) return "1 月前";
            if (timeSpan < _OneYear) return String.Format("{0} 月前", timeSpan.Days / 31);
            if (timeSpan < _TwoYears) return "1 年前";

            return String.Format("{0} 年前", timeSpan.Days / 365);
        }

        public static int WeekOfYear(this DateTime datetime)
        {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            System.Globalization.CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime, System.Globalization.CalendarWeekRule weekrule)
        {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime, DayOfWeek firstDayOfWeek)
        {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            System.Globalization.CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime, System.Globalization.CalendarWeekRule weekrule, DayOfWeek firstDayOfWeek)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            return ciCurr.Calendar.GetWeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int GetQuarter(int month)
        {
            if (month <= 3) return 1;
            if (month <= 6) return 2;
            if (month <= 9) return 3;
            return 4;
        }

        public static bool IsWeekDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        public static bool IsWeekend(this DateTime value)
        {
            return value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;
        }
    }
}
