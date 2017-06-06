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
    /// 集合添加
    /// </summary>
    public class Op_APPEND : IOperatorExecution
    {
        public static Operator THIS_OPERATOR = Operator.APPEND;

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
            return Append(first, second);
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
            //APPEND接受任何类型的参数，总是返回Collection类型的常量
            return new Constant(DataType.DATATYPE_LIST, null);

        }

        // 合并两个常量对象
        private Constant Append(Constant arg1, Constant arg2)
        {
            if (arg1 == null || arg2 == null)
            {
                throw new ArgumentException("操作符\"" + THIS_OPERATOR.Token + "\"参数丢失");
            }

            List<object> resultCollection = new List<object>();
            //合并参数一
            if (DataType.DATATYPE_LIST == arg1.GetDataType())
            {
                if (arg1.GetCollection() != null)
                {
                    resultCollection.AddRange(arg1.GetCollection());
                }
            }
            else
            {
                //try
                //{
                //    object object = arg1.toJavaObject();
                //    resultCollection.add(object);
                //}
                //catch (ParseException e)
                //{
                //    e.printStackTrace();

                //}
            }
            //合并参数二
            if (DataType.DATATYPE_LIST == arg2.GetDataType())
            {
                if (arg2.GetCollection() != null)
                {
                    resultCollection.AddRange(arg2.GetCollection());
                }
            }
            else
            {
                //try
                //{
                //    Object object = arg2.toJavaObject();
                //    resultCollection.add(object);
                //}
                //catch (ParseException e)
                //{
                //    e.printStackTrace();

                //}
            }

            //构造新的collection 常量
            Constant result = new Constant(DataType.DATATYPE_LIST, resultCollection);
            return result;
        }
    }
}
