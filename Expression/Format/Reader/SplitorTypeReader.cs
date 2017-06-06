using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Format.Element;

namespace Expression.Format.Reader
{
    public class SplitorTypeReader : IElementReader
    {
        public static string SPLITOR_CHAR = "(),";//所有分割符

        public Element Read(ExpressionReader sr)
        {

            int index = sr.GetCurrentIndex();
            int b = sr.Read();
            char c = (char)b;
            if (b == -1 || SPLITOR_CHAR.IndexOf(c) == -1)
            {
                throw new FormatException("不是有效的分割字符");
            }
            return new Element(c.ToString(), index,
                    ElementType.SPLITOR);
        }
    }
}
