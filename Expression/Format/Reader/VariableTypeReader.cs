using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class VariableTypeReader : IElementReader
    {
        public static string STOP_CHAR = "+-*/%^<>=&|!?:#$(),[]'\" \r\n\t";//词段的结束符

        public static string TRUE_WORD = "true";
        public static string FALSE_WORD = "false";

        public static string NULL_WORD = "null";


        private string ReadWord(ExpressionReader sr)
        {
            var sb = new StringBuilder();
            bool readStart = true;
            int b = -1;
            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (STOP_CHAR.IndexOf(c) >= 0 && !readStart)
                {//单词停止符,并且忽略第一个字符
                    sr.Reset();
                    return sb.ToString();
                }
                if (readStart)
                {
                    readStart = false;
                }
                sb.Append(c);
                sr.Mark(0);
            }
            return sb.ToString();
        }

        public Element Read(ExpressionReader sr)
        {
            int index = sr.GetCurrentIndex();
            string word = ReadWord(sr);

            if (TRUE_WORD.Equals(word) || FALSE_WORD.Equals(word))
            {
                return new Element(word, index, ElementType.BOOLEAN);
            }
            else if (NULL_WORD.Equals(word))
            {
                return new Element(word, index, ElementType.NULL);
            }
            else
            {
                return new Element(word, index, ElementType.VARIABLE);
            }
        }
    }
}
