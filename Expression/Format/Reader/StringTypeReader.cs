using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class StringTypeReader : IElementReader
    {
        public static char START_MARK = '"';//字符窜开始标志
        public static char END_MARK = '"';//字符窜结束标志

        public static char ESCAPE_MARK = '\\';//转义符号

        
        public Element Read(ExpressionReader sr)
        {

            int index = sr.GetCurrentIndex();
            var sb = new StringBuilder();
            int b = sr.Read();
            if (b == -1 || b != START_MARK)
            {
                throw new FormatException("不是有效的字符窜开始");
            }

            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (c == ESCAPE_MARK)
                {//遇到转义字符
                    c = GetEscapeValue((char)sr.Read());
                }
                else if (c == END_MARK)
                {//遇到非转义的引号
                    return new Element(sb.ToString(), index, ElementType.STRING);
                }
                sb.Append(c);
            }
            throw new FormatException("不是有效的字符窜结束");
        }

        /// <summary>
        /// 可转义字符有\"nt
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static char GetEscapeValue(char c)
        {
            if (c == '\\' || c == '\"')
            {
                return c;
            }
            else if (c == 'n')
            {
                return '\n';
            }
            else if (c == 'r')
            {
                return '\r';
            }
            else if (c == 't')
            {
                return '\t';
            }
            throw new FormatException("字符转义出错");
        }
    }
}
