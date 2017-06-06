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
    /// 逻辑大等于
    /// </summary>
    public class Op_GE : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.GE;

        public Constant Execute(Constant[] args)
        {
            if (args == null || args.Length != 2)
            {
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "参数个数不匹配");
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

            //如果第一参数为引用，则执行引用
            if (first.IsReference)
            {
                Reference firstRef = (Reference)first.DataValue;
                first = firstRef.Execute();
            }
            //如果第二参数为引用，则执行引用
            if (second.IsReference)
            {
                Reference secondRef = (Reference)second.DataValue;
                second = secondRef.Execute();
            }


            if (DataType.DATATYPE_DATE == first.GetDataType()
                    && DataType.DATATYPE_DATE == second.GetDataType())
            {
                //日期类型比较	
                int result = first.GetDateValue().CompareTo(second.GetDateValue());
                if (result >= 0)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                }
                else
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                }

            }
            else if (DataType.DATATYPE_STRING == first.GetDataType()
               && DataType.DATATYPE_STRING == second.GetDataType())
            {
                //字窜类型比较
                int result = first.GetStringValue().CompareTo(second.GetStringValue());
                if (result >= 0)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                }
                else
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                }

            }
            else if ((DataType.DATATYPE_DOUBLE == first.GetDataType()
                   || DataType.DATATYPE_FLOAT == first.GetDataType()
                   || DataType.DATATYPE_LONG == first.GetDataType()
                   || DataType.DATATYPE_INT == first.GetDataType())
               &&
               (DataType.DATATYPE_DOUBLE == second.GetDataType()
                       || DataType.DATATYPE_FLOAT == second.GetDataType()
                       || DataType.DATATYPE_LONG == second.GetDataType()
                       || DataType.DATATYPE_INT == second.GetDataType())

               )
            {
                //数值类型比较，全部转换成double	
                int result = first.GetDoubleValue().CompareTo(second.GetDoubleValue());
                if (result >= 0)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                }
                else
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                }
            }
            else
            {
                //GE操作不支持其他类型，抛异常
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

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
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数个数不匹配"
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

            if (DataType.DATATYPE_DATE == first.GetDataType()
                    && DataType.DATATYPE_DATE == second.GetDataType())
            {
                //日期类型比较	
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else if (DataType.DATATYPE_STRING == first.GetDataType()
               && DataType.DATATYPE_STRING == second.GetDataType())
            {
                //字窜类型比较
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else if ((DataType.DATATYPE_DOUBLE == first.GetDataType()
                   || DataType.DATATYPE_FLOAT == first.GetDataType()
                   || DataType.DATATYPE_LONG == first.GetDataType()
                   || DataType.DATATYPE_INT == first.GetDataType())
               &&
               (DataType.DATATYPE_DOUBLE == second.GetDataType()
                       || DataType.DATATYPE_FLOAT == second.GetDataType()
                       || DataType.DATATYPE_LONG == second.GetDataType()
                       || DataType.DATATYPE_INT == second.GetDataType())

               )
            {
                //数值类型比较
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else
            {
                //GE操作不支持其他类型，抛异常
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

            }
        }
    }
}
