using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.ExecuteTask.Execute
{
    internal static class ExecuteTaskUtility
    {
        public const string AddMethodName = "Add";
        public const string AddRangeMethodName = "AddRange";
        public const string ApplicationUnderTest = "ApplicationUnderTest";
        public const string AssertClassName = "Assert";
        public const string BrowserBackMethodName = "Back";
        public const string BrowserCloseMethodName = "Close";
        public const string BrowserDialogAction = "BrowserDialogAction";
        public const string BrowserForwardMethodName = "Forward";
        public const string BrowserPerformDialogAction = "PerformDialogAction";
        public const string BrowserRefreshMethodName = "Refresh";
        public const string BrowserStopMethodName = "StopPageLoad";
        public const string BrowserWindow = "BrowserWindow";
        public const string ClickMethodName = "Click";
        public const string ContainerArgumentName = "searchLimitContainer";
        public const string ContainerPropertyName = "Container";
        public const string ContinueOnErrorPropertyName = "ContinueOnError";
        public const string ConvertMethodName = "Convert";
        public const string CopyFromMethodName = "CopyFrom";
        public const string DataRowPropertyName = "DataRow";
        private const string DisambiguateChildName = "DisambiguateChild";
        public const string DoubleClickMethodName = "DoubleClick";
        public const string EditableItemPropertyName = "EditableItem";
        public const string EncryptMethodName = "EncryptText";
        public const string EnsureClickableMethodName = "EnsureClickable";
        private const string ExpandWhileSearchingName = "ExpandWhileSearching";
        public const string FilterPropertyName = "FilterProperties";
        public const string FindMethodName = "Find";
        private const string GeneratedCodeAttribute = "GeneratedCode";
        public const string GetPropertyMethodName = "GetProperty";
        public const string KeyboardClassName = "Keyboard";
        public const string Launch = "Launch";
        public const string LaunchMethodName = "Launch";
        public const string ModifierKeysAlt = "ModifierKeys.Alt";
        public const string ModifierKeysControl = "ModifierKeys.Control";
        public const string ModifierKeysNone = "ModifierKeys.None";
        public const string ModifierKeysShift = "ModifierKeys.Shift";
        public const string ModifierKeysWindows = "ModifierKeys.Windows";
        public const string ModifierOrFormat = "{0}|{1}";
        public const string MouseButtonsLeft = "MouseButtons.Left";
        public const string MouseButtonsMiddle = "MouseButtons.Middle";
        public const string MouseButtonsRight = "MouseButtons.Right";
        public const string MouseButtonsXButton1 = "MouseButtons.XButton1";
        public const string MouseButtonsXButton2 = "MouseButtons.XButton2";
        public const string MouseClassName = "Mouse";
        public const string MouseHoverMethodName = "Hover";
        public const string MoveScrollWheelMethodName = "MoveScrollWheel";
        public const string MuteMethodName = "Mute";
        public const string NavigateToUrl = "NavigateToUrl";
        private const string NextSiblingName = "NextSibling";
        public const string NextToPropertyName = "NextTo";
        public const string PauseMethodName = "Pause";
        public const string PlaybackClassName = "Playback";
        public const string PlaybackSettingsPropertyName = "PlaybackSettings";
        public const string PlayMethodName = "Play";
        public const string PropertyExpressionClassName = "PropertyExpression";
        public const string RemoveMethodName = "Remove";
        public const string SearchConfigurationsPropertyName = "SearchConfigurations";
        public const string SearchPropertyName = "SearchProperties";
        public const string SeekMethodName = "Seek";
        public const string SendKeysMethodName = "SendKeys";
        public const string SendKeysRecordedMethodParamName = "SendKeys";
        public const string SetPropertyMethodName = "SetProperty";
        public const string SetValueMethodName = "SetValue";
        public const string SetVolumeMethodName = "SetVolume";
        public const string StartDraggingMethodName = "StartDragging";
        public const string StatePropertyName = "State";
        public const string StopDraggingMethodName = "StopDragging";
        public const string StringAssertClassName = "StringAssert";
        public const string TechnologyName = "TechnologyName";
        public const string TestContextPropertyName = "TestContext";
        public const string toStringMethodName = "ToString";
        public const string TypeClassName = "Type";
        public const string TypeMethodName = "GetType";
        private static Dictionary<Type, FieldInfo[]> typeToFieldNameCache = new Dictionary<Type, FieldInfo[]>();
        public const string ScreenMapLaunchUrlMethodName = "LaunchUrl";
        public const string ScreenMapLaunchUrlParamName = "url";
        public const string UnmuteMethodName = "Unmute";
        public const string ValuePropertyName = "Value";
        public const string VisibleOnlyName = "VisibleOnly";
        public const string WaitMethodName = "Wait";
        public const string WindowTitlesPropertyName = "WindowTitles";

                                                                
                                                                                
                                                        
                                        
                                                
                                        
                                                                
                                                                                                                                                                                                
                                        
                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                
                
                                                                        
                                                
                
                
                
                                        
        public static string[] GetControlStateProperty(ZappyTaskControl uiControl, ControlStates actionStates, out bool[] propertyValues)
        {
            string[] strArray = new string[0];
            propertyValues = new bool[0];
            ZappyTaskPropertyProvider propertyProvider = PropertyProviderManager.Instance.GetPropertyProvider(uiControl);
            try
            {
                strArray = propertyProvider.GetPropertyForControlState(uiControl, actionStates, out propertyValues);
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            return strArray;
        }

        public static string GetControlValueProperty(ZappyTaskControl uiControl, ZappyTaskAction action, out Type propertyDataType)
        {
            propertyDataType = typeof(string);
            ZappyTaskPropertyProvider propertyProvider = PropertyProviderManager.Instance.GetPropertyProvider(uiControl);
            string propertyForAction = propertyProvider.GetPropertyForAction(uiControl, action);
            if (!string.IsNullOrEmpty(propertyForAction))
            {
                ZappyTaskPropertyDescriptor propertyDescriptor = propertyProvider.GetPropertyDescriptor(uiControl, propertyForAction);
                if (propertyDescriptor != null && propertyDescriptor != null)
                {
                    propertyDataType = propertyDescriptor.DataType;
                }
            }
            return propertyForAction;
        }

                                                                                                                                
                                                                        
                                                                                                        
                                                                                                                                                                                                
                                                
                
                
                
                                                                        
                                                                                                                                                                                                
                                                                                                                                                                                                                                                
                
                                                                                                                                                        
                                                                        
                                                                                                                                        
                                                                                                                                                                                                                                                                                                                                                                                                                
                                                                                                                                        
        public static object GetSetValueActionDataForType(SetValueAction action, Type propertyDataType)
        {
            if (propertyDataType == typeof(string))
            {
                if (action.IsActionOnProtectedElement())
                {
                    return action.TextValue.Value;
                }
                return action.ValueAsString;
            }
            if (propertyDataType == typeof(string[]))
            {
                return CommaListBuilder.GetCommaSeparatedValues(action.ValueAsString).ToArray();
            }
            return SafeConvertToType(action.ValueAsString, propertyDataType);
        }

                                                                                        
                                                
                                                                                                                                                                                                                
                                                
                                                                                                                                                                                                                                                                                        
                                                                                
                                                                                                                
        private static object SafeConvertToType(string dataValue, Type dataType)
        {
            try
            {
                return Convert.ChangeType(dataValue, dataType, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                object[] args = { dataType, dataValue };
                CrapyLogger.log.ErrorFormat("Invalid data type {0} for value {1}", args);
                if (dataType.IsPrimitive)
                {
                    return 0;
                }
                if (dataType.IsValueType)
                {
                    return Activator.CreateInstance(dataType);
                }
                return null;
            }
        }

        public static void UpdateControlProperties(TaskActivityObject uiObject, ZappyTaskControl uiControl)
        {
            PropertyExpressionCollection expressions;
            PropertyExpressionCollection expressions2;
            PropertyExpressionCollection.GetProperties(uiObject.Condition, out expressions, out expressions2);
            uiControl.SearchProperties.AddRange(expressions);
            uiControl.FilterProperties.AddRange(expressions2);
        }
    }
}