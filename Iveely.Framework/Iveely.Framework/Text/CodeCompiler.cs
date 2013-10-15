﻿/*==========================================
 *创建人：刘凡平
 *邮  箱：liufanping@iveely.com
 *电  话：13896622743
 *版  本：0.1.0
 *Iveely=I void everything,except love you!
 *========================================*/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iveely.Framework.Text
{
    /// <summary>
    /// C# 代码编译器
    /// 系统引用命名空间：system.dll
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class CodeCompiler
    {
        /// <summary>
        /// 编译源码
        /// </summary>
        /// <param name="sourceCode">源码</param>
        /// <returns>返回错误集合</returns>
        public static string Compile(string[] sourceCode, List<string> references)
        {
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            if (references != null)
            {
                foreach (var reference in references)
                {
                    objCompilerParameters.ReferencedAssemblies.Add(reference);
                }
            }
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, GenerateCode(sourceCode));

            if (cr.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder();
                foreach (CompilerError err in cr.Errors)
                {
                    string errorInformation = string.Format("line:{0},Cloumn:{1},error:{2}", err.Line, err.Column,
                        err.ErrorText);
                    errors.AppendLine(errorInformation);
                }
                return errors.ToString();
            }
            return string.Empty;
        }

        public static object Execode(string code, string className, List<string> libraries, object[] parameters)
        {
            string functionName = "Run";
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            if (libraries != null)
            {
                foreach (var library in libraries)
                {
                    compilerParameters.ReferencedAssemblies.Add(library);
                }
            }

            CompilerResults compilerResults = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(compilerParameters, code);
            Assembly assembly = compilerResults.CompiledAssembly;
            object instance = assembly.CreateInstance(className);
            if (instance != null)
            {
                MethodInfo method = instance.GetType().GetMethod(functionName);
                return method.Invoke(instance, parameters);
            }
            throw new NullReferenceException("Instance can not be null.");
        }

        /// <summary>
        /// 生成源码文本
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        private static string GenerateCode(string[] sourceCode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var code in sourceCode)
            {
                builder.AppendLine(code);
            }
            return builder.ToString();
        }
#if DEBUG

        [TestMethod]
        public void TestCompiler()
        {
            List<string> wrongCode = new List<string>();
            wrongCode.Add("using System;");
            wrongCode.Add("public class Program");
            wrongCode.Add("{");
            wrongCode.Add("public static void Main(string[] args)");
            wrongCode.Add("{");
            wrongCode.Add("Console.WriteLine()");
            wrongCode.Add("}");
            wrongCode.Add("}");
            string[] wrongCodes = wrongCode.ToArray();
            Assert.IsFalse(Compile(wrongCodes, null) == string.Empty);

            string[] rightCodes = wrongCodes;
            rightCodes[5] = "";
            Assert.IsTrue(Compile(rightCodes, null) == string.Empty);
        }

        [TestMethod]
        public void TestExecode()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("namespace test");
            builder.AppendLine("{");
            builder.AppendLine("public class Program");
            builder.AppendLine("{");
            builder.AppendLine("public int Run()");
            builder.AppendLine("{");
            builder.AppendLine("return 100;");
            builder.AppendLine("}");
            builder.AppendLine("}");
            builder.AppendLine("}");
            int result = int.Parse(Execode(builder.ToString(), "test.Program", null, null).ToString());
            Assert.IsTrue(result == 100);
        }

#endif
    }
}
