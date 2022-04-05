


using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ZappyMessages.ExcelMessages;
using ControlStates = Zappy.ActionMap.Enums.ControlStates;


namespace Zappy.Plugins.Excel
{
                internal class ExcelPropertyProvider : ZappyTaskPropertyProvider
    {

                                                        public override int GetControlSupportLevel(ZappyTaskControl ZappyTaskControl)
        {
                        if (string.Equals(ZappyTaskControl.TechnologyName, ExcelTechnologyUtilities.ExcelTechnologyName, StringComparison.OrdinalIgnoreCase) &&
                ZappyTaskControl.ControlType == ControlType.Cell)
            {
                return 101;
            }

            return 0;
        }

                                                public override ICollection<string> GetPropertyNames(ZappyTaskControl ZappyTaskControl)
        {
                        return propertiesMap.Keys;
        }

                                                        public override ZappyTaskPropertyDescriptor GetPropertyDescriptor(ZappyTaskControl ZappyTaskControl, string propertyName)
        {
                        if (propertiesMap.ContainsKey(propertyName))
            {
                return propertiesMap[propertyName];
            }

            return null;
        }

                                                                                        public override object GetPropertyValue(ZappyTaskControl ZappyTaskControl, string propertyName)
        {
                        ExcelCellInfo cellInfo = GetCellInfo(ZappyTaskControl);
            return ExcelCommunicator.Instance.GetCellProperty(cellInfo, propertyName);
        }

                                                                                        public override void SetPropertyValue(ZappyTaskControl ZappyTaskControl, string propertyName, object propertyValue)
        {
                        ExcelCellInfo cellInfo = GetCellInfo(ZappyTaskControl);
                                    if (string.Equals(propertyName, PropertyNames.Value, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(propertyName, PropertyNames.Formula, StringComparison.OrdinalIgnoreCase))
            {
                Keyboard.SendKeys(ZappyTaskControl, "{Delete}");
                Keyboard.SendKeys(ZappyTaskControl, propertyValue as string);
                return;
            }
            ExcelCommunicator.Instance.SetCellProperty(cellInfo, propertyName, propertyValue);
        }

        #region Code Generation Customization Methods - Not Implemented in this sample

                                                public override Type GetSpecializedClass(ZappyTaskControl ZappyTaskControl)
        {
            string controlType = ZappyTaskControl.SearchProperties[PropertyNames.ControlType];
            if (ControlType.Cell.NameEquals(controlType))
            {
                return typeof(ExcelCell);
            }
            else if (ControlType.Table.NameEquals(controlType))
            {
                return typeof(ExcelTable);
            }

            return typeof(ZappyTaskControl);
        }

                                                public override string[] GetPredefinedSearchProperties(Type specializedClassType)
        {
            return new string[] { PropertyNames.ControlType };
        }

                                                                                                public override string GetPropertyForAction(ZappyTaskControl ZappyTaskControl, ZappyTaskAction action)
        {
            return null;
        }

                                                                                                        internal override string[] GetPropertyForControlState(ZappyTaskControl ZappyTaskControl, ControlStates uiState, out bool[] stateValues)
        {
            stateValues = null;
            return null;
        }

                                                public override Type GetPropertyNamesClassType(ZappyTaskControl ZappyTaskControl)
        {
            string controlType = ZappyTaskControl.SearchProperties[PropertyNames.ControlType];
            if (ControlType.Cell.NameEquals(controlType))
            {
                return typeof(ExcelCell.PropertyNames);
            }
            else if (ControlType.Table.NameEquals(controlType))
            {
                return typeof(ExcelTable.PropertyNames);
            }
            return typeof(ZappyTaskControl.PropertyNames);
        }

        #endregion

        #region Private Members

                                        private static Dictionary<string, ZappyTaskPropertyDescriptor> InitializePropertiesMap()
        {
            Dictionary<string, ZappyTaskPropertyDescriptor> map = new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase);
            ZappyTaskPropertyAttributes read = ZappyTaskPropertyAttributes.Readable | ZappyTaskPropertyAttributes.DoNotGenerateProperties;
            ZappyTaskPropertyAttributes readWrite = read | ZappyTaskPropertyAttributes.Writable;
            ZappyTaskPropertyAttributes readSearch = read | ZappyTaskPropertyAttributes.Searchable;

            map.Add(PropertyNames.WorksheetName, new ZappyTaskPropertyDescriptor(typeof(string), read));
            map.Add(PropertyNames.RowIndex, new ZappyTaskPropertyDescriptor(typeof(int), readSearch));
            map.Add(PropertyNames.ColumnIndex, new ZappyTaskPropertyDescriptor(typeof(int), readSearch));
            map.Add(PropertyNames.Value, new ZappyTaskPropertyDescriptor(typeof(string), readWrite));
            map.Add(PropertyNames.Text, new ZappyTaskPropertyDescriptor(typeof(string), read));
            map.Add(PropertyNames.WidthInChars, new ZappyTaskPropertyDescriptor(typeof(double), readWrite));
            map.Add(PropertyNames.HeightInPoints, new ZappyTaskPropertyDescriptor(typeof(double), readWrite));
            map.Add(PropertyNames.Formula, new ZappyTaskPropertyDescriptor(typeof(string), readWrite));
            map.Add(PropertyNames.WrapText, new ZappyTaskPropertyDescriptor(typeof(bool), readWrite));

            return map;
        }

                                                public static ExcelCellInfo GetCellInfo(ZappyTaskControl ZappyTaskControl)
        {
                                    ExcelCellElement cellElement = ZappyTaskControl.GetProperty(ZappyTaskControl.PropertyNames.UITechnologyElement) as ExcelCellElement;
                        if (cellElement != null)
            {
                return cellElement.CellInfo;
            }

            return null;
        }

        private static ExcelWorksheetInfo GetWorkSheetInfo(ZappyTaskControl uiTestControl)
        {
                        ExcelWorksheetElement workSheetElement = uiTestControl.GetProperty(ZappyTaskControl.PropertyNames.UITechnologyElement) as ExcelWorksheetElement;
            if (workSheetElement != null)
            {
                return workSheetElement.WorksheetInfo;
            }

            return null;
        }

                                private static Dictionary<string, ZappyTaskPropertyDescriptor> propertiesMap = InitializePropertiesMap();

        #endregion
    }
}
