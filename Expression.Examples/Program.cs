using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            BoolExample();
        }

        static void StringExample()
        {
            string expression = "\"Hello\" + variablename + \"He\"";
             
            var variables = new List<Variable>();
            variables.Add(Variable.CreateVariable("variablename", "dddddd"));
            
            object result = ExpressionEvaluator.Evaluate(expression, variables);
            Console.WriteLine("Result = " + result);
        }

        static void BoolExample()
        {
            // strings should be surrounded by ""
            // methods should start with $
            string expression = "1<3&&(4>5||$Contains(\"test\",\"te\"))";

            object result = ExpressionEvaluator.Evaluate(expression);

            Console.WriteLine("Result = " + result);
        }

    }
}
