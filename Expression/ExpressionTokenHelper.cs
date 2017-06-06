using Expression.Format;
using Expression.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression
{
    public class ExpressionTokenHelper
    {

        public static bool IsNull(string s)
        {
            return "null".Equals(s);
        }

        public static bool IsBoolean(string s)
        {
            return bool.TrueString.Equals(s) || bool.FalseString.Equals(s);
        }

        /**
         * 判断是否是整数
         * @param s
         * @return
         */
        public static bool IsInteger(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 0)
            {
                if (s.Length == 1)
                {
                    return IsNumber(s[0]) && '.' != s[0];
                }
                else
                {
                    return (IsNumber(s[0]) && IsNumber(s[s.Length - 1]) && s.IndexOf('.') < 0);
                }
            }
            else
            {
                return false;
            }
        }

        /**
         * 判断是否是双精度浮点数
         * @param s
         * @return
         */
        public static bool IsDouble(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (IsNumber(s[0]) && IsNumber(s[s.Length - 1]) && s.IndexOf('.') >= 0);
            }
            else
            {
                return false;
            }
        }

        /**
         * 判断是否是长整数
         * @param s
         * @return
         */
        public static bool IsLong(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (IsNumber(s[0]) && s.EndsWith("L"));
            }
            else
            {
                return false;
            }

        }

        /**
         * 判断是否是浮点数
         * @param s
         * @return
         */
        public static bool IsFloat(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (IsNumber(s[0]) && s.EndsWith("F"));
            }
            else
            {
                return false;
            }

        }

        public static bool IsString(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (s[0] == '"');
            }
            else
            {
                return false;
            }

        }

        public static bool IsDateTime(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (s[0] == '[');
            }
            else
            {
                return false;
            }
        }

        /**
         * 是否是分隔符词元
         * @param s
         * @return
         */
        public static bool IsSplitor(string s)
        {
            return ",".Equals(s) || "(".Equals(s) || ")".Equals(s);
        }

        /**
         * 是否是函数词元
         * @param s
         * @return
         */
         
        public static bool IsFunction(string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return (s[0] == '$');
            }
            else
            {
                return false;
            }
        }

        /**
         * 是否是操作符
         * @param s
         * @return
         */
        public static bool IsOperator(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                try
                {
                    if (ExpressionParser.GetOperator(s) != null)
                        return true;
                    else
                        return false;
                }
                catch (ArgumentException e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool IsNumber(char c)
        {
            if ((c >= '0' && c <= '9') || c == '.')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
