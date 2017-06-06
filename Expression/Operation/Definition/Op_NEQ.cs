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
    /// 逻辑不等
    /// </summary>
    public class Op_NEQ : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.NEQ;

        public Constant Execute(Constant[] args)
        {
            if (args == null || args.Length != 2)
            {
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "参数个数不匹配");
            }

            Constant first = args[1];
            Constant second = args[0];
            if (first == null || second == null)
            {
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

            //集合类型EQ运算单独处理
            if (DataType.DATATYPE_LIST == first.GetDataType()
                    || DataType.DATATYPE_LIST == second.GetDataType())
            {
                //目前不支持集合EQ比较，（太麻烦鸟）.考虑使用后期使用函数实现
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

            }

            //NEQ支持不同类型数据的null比较，专门对null的判断
            if (DataType.DATATYPE_NULL == first.GetDataType())
            {
                if (null != second.DataValue)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                }
                else
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                }

            }
            else if (DataType.DATATYPE_NULL == second.GetDataType())
            {
                if (null != first.DataValue)
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
                if (DataType.DATATYPE_BOOLEAN == first.GetDataType()
                        && DataType.DATATYPE_BOOLEAN == second.GetDataType())
                {
                    Boolean firstValue = first.GetBooleanValue();
                    Boolean secondValue = second.GetBooleanValue();
                    //if (firstValue != null)
                    //{
                        return new Constant(DataType.DATATYPE_BOOLEAN, !firstValue.Equals(secondValue));
                    //}
                    //else if (secondValue == null)
                    //{
                    //    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    //}
                    //else
                    //{
                    //    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                    //}

                }
                else if (DataType.DATATYPE_DATE == first.GetDataType()
                       && DataType.DATATYPE_DATE == second.GetDataType())
                {
                    //日期比较精确到秒
                    string firstValue = first.GetDataValueText();
                    string secondValue = second.GetDataValueText();
                    if (firstValue != null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, !firstValue.Equals(secondValue));
                    }
                    else if (secondValue == null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    }
                    else
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, true);
                    }

                }
                else if (DataType.DATATYPE_STRING == first.GetDataType()
                       && DataType.DATATYPE_STRING == second.GetDataType())
                {
                    string firstValue = first.GetStringValue();
                    string secondValue = second.GetStringValue();
                    if (firstValue != null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, !firstValue.Equals(secondValue));
                    }
                    else if (secondValue == null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    }
                    else
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, true);
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
                    double firstValue = first.GetDoubleValue();
                    double secondValue = second.GetDoubleValue();
                    //if (firstValue != null && secondValue != null)
                    //{
                    int result = firstValue.CompareTo(secondValue);
                    if (result != 0)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, true);
                    }
                    else
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    }
                    //}
                    //else if (firstValue == null && secondValue == null)
                    //{
                    //    return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    //}
                    //else
                    //{
                    //    return new Constant(DataType.DATATYPE_BOOLEAN, true);
                    //}

                }
                else if (DataType.DATATYPE_OBJECT == first.GetDataType()
                       && DataType.DATATYPE_OBJECT == second.GetDataType())
                {
                    object firstValue = first.DataValue;
                    object secondValue = second.DataValue;
                    if (firstValue != null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, !firstValue.Equals(secondValue));
                    }
                    else if (secondValue == null)
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, false);
                    }
                    else
                    {
                        return new Constant(DataType.DATATYPE_BOOLEAN, true);
                    }

                }
                else
                {
                    //如果操作数没有NULL型，且类型不同，抛异常（如果有校验，校验时就应该抛异常）
                    throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");
                }
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

            //集合类型NEQ运算单独处理
            if (DataType.DATATYPE_LIST == first.GetDataType()
                    || DataType.DATATYPE_LIST == second.GetDataType())
            {
                //目前不支持集合EQ比较，（太麻烦鸟）.考虑使用后期使用函数实现
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误"
                        , THIS_OPERATOR.Token
                        , opPositin);

            }

            if (DataType.DATATYPE_NULL == first.GetDataType()
                    || DataType.DATATYPE_NULL == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else if (DataType.DATATYPE_BOOLEAN == first.GetDataType()
               && DataType.DATATYPE_BOOLEAN == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else if (DataType.DATATYPE_DATE == first.GetDataType()
               && DataType.DATATYPE_DATE == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else if (DataType.DATATYPE_STRING == first.GetDataType()
               && DataType.DATATYPE_STRING == second.GetDataType())
            {
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
            else if (DataType.DATATYPE_OBJECT == first.GetDataType()
               && DataType.DATATYPE_OBJECT == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_BOOLEAN, false);

            }
            else
            {
                //抛异常
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误"
                        , THIS_OPERATOR.Token
                        , opPositin);
            }
        }
    }
}
