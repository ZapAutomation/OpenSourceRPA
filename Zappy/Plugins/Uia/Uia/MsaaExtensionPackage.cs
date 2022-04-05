using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Mssa;
using Zappy.Properties;

namespace Zappy.Plugins.Uia.Uia
{
    internal sealed class MsaaExtensionPackage : ZappyTaskExtensionPackageBase
    {
        public MsaaExtensionPackage() : base("MsaaExtensionPackage", Resources.PackageDescription)
        {
            UITechnologyManager service = MsaaZappyPlugin.Instance;
            AddService(typeof(UITechnologyManager), service);
        }
    }
}