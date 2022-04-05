using Crapy.ActionMap.TaskTechnology;
using Crapy.Decode.Helper;

namespace Crapy.ActionMap.ZappyTaskUtil
{
    internal class SqmUpdater : ISqmUpdater
    {
        private static ISqmUpdater s_Instance;

        private SqmUpdater()
        {
        }

        public void UpdateSqmForElement(ITaskActivityElement element)
        {
            if (element != null)
            {
                switch (element.Framework)
                {
                    case "MSAA":
                        ZappyTaskUtilities.UpdateSqmForMsaaControl(element.ClassName);
                        break;

                    case "Web":
                        SqmUtility.SqmPluginInfo |= PluginEnum.IePlugin;
                        SqmUtility.IeElementCount++;
                        break;

                    case "Silverlight":
                        SqmUtility.SqmPluginInfo |= PluginEnum.SilverLight;
                        break;

                    case "UiaWidget":
                        SqmUtility.SqmPluginInfo |= PluginEnum.UiaWidget;
                        SqmUtility.UiaWidgetElementCount++;
                        break;

                    case "UIA":
                        SqmUtility.SqmPluginInfo |= PluginEnum.UiaPlugin;
                        SqmUtility.UiaElementCount++;
                        break;

                    default:
                        SqmUtility.SqmPluginInfo |= PluginEnum.OtherPlugin;
                        SqmUtility.OtherElementCount++;
                        break;
                }
                if (element.Framework != "UiaWidget" && string.IsNullOrWhiteSpace(element.FriendlyName))
                {
                    SqmUtility.UnknownErrorCount++;
                }
            }
        }

        public static ISqmUpdater Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SqmUpdater();
                }
                return s_Instance;
            }
        }
    }
}