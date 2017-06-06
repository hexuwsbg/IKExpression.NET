using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class FunctionTypeReader : IElementReader
    {
        public static char START_MARK = '$';//函数开始
        public static char END_MARK = '(';//函数结束

        /**
         * 从流中读取函数类型的ExpressionToken
         * @param sr
         * @return
         * @throws FormatException
         * @throws IOException
         */
        public Element Read(ExpressionReader sr)
        {

            int index = sr.GetCurrentIndex();
            var sb = new StringBuilder();
            int b = sr.Read();
            if (b == -1 || b != START_MARK)
            {
                throw new FormatException("不是有效的函数开始");
            }
            bool readStart = true;
            while ((b = sr.Read()) != -1)
            {
                char c = (char)b;
                if (c == END_MARK)
                {
                    if (sb.Length == 0)
                    {
                        throw new FormatException("函数名称不能为空");
                    }
                    sr.Reset();
                    return new Element(sb.ToString(), index, ElementType.FUNCTION);
                }
                //if (!Character.isJavaIdentifierPart(c)) {
                //	throw new FormatException("名称不能为非法字符：" + c);
                //}
                if (readStart)
                {
                    //if (!Character.isJavaIdentifierStart(c)) {
                    //	throw new FormatException("名称开头不能为字符：" + c);
                    //}
                    readStart = false;
                }
                sb.Append(c);
                sr.Mark(0);
            }
            throw new FormatException("不是有效的函数结束");
        }
    }
}
