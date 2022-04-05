using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Plugins.Excel;
using Resources = Zappy.Properties.Resources;


namespace Zappy.ExecuteTask.Extension
{
    internal sealed class ZappyTaskingExtensionPackage : ZappyTaskExtensionPackageBase
    {
        public ZappyTaskingExtensionPackage() : base("ZappyTaskingExtensionPackage", Resources.ZappyTaskingExtensionPackageDescription)
        {
            ZappyTaskPropertyProvider[] service = new ZappyTaskPropertyProvider[] {
                new WinPropertyProvider(),
                new HtmlPropertyProvider(), new UiaPropertyProvider(), new  ExcelPropertyProvider(), 
            TechnologyElementPropertyProvider.Instance };            AddService(typeof(ZappyTaskPropertyProvider),
                service);
                                            }
    }
}
