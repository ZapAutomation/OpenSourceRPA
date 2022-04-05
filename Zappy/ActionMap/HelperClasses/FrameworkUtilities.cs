using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.HelperClasses
{
    internal static class FrameworkUtilities
    {
        public static bool IsTopLevelElement(ITaskActivityElement element) =>
            element != null && element.Equals(TopLevelElement(element));

        public static bool IsWindowlessSwitchContainer(ITaskActivityElement element)
        {
            ZappyTaskUtilities.CheckForNull(element, "element");
            try
            {
                return element.IsTreeSwitchingRequired;
            }
            catch (COMException)
            {
            }
            catch (NotImplementedException)
            {
            }
            catch (NotSupportedException)
            {
            }
            return false;
        }

        public static bool TechnologySupportsSkippingIntermediateElements(string technologyName, string controlTypeName)
        {
            UITechnologyManager defaultTechnologyManager = null;
            if (string.IsNullOrEmpty(technologyName))
            {
                defaultTechnologyManager = ZappyTaskService.Instance.PluginManager.DefaultTechnologyManager;
            }
            else
            {
                defaultTechnologyManager = ZappyTaskService.Instance.TechnologyManagerByName(technologyName);
            }
            if (defaultTechnologyManager != null)
            {
                object technologyManagerProperty = UITechnologyManager.GetTechnologyManagerProperty<object>(defaultTechnologyManager, UITechnologyManagerProperty.ExactQueryIdMatch);
                if (technologyManagerProperty is bool)
                {
                    return !(bool)technologyManagerProperty;
                }
                if (!string.IsNullOrEmpty(controlTypeName))
                {
                    IList<ControlType> list = technologyManagerProperty as IList<ControlType>;
                    ControlType controlType = ControlType.GetControlType(controlTypeName);
                    if (list != null && list.Contains(controlType))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static TaskActivityElement TopLevelElement(ITaskActivityElement element)
        {
            TaskActivityElement topLevelElement = null;
            if (element != null)
            {
                TaskActivityElement element3 = element as TaskActivityElement;
                if (element3 != null)
                {
                    topLevelElement = element3.TopLevelElement;
                }
                if (topLevelElement != null)
                {
                    return topLevelElement;
                }
                if (element.QueryId != null && element.QueryId.Ancestor != null)
                {
                    topLevelElement = TopLevelElement(element.QueryId.Ancestor);
                }
                if (topLevelElement == null)
                {
                    topLevelElement = ZappyTaskService.Instance.GetTopLevelElementUsingWindowTree(element);
                }
            }
            return topLevelElement;
        }
    }
}
