using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format.Reader
{
    public class ElementReaderFactory
    {
        /// <summary>
        /// 根据流开头构造不同的词元读取  流应该非空格开头
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IElementReader CreateElementReader(ExpressionReader reader)
        {
            //读一个char
            reader.Mark(0);
            int b = reader.Read();
            reader.Reset();
            if (b != -1)
            {
                char c = (char)b;
                try
                {
                    if (c == StringTypeReader.START_MARK)
                    {//"开头，构造字符串读取器
                        return new StringTypeReader();
                    }
                    else if (c == DateTypeReader.START_MARK)
                    {//[开头，构造日期读取器
                        return new DateTypeReader();
                    }
                    else if (c == FunctionTypeReader.START_MARK)
                    {//$开头，构造函数读取器
                        return new FunctionTypeReader();
    }
                    else if (SplitorTypeReader.SPLITOR_CHAR.IndexOf(c) >= 0)
                    {//如果是分隔符，构造分隔符读取器
                        return new SplitorTypeReader();
                    }
                    else if (NumberTypeReader.NUMBER_CHARS.IndexOf(c) >= 0)
                    {//以数字开头，构造数字类型读取器
                        return new NumberTypeReader();
                    }
                    else if (OperatorTypeReader.IsOperatorStart(reader))
                    {//如果前缀是运算符，构造运算符读取器
                        return new OperatorTypeReader();
                    }
                    else
                    {
                        return new VariableTypeReader();//否则构造一个变量读取器
                    }
                }
                catch (Exception e)
                {
                    throw new FormatException("", e);
                }

            }
            else
            {
                throw new FormatException("流已结束");
            }
        }
    }
}
