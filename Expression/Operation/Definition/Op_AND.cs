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
    /// 逻辑与
    /// </summary>
    public class Op_AND : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.AND;

        public Constant Execute(Constant[] args)
        {
            //参数校验
            if (args == null || args.Length != 2)
            {
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "操作缺少参数");
            }
            Constant first = args[1];
            if (null == first || null == first.DataValue)
            {
                //抛NULL异常
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");
            }
            Constant second = args[0];
            if (null == second || null == second.DataValue)
            {
                //抛NULL异常
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");
            }
            //运算：
            //如果第一参数为引用，则执行引用
            if (first.IsReference)
            {
                Reference firstRef = (Reference)first.DataValue;
                first = firstRef.Execute();
            }
            if (DataType.DATATYPE_BOOLEAN == first.GetDataType())
            {
                //对AND操作的优化处理，first为false，则忽略计算第二参数
                if (first.GetBooleanValue())
                {
                    //如果第二参数为引用，则执行引用
                    if (second.IsReference)
                    {
                        Reference secondRef = (Reference)second.DataValue;
                        second = secondRef.Execute();
                    }
                    if (DataType.DATATYPE_BOOLEAN == second.GetDataType())
                    {
                        return second;
                    }
                    else
                    {
                        //抛异常
                        throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"第二参数类型错误");
                    }

                }
                else
                {
                    return first;
                }
            }
            else
            {
                //抛异常
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"第一参数类型错误");

            }
        }
        
        public Constant Verify(int opPositin, BaseMetadata[] args)
        {

            if (args == null)
            {
                throw new ArgumentException("运算操作符参数为空");
            }
            if (args.Length != 2)
            {
                //抛异常
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数丢失"
                            , THIS_OPERATOR.Token
                            , opPositin
                        );
            }

            BaseMetadata first = args[1];
            BaseMetadata second = args[0];
            if (first == null || second == null)
            {
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");
            }

            if (DataType.DATATYPE_BOOLEAN == first.GetDataType()
                    && DataType.DATATYPE_BOOLEAN == second.GetDataType())
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
