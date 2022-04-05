using System.Collections.Generic;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class TechnologyManagerSettings
    {
        internal static SettingGroup CaptureAllSettings()
        {
            SettingGroup group = new SettingGroup
            {
                GroupName = "TechnologyManagers"
            };
            if (ZappyTaskService.Instance != null)
            {
                IList<UITechnologyManager> extensions = ZappyTaskService.Instance.GetExtensions<UITechnologyManager>();
                if (extensions == null)
                {
                    return group;
                }
                foreach (UITechnologyManager manager in extensions)
                {
                    group.Setting.Add(new Setting(manager.TechnologyName, null, 1));
                }
            }
            return group;
        }
    }
}