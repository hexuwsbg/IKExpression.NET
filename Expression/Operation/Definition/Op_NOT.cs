using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Metadata.BaseMetadata;

namespace Expression.Operation.Definition
{
    /// <summary>
    /// 逻辑否
    /// </summary>
    public class Op_NOT : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.NOT;


        public Constant Execute(Constant[] args)
        {

            if (args == null || args.Length != 1)
            {
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "参数个数不匹配");
            }

            Constant first = args[0];
            if (null == first || null == first.DataValue)
            {
                //抛NULL异常
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");
            }

            //如果第一参数为引用，则执行引用
            if (first.IsReference)
            {
                Reference firstRef = (Reference)first.DataValue;
                first = firstRef.Execute();
            }

            if (DataType.DATATYPE_BOOLEAN == first.GetDataType())
            {
                Boolean result = !first.GetBooleanValue();
                return new Constant(DataType.DATATYPE_BOOLEAN, result);

            }
            else
            {
                //抛异常
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

            }

        }

        /* (non-Javadoc)
         * @see org.wltea.expression.op.IOperatorExecution#verify(int, org.wltea.expression.ExpressionToken[])
         */
        public Constant Verify(int opPositin, BaseMetadata[] args)


        {

            if (args == null)
            {
                throw new ArgumentException("运算操作符参数为空");
            }
            if (args.Length != 1)
            {
                //抛异常
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数个数不匹配"
                            , THIS_OPERATOR.Token
                            , opPositin
                        );
            }

            BaseMetadata first = args[0];
            if (first == null)
            {
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");
            }

            if (DataType.DATATYPE_BOOLEAN == first.GetDataType())
            {
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else
            {
                //抛异常
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误"
                        , THIS_OPERATOR.Token
                        , opPositin
                        );

            }
        }
    }
}
