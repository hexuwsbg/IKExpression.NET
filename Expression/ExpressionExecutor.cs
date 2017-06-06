using Expression.Format;
using Expression.Functions;
using Expression.Metadata;
using Expression.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Expression.ExpressionToken;
using static Expression.Metadata.BaseMetadata;

namespace Expression
{
    public class ExpressionExecutor
    {
        /// <summary>
        /// 对表达式进行语法分析，将其转换成Token对象队列
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<ExpressionToken> Analyze(string expression)
        {

            ExpressionParser expParser = new ExpressionParser();
            List<ExpressionToken> list = null;
            try
            {
                list = expParser.GetExpressionTokens(expression);
                return list;

            }
            catch (Format.FormatException e)
            {
                throw new IllegalExpressionException(e.Message);
            }

        }

        /// <summary>
        /// 将正常表达式词元序列，转换成逆波兰式序列  同时检查表达式语法
        /// </summary>
        /// <param name="expTokens"></param>
        /// <returns></returns>
        public List<ExpressionToken> Compile(List<ExpressionToken> expTokens)
        {

            if (expTokens == null || expTokens.Count == 0)
            {
                throw new ArgumentException("无法转化空的表达式");
            }

            //1.初始化逆波兰式队列和操作符栈
            var _RPNExpList = new List<ExpressionToken>();
            Stack<ExpressionToken> opStack = new Stack<ExpressionToken>();
            //初始化检查栈
            Stack<ExpressionToken> verifyStack = new Stack<ExpressionToken>();

            //2.出队列中从左向右依次遍历token
            //2-1. 声明一个存储函数词元的临时变量
            ExpressionToken _function = null;

            foreach (ExpressionToken expToken in expTokens)
            {

                if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == expToken.TokenType)
                {
                    //读入一个常量，压入逆波兰式队列
                    _RPNExpList.Add(expToken);
                    //同时压入校验栈
                    verifyStack.Push(expToken);

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_VARIABLE == expToken.TokenType)
                {
                    //验证变量声明	
                    Variable var = VariableContainer.GetVariable(expToken.Variable.VariableName);
                    if (var == null)
                    {
                        //当变量没有定义时，视为null型
                        expToken.Variable.SetDataType(DataType.DATATYPE_NULL);

                    }
                    //else if (var.GetDataType() == null)
                    //{
                    //    throw new IllegalExpressionException("表达式不合法，变量\"" + expToken.ToString() + "\"缺少定义;位置:" + expToken.StartPosition
                    //                , expToken.ToString()
                    //                , expToken.StartPosition);
                    //}
                    else
                    {
                        //设置Token中的变量类型定义
                        expToken.Variable.SetDataType(var.GetDataType());
                    }

                    //读入一个变量，压入逆波兰式队列
                    _RPNExpList.Add(expToken);
                    //同时压入校验栈
                    verifyStack.Push(expToken);


                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == expToken.TokenType)
                {
                    //读入一个操作符
                    if (opStack.Count == 0)
                    {//如果操作栈为空
                        if (Operator.COLON == expToken.Op)
                        {
                            //:操作符不可能单独出现，前面必须有对应的？
                            throw new IllegalExpressionException("在读入\"：\"时，操作栈中找不到对应的\"？\" "

                                    , expToken.ToString()
                                    , expToken.StartPosition);

                        }
                        else
                        {
                            //一般操作符，则压入栈内；
                            opStack.Push(expToken);
                        }
                    }
                    else
                    {
                        bool doPeek = true;
                        while (!(opStack.Count == 0) && doPeek)
                        {
                            //如果栈不为空，则比较栈顶的操作符的优先级
                            ExpressionToken onTopOp = opStack.Peek();

                            //如果栈顶元素是函数,直接将操作符压入栈
                            if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == onTopOp.TokenType)
                            {
                                if (Operator.COLON == expToken.Op)
                                {
                                    //:操作符不可能直接遇见函数，前面必须有对应的？
                                    throw new IllegalExpressionException("在读入\"：\"时，操作栈中找不到对应的\"？\""

                                            , expToken.ToString()
                                            , expToken.StartPosition);

                                }
                                else
                                {
                                    opStack.Push(expToken);
                                    doPeek = false;
                                }

                            }
                            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_SPLITOR == onTopOp.TokenType
                                       && "(".Equals(onTopOp.GetSplitor()))
                            {//如果栈顶元素是(,直接将操作符压入栈							
                                if (Operator.COLON == expToken.Op)
                                {
                                    //:操作符不可能直接遇见(，前面必须有对应的？
                                    throw new IllegalExpressionException("在读入\"：\"时，操作栈中找不到对应的\"？\""

                                            , expToken.ToString()
                                            , expToken.StartPosition);

                                }
                                else
                                {
                                    opStack.Push(expToken);
                                    doPeek = false;
                                }

                            }
                            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == onTopOp.TokenType)
                            {
                                //如果栈顶元素是操作符							
                                if (expToken.Op.Priority > onTopOp.Op.Priority)
                                {
                                    if (Operator.COLON == expToken.Op)
                                    {
                                        //注意：如果在后期的功能扩展中，存在有比:优先级更低的操作符
                                        //则必须在此处做出栈处理
                                    }
                                    else
                                    {
                                        //当前操作符的优先级 > 栈顶操作符的优先级 ，则将当前操作符入站
                                        opStack.Push(expToken);
                                        doPeek = false;
                                    }

                                }
                                else if (expToken.Op.Priority == onTopOp.Op.Priority)
                                {
                                    if (Operator.QUES == expToken.Op)
                                    {
                                        //? , ?  -- >不弹出
                                        //?: , ? -- >不弹出
                                        opStack.Push(expToken);
                                        doPeek = false;

                                    }
                                    else if (Operator.COLON == expToken.Op)
                                    {
                                        //? , : -- > 弹出？ ,将操作符转变成?: , 再压入栈
                                        if (Operator.QUES == onTopOp.Op)
                                        {
                                            //弹出?
                                            opStack.Pop();
                                            //将操作符转变成?:
                                            ExpressionToken opSelectToken = ExpressionToken.CreateOperatorToken(Operator.SELECT);
                                            opSelectToken.StartPosition = onTopOp.StartPosition;
                                            //再压入栈
                                            opStack.Push(opSelectToken);
                                            doPeek = false;

                                        }
                                        else if (Operator.SELECT == onTopOp.Op)
                                        { // ?: , : -->弹出?: ,执行校验
                                          //执行操作符校验
                                            ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                                            //把校验结果压入检验栈
                                            verifyStack.Push(result);
                                            //校验通过，弹出栈顶操作符，加入到逆波兰式队列
                                            opStack.Pop();
                                            _RPNExpList.Add(onTopOp);

                                        }
                                    }
                                    else
                                    {
                                        //当前操作符的优先级 = 栈顶操作符的优先级,且执行顺序是从左到右的，则将栈顶的操作符弹出
                                        //执行操作符校验
                                        ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                                        //把校验结果压入检验栈
                                        verifyStack.Push(result);
                                        //校验通过，弹出栈顶操作符，加入到逆波兰式队列
                                        opStack.Pop();
                                        _RPNExpList.Add(onTopOp);

                                    }
                                }
                                else
                                {
                                    //当前操作符的优先级 < 栈顶操作符的优先级，则将栈顶的操作符弹出
                                    //执行操作符校验
                                    ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                                    //把校验结果压入检验栈
                                    verifyStack.Push(result);
                                    //校验通过，弹出栈顶操作符，加入到逆波兰式队列
                                    opStack.Pop();
                                    _RPNExpList.Add(onTopOp);

                                }
                            }
                        }
                        //当前操作符的优先级 <= 栈内所有的操作符优先级
                        if (doPeek && opStack.Count == 0)
                        {
                            if (Operator.COLON == expToken.Op)
                            {
                                //:操作符不可能直接入栈，前面必须有对应的？
                                throw new IllegalExpressionException("在读入\"：\"时，操作栈中找不到对应的\"？\""

                                        , expToken.ToString()
                                        , expToken.StartPosition);

                            }
                            else
                            {
                                opStack.Push(expToken);
                            }
                        }
                    }

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == expToken.TokenType)
                {
                    //遇到函数名称，则使用临时变量暂存下来，等待(的来临
                    _function = expToken;

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_SPLITOR == expToken.TokenType)
                {
                    //处理读入的“（”
                    if ("(".Equals(expToken.GetSplitor()))
                    {
                        //如果此时_function != null,说明是函数的左括号
                        if (_function != null)
                        {
                            //向逆波兰式队列压入"("
                            _RPNExpList.Add(expToken);
                            //向校验栈压入
                            verifyStack.Push(expToken);

                            //将"("及临时缓存的函数压入操作符栈,括号在前
                            opStack.Push(expToken);
                            opStack.Push(_function);

                            //清空临时变量
                            _function = null;

                        }
                        else
                        {
                            //说明是普通的表达式左括号
                            //将"("压入操作符栈
                            opStack.Push(expToken);
                        }

                        //处理读入的“）”	
                    }
                    else if (")".Equals(expToken.GetSplitor()))
                    {

                        bool doPop = true;

                        while (doPop && !(opStack.Count == 0))
                        {
                            // 从操作符栈顶弹出操作符或者函数，
                            ExpressionToken onTopOp = opStack.Pop();
                            if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == onTopOp.TokenType)
                            {
                                if (Operator.QUES == onTopOp.Op)
                                {
                                    //)分割符不可能遇到？，这说明缺少：号
                                    throw new IllegalExpressionException("在读入\")\"时，操作栈中遇到\"？\" ,缺少\":\"号"

                                            , onTopOp.ToString()
                                            , onTopOp.StartPosition);

                                }
                                else
                                {
                                    //如果栈顶元素是普通操作符,执行操作符校验
                                    ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                                    //把校验结果压入检验栈
                                    verifyStack.Push(result);

                                    // 校验通过，则添加到逆波兰式对列
                                    _RPNExpList.Add(onTopOp);
                                }

                            }
                            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == onTopOp.TokenType)
                            {
                                // 如果遇到函数，则说明")"是函数的右括号
                                //执行函数校验
                                ExpressionToken result = VerifyFunction(onTopOp, verifyStack);
                                //把校验结果压入检验栈
                                verifyStack.Push(result);

                                //校验通过，添加")"到逆波兰式中
                                _RPNExpList.Add(expToken);
                                //将函数加入逆波兰式
                                _RPNExpList.Add(onTopOp);

                            }
                            else if ("(".Equals(onTopOp.GetSplitor()))
                            {
                                // 如果遇到"(", 则操作结束
                                doPop = false;
                            }
                        }

                        if (doPop && opStack.Count == 0)
                        {
                            throw new IllegalExpressionException("在读入\")\"时，操作栈中找不到对应的\"(\" "

                                    , expToken.GetSplitor()
                                    , expToken.StartPosition);
                        }

                        //处理读入的“,”	
                    }
                    else if (",".Equals(expToken.GetSplitor()))
                    {
                        //依次弹出操作符栈中的所有操作符，压入逆波兰式队列，直到遇见函数词元
                        bool doPeek = true;

                        while (!(opStack.Count == 0) && doPeek)
                        {
                            ExpressionToken onTopOp = opStack.Peek();

                            if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == onTopOp.TokenType)
                            {
                                if (Operator.QUES == onTopOp.Op)
                                {
                                    //,分割符不可能遇到？，这说明缺少：号
                                    throw new IllegalExpressionException("在读入\",\"时，操作栈中遇到\"？\" ,缺少\":\"号"

                                            , onTopOp.ToString()
                                            , onTopOp.StartPosition);

                                }
                                else
                                {
                                    //弹出操作符栈顶的操作符
                                    opStack.Pop();
                                    //执行操作符校验
                                    ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                                    //把校验结果压入检验栈
                                    verifyStack.Push(result);
                                    //校验通过，，压入逆波兰式队列
                                    _RPNExpList.Add(onTopOp);
                                }

                            }
                            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == onTopOp.TokenType)
                            {
                                //遇见函数词元,结束弹出
                                doPeek = false;

                            }
                            else if (ExpressionToken.ETokenType.ETOKEN_TYPE_SPLITOR == onTopOp.TokenType
                                       && "(".Equals(onTopOp.GetSplitor()))
                            {
                                //在读入","时，操作符栈顶为"(",则报错
                                throw new IllegalExpressionException("在读入\",\"时，操作符栈顶为\"(\",,(函数丢失) 位置：" + onTopOp.StartPosition
                                        , expToken.GetSplitor()
                                        , expToken.StartPosition);
                            }
                        }
                        //栈全部弹出，但没有遇见函数词元
                        if (doPeek && opStack.Count == 0)
                        {
                            throw new IllegalExpressionException("在读入\",\"时，操作符栈弹空，没有找到相应的函数词元 "

                                    , expToken.GetSplitor()
                                    , expToken.StartPosition);
                        }
                    }
                }
            }

            //将操作栈内剩余的操作符逐一弹出，并压入逆波兰式队列
            while (!(opStack.Count == 0))
            {
                ExpressionToken onTopOp = opStack.Pop();

                if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == onTopOp.TokenType)
                {
                    if (Operator.QUES == onTopOp.Op)
                    {
                        //遇到单一的剩下的？，这说明缺少：号
                        throw new IllegalExpressionException("操作栈中遇到剩余的\"？\" ,缺少\":\"号"

                                , onTopOp.ToString()
                                , onTopOp.StartPosition);

                    }
                    else
                    {
                        //执行操作符校验
                        ExpressionToken result = VerifyOperator(onTopOp, verifyStack);
                        //把校验结果压入检验栈
                        verifyStack.Push(result);

                        //校验成功,将操作符加入逆波兰式				
                        _RPNExpList.Add(onTopOp);
                    }

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == onTopOp.TokenType)
                {
                    //如果剩余是函数，则函数缺少右括号")"
                    throw new IllegalExpressionException("函数" + onTopOp.TokenText + "缺少\")\""
                            , onTopOp.TokenText
                            , onTopOp.StartPosition);

                }
                else if ("(".Equals(onTopOp.GetSplitor()))
                {
                    //剩下的就只有“(”了，则说明表达式的算式缺少右括号")"
                    throw new IllegalExpressionException("左括号\"(\"缺少配套的右括号\")\""

                            , onTopOp.TokenText
                            , onTopOp.StartPosition);
                }
            }

            //表达式校验完成，这是校验栈内应该只有一个结果,否则视为表达式不完成
            if (verifyStack.Count != 1)
            {

                var errorBuffer = new StringBuilder("\r\n");
                while (!(verifyStack.Count == 0))
                {
                    ExpressionToken onTop = verifyStack.Pop();
                    errorBuffer.Append("\t").Append(onTop.ToString()).Append("\r\n");
                }
                throw new IllegalExpressionException("表达式不完整.\r\n 校验栈状态异常:" + errorBuffer);

            }

            return _RPNExpList;
        }


        /**
         * 执行逆波兰式
         * @return
         */
        public Constant Execute(List<ExpressionToken> _RPNExpList)
        {
            if (_RPNExpList == null || _RPNExpList.Count == 0)
            {
                throw new ArgumentException("无法执行空的逆波兰式队列");
            }

            //初始化编译栈
            Stack<ExpressionToken> compileStack = new Stack<ExpressionToken>();

            foreach (ExpressionToken expToken in _RPNExpList)
            {

                if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == expToken.TokenType)
                {
                    //读取一个常量，压入栈
                    compileStack.Push(expToken);

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_VARIABLE == expToken.TokenType)
                {
                    //读取一个变量
                    //从上下文获取变量的实际值，将其转化成常量Token，压入栈
                    Variable varWithValue = VariableContainer.GetVariable(expToken.Variable.VariableName);
                    if (varWithValue != null)
                    {
                        //生成一个有值常量，varWithValue.getDataValue有可能是空值
                        ExpressionToken constantToken = ExpressionToken.CreateConstantToken(
                                            varWithValue.GetDataType()
                                            , varWithValue.DataValue);
                        compileStack.Push(constantToken);

                    }
                    else
                    {
                        //throw new IllegalStateException("变量\"" +expToken.getVariable().getVariableName() + "\"不是上下文合法变量" );						
                        //当变量没有定义时，视为null型
                        ExpressionToken constantToken = ExpressionToken.CreateConstantToken(
                                DataType.DATATYPE_NULL
                                , null);
                        compileStack.Push(constantToken);
                    }


                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_OPERATOR == expToken.TokenType)
                {
                    Operator op = expToken.Op;
                    //判定几元操作符
                    int opType = op.OpType;
                    //取得相应的参数个数
                    Constant[] args = new Constant[opType];
                    ExpressionToken argToken = null;
                    for (int i = 0; i < opType; i++)
                    {
                        if (!(compileStack.Count == 0))
                        {
                            argToken = compileStack.Pop();
                            if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == argToken.TokenType)
                            {
                                args[i] = argToken.Constant;
                            }
                            else
                            {
                                //如果取出的Token不是常量，则抛出错误
                                throw new Exception("操作符" + op.Token + "找不到相应的参数，或参数个数不足;位置：" + expToken.StartPosition);
                            }
                        }
                        else
                        {
                            //栈已经弹空，没有取道操作符对应的操作数
                            throw new Exception("操作符" + op.Token + "找不到相应的参数，或参数个数不足;位置：" + expToken.StartPosition);
                        }
                    }
                    //构造引用常量对象
                    Reference reference = new Reference(expToken, args);
                    ExpressionToken resultToken = ExpressionToken.CreateReference(reference);
                    //将引用对象压入栈
                    compileStack.Push(resultToken);

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_FUNCTION == expToken.TokenType)
                {

                    if (!(compileStack.Count == 0))
                    {

                        ExpressionToken onTop = compileStack.Pop();
                        //检查在遇到函数词元后，执行栈中弹出的第一个词元是否为“）”
                        if (")".Equals(onTop.GetSplitor()))
                        {

                            bool doPop = true;
                            List<Constant> argsList = new List<Constant>();
                            ExpressionToken parameter = null;
                            //弹出函数的参数，直到遇到"("时终止
                            while (doPop && !(compileStack.Count == 0))
                            {
                                parameter = compileStack.Pop();

                                if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == parameter.TokenType)
                                {
                                    argsList.Add(parameter.Constant);
                                }
                                else if ("(".Equals(parameter.GetSplitor()))
                                {
                                    doPop = false;
                                }
                                else
                                {
                                    //在函数中遇到的既不是常量，也不是"(",则报错
                                    throw new Exception("函数" + expToken.TokenText + "执行时遇到非法参数" + parameter.ToString());
                                }
                            }

                            if (doPop && (compileStack.Count == 0))
                            {
                                //操作栈以空，没有找到函数的左括号（
                                throw new Exception("函数" + expToken.TokenText + "执行时没有找到应有的\"(\"");
                            }

                            //执行函数
                            Constant[] arguments = new Constant[argsList.Count];
                            arguments = argsList.ToArray();
                            //构造引用常量对象
                            Reference reference = new Reference(expToken, arguments);
                            ExpressionToken resultToken = ExpressionToken.CreateReference(reference);
                            //将引用对象压入栈
                            compileStack.Push(resultToken);

                        }
                        else
                        {
                            //没有找到应该存在的右括号
                            throw new Exception("函数" + expToken.TokenText + "执行时没有找到应有的\")\"");

                        }

                    }
                    else
                    {
                        //没有找到应该存在的右括号
                        throw new Exception("函数" + expToken.TokenText + "执行时没有找到应有的\")\"");
                    }

                }
                else if (ExpressionToken.ETokenType.ETOKEN_TYPE_SPLITOR == expToken.TokenType)
                {
                    //读取一个分割符，压入栈，通常是"("和")"
                    compileStack.Push(expToken);

                }
            }

            //表达式编译完成，这是编译栈内应该只有一个编译结果
            if (compileStack.Count == 1)
            {
                ExpressionToken token = compileStack.Pop();
                Constant result = token.Constant;
                //执行Reference常量
                if (result.IsReference)
                {
                    Reference resultRef = (Reference)result.DataValue;
                    return resultRef.Execute();

                }
                else
                {
                    //返回普通的常量
                    return result;
                }
            }
            else
            {
                var errorBuffer = new StringBuilder("\r\n");
                while (!(compileStack.Count == 0))
                {
                    ExpressionToken onTop = compileStack.Pop();
                    errorBuffer.Append("\t").Append(onTop.ToString()).Append("\r\n");
                }
                throw new Exception("表达式不完整.\r\n 结果状态异常:" + errorBuffer.ToString());
            }
        }

        /// <summary>
        /// 将表达式词元列表转化为字符窜
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public string TokensToString(List<ExpressionToken> tokenList)
        {
            if (tokenList == null)
            {
                throw new ArgumentException("参数tokenList为空");
            }

            var expressionText = new StringBuilder();
            foreach (ExpressionToken token in tokenList)
            {

                ExpressionToken.ETokenType tokenType = token.TokenType;

                if (ETokenType.ETOKEN_TYPE_CONSTANT == tokenType)
                {

                    Constant c = token.Constant;
                    if (DataType.DATATYPE_BOOLEAN == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append(" ");

                    }
                    else if (DataType.DATATYPE_DATE == c.GetDataType())
                    {
                        expressionText.Append("[").Append(c.GetDataValueText()).Append("] ");

                    }
                    else if (DataType.DATATYPE_DOUBLE == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append(" ");

                    }
                    else if (DataType.DATATYPE_FLOAT == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append("F ");

                    }
                    else if (DataType.DATATYPE_INT == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append(" ");

                    }
                    else if (DataType.DATATYPE_LONG == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append("L ");

                    }
                    else if (DataType.DATATYPE_NULL == c.GetDataType())
                    {
                        expressionText.Append(c.GetDataValueText()).Append(" ");

                    }
                    else if (DataType.DATATYPE_STRING == c.GetDataType())
                    {
                        expressionText.Append("\"").Append(c.GetDataValueText()).Append("\" ");

                    }

                }
                else if (ETokenType.ETOKEN_TYPE_VARIABLE == tokenType)
                {
                    expressionText.Append(token.Variable.VariableName).Append(" ");

                }
                else if (ETokenType.ETOKEN_TYPE_FUNCTION == tokenType)
                {
                    expressionText.Append('$').Append(token.TokenText).Append(" ");

                }
                else if (ETokenType.ETOKEN_TYPE_OPERATOR == tokenType)
                {
                    expressionText.Append(token.Op.ToString()).Append(" ");

                }
                else if (ETokenType.ETOKEN_TYPE_SPLITOR == tokenType)
                {
                    expressionText.Append(token.GetSplitor()).Append(" ");

                }
            }
            return expressionText.ToString();
        }

        /// <summary>
        /// 将表达式子窜（格式化好的），转换成词元列表
        /// </summary>
        /// <param name="tokenExpression"></param>
        /// <returns></returns>
        public List<ExpressionToken> StringToTokens(string tokenExpression)
        {

            if (tokenExpression == null)
            {
                throw new ArgumentException("参数tokenExpression为空");
            }

            List<ExpressionToken> tokens = new List<ExpressionToken>();

            char[] expChars = tokenExpression.ToCharArray();
            //字符串扫描状态，0：普通 ； 1：日期 ； 2：字符串 3：转义符
            int status = 0;
            var tokenBuffer = new StringBuilder();
            for (int i = 0; i < expChars.Length; i++)
            {
                //读入空格
                if (' ' == expChars[i])
                {
                    if (status == 0)
                    {
                        //一般情况下，读入空格，分割token
                        AddToken(tokenBuffer.ToString(), tokens);
                        //清空buffer
                        tokenBuffer.Clear();
                    }
                    else if (status == 1 || status == 2)
                    {
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        throw new IllegalExpressionException("非法的转义符\"" + expChars[i] + "\" ，位置：" + i);
                    }
                }
                else if ('[' == expChars[i])
                {//读入'['
                    if (status == 0)
                    {
                        status = 1;//进入日期
                        tokenBuffer.Append(expChars[i]);
                    }
                    else if (status == 1)
                    {
                        throw new IllegalExpressionException("非法的日期开始字符，位置：" + i);
                    }
                    else if (status == 2)
                    {
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        throw new IllegalExpressionException("非法的转义符\"" + expChars[i] + "\" ，位置：" + i);
                    }

                }
                else if (']' == expChars[i])
                {//读入']'
                    if (status == 0)
                    {
                        throw new IllegalExpressionException("非法的日期结束字符，位置：" + i);
                    }
                    else if (status == 1)
                    {
                        status = 0; //离开日期
                        tokenBuffer.Append(expChars[i]);

                    }
                    else if (status == 2)
                    {
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        throw new IllegalExpressionException("非法的转义符\"" + expChars[i] + "\" ，位置：" + i);
                    }

                }
                else if ('"' == expChars[i])
                {//读入'"'
                    if (status == 0)
                    {
                        status = 2;//进入字符窜
                        tokenBuffer.Append(expChars[i]);
                    }
                    else if (status == 1)
                    {
                        throw new IllegalExpressionException("非法的日期字符\"" + expChars[i] + "\" ，位置：" + i);
                    }
                    else if (status == 2)
                    {
                        status = 0; //离开字符窜
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        status = 2; //转义”号，离开转义，变为字符串状态
                        tokenBuffer.Append(expChars[i]);
                    }

                }
                else if ('\\' == expChars[i])
                {//读入'\'
                    if (status == 0)
                    {
                        throw new IllegalExpressionException("非法的字符\"" + expChars[i] + "\" ，位置：" + i);
                    }
                    else if (status == 1)
                    {
                        throw new IllegalExpressionException("非法的日期字符\"" + expChars[i] + "\" ，位置：" + i);
                    }
                    else if (status == 2)
                    {
                        status = 3; //进入转义状态
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        status = 2; //转义\号，离开转义，变为字符串状态
                        tokenBuffer.Append(expChars[i]);
                    }

                }
                else
                {//读入其他字符
                    if (status == 0 || status == 1 || status == 2)
                    {
                        tokenBuffer.Append(expChars[i]);
                    }
                    else
                    {
                        throw new IllegalExpressionException("非法的转义符\"" + expChars[i] + "\" ，位置：" + i);
                    }
                }

            }
            if (tokenBuffer.Length > 0)
            {

                AddToken(tokenBuffer.ToString(), tokens);
            }
            return tokens;
        }

        /// <summary>
        /// 将子窜转化成Token并加入列表
        /// </summary>
        /// <param name="tokenString"></param>
        /// <param name="tokens"></param>
        private void AddToken(string tokenString, List<ExpressionToken> tokens)
        {

            ExpressionToken token = null;
            //null
            if (ExpressionTokenHelper.IsNull(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_NULL, null);
                tokens.Add(token);

            }
            else
            //boolean
            if (ExpressionTokenHelper.IsBoolean(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_BOOLEAN, Convert.ToBoolean(tokenString));
                tokens.Add(token);

            }
            else
            //integer
            if (ExpressionTokenHelper.IsInteger(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_INT, Convert.ToInt32(tokenString));
                tokens.Add(token);

            }
            else
            //long 
            if (ExpressionTokenHelper.IsLong(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_LONG, Convert.ToInt64(tokenString.Substring(0, tokenString.Length - 1)));
                tokens.Add(token);

            }
            else
            //float 
            if (ExpressionTokenHelper.IsFloat(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_FLOAT, Convert.ToSingle(tokenString.Substring(0, tokenString.Length - 1)));
                tokens.Add(token);

            }
            else
            //double
            if (ExpressionTokenHelper.IsDouble(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_DOUBLE, Convert.ToDouble(tokenString));
                tokens.Add(token);

            }
            else
            //Date 
            if (ExpressionTokenHelper.IsDateTime(tokenString))
            {
                try
                {
                    token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_DATE, Convert.ToDateTime(tokenString.Substring(1, tokenString.Length - 1)));
                }
                catch (Exception e)
                {
                    throw new IllegalExpressionException("日期参数格式错误");
                }
                tokens.Add(token);

            }
            else
            //String
            if (ExpressionTokenHelper.IsString(tokenString))
            {
                token = ExpressionToken.CreateConstantToken(DataType.DATATYPE_STRING, tokenString.Substring(1, tokenString.Length - 1));
                tokens.Add(token);
            }
            else
            //分割符
            if (ExpressionTokenHelper.IsSplitor(tokenString))
            {
                token = ExpressionToken.CreateSplitorToken(tokenString);
                tokens.Add(token);

            }
            else
            //函数
            if (ExpressionTokenHelper.IsFunction(tokenString))
            {
                token = ExpressionToken.CreateFunctionToken(tokenString.Substring(1, tokenString.Length));
                tokens.Add(token);

            }
            else
            //操作符
            if (ExpressionTokenHelper.IsOperator(tokenString))
            {
                Operator op = ExpressionParser.GetOperator(tokenString);
                token = ExpressionToken.CreateOperatorToken(op);
                tokens.Add(token);

            }
            else
            {
                //剩下的都应该是变量，这个判断依赖于生成的RPN是正确的前提
                //变量,在boolean型和null型判别后，只要是字母打头的，不是$的就是变量
                token = ExpressionToken.CreateVariableToken(tokenString);
                tokens.Add(token);

            }
        }

        /// <summary>
        /// 执行操作符校验
        /// </summary>
        /// <param name="opToken"></param>
        /// <param name="verifyStack"></param>
        /// <returns></returns>
        private ExpressionToken VerifyOperator(ExpressionToken opToken, Stack<ExpressionToken> verifyStack)
        {
            //判定几元操作符
            Operator op = opToken.Op;
            int opType = op.OpType;
            //取得相应的参数个数
            var args = new BaseMetadata[opType];
            ExpressionToken argToken = null;
            for (int i = 0; i < opType; i++)
            {
                if (!(verifyStack.Count == 0))
                {
                    argToken = verifyStack.Pop();

                    if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == argToken.TokenType)
                    {
                        args[i] = argToken.Constant;

                    }
                    else if (ExpressionToken.ETokenType.ETOKEN_TYPE_VARIABLE == argToken.TokenType)
                    {
                        args[i] = argToken.Variable;

                    }
                    else
                    {
                        //如果取到的Token不是常量，也不是变量，则抛出错误
                        throw new IllegalExpressionException("表达式不合法，操作符\"" + op.Token + "\"参数错误;位置：" + argToken.StartPosition
                                , opToken.ToString()
                                , opToken.StartPosition);
                    }

                }
                else
                {
                    //栈已经弹空，没有取道操作符对应的操作数
                    throw new IllegalExpressionException("表达式不合法，操作符\"" + op.Token + "\"找不到相应的参数，或参数个数不足;"
                                    , opToken.ToString()
                                    , opToken.StartPosition);
                }
            }
            //执行操作符校验，并返回校验
            Constant result = op.Verify(opToken.StartPosition, args);
            return ExpressionToken.CreateConstantToken(result);

        }

        //执行函数校验
        private ExpressionToken VerifyFunction(ExpressionToken funtionToken, Stack<ExpressionToken> verifyStack)
        {

            if (!(verifyStack.Count == 0))
            {

                bool doPop = true;
                var args = new List<BaseMetadata>();
                ExpressionToken parameter = null;
                //弹出函数的参数，直到遇到"("时终止
                while (doPop && !(verifyStack.Count == 0))
                {
                    parameter = verifyStack.Pop();

                    if (ExpressionToken.ETokenType.ETOKEN_TYPE_CONSTANT == parameter.TokenType)
                    {
                        //常量
                        args.Add(parameter.Constant);

                    }
                    else if (ExpressionToken.ETokenType.ETOKEN_TYPE_VARIABLE == parameter.TokenType)
                    {
                        args.Add(parameter.Variable);

                    }
                    else if ("(".Equals(parameter.GetSplitor()))
                    {
                        doPop = false;

                    }
                    else
                    {
                        //没有找到应该存在的右括号
                        throw new IllegalExpressionException("表达式不合法，函数\"" + funtionToken.TokenText + "\"遇到非法参数" + parameter.ToString() + ";位置:" + parameter.StartPosition
                                , funtionToken.ToString()
                                , funtionToken.StartPosition);
                    }
                }

                if (doPop && (verifyStack.Count == 0))
                {
                    //操作栈以空，没有找到函数的左括号（
                    throw new IllegalExpressionException("表达式不合法，函数\"" + funtionToken.TokenText + "\"缺少\"(\"；位置:" + (funtionToken.StartPosition + funtionToken.ToString().Length)
                            , funtionToken.ToString()
                            , funtionToken.StartPosition);
                }

                //校验函数
                var arguments = new BaseMetadata[args.Count];
                arguments = args.ToArray();
                Constant result = FunctionExecution.Verify(funtionToken.TokenText, funtionToken.StartPosition, arguments);
                return ExpressionToken.CreateConstantToken(result);


            }
            else
            {
                //没有找到应该存在的右括号
                throw new IllegalExpressionException("表达式不合法，函数\"" + funtionToken.TokenText + "\"不完整"
                        , funtionToken.ToString()
                        , funtionToken.StartPosition);
            }
        }
    }
}
