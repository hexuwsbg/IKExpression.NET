using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format.Reader
{
    public interface IElementReader
    {
        /// <summary>
        /// 从流中读取字符窜类型的ExpressionToken
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        Element Read(ExpressionReader sr);
    }
}
