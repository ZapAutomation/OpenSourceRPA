using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Properties;

namespace Zappy.Plugins.Uia.Uia
{
    internal sealed class UiaExtensionPackage : ZappyTaskExtensionPackageBase
    {
        public UiaExtensionPackage() : base("UiaExtensionPackage", Resources.PackageDescription)
        {
            UITechnologyManager service = new UiaTechnologyManager();
            AddService(typeof(UITechnologyManager), service);
        }
    }
}

