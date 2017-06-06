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
    /// 取负
    /// </summary>
    public class Op_NG : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.NG;

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

            if (DataType.DATATYPE_DOUBLE == first.GetDataType())
            {
                double result = 0 - first.GetDoubleValue();
                return new Constant(DataType.DATATYPE_DOUBLE, result);

            }
            else if (DataType.DATATYPE_FLOAT == first.GetDataType())
            {
                float result = 0 - first.GetFloatValue();
                return new Constant(DataType.DATATYPE_FLOAT, result);

            }
            else if (DataType.DATATYPE_LONG == first.GetDataType())
            {
                long result = 0 - first.GetLongValue();
                return new Constant(DataType.DATATYPE_LONG, result);

            }
            else if (DataType.DATATYPE_INT == first.GetDataType())
            {
                int result = 0 - first.GetIntegerValue();
                return new Constant(DataType.DATATYPE_INT, result);

            }
            else
            {
                //抛异常
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

            }

        }


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

            if (DataType.DATATYPE_DOUBLE == first.GetDataType())
            {
                return new Constant(DataType.DATATYPE_DOUBLE, 0.0D);

            }
            else if (DataType.DATATYPE_FLOAT == first.GetDataType())
            {
                return new Constant(DataType.DATATYPE_FLOAT, 0F);

            }
            else if (DataType.DATATYPE_LONG == first.GetDataType())
            {
                return new Constant(DataType.DATATYPE_LONG, 0L);

            }
            else if (DataType.DATATYPE_INT == first.GetDataType())
            {
                return new Constant(DataType.DATATYPE_INT, 0);

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
