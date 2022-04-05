using System;
using System.Collections.Generic;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension
{
    internal interface IUiaControlsPropertyProvider
    {
        int GetControlSupportLevel(ZappyTaskControl control);
        Dictionary<string, ZappyTaskPropertyDescriptor> GetPropertiesMap();
        Type GetPropertyClassName();
        string GetPropertyForAction(ZappyTaskAction action);
        List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues);
        object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName);
        Type GetSpecializedClass();
        bool IsCommonReadableProperty(string propertyName);
        bool IsCommonWritableProperty(string propertyName);
        void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value);

        ControlType SupportedControlType { get; }
    }
}