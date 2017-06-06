using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expression.Operation;
using System.IO;
using static Expression.Format.Element;
using static Expression.Metadata.BaseMetadata;
using static Expression.ExpressionToken;

namespace Expression.Format
{
    public class ExpressionParser
    {
        private static Dictionary<string, Operator> operators = new Dictionary<string, Operator>();

        static ExpressionParser()
        {
            operators.Add(Operator.NOT.Token, Operator.NOT);

            //	operators.put("-", NG); 负号和减号的差异通过上下文区分
            operators.Add(Operator.MUTI.Token, Operator.MUTI);
            operators.Add(Operator.DIV.Token, Operator.DIV);
            operators.Add(Operator.MOD.Token, Operator.MOD);

            operators.Add(Operator.PLUS.Token, Operator.PLUS);
            operators.Add(Operator.MINUS.Token, Operator.MINUS);


            operators.Add(Operator.LT.Token, Operator.LT);
            operators.Add(Operator.LE.Token, Operator.LE);
            operators.Add(Operator.GT.Token, Operator.GT);
            operators.Add(Operator.GE.Token, Operator.GE);

            operators.Add(Operator.EQ.Token, Operator.EQ);
            operators.Add(Operator.NEQ.Token, Operator.NEQ);

            operators.Add(Operator.AND.Token, Operator.AND);

            operators.Add(Operator.OR.Token, Operator.OR);

            operators.Add(Operator.APPEND.Token, Operator.APPEND);

            operators.Add(Operator.SELECT.Token, Operator.SELECT);
            operators.Add(Operator.QUES.Token, Operator.QUES);
            operators.Add(Operator.COLON.Token, Operator.COLON);

        }

        /// <summary>
        /// 通过名称取得操作符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Operator GetOperator(string name)
        {
            if (operators.ContainsKey(name))
            {
                return operators[name];
            }
            return null;
        }

        private Stack<string> parenthesis = new Stack<string>();//匹配圆括号的栈

        public List<ExpressionToken> GetExpressionTokens(string expression)
        {
            ExpressionReader eReader = new ExpressionReader(expression);
            List<ExpressionToken> list = new List<ExpressionToken>();
            ExpressionToken expressionToken = null;//上一次读取的ExpressionToken
            Element ele = null;
            try
            {
                while ((ele = eReader.ReadToken()) != null)
                {
                    expressionToken = ChangeToToken(expressionToken, ele);
                    //如果是括号，则记录下来，最后进行最后进行匹配
                    PushParenthesis(ele);
                    list.Add(expressionToken);
                }
            }
            catch (IOException e)
            {
                //e.printStackTrace();
            }
            catch (Exception e)
            {
                throw new FormatException("表达式词元格式异常");
            }
            if (!(parenthesis.Count == 0))
            {
                throw new FormatException("括号匹配出错");
            }

            return list;
        }

        /// <summary>
        /// 如果是括号，则记录下来，最后进行最后进行匹配
        /// </summary>
        /// <param name="ele"></param>
        public void PushParenthesis(Element ele)
        {
            if (ElementType.SPLITOR == ele.Type)
            {
                if (ele.Text.Equals("("))
                {
                    parenthesis.Push("(");
                }
                else if (ele.Text.Equals(")"))
                {
                    if (parenthesis.Count == 0 || !parenthesis.Peek().Equals("("))
                    {
                        throw new FormatException("括号匹配出错");
                    }
                    else
                    {
                        parenthesis.Pop();
                    }
                }
            }
        }

        /**
         * 将切分的元素转化成ExpressionToken，并验证减号还是负号
         * @param previousToken
         * @param ele
         * @return
         * @throws ParseException 
         */
        public ExpressionToken ChangeToToken(ExpressionToken previousToken, Element ele)
        {
            if (ele == null)
            {
                throw new ArgumentException();
            }
            ExpressionToken token = null;

            //转成ExpressionToken
            if (ElementType.NULL == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_NULL, null);
            }
            else if (ElementType.STRING == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_STRING, ele.Text);
            }
            else if (ElementType.BOOLEAN == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_BOOLEAN, Convert.ToBoolean(ele.Text));
            }
            else if (ElementType.INT == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_INT, Convert.ToInt32(ele.Text));
            }
            else if (ElementType.LONG == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_LONG, Convert.ToInt64(ele.Text));
            }
            else if (ElementType.FLOAT == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_FLOAT, Convert.ToSingle(ele.Text));
            }
            else if (ElementType.DOUBLE == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_DOUBLE, Convert.ToDouble(ele.Text));
            }
            else if (ElementType.DATE == ele.Type)
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_DATE, Convert.ToDateTime(ele.Text));
            }
            else if (ElementType.VARIABLE == ele.Type)
            {
                token = ExpressionToken.CreateVariableToken(ele.Text);
            }
            else if (ElementType.OPERATOR == ele.Type)
            {
                //区分负号
                if (ele.Text.Equals("-") && (
                        previousToken == null //以“-”开头肯定是负号
                        || previousToken.TokenType == ETokenType.ETOKEN_TYPE_OPERATOR //运算符后面肯定是负号
                        || previousToken.TokenType == ETokenType.ETOKEN_TYPE_SPLITOR //“(”或“,”后面肯定是负号
                        && !")".Equals(previousToken.GetSplitor())
                    ))
                {
                    token = ExpressionToken.CreateOperatorToken(Operator.NG);
                }
                else
                {
                    token = ExpressionToken.CreateOperatorToken(GetOperator(ele.Text));
                }
            }
            else if (ElementType.FUNCTION == ele.Type)
            {
                token = ExpressionToken.CreateFunctionToken(ele.Text);
            }
            else if (ElementType.SPLITOR == ele.Type)
            {
                token = ExpressionToken.CreateSplitorToken(ele.Text);
            }
            token.StartPosition = ele.Index;

            return token;
        }

    }
}
