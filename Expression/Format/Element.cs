using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format
{
    public class Element
    {
        public enum ElementType
        {
            //NULL类型
            NULL,
            //字符窜
            STRING,
            //布尔类
            BOOLEAN,
            //整数
            INT,
            //长整数
            LONG,
            //浮点数
            FLOAT,
            //双精度浮点
            DOUBLE,
            //日期时间
            DATE,

            //变量
            VARIABLE,
            //操作符
            OPERATOR,
            //函数
            FUNCTION,
            //分隔符
            SPLITOR
        }

        public string Text { set; get; }
        public ElementType Type { set; get; }//类型
        public int Index { set; get; }//元素在表达式中的起始索引号，从0算起

        public Element(string text, int index, ElementType type)
        {
            this.Text = text;
            this.Index = index;
            this.Type = type;
        }
        
    }
}
