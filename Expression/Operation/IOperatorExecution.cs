using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Operation
{
    public interface IOperatorExecution
    {
        /// <summary>
        /// 执行操作符运算
        /// </summary>
        /// <param name="args">注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数</param>
        /// <returns>常量型的执行结果</returns>
        Constant Execute(Constant[] args);

        /// <summary>
        /// 验证操作符参数是否合法
        /// </summary>
        /// <param name="opPositin"></param>
        /// <param name="args">注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数</param>
        /// <returns>常量型的执行结果</returns>
        Constant Verify(int opPositin, BaseMetadata[] args);
    }
}
