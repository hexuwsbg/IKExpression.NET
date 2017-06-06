using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Functions
{
    public class Parameter
    {
        Type type;//参数类型
        object value;//参数值

        public Parameter(string _type, string _value)
        {
            try
            {

                type = getTypeClass(_type);
                //        Constructor c = type.getConstructor(new Class[] { String.class});
                //value = c.newInstance(_value);

            }
            catch (Exception e)
            {

            }

        }

        public Parameter(String _type)
        {
            try
            {
                type = getTypeClass(_type);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

        }

        private Type getTypeClass(string _type)
        {
            if ("bool".Equals(_type))
            {
                return typeof(bool);
            }
            else if ("byte".Equals(_type))
            {
                return typeof(byte);
            }
            else if ("char".Equals(_type))
            {
                return typeof(char);
            }
            else if ("double".Equals(_type))
            {
                return typeof(double);
            }
            else if ("float".Equals(_type))
            {
                return typeof(float);
            }
            else if ("int".Equals(_type))
            {
                return typeof(int);
            }
            else if ("long".Equals(_type))
            {
                return typeof(long);
            }
            else if ("short".Equals(_type))
            {
                return typeof(short);
            }
            return Type.GetType(_type);

        }
    }
}
