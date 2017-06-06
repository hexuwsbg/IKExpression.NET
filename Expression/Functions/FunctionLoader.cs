using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Functions
{
    public class FunctionLoader
    {
        private static string FILE_NAME = "/IKExpression.cfg.xml";

        public static FunctionLoader SingleLoader = new FunctionLoader();

        //所有方法Map
        public Dictionary<string, StaticFunctionInvoker> functionDict = new Dictionary<string, StaticFunctionInvoker>();

        /**
         * 私有，禁止外部新建
         */
        private FunctionLoader()
        {
            try
            {
                //init();
                Init();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message, e);
            }
        }

        //初始化  根据SystemFunctions包含的类去加载
        private void Init()
        {
            Type type = typeof(SystemFunctions);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach(var method in methods)
            {
                StaticFunctionInvoker invoker = new StaticFunctionInvoker(method);
                functionDict.Add(method.Name, invoker);
            }
        }


        /**
         * 初始化,解析XML配置
         * @throws ParserConfigurationException 
         * @throws Exception 
         */
        /*
       private void init()
       {
           DocumentBuilderFactory factory=DocumentBuilderFactory.newInstance();
           DocumentBuilder builder=factory.newDocumentBuilder();
           Document doc = builder.parse(FunctionLoader.class.getResourceAsStream(FILE_NAME));
           NodeList rootNodes = doc.getElementsByTagName("function-configuration");
           if (rootNodes.getLength() < 1) {
               return;
           }
           rootNodes = rootNodes.item(0).getChildNodes();
           for (int i = 0; i<rootNodes.getLength(); i++) {
               Node beanNode = rootNodes.item(i);
               if(!beanNode.getNodeName().equals("bean")) {
                   continue;
               }
               //读class类
               String className = beanNode.getAttributes().getNamedItem("class").getNodeValue();
       Class _class = Class.forName(className);

       NodeList subNodes = beanNode.getChildNodes();
       List<Parameter> constructorArgs = null;
       HashSet<Function> functions = new HashSet<Function>();
               for (int j = 0; j<subNodes.getLength(); j++) {
                   Node subNode = subNodes.item(j);
                   if(subNode.getNodeName().equals("constructor-args") && constructorArgs == null) {
                       //读取类的构造参数
                       constructorArgs = parseConstructorArgs(subNode);
   } else if (subNode.getNodeName().equals("function")) {
                       //读取方法描述
                       if (!functions.add(parseFunctions(subNode))) {
                           throw new SAXException("方法名不能重复");
}
                   }
               }
               if (functions.size() <= 0) {
                   continue;
               }
               Object ins = null;
               if (constructorArgs == null || constructorArgs.size() <= 0) {
                   //使用默认构造函数
                   ins = _class.newInstance();
               } else {
                   //使用给定的构造函数
                   Class[] cs = getParameterTypes(constructorArgs);
Object[] ps = getParameterValues(constructorArgs);
Constructor c = _class.getConstructor(cs);
ins = c.newInstance(ps);
               }
               //封装FunctionInvoker放入Map中
               for (Function f : functions) {
                   Method m = _class.getMethod(f.methodName, getParameterTypes(f.types));
functionMap.put(f.name, new FunctionInvoker(m, ins));
               }

           }

   }*/

        /**
         * 解析构参数法的描述
         * @param argRootNode
         * @return
         */
        /*
       private List<Parameter> parseConstructorArgs(Node argRootNode)
   {
       NodeList argsNode = argRootNode.getChildNodes();
       List<Parameter> args = new List<Parameter>();
       for (int i = 0; i < argsNode.getLength(); i++)
       {
           Node argNode = argsNode.item(i);
           if (argNode.getNodeName().equals("constructor-arg"))
           {
               //参数类型
               String type = argNode.getAttributes().getNamedItem("type").getNodeValue();
               //参数值
               String value = argNode.getTextContent();
               args.add(new Parameter(type, value));
           }
       }
       return args;
   }
   */

        /**
         * 解析方法的描述
         * @param funRootNode
         * @return
         *//*
        private Function parseFunctions(Node funRootNode)
        {
            String name = funRootNode.getAttributes().getNamedItem("name").getNodeValue();
            String methodName = funRootNode.getAttributes().getNamedItem("method").getNodeValue();
            Function f = new Function(name, methodName);
            NodeList argsNode = funRootNode.getChildNodes();
            for (int i = 0; i < argsNode.getLength(); i++)
            {
                Node argNode = argsNode.item(i);
                if (argNode.getNodeName().equals("parameter-type"))
                {
                    //参数类型
                    f.addType(argNode.getTextContent());
                }
            }
            return f;
        }
        */

        /**
         * 表达式可用函数除了从配置文件“functionConfig.xml”加载外，
         * 还可以通过此方法运行时添加
         * @param functionName 方法别名，表达式使用的名称
         * @param instance 调用的实例名
         * @param method 调用的方法
         */
        public static void AddFunction(string functionName, MethodInfo method)
        {
            if (functionName == null || method == null)
            {
                return;
            }
            SingleLoader.functionDict.Add(functionName, new StaticFunctionInvoker(method));
        }
        
        /// <summary>
        /// 加载函数
        /// </summary>
        /// <param name="functionName">upper case</param>
        /// <returns></returns>
        public static MethodInfo LoadFunction(string functionName)
        {
            if (SingleLoader.functionDict.ContainsKey(functionName))
            {
                return SingleLoader.functionDict[functionName].Method;
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="functionName">upper case</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeFunction(string functionName, object[] parameters)
        {
            if (SingleLoader.functionDict.ContainsKey(functionName))
            {
                StaticFunctionInvoker f = SingleLoader.functionDict[functionName];
                return f.invoke(parameters);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
