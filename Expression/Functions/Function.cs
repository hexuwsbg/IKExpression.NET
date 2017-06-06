using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Functions
{
    public class Function
    {
        string name;//表达式使用的方法名称
        string methodName;//类是的实际方法名称
        List<Parameter> types;//方法的参数类型
        public Function(string _name, string _methodName)
        {
            if (_name == null || _methodName == null)
            {
                throw new ArgumentException();
            }
            name = _name;
            methodName = _methodName;
            types = new List<Parameter>();
        }

        public void addType(string type)
        {
            types.Add(new Parameter(type));
        }
        public int hashCode()
        {
            return name.GetHashCode();
        }

        public bool equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Function other = (Function)obj;
            return name.Equals(other.name);
        }
    }
}
