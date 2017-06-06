using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expression.Metadata;
using System.Collections.Generic;

namespace Expression.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string expression = "\"Hello\" + variable + \"He\"";
            //给表达式中的变量 "用户名" 付上下文的值  
            var variables = new List<Variable>();
            variables.Add(Variable.CreateVariable("variable", "dddd"));
            //执行表达式  
            object result = ExpressionEvaluator.Evaluate(expression, variables);
            Console.WriteLine("Result = " + result);
        }
    }
}
