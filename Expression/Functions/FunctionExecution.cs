using Expression;
using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Expression.Metadata.BaseMetadata;

namespace Expression.Functions
{
    public class FunctionExecution
    {
        private FunctionExecution()
        {
        }

        /**
         * 根据函数名、参数数组，执行操作，并返回结果Token
         * @param functionName 函数名
         * @param position
         * @param args 注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数
         * @return
         * @throws IllegalExpressionException 
         */
        public static Constant Execute(string functionName, int position, Constant[] args)
        {
            if (functionName == null)
            {
                throw new ArgumentException("函数名为空");
            }
            if (args == null)
            {
                throw new ArgumentException("函数参数列表为空");
            }
            for (int i = 0; i < args.Length; i++)
            {
                //如果参数为引用类型，则执行引用
                if (args[i].IsReference)
                {
                    Reference reference = (Reference)args[i].DataValue;
                    args[i] = reference.Execute();
                }
            }

            //转化方法参数
            object[] parameters;
            try
            {
                parameters = ConvertParameters(functionName, position, args);
            }
            catch (IllegalExpressionException e)
            {
                throw new ArgumentException("函数\"" + functionName + "\"运行时参数类型错误");
            }

            try
            {
                object result = FunctionLoader.InvokeFunction(functionName, parameters);

                if (result is bool)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, result);

                }
                else if (result is DateTime)
                {
                    return new Constant(DataType.DATATYPE_DATE, result);

                }
                else if (result is double)
                {
                    return new Constant(DataType.DATATYPE_DOUBLE, result);

                }
                else if (result is float)
                {
                    return new Constant(DataType.DATATYPE_FLOAT, result);

                }
                else if (result is int)
                {
                    return new Constant(DataType.DATATYPE_INT, result);

                }
                else if (result is long)
                {
                    return new Constant(DataType.DATATYPE_LONG, result);

                }
                else if (result is string)
                {
                    return new Constant(DataType.DATATYPE_STRING, result);

                }
                else if (result is List<object>)
                {
                    return new Constant(DataType.DATATYPE_LIST, result);

                }
                else
                {
                    return new Constant(DataType.DATATYPE_OBJECT, result);

                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
                throw new Exception("函数\"" + functionName + "\"访问异常:" + e.Message, e);

            }
        }

        /**
         * 检查函数和参数是否合法，是可执行的
         * 如果合法，则返回含有执行结果类型的Token
         * 如果不合法，则返回null
         * @param functionName
         * @param position
         * @param args 注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数
         * @return
         * @throws IllegalExpressionException 
         */
        public static Constant Verify(string functionName, int position, BaseMetadata[] args)
        {
            if (functionName == null)
            {
                throw new ArgumentException("函数名为空");
            }

            //通过方法名和参数数组，获取方法，及方法的返回值，并转化成ExpressionToken
            try
            {

                MethodInfo method = FunctionLoader.LoadFunction(functionName);
                //校验方法参数类型
                var parametersType = method.GetParameters().Select(p => p.ParameterType).ToArray();
                if (args.Length == parametersType.Length)
                {
                    //注意，传入参数的顺序是颠倒的
                    for (int i = args.Length - 1; i >= 0; i--)
                    {
                        Type type = args[i].mapTypeToJavaClass();
                        if (type != null)
                        {
                            if (!IsCompatibleType(parametersType[parametersType.Length - i - 1], type))
                            {
                                //抛异常
                                throw new IllegalExpressionException("函数\"" + functionName + "\"参数类型不匹配,函数参数定义类型为：" + parametersType[i].Name + " 传入参数实际类型为：" + type.Name
                                        , functionName
                                        , position
                                        );
                            }
                        }
                        else
                        {
                            //传入参数为null，忽略类型校验						
                        }
                    }
                }
                else
                {
                    //抛异常
                    throw new IllegalExpressionException("函数\"" + functionName + "\"参数个数不匹配"
                            , functionName
                            , position
                            );

                }

                Type returnType = method.ReturnType;

                //转换成ExpressionToken
                if (typeof(bool) == returnType)
                {
                    return new Constant(DataType.DATATYPE_BOOLEAN, false);

                }
                else if (typeof(DateTime) == returnType)
                {
                    return new Constant(DataType.DATATYPE_DATE, DateTime.MinValue);

                }
                else if (typeof(double) == returnType)
                {
                    return new Constant(DataType.DATATYPE_DOUBLE, 0D);

                }
                else if (typeof(float) == returnType)
                {
                    return new Constant(DataType.DATATYPE_FLOAT, 0F);

                }
                else if (typeof(int) == returnType)
                {
                    return new Constant(DataType.DATATYPE_INT, 0);

                }
                else if (typeof(long) == returnType)
                {
                    return new Constant(DataType.DATATYPE_LONG, 0L);

                }
                else if (typeof(string) == returnType)
                {
                    return new Constant(DataType.DATATYPE_STRING, null);

                }
                else if (typeof(List<object>) == returnType)
                {
                    return new Constant(DataType.DATATYPE_LIST, new List<object>());

                }
                else if (typeof(object) == returnType)
                {
                    return new Constant(DataType.DATATYPE_OBJECT, null);

                }
                else if (typeof(void) == returnType)
                {
                    return new Constant(DataType.DATATYPE_OBJECT, null);
                }
                else
                {
                    throw new Exception("解析器内部错误：不支持的函数返回类型");
                }

            }
            catch (Exception e)
            {
                //抛异常
                throw new IllegalExpressionException("函数\"" + functionName + "\"不存在或参数类型不匹配"
                        , functionName

                        , position

                        );
            }
        }

        /**
         * 函数参数转化
         * @param args
         * @return
         * @throws IllegalExpressionException 
         */
        private static object[] ConvertParameters(string functionName, int position, Constant[] args)
        {
            //参数为空，返回空数组
            if (args == null)
            {
                return new object[0];
            }

            //转化方法参数类型数组
            var parameters = new object[args.Length];
            for (int i = args.Length - 1; i >= 0; i--)
            {
                try
                {
                    parameters[args.Length - 1 - i] = args[i].GetObjectValue();
                }
                catch (Exception e1)
                {
                    //抛异常
                    throw new IllegalExpressionException("函数\"" + functionName + "\"参数转化Java对象错误");
                }
            }
            return parameters;
        }

        /**
         * 检查数据类型的兼容性
         * 类型相同，一定兼容
         * 如果parametersType 为Object 则兼容所有类型
         * 如果parametersType 为double 则兼容 int ，long ，float
         * 如果parametersType 为float 则兼容 int ，long 
         * 如果parametersType 为long 则兼容 int  
         * @param parametersType 方法定义的参数类型
         * @param argType 实际参数类型
         * @return
         */
        private static bool IsCompatibleType(Type parametersType, Type argType)
        {
            if (typeof(object) == parametersType)
            {
                return true;

            }
            else if (parametersType == argType)
            {
                return true;

            }
            else if (typeof(double) == parametersType)
            {
                return typeof(float) == argType || typeof(long) == argType || typeof(int) == argType;

            }
            else if (typeof(double) == parametersType)
            {
                return typeof(double) == argType;

            }
            else if (typeof(float) == parametersType)
            {
                return typeof(long) == argType || typeof(int) == argType;

            }
            else if (typeof(float) == parametersType)
            {
                return typeof(float) == argType;

            }
            else if (typeof(long) == parametersType)
            {
                return typeof(int) == argType;

            }
            else if (typeof(long) == parametersType)
            {
                return typeof(long) == argType;

            }
            else if (typeof(int) == parametersType)
            {
                return typeof(int) == argType;

            }
            return false;

        }
    }
}
