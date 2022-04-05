using System;
using System.Collections.Generic;
using System.Drawing;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;

namespace Zappy.ExecuteTask.TaskExecutor
{
    internal static class ZappyTaskControlProperties
    {
        private static Dictionary<string, string> commonPropertyNameDictionary = InitializeCommonList();
        private static Dictionary<string, Type> nonAssertablePropertyNames = InitializeNonAssertableProperties();
        private static Dictionary<string, Type> readablePropertyNames = InitializeReadableProperties();

        private static Dictionary<string, string> InitializeCommonList() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.BoundingRectangle,
                    ZappyTaskControl.PropertyNames.BoundingRectangle
                },
                {
                    ZappyTaskControl.PropertyNames.ClassName,
                    ZappyTaskControl.PropertyNames.ClassName
                },
                {
                    ZappyTaskControl.PropertyNames.ControlType,
                    ZappyTaskControl.PropertyNames.ControlType
                },
                {
                    ZappyTaskControl.PropertyNames.Exists,
                    ZappyTaskControl.PropertyNames.Exists
                },
                {
                    ZappyTaskControl.PropertyNames.Enabled,
                    ZappyTaskControl.PropertyNames.Enabled
                },
                {
                    ZappyTaskControl.PropertyNames.FriendlyName,
                    ZappyTaskControl.PropertyNames.FriendlyName
                },
                {
                    ZappyTaskControl.PropertyNames.HasFocus,
                    ZappyTaskControl.PropertyNames.HasFocus
                },
                {
                    ZappyTaskControl.PropertyNames.IsTopParent,
                    ZappyTaskControl.PropertyNames.IsTopParent
                },
                {
                    ZappyTaskControl.PropertyNames.Name,
                    ZappyTaskControl.PropertyNames.Name
                },
                {
                    ZappyTaskControl.PropertyNames.NativeElement,
                    ZappyTaskControl.PropertyNames.NativeElement
                },
                {
                    ZappyTaskControl.PropertyNames.QueryId,
                    ZappyTaskControl.PropertyNames.QueryId
                },
                {
                    ZappyTaskControl.PropertyNames.TechnologyName,
                    ZappyTaskControl.PropertyNames.TechnologyName
                },
                {
                    ZappyTaskControl.PropertyNames.TopParent,
                    ZappyTaskControl.PropertyNames.TopParent
                },
                {
                    ZappyTaskControl.PropertyNames.WindowHandle,
                    ZappyTaskControl.PropertyNames.WindowHandle
                },
                {
                    ZappyTaskControl.PropertyNames.Height,
                    ZappyTaskControl.PropertyNames.Height
                },
                {
                    ZappyTaskControl.PropertyNames.Width,
                    ZappyTaskControl.PropertyNames.Width
                },
                {
                    ZappyTaskControl.PropertyNames.Top,
                    ZappyTaskControl.PropertyNames.Top
                },
                {
                    ZappyTaskControl.PropertyNames.Left,
                    ZappyTaskControl.PropertyNames.Left
                }
            };

        private static Dictionary<string, Type> InitializeNonAssertableProperties() =>
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.BoundingRectangle,
                    typeof(Rectangle)
                },
                {
                    ZappyTaskControl.PropertyNames.NativeElement,
                    typeof(object)
                },
                {
                    ZappyTaskControl.PropertyNames.UITechnologyElement,
                    typeof(TaskActivityElement)
                },
                {
                    ZappyTaskControl.PropertyNames.WindowHandle,
                    typeof(IntPtr)
                },
                {
                    ZappyTaskControl.PropertyNames.TopParent,
                    typeof(ZappyTaskControl)
                }
            };

        private static Dictionary<string, Type> InitializeReadableProperties() =>
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) {
                {
                    ZappyTaskControl.PropertyNames.BoundingRectangle,
                    typeof(Rectangle)
                },
                {
                    ZappyTaskControl.PropertyNames.ClassName,
                    typeof(string)
                },
                {
                    ZappyTaskControl.PropertyNames.ControlType,
                    typeof(ControlType)
                },
                {
                    ZappyTaskControl.PropertyNames.TechnologyName,
                    typeof(string)
                },
                {
                    ZappyTaskControl.PropertyNames.FriendlyName,
                    typeof(string)
                },
                {
                    ZappyTaskControl.PropertyNames.HasFocus,
                    typeof(bool)
                },
                {
                    ZappyTaskControl.PropertyNames.Exists,
                    typeof(bool)
                },
                {
                    ZappyTaskControl.PropertyNames.Enabled,
                    typeof(bool)
                },
                {
                    ZappyTaskControl.PropertyNames.Name,
                    typeof(string)
                },
                {
                    ZappyTaskControl.PropertyNames.NativeElement,
                    typeof(object)
                },
                {
                    ZappyTaskControl.PropertyNames.WindowHandle,
                    typeof(IntPtr)
                },
                {
                    ZappyTaskControl.PropertyNames.TopParent,
                    typeof(ZappyTaskControl)
                },
                {
                    ZappyTaskControl.PropertyNames.IsTopParent,
                    typeof(bool)
                },
                {
                    ZappyTaskControl.PropertyNames.Height,
                    typeof(int)
                },
                {
                    ZappyTaskControl.PropertyNames.Width,
                    typeof(int)
                },
                {
                    ZappyTaskControl.PropertyNames.Top,
                    typeof(int)
                },
                {
                    ZappyTaskControl.PropertyNames.Left,
                    typeof(int)
                }
            };

        internal static Dictionary<string, string> CommonPropertyNames =>
            commonPropertyNameDictionary;

        internal static Dictionary<string, Type> NonAssertablePropertyNames =>
            nonAssertablePropertyNames;

        internal static Dictionary<string, Type> ReadablePropertyNames =>
            readablePropertyNames;
    }
}