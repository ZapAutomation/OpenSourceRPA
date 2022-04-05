using System;

namespace Zappy.Decode.Mssa
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ZappyTaskExtensionPackageAttribute : Attribute
    {
        private Type extensionPackageType;
        private string name;

        public ZappyTaskExtensionPackageAttribute(string name, Type extensionPackageType)
        {
            this.name = name;
            this.extensionPackageType = extensionPackageType;
        }

        public Type ExtensionPackageType =>
            extensionPackageType;

        public string Name =>
            name;
    }
}