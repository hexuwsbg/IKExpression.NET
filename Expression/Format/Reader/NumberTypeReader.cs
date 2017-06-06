using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class NumberTypeReader : IElementReader
    {
        public static string NUMBER_CHARS = "01234567890.";//表示数值的字符
        public static string LONG_MARKS = "lL";//long的结尾标志
        public static string FLOAT_MARKS = "fF";//float的结尾标志
        public static string DOUBLE_MARKS = "dD";//double的结尾标志
        
        public Element Read(ExpressionReader sr)
        {

            int index = sr.GetCurrentIndex();
            string s = string.Empty;
            int b = -1;
            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (NUMBER_CHARS.IndexOf(c) == -1)
                {
                    if (LONG_MARKS.IndexOf(c) >= 0)
                    {
                        if (s.IndexOf(".") >= 0)
                        {//有小数点
                            throw new FormatException("long类型不能有小数点");
                        }
                        return new Element(s.ToString(), index, ElementType.LONG);
                    }
                    else if (FLOAT_MARKS.IndexOf(c) >= 0)
                    {

                        CheckDecimal(s);
                        return new Element(s.ToString(), index, ElementType.FLOAT);
                    }
                    else if (DOUBLE_MARKS.IndexOf(c) >= 0)
                    {

                        CheckDecimal(s);
                        return new Element(s.ToString(), index, ElementType.DOUBLE);
                    }
                    else
                    {
                        sr.Reset();
                        if (s.IndexOf(".") >= 0)
                        {//没有结束标志，有小数点，为double

                            CheckDecimal(s);
                            return new Element(s.ToString(), index, ElementType.DOUBLE);
                        }
                        else
                        {//没有结束标志，无小数点，为int
                            return new Element(s.ToString(), index, ElementType.INT);
                        }
                    }
                }
                s += c;
                sr.Mark(0);
            }
            //读到结未
            if (s.IndexOf(".") >= 0)
            {//没有结束标志，有小数点，为double

                CheckDecimal(s);
                return new Element(s.ToString(), index, ElementType.DOUBLE);
            }
            else
            {//没有结束标志，无小数点，为int
                return new Element(s.ToString(), index, ElementType.INT);
            }
        }

        /// <summary>
        /// 检查是否只有一个小数点
        /// </summary>
        /// <param name="s"></param>
        public static void CheckDecimal(string s)
        {
            if (s.IndexOf(".") != s.LastIndexOf("."))
            {
                throw new FormatException("数字最多只能有一个小数点");
            }
        }
    }
}
