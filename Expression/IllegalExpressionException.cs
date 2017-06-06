using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression
{
    public class IllegalExpressionException : Exception
    {
        /**
	 * 错误标识
	 */
        public string ErrorTokenText { set; get; }
        /**
         * 错误位置
         */
        public int ErrorPosition { set; get; } = -1;

        public IllegalExpressionException() : base()
        {
        }

        public IllegalExpressionException(string msg) : base(msg)
        {
        }

        public IllegalExpressionException(string msg, string errorTokenText) : base(msg)
        {
            this.ErrorTokenText = errorTokenText;
        }

        public IllegalExpressionException(string msg, string errorTokenText, int errorPosition) : base(msg)
        {
            this.ErrorPosition = errorPosition;
            this.ErrorTokenText = errorTokenText;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder(this.Message);
            sb.Append("\r\n处理对象：").Append(ErrorTokenText);
            sb.Append("\r\n处理位置：").Append(ErrorPosition == -1 ? " unknown " : ErrorPosition.ToString());
            return sb.ToString();
        }
    }
}
