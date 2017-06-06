using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Functions
{
    public class StaticFunctionInvoker
    {
        public MethodInfo Method { private set; get; }

        public StaticFunctionInvoker(MethodInfo m)
        {
            Method = m;

        }

        /**
         * 执行方法
         * @param args 参数值列表
         * @return
         * @throws NoSuchMethodException
         * @throws IllegalAccessException
         * @throws InvocationTargetException
         */
        public object invoke(object[] args)
        {
            return Method.Invoke(null, args);
        }

    }
}
