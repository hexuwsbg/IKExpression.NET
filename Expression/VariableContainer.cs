using Expression.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Expression
{
    public class VariableContainer
    {
        private static ThreadLocal<Dictionary<string, Variable>> variableMapThreadLocal = new ThreadLocal<Dictionary<string, Variable>>();
        
        public static Dictionary<string, Variable> GetVariableDict()
        {
            Dictionary<string, Variable> variableMap = variableMapThreadLocal.Value;
            if (variableMap == null)
            {
                variableMap = new Dictionary<string, Variable>();
                variableMapThreadLocal.Value = variableMap;
            }
            return variableMap;
        }

        public static void SetVariableDict(Dictionary<string, Variable> variableMap)
        {
            RemoveVariableDict();
            if (variableMap != null)
            {
                variableMapThreadLocal.Value = variableMap;
            }
        }

        public static void RemoveVariableDict()
        {
            Dictionary<string, Variable> variableMap = variableMapThreadLocal.Value;
            if (variableMap != null)
            {
                variableMap.Clear();
            }
            //variableMapThreadLocal.remove();
        }

        public static void AddVariable(Variable variable)
        {
            if (variable != null)
            {
                GetVariableDict().Add(variable.VariableName, variable);
            }
        }

        public static Variable GetVariable(string variableName)
        {
            if (variableName != null)
            {
                return GetVariableDict()[variableName];
            }
            else
            {
                return null;
            }
        }

        public static Variable removeVariable(string variableName)
        {
            var variable = GetVariableDict()[variableName];
            if(variable != null)
            {
                GetVariableDict().Remove(variableName);
            }
            return variable;
        }

        public static bool removeVariable(Variable variable)
        {

            return GetVariableDict().Remove(variable.VariableName);

        }
    }
}
