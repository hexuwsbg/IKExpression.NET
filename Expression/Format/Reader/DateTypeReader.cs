using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class DateTypeReader : IElementReader
    {
        public static char START_MARK = '[';//时间开始标志
        public static char END_MARK = ']';//时间结束标志

        public static string DATE_CHARS = "0123456789-:. ";

        public Element Read(ExpressionReader sr)
        {
            int index = sr.GetCurrentIndex();
            StringBuilder sb = new StringBuilder();
            int b = sr.Read();
            if (b == -1 || b != START_MARK)
            {
                throw new FormatException("不是有效的时间开始");
            }

            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (c == END_MARK)
                {
                    return new Element(FormatTime(sb.ToString()),
                            index, ElementType.DATE);
                }
                if (DATE_CHARS.IndexOf(c) == -1)
                {
                    throw new FormatException("时间类型不能包函非法字符：" + c);
                }
                sb.Append(c);
            }
            throw new FormatException("不是有效的时间结束");
        }

        /// <summary>
        ///  格式化时间字符窜 如2007-12-1 12:2会被格式化成2007-12-01 12:02:00
        /// </summary>
        /// <param name="time">字符窜表示的时间</param>
        /// <returns></returns>
        public static string FormatTime(string time)
        {
            if (time == null)
            {
                throw new FormatException("不是有效的时间表达式");
            }
            var sr = new CustomStringReader(time.Trim());
            var sb = new StringBuilder();
            int b = -1;
            try
            {
                while ((b = sr.Read()) != -1)
                {
                    char c = (char)b;
                    if (sb.Length < 4)
                    {//年
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("年份必需为4位数字");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 4)
                    {//
                        if (c != '-')
                        {
                            throw new FormatException("日期分割符必需为“－”");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 5)
                    {//月
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("月份必需为2位以内的数字");
                        }
                        sb.Append(c);
                        sr.Mark(0);
                        c = (char)sr.Read();
                        find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            sb.Insert(5, '0');
                            sr.Reset();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else if (sb.Length == 7)
                    {//
                        if (c != '-')
                        {
                            throw new FormatException("日期分割符必需为“－”");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 8)
                    {//日
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("日必需为2位以内的数字");
                        }
                        sb.Append(c);
                        sr.Mark(0);
                        c = (char)sr.Read();
                        find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            sb.Insert(8, '0');
                            sr.Reset();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else if (sb.Length == 10)
                    {//
                        if (c != ' ')
                        {
                            throw new FormatException("日期后分割符必需为“ ”");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 11)
                    {//小时
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("小时必需为2位以内的数字");
                        }
                        sb.Append(c);
                        sr.Mark(0);
                        c = (char)sr.Read();
                        find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            sb.Insert(11, '0');
                            sr.Reset();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else if (sb.Length == 13)
                    {//
                        if (c != ':')
                        {
                            throw new FormatException("时间分割符必需为“:”");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 14)
                    {//分
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("分钟必需为2位以内的数字");
                        }
                        sb.Append(c);
                        sr.Mark(0);
                        c = (char)sr.Read();
                        find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            sb.Insert(14, '0');
                            sr.Reset();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else if (sb.Length == 16)
                    {//
                        if (c != ':')
                        {
                            throw new FormatException("时间分割符必需为“:”");
                        }
                        sb.Append(c);
                    }
                    else if (sb.Length == 17)
                    {//秒
                        int find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            throw new FormatException("秒必需为2位以内的数字");
                        }
                        sb.Append(c);
                        sr.Mark(0);
                        c = (char)sr.Read();
                        find = DATE_CHARS.IndexOf(c);
                        if (find == -1 || find > 9)
                        {
                            sb.Insert(17, '0');
                            sr.Reset();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else
                    {
                        throw new FormatException("不是有效的时间表达式");
                    }
                }
            }
            catch (IOException e)
            {

                throw new FormatException("不是有效的时间表达式");
            }
            if (sb.Length == 10)
            {//补时间
                sb.Append(" 00:00:00");
            }
            else if (sb.Length == 16)
            {//补秒
                sb.Append(":00");
            }
            if (sb.Length != 19)
            {
                throw new FormatException("不是有效的时间表达式");
            }
            return sb.ToString();

        }
    }
}
