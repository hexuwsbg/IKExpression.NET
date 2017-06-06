using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Metadata
{
    public class Variable : BaseMetadata
    {
        //变量名
        public string VariableName { protected set; get; }

        /**
         * 根据别名和参数值，构造 Variable 实例
         * @param variableName
         * @param variableValue
         * @return Variable
         */
        public static Variable CreateVariable(string variableName, object variableValue)
        {

            if (variableValue is bool)
            {
                return new Variable(variableName, DataType.DATATYPE_BOOLEAN, variableValue);

            }
            else if (variableValue is DateTime)
            {
                return new Variable(variableName, DataType.DATATYPE_DATE, variableValue);

            }
            else if (variableValue is double)
            {
                return new Variable(variableName, DataType.DATATYPE_DOUBLE, variableValue);

            }
            else if (variableValue is float)
            {
                return new Variable(variableName, DataType.DATATYPE_FLOAT, variableValue);

            }
            else if (variableValue is int)
            {
                return new Variable(variableName, DataType.DATATYPE_INT, variableValue);

            }
            else if (variableValue is long)
            {
                return new Variable(variableName, DataType.DATATYPE_LONG, variableValue);

            }
            else if (variableValue is string)
            {
                return new Variable(variableName, DataType.DATATYPE_STRING, variableValue);

            }
            else if (variableValue is IList)
            {
                return new Variable(variableName, DataType.DATATYPE_LIST, variableValue);

            }
            else if (variableValue is object)
            {
                return new Variable(variableName, DataType.DATATYPE_OBJECT, variableValue);

            }
            else if (variableValue == null)
            {
                return new Variable(variableName, DataType.DATATYPE_NULL, variableValue);

            }
            else
            {
                throw new ArgumentException("非法参数：无法识别的变量类型");
            }

        }

        public Variable(string variableName) : this(variableName, DataType.DATATYPE_NULL, null)
        {
        }

        public Variable(string variableName, DataType variableDataType, object variableValue)
            : base(variableDataType, variableValue)
        {
            if (variableName == null)
            {
                throw new ArgumentException("非法参数：变量名为空");
            }

            this.VariableName = variableName;
        }

        public void SetVariableValue(object variableValue)
        {
            this.DataValue = variableValue;
            //参数类型校验
            VerifyMetadata();
        }

        public override void SetDataType(DataType dataType)
        {
            base.SetDataType(dataType);
            //参数类型校验
            VerifyMetadata();
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;

            }
            else if (o is Variable

                && base.Equals(o))
            {

                Variable var = (Variable)o;
                if (VariableName != null
                        && VariableName.Equals(var.VariableName))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
