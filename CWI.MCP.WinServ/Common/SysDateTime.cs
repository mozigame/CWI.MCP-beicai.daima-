//---------------------------------------------
// 版权信息：版权所有(C) 2017，Yingmei.me
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2017/04/19        创建
//---------------------------------------------
using System;
using System.Diagnostics;

namespace CWI.MCP.WinServ.Common
{
    public struct SysDateTime
    {
        /// <summary>
        /// 最小时间1900-01-01 00:00:00
        /// </summary>
        public static DateTime MinValue { get { return DateTime.MinValue; } }

        /// <summary>
        /// 1970
        /// </summary>
        public static DateTime _1970MinValue { get { return new DateTime(1970, 1, 1, 0, 0, 0); } }

        /// <summary>
        /// 自定义起始时间
        /// </summary>
        private static readonly DateTime _selftStartTime = new DateTime(1900, 1, 1);

        /// <summary>
        /// 锁
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// 时间
        /// </summary>
        private static long _secondticks = 0;

        /// <summary>
        /// 时间秒数
        /// </summary>
        private static long _secondticks_1970 = 0;

        /// <summary>
        /// 计时器
        /// </summary>
        private static Stopwatch sw = new Stopwatch();

        /// <summary>
        /// 1900-01-01 00:00:00 距离现在的秒数
        /// </summary>
        public static long SecondTicks { get { return _secondticks; } }

        /// <summary>
        /// 1970-01-01 00:00:00 距离现在的秒数
        /// </summary>
        public static long SecondTicks_1970
        {
            get
            {
                _secondticks_1970 = (long)(SysDateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                return _secondticks_1970;
            }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return _selftStartTime.AddSeconds(_secondticks + sw.ElapsedMilliseconds / 1000);
            }
        }

        static SysDateTime()
        {
            _secondticks = (long)(DateTime.Now - new DateTime(1900, 1, 1, 0, 0, 0)).TotalSeconds;
            sw.Start();
        }


        /// <summary>
        /// 初始化时间
        /// <para>初始化时执行</para>
        /// </summary>
        /// <param name="time">服务器时间</param>
        public static void InitDateTime(DateTime time)
        {
            lock (_locker)
            {
                _secondticks = (long)(time - new DateTime(1900, 1, 1, 0, 0, 0)).TotalSeconds;
                sw.Restart();
            }
        }
    }
}
