using Expression.Functions;
using Expression.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.ExpressionToken;
using static Expression.Metadata.BaseMetadata;

namespace Expression.Metadata
{
    public class Reference
    {
        public ExpressionToken Token { set; get; }

        public Constant[] Arguments { set; get; }
        //引用对象实际的数据类型
        private DataType dataType;

        public Reference(ExpressionToken token, Constant[] args)
        {
            this.Token = token;
            this.Arguments = args;
            //记录Reference实际的数据类型
            if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == token.TokenType)
            {
                Constant result = FunctionExecution.Verify(token.TokenText, token.StartPosition, args);
                dataType = result.GetDataType();
            }
            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == token.TokenType)
            {
                Operator op = token.Op;
                Constant result = op.Verify(token.StartPosition, args);
                dataType = result.GetDataType();
            }
        }

        public DataType GetDataType()
        {
            return dataType;
        }

        /**
         * 执行引用对象指待的表达式（操作符或者函数）
         * @return
         */
        public Constant Execute()
        {

            if (ETokenType.ETOKEN_TYPE_OPERATOR == Token.TokenType)
            {
                //执行操作符
                Operator op = Token.Op;
                return op.Execute(Arguments);

            }
            else if (ETokenType.ETOKEN_TYPE_FUNCTION == Token.TokenType)
            {
                //执行函数
                return FunctionExecution.Execute(Token.TokenText, Token.StartPosition, Arguments);

            }
            else
            {
                throw new IllegalExpressionException("不支持的Reference执行异常");
            }
        }
    }
}
