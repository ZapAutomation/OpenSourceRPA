using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.Decode.Mssa
{
    internal static class PluginUtilities
    {
        private static Dictionary<string, string> controlTypesForCheckingNamesAndValue;

        public static bool CheckForValueAndNameEquals(string controlType) =>
            ControlTypesForCheckingNamesAndValue.ContainsKey(controlType);

        private static Dictionary<string, string> ControlTypesForCheckingNamesAndValue
        {
            get
            {
                if (controlTypesForCheckingNamesAndValue == null)
                {
                    controlTypesForCheckingNamesAndValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.Calendar.Name, ControlType.Calendar.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.ComboBox.Name, ControlType.ComboBox.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.DatePicker.Name, ControlType.DatePicker.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.DateTimePicker.Name, ControlType.DateTimePicker.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.Edit.Name, ControlType.Edit.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.List.Name, ControlType.List.Name);
                    controlTypesForCheckingNamesAndValue.Add(ControlType.Cell.Name, ControlType.Cell.Name);
                }
                return controlTypesForCheckingNamesAndValue;
            }
        }
    }
}