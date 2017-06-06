using Expression.Metadata;
using Expression.Operation.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Operation
{
    public class Operator
    {
        #region defined operator
        //逻辑否
        public static Operator NOT = new Operator("!", 80, 1);
        //取负
        public static Operator NG = new Operator("-", 80, 1);
        //算术乘
        public static Operator MUTI = new Operator("*", 70, 2);
        //算术除
        public static Operator DIV = new Operator("/", 70, 2);
        //算术取余
        public static Operator MOD = new Operator("%", 70, 2);
        //算术加
        public static Operator PLUS = new Operator("+", 60, 2);
        //算术减
        public static Operator MINUS = new Operator("-", 60, 2);
        //逻辑小于
        public static Operator LT = new Operator("<", 50, 2);
        //逻辑小等于
        public static Operator LE = new Operator("<=", 50, 2);
        //逻辑大于
        public static Operator GT = new Operator(">", 50, 2);
        //逻辑大等于
        public static Operator GE = new Operator(">=", 50, 2);
        //逻辑等
        public static Operator EQ = new Operator("==", 40, 2);
        //逻辑不等
        public static Operator NEQ = new Operator("!=", 40, 2);

        //逻辑与
        public static Operator AND = new Operator("&&", 30, 2);

        //逻辑或
        public static Operator OR = new Operator("||", 20, 2);

        //集合添加
        public static Operator APPEND = new Operator("#", 10, 2);


        //三元选择
        public static Operator QUES = new Operator("?", 0, 0);

        public static Operator COLON = new Operator(":", 0, 0);

        public static Operator SELECT = new Operator("?:", 0, 3);
        #endregion

        #region private static field
        private static HashSet<string> OP_RESERVE_WORD = new HashSet<string>();

        private static Dictionary<Operator, IOperatorExecution> OP_EXEC_MAP

                        = new Dictionary<Operator, IOperatorExecution>();

        static Operator()
        {

            OP_RESERVE_WORD.Add(NOT.Token);
            OP_RESERVE_WORD.Add(NG.Token);

            OP_RESERVE_WORD.Add(MUTI.Token);
            OP_RESERVE_WORD.Add(DIV.Token);
            OP_RESERVE_WORD.Add(MOD.Token);

            OP_RESERVE_WORD.Add(PLUS.Token);
            OP_RESERVE_WORD.Add(MINUS.Token);


            OP_RESERVE_WORD.Add(LT.Token);
            OP_RESERVE_WORD.Add(LE.Token);
            OP_RESERVE_WORD.Add(GT.Token);
            OP_RESERVE_WORD.Add(GE.Token);

            OP_RESERVE_WORD.Add(EQ.Token);
            OP_RESERVE_WORD.Add(NEQ.Token);

            OP_RESERVE_WORD.Add(AND.Token);

            OP_RESERVE_WORD.Add(OR.Token);

            OP_RESERVE_WORD.Add(APPEND.Token);

            OP_RESERVE_WORD.Add(SELECT.Token);
            OP_RESERVE_WORD.Add(QUES.Token);
            OP_RESERVE_WORD.Add(COLON.Token);


            OP_EXEC_MAP.Add(NOT, new Op_NOT());
            OP_EXEC_MAP.Add(NG, new Op_NG());

            OP_EXEC_MAP.Add(MUTI, new Op_MUTI());
            OP_EXEC_MAP.Add(DIV, new Op_DIV());
            OP_EXEC_MAP.Add(MOD, new Op_MOD());

            OP_EXEC_MAP.Add(PLUS, new Op_PLUS());
            OP_EXEC_MAP.Add(MINUS, new Op_MINUS());

            OP_EXEC_MAP.Add(LT, new Op_LT());
            OP_EXEC_MAP.Add(LE, new Op_LE());
            OP_EXEC_MAP.Add(GT, new Op_GT());
            OP_EXEC_MAP.Add(GE, new Op_GE());

            OP_EXEC_MAP.Add(EQ, new Op_EQ());
            OP_EXEC_MAP.Add(NEQ, new Op_NEQ());

            OP_EXEC_MAP.Add(AND, new Op_AND());

            OP_EXEC_MAP.Add(OR, new Op_OR());

            OP_EXEC_MAP.Add(APPEND, new Op_APPEND());

            //OP_EXEC_MAP.Add(SELECT, new Op_SELECT());
            //OP_EXEC_MAP.Add(QUES, new Op_QUES());
            //OP_EXEC_MAP.Add(COLON, new Op_COLON());
        }
        #endregion

        public string Token { protected set; get; }

        public int Priority { protected set; get; }

        public int OpType { protected set; get; }

        public Operator(string token, int priority, int opType)
        {
            this.Token = token;
            this.Priority = priority;
            this.OpType = opType;
        }

        /// <summary>
        /// 执行操作，并返回结果Token
        /// </summary>
        /// <param name="args">注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数</param>
        /// <returns>常量型的执行结果</returns>
        public Constant Execute(Constant[] args)
        {

            IOperatorExecution opExec = OP_EXEC_MAP[this];
            if (opExec == null)
            {
                throw new Exception("系统内部错误：找不到操作符对应的执行定义");
            }
            return opExec.Execute(args);
        }

        /// <summary>
        /// 检查操作符和参数是否合法，是可执行的
        /// 如果合法，则返回含有执行结果类型的Token
        /// 如果不合法，则返回null
        /// </summary>
        /// <param name="opPositin">操作符位置</param>
        /// <param name="args">注意args中的参数由于是从栈中按LIFO顺序弹出的，所以必须从尾部倒着取数</param>
        /// <returns></returns>
        public Constant Verify(int opPositin, BaseMetadata[] args)
        {

            IOperatorExecution opExec = OP_EXEC_MAP[this];
            if (opExec == null)
            {
                throw new Exception("系统内部错误：找不到操作符对应的执行定义");
            }
            return opExec.Verify(opPositin, args);
        }
    }
}
