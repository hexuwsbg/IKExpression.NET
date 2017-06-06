using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class OperatorTypeReader : IElementReader
    {
        private static HashSet<string> OPERATOR_WORDS = new HashSet<string>();

        static OperatorTypeReader()
        {

            OPERATOR_WORDS.Add("+");
            OPERATOR_WORDS.Add("-");
            OPERATOR_WORDS.Add(">");
            OPERATOR_WORDS.Add("<");
            OPERATOR_WORDS.Add(">=");
            OPERATOR_WORDS.Add("<=");
            OPERATOR_WORDS.Add("==");
            OPERATOR_WORDS.Add("!=");
            OPERATOR_WORDS.Add("*");
            OPERATOR_WORDS.Add("/");
            OPERATOR_WORDS.Add("%");
            OPERATOR_WORDS.Add("&&");
            OPERATOR_WORDS.Add("||");
            OPERATOR_WORDS.Add("!");
            OPERATOR_WORDS.Add("#");
            OPERATOR_WORDS.Add("?:");
            OPERATOR_WORDS.Add("?");
            OPERATOR_WORDS.Add(":");
        }

        /// <summary>
        /// 判断字符串是否是合法的操作符
        /// </summary>
        /// <param name="tokenText"></param>
        /// <returns></returns>
        public static bool IsOperatorWord(string tokenText)
        {
            return OPERATOR_WORDS.Contains(tokenText);
        }


        public Element Read(ExpressionReader sr)
        {
            int index = sr.GetCurrentIndex();
            var sb = new StringBuilder();
            int b = sr.Read();
            if (b == -1)
            {
                throw new FormatException("表达式已结束");
            }
            char c = (char)b;
            sb.Append(c);
            if (IsOperatorWord(sb.ToString()))
            {
                if (sb.Length == 1)
                {//两个符号的运算符优先，如<=，不应该认为是<运算符
                    sr.Mark(0);
                    b = sr.Read();
                    if (b != -1)
                    {
                        if (IsOperatorWord(sb.ToString() + (char)b))
                        {
                            return new Element(sb.ToString() + (char)b, index,
                                    ElementType.OPERATOR);
                        }
                    }
                    sr.Reset();
                }
                return new Element(sb.ToString(), index,
                        ElementType.OPERATOR);
            }

            while ((b = sr.Read()) != -1)
            {
                c = (char)b;
                sb.Append(c);
                if (IsOperatorWord(sb.ToString()))
                {
                    return new Element(sb.ToString(), index,
                            ElementType.OPERATOR);
                }
                if (VariableTypeReader.STOP_CHAR.IndexOf(c) >= 0)
                {//单词停止符
                    throw new FormatException("不是有效的运算符：" + sb.ToString());
                }
            }
            throw new FormatException("不是有效的运算符结束");
        }

        /// <summary>
        /// 测试是否为运算符
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static bool IsOperatorStart(ExpressionReader sr)
        {
            sr.Mark(0);
            try
            {
                var sb = new StringBuilder();
                int b = sr.Read();
                if (b == -1)
                {
                    return false;
                }
                char c = (char)b;
                sb.Append(c);
                if (IsOperatorWord(sb.ToString()))
                {
                    return true;
                }
                while ((b = sr.Read()) != -1)
                {
                    c = (char)b;
                    sb.Append(c);
                    if (IsOperatorWord(sb.ToString()))
                    {
                        return true;
                    }
                    if (VariableTypeReader.STOP_CHAR.IndexOf(c) >= 0)
                    {//单词停止符
                        return false;
                    }

                }
                return false;
            }
            finally
            {
                sr.Reset();
            }

        }
    }
}
