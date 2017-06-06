using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format
{
    public class FormatException : Exception
    {
        public FormatException() : base()
        {
        }

        public FormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FormatException(string message) : base(message)
        {
        }
    }
}
