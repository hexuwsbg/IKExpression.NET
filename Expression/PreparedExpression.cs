using Expression.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression
{
    public class PreparedExpression
    {
        //原始表达式字符
        private string orgExpression;

        //编译验证后生成的RPN
        private List<ExpressionToken> expTokens;

        //编译验证后生成的表达式的变量表
        private ConcurrentDictionary<string, Variable> variableDict;


        PreparedExpression(string orgExpression, List<ExpressionToken> expTokens, ConcurrentDictionary<string, Variable> variableMap)
        {
            this.orgExpression = orgExpression;
            this.expTokens = expTokens;
            this.variableDict = new ConcurrentDictionary<string, Variable>(variableMap);
        }

        /// <summary>
        /// 设置指定参数的值
        /// 如果参数不存在，则抛出IllegalArgumentException运行时异常
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetArgument(string name, object value)
        {
            if (variableDict.ContainsKey(name))
            {
                variableDict.AddOrUpdate(name, variableDict[name], (key, oldValue) => { oldValue.SetVariableValue(value); return oldValue; });
            }
            else
            {
                throw new ArgumentException("无法识别的表达式参数：" + name);
            }
        }

        /// <summary>
        /// 执行当前的预编译表达式
        /// </summary>
        /// <returns></returns>
        public object Execute()
        {
            ExpressionExecutor ee = new ExpressionExecutor();
            //执行RPN		
            try
            {
                //添加变来到脚本变量容器
                VariableContainer.SetVariableDict(new Dictionary<string, Variable>(variableDict));
                Constant constant = ee.Execute(expTokens);
                return constant.GetObjectValue();
            }
            catch (Exception e)
            {
                throw new Exception("表达式：\"" + orgExpression + "\" 执行异常", e);
            }
            finally
            {
                //释放脚本变量容器
                VariableContainer.RemoveVariableDict();
            }
        }
    }
}
