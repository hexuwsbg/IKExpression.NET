using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.Metadata.BaseMetadata;
using Expression.Operation;

namespace Expression
{
    public class ExpressionToken
    {
        //词元的语法类型
        public enum ETokenType
        {
            //常量
            ETOKEN_TYPE_CONSTANT,
            //变量
            ETOKEN_TYPE_VARIABLE,
            //操作符
            ETOKEN_TYPE_OPERATOR,
            //函数
            ETOKEN_TYPE_FUNCTION,
            //分隔符
            ETOKEN_TYPE_SPLITOR,
        }

        //Token的词元类型：常量，变量，操作符，函数，分割符
        public ETokenType TokenType { protected set; get; }
        //当TokenType = ETOKEN_TYPE_CONSTANT 时,constant存储常量描述
        public Constant Constant { protected set; get; }
        //当TokenType = ETOKEN_TYPE_VARIABLE 时,variable存储变量描述
        public Variable Variable { protected set; get; }
        //当TokenType = ETOKEN_TYPE_OPERATOR 时, operator存储操做符描述
        public Operator Op { protected set; get; }
        //存储字符描述
        public string TokenText { protected set; get; }
        //词元在表达式中的起始位置
        public int StartPosition { set; get; } = -1;

        public static ExpressionToken CreateConstantToken(DataType dataType, object dataValue)
        {
            ExpressionToken instance = new ExpressionToken();
            instance.Constant = new Constant(dataType, dataValue);
            instance.TokenType = ETokenType.ETOKEN_TYPE_CONSTANT;
            if (dataValue != null)
            {
                instance.TokenText = instance.Constant.GetDataValueText();
            }
            return instance;
        }

        public static ExpressionToken CreateConstantToken(Constant constant)
        {
            if (constant == null)
            {
                throw new ArgumentException("非法参数异常：常量为null");
            }
            ExpressionToken instance = new ExpressionToken();
            instance.Constant = constant;
            instance.TokenType = ETokenType.ETOKEN_TYPE_CONSTANT;
            if (constant.DataValue != null)
            {
                instance.TokenText = constant.GetDataValueText();
            }
            return instance;
        }

        public static ExpressionToken CreateVariableToken(string variableName)
        {
            ExpressionToken instance = new ExpressionToken();
            instance.Variable = new Variable(variableName);
            instance.TokenType = ETokenType.ETOKEN_TYPE_VARIABLE;
            instance.TokenText = variableName;
            return instance;
        }

        public static ExpressionToken CreateReference(Reference reference)
        {
            ExpressionToken instance = new ExpressionToken();
            instance.Constant = new Constant(reference);
            instance.TokenType = ETokenType.ETOKEN_TYPE_CONSTANT;
            if (reference != null)
            {
                instance.TokenText = instance.Constant.GetDataValueText();
            }
            return instance;
        }

        public static ExpressionToken CreateFunctionToken(string functionName)
        {
            if (functionName == null)
            {
                throw new ArgumentException("非法参数：函数名称为空");
            }
            ExpressionToken instance = new ExpressionToken();
            instance.TokenText = functionName;
            instance.TokenType = ETokenType.ETOKEN_TYPE_FUNCTION;
            return instance;
        }

        public static ExpressionToken CreateOperatorToken(Operator op)
        {
            if (op == null)
            {
                throw new ArgumentException("非法参数：操作符为空");
            }
            ExpressionToken instance = new ExpressionToken();
            instance.Op = op;
            instance.TokenText = op.Token;
            instance.TokenType = ETokenType.ETOKEN_TYPE_OPERATOR;
            return instance;
        }

        public static ExpressionToken CreateSplitorToken(string splitorText)
        {
            if (splitorText == null)
            {
                throw new ArgumentException("非法参数：分隔符为空");
            }
            ExpressionToken instance = new ExpressionToken();
            instance.TokenText = splitorText;
            instance.TokenType = ETokenType.ETOKEN_TYPE_SPLITOR;
            return instance;
        }

        
        private ExpressionToken()
        {
        }

        /// <summary>
        /// 获取Token的分隔符类型值
        /// </summary>
        /// <returns></returns>
        public string GetSplitor()
        {
            return this.TokenText;
        }

        public override string ToString()
        {
            return TokenText;
        }
    }
}
