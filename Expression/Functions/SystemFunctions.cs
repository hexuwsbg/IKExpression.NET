using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Functions
{
    public class SystemFunctions
    {
        //	//字符串包含比较
        //	CONTAINS
        //	//字符串前缀比较
        //	STARTSWITH
        //	//字符串后缀比较
        //	ENDSWITH
        //  日期计算函数
        //  CALCDATE
        //  当前日期函数
        //  SYSDATE
        //  日期相等比较
        //  DAYEQUALS

        /**
         * 字符串包含比较
         * @param str1
         * @param str2
         * @return
         */
        public static bool Contains(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new ArgumentNullException("函数\"CONTAINS\"参数为空");
            }
            return str1.IndexOf(str2) >= 0;
        }

        /**
         * 字符串前缀比较
         * @param str1
         * @param str2
         * @return
         */
        public static bool StartsWith(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new ArgumentNullException("函数\"STARTSWITH\"参数为空");
            }
            return str1.StartsWith(str2);
        }

        /**
         * 字符串后缀比较
         * @param str1
         * @param str2
         * @return
         */
        public static bool EndsWith(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new ArgumentNullException("函数\"ENDSWITH\"参数为空");
            }
            return str1.EndsWith(str2);
        }

        /**
         * 日期计算
         * @param date 原始的日期
         * @param years 年份偏移量
         * @param months 月偏移量
         * @param days 日偏移量
         * @param hours 小时偏移量
         * @param minutes 分偏移量
         * @param seconds 秒偏移量
         * @return 偏移后的日期
         */
        public static DateTime CalcDate(DateTime date, int years, int months, int days, int hours, int minutes, int seconds)
        {
            if (date == null)
            {
                throw new ArgumentNullException("函数\"CALCDATE\"参数为空");
            }
            date = date.AddYears(years);
            date = date.AddMonths(months);
            date = date.AddDays(days);
            date = date.AddHours(hours);
            date = date.AddMinutes(minutes);
            date = date.AddSeconds(seconds);
            return date;
        }

        /**
         * 获取系统当前时间
         * @return
         */
        public static DateTime SysDate()
        {
            return DateTime.Now;
        }

        /**
         * 日期相等比较，精确到天
         * @param date1
         * @param date2
         * @return
         */
        public static bool DayEquals(DateTime date1, DateTime date2)
        {
            return date1.Date.Equals(date2.Date);
        }
    }
}
