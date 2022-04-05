using System;

namespace Zappy.Helpers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class RunScriptFunctionType : Attribute
    {
        public Type FunctionType { get; set; }
        public RunScriptFunctionType(Type FunctionType)
        {
            this.FunctionType = FunctionType;
        } //...
    }
}
