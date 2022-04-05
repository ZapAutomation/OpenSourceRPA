using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    internal class CompilerRunner
    {
        private Assembly assembly;
        private string className;
        private string methodName;

        public CompilerRunner(string code, string className, string methodName, int errLineOffset = 0)
        {
            this.className = className;
            this.methodName = methodName;
            this.Compile(code, errLineOffset);
        }

        private void Compile(string code, int errLineOffset)
        {
            CompilerParameters options = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };
            string[] strArray = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                 where !a.IsDynamic
                                 select a.Location into location
                                 where !string.IsNullOrWhiteSpace(location)
                                 select location).ToArray<string>();
            options.ReferencedAssemblies.AddRange(strArray);
            string[] sources = new string[] { code };
                        CompilerResults res = new VBCodeProvider().CompileAssemblyFromSource(options, sources);
            if (!res.Errors.HasErrors)
            {
                this.assembly = res.CompiledAssembly;
            }
            else
            {
                this.assembly = null;
                throw new ArgumentException("Error compiling code\n" + GetErrorText(res, errLineOffset));
            }
        }

        private static string GetErrorText(CompilerResults res, int errLineOffset)
        {
            string str = "";
            foreach (CompilerError error in res.Errors)
            {
                if (!error.IsWarning)
                {
                    string str2 = $"error {error.ErrorNumber}: {error.ErrorText} At line {error.Line - errLineOffset}";
                    str = str + str2 + "\n";
                }
            }
            return str;
        }

        public object Run(object[] args) =>
            this.assembly?.GetType(this.className).InvokeMember(this.methodName, BindingFlags.InvokeMethod, null, this.assembly, args);


    }
}