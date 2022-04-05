using System;
using System.Reflection;

namespace Zappy.Helpers
{
    public class TypeProxy : MarshalByRefObject
    {
        public Type LoadFromAssembly(string assemblyPath, string typeName)
        {
            try
            {
                var asm = Assembly.LoadFile(assemblyPath);
                                return asm.GetType(typeName);
            }
            catch (Exception) { return null; }
        }
    }
}