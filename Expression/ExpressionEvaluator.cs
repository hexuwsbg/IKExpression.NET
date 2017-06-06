using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression
{
    public class ExpressionEvaluator
    {
        /// <summary>
        /// 验证表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string Compile(string expression)
        {
            return Compile(expression, null);
        }

        /// <summary>
        /// 验证表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static string Compile(string expression, ICollection<Variable> variables)
        {
            if (expression == null)
            {
                throw new ArgumentException("表达式为空");
            }

            ExpressionExecutor ee = new ExpressionExecutor();
            try
            {
                //获取上下文的变量，设置到脚本执行器中
                if (variables != null && variables.Count > 0)
                {
                    foreach (Variable var in variables)
                    {
                        //添加变量到脚本变量容器
                        VariableContainer.AddVariable(var);
                    }
                }
                //解析表达式词元
                List<ExpressionToken> expTokens = ee.Analyze(expression);
                //转化RPN，并验证
                expTokens = ee.Compile(expTokens);
                //以字符串形式输出RPN
                return ee.TokensToString(expTokens);
            }
            catch (IllegalExpressionException e)
            {
                throw new Exception("表达式：\"" + expression + "\" 编译期检查异常", e);
            }
            finally
            {
                //释放脚本变量容器
                VariableContainer.RemoveVariableDict();
            }
        }

        /// <summary>
        /// 获取预编译的表达式对象
        /// @param expression 表达式的字符串表示
        /// @param variables 表达式的参数集合
        /// @return PreparedExpression 编译的表达式对象
        /// </summary>
        //public static PreparedExpression preparedCompile(String expression, Collection<Variable> variables)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentException("表达式为空");
        //    }

        //    ExpressionExecutor ee = new ExpressionExecutor();
        //    try
        //    {
        //        //获取上下文的变量，设置到脚本执行器中
        //        if (variables != null && variables.Count > 0)
        //        {
        //            foreach (Variable var in variables)
        //            {
        //                //添加变来到脚本变量容器
        //                VariableContainer.AddVariable(var);
        //            }
        //        }
        //        //解析表达式词元
        //        List<ExpressionToken> expTokens = ee.analyze(expression);
        //        //转化RPN，并验证
        //        expTokens = ee.compile(expTokens);
        //        //生成预编译表达式
        //        PreparedExpression pe = new PreparedExpression(expression, expTokens, VariableContainer.GetVariableDict());
        //        return pe;
        //    }
        //    catch (IllegalExpressionException e)
        //    {
        //        throw new Exception("表达式：\"" + expression + "\" 预编译异常", e);
        //    }
        //    finally
        //    {
        //        //释放脚本变量容器
        //        VariableContainer.RemoveVariableDict();
        //    }
        //}

        ///<summary>
        /// 执行无变量表达式
        ///</summary>
        public static object Evaluate(string expression)
        {
            return Evaluate(expression, null);
        }

        /// <summary>
        /// 根据流程上下文，执行公式语言
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static object Evaluate(string expression, ICollection<Variable> variables)
        {
            if (expression == null)
            {
                return null;
            }

            ExpressionExecutor ee = new ExpressionExecutor();
            try
            {
                //获取上下文的变量，设置到脚本执行器中
                if (variables != null && variables.Count > 0)
                {
                    foreach (Variable var in variables)
                    {
                        //添加变来到脚本变量容器
                        VariableContainer.AddVariable(var);
                    }
                }
                //解析表达式词元
                List<ExpressionToken> expTokens = ee.Analyze(expression);
                //转化RPN，并验证
                expTokens = ee.Compile(expTokens);
                //执行RPN
                Constant constant = ee.Execute(expTokens);
                return constant.GetObjectValue();

            }
            catch (IllegalExpressionException e)
            {
                throw new Exception("表达式：\"" + expression + "\" 执行异常", e);
            }
            finally
            {
                //释放脚本变量容器
                VariableContainer.RemoveVariableDict();
            }
        }

        /// <summary>
        /// 逐个添加表达式上下文变量
        /// </summary>
        /// <param name="variable"></param>
        public static void AddVarible(Variable variable)
        {
            //添加变来到脚本变量容器
            VariableContainer.AddVariable(variable);
        }

        /// <summary>
        /// 批量添加表达式上下文变量
        /// </summary>
        /// <param name="variables"></param>
        public static void AddVaribles(Collection<Variable> variables)
        {
            //获取上下文的变量，设置到脚本执行器中
            if (variables != null && variables.Count > 0)
            {
                foreach (Variable var in variables)
                {
                    //添加变来到脚本变量容器
                    VariableContainer.AddVariable(var);
                }
            }
        }
    }
}
