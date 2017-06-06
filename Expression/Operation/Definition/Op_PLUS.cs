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
    /// 算术加
    /// </summary>
    public class Op_PLUS : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.PLUS;

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

            //集合类型PLUS运算单独处理
            if (DataType.DATATYPE_LIST == first.GetDataType()
                    || DataType.DATATYPE_LIST == second.GetDataType())
            {
                //目前不支持集合PLUS
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误");

            }
            else if (DataType.DATATYPE_STRING == first.GetDataType()
               || DataType.DATATYPE_STRING == second.GetDataType()
               || DataType.DATATYPE_NULL == first.GetDataType()
               || DataType.DATATYPE_NULL == second.GetDataType()
               || DataType.DATATYPE_BOOLEAN == first.GetDataType()
               || DataType.DATATYPE_BOOLEAN == second.GetDataType()
               || DataType.DATATYPE_DATE == first.GetDataType()
               || DataType.DATATYPE_DATE == second.GetDataType())
            {
                string firstString = "";
                string secondString = "";
                //字符窜连接运算,如果参数是null，则当作空字符窜处理
                if (null != first.GetStringValue())
                {
                    firstString = first.GetStringValue();
                }
                if (null != second.GetStringValue())
                {
                    secondString = second.GetStringValue();
                }
                string result = firstString + secondString;
                return new Constant(DataType.DATATYPE_STRING, result);

            }
            else if (null == first.DataValue || null == second.DataValue)
            {
                //抛NULL异常
                throw new NullReferenceException("操作符\"" + THIS_OPERATOR.Token + "\"参数为空");

            }
            else if (DataType.DATATYPE_DOUBLE == first.GetDataType()
               || DataType.DATATYPE_DOUBLE == second.GetDataType())
            {

                double result = first.GetDoubleValue() + second.GetDoubleValue();
                return new Constant(DataType.DATATYPE_DOUBLE, result);

            }
            else if (DataType.DATATYPE_FLOAT == first.GetDataType()
               || DataType.DATATYPE_FLOAT == second.GetDataType())
            {

                float result = first.GetFloatValue() + second.GetFloatValue();
                return new Constant(DataType.DATATYPE_FLOAT, result);

            }
            else if (DataType.DATATYPE_LONG == first.GetDataType()
               || DataType.DATATYPE_LONG == second.GetDataType())
            {

                long result = first.GetLongValue() + second.GetLongValue();
                return new Constant(DataType.DATATYPE_LONG, result);

            }
            else
            {

                int result = first.GetIntegerValue() + second.GetIntegerValue();
                return new Constant(DataType.DATATYPE_INT, result);
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

            //集合类型EQ运算单独处理
            if (DataType.DATATYPE_LIST == first.GetDataType()
                    || DataType.DATATYPE_LIST == second.GetDataType())
            {
                //目前不支持集合EQ比较，（太麻烦鸟）.考虑使用后期使用函数实现
                throw new IllegalExpressionException("操作符\"" + THIS_OPERATOR.Token + "\"参数类型错误"
                        , THIS_OPERATOR.Token
                        , opPositin);

            }

            if (DataType.DATATYPE_STRING == first.GetDataType()
                    || DataType.DATATYPE_STRING == second.GetDataType()
                    || DataType.DATATYPE_NULL == first.GetDataType()
                    || DataType.DATATYPE_NULL == second.GetDataType()
                    || DataType.DATATYPE_BOOLEAN == first.GetDataType()
                    || DataType.DATATYPE_BOOLEAN == second.GetDataType()
                    || DataType.DATATYPE_DATE == first.GetDataType()
                    || DataType.DATATYPE_DATE == second.GetDataType())
            {

                return new Constant(DataType.DATATYPE_STRING, null);

            }
            else if (DataType.DATATYPE_DOUBLE == first.GetDataType()
               || DataType.DATATYPE_DOUBLE == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_DOUBLE, 0.0D);

            }
            else if (DataType.DATATYPE_FLOAT == first.GetDataType()
               || DataType.DATATYPE_FLOAT == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_FLOAT, 0.0F);

            }
            else if (DataType.DATATYPE_LONG == first.GetDataType()
               || DataType.DATATYPE_LONG == second.GetDataType())
            {
                return new Constant(DataType.DATATYPE_LONG, 0L);

            }
            else
            {
                return new Constant(DataType.DATATYPE_INT, 0);
            }
        }
    }
}
