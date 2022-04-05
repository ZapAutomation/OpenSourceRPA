using System;
using System.Collections.Generic;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.UiaPropertyProviders;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension
{
    internal class UiaPropertyProvider : ZappyTaskPropertyProvider
    {
        private List<IUiaControlsPropertyProvider> defaultProviders = new List<IUiaControlsPropertyProvider>();
        private ZappyTaskControl lastControl;
        private IUiaControlsPropertyProvider lastProvider;
        private Dictionary<string, List<IUiaControlsPropertyProvider>> providersDictionary = new Dictionary<string, List<IUiaControlsPropertyProvider>>();
        private static ZappyTaskPropertyDescriptor s_automationIdPropertyDescriptorForPhoneShellControls = new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.DoNotGenerateProperties | ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable);

        public UiaPropertyProvider()
        {
            List<IUiaControlsPropertyProvider> list = new List<IUiaControlsPropertyProvider> {
                new WpfButtonPropertyProvider(),
                new WpfCalendarPropertyProvider(),
                new WpfCellPropertyProvider(),
                new WpfCheckBoxPropertyProvider(),
                new WpfComboBoxPropertyProvider(),
                new WpfControlPropertyProvider(),
                new WpfCustomPropertyProvider(),
                new WpfDatePickerPropertyProvider(),
                new WpfEditPropertyProvider(),
                new WpfExpanderPropertyProvider(),
                new WpfGroupPropertyProvider(),
                new WpfHyperlinkPropertyProvider(),
                new WpfImagePropertyProvider(),
                new WPFListPropertyProvider(),
                new WpfListItemPropertyProvider(),
                new WpfMenuPropertyProvider(),
                new WpfMenuItemPropertyProvider(),
                new WpfPanePropertyProvider(),
                new WpfHostingPanePropertyProvider(),
                new WpfProgressBarPropertyProvider(),
                new WpfRadioButtonPropertyProvider(),
                new WpfRowPropertyProvider(),
                new WpfScrollViewerPropertyProvier(),
                new WpfSrollBarPropertyProvider(),
                new WpfSeperatorPropertyProvider(),
                new WpfSliderPropertyProvider(),
                new WpfStatusBarPropertyProvider(),
                new WpfTablePropertyProvider(),
                new WpfTabListPropertyProvider(),
                new WpfTabPagePropertyProvider(),
                new WpfTextPropertyProvider(),
                new WpfTitleBarPropertyProvider(),
                new WpfToggleButtonPropertyProvider(),
                new WpfToolBarPropertyProvider(),
                new WpfToolTipPropertyProvider(),
                new WpfTreePropertyProvider(),
                new WpfTreeItemPropertyProvider(),
                new WpfWindowPropertyProvider()
            };
            foreach (IUiaControlsPropertyProvider provider in list)
            {
                ControlType supportedControlType = provider.SupportedControlType;
                if (supportedControlType != null)
                {
                    if (this.providersDictionary.ContainsKey(supportedControlType.Name))
                    {
                        this.providersDictionary[supportedControlType.Name].Add(provider);
                    }
                    else
                    {
                        List<IUiaControlsPropertyProvider> list2 = new List<IUiaControlsPropertyProvider> {
                            provider
                        };
                        this.providersDictionary.Add(supportedControlType.Name, list2);
                    }
                }
                else
                {
                    this.defaultProviders.Add(provider);
                }
            }
        }

        private IUiaControlsPropertyProvider GetBestControlProvider(ZappyTaskControl control)
        {
            if (control != null)
            {
                string name;
                if (control.SearchProperties.Contains(ZappyTaskControl.PropertyNames.ControlType))
                {
                    name = control.SearchProperties[ZappyTaskControl.PropertyNames.ControlType];
                }
                else
                {
                    name = control.ControlType.Name;
                }
                if (this.providersDictionary.ContainsKey(name))
                {
                    IUiaControlsPropertyProvider provider = null;
                    int controlSupportLevel = 0;
                    int num2 = 0;
                    foreach (IUiaControlsPropertyProvider provider2 in this.providersDictionary[name])
                    {
                        controlSupportLevel = provider2.GetControlSupportLevel(control);
                        if (controlSupportLevel > num2)
                        {
                            num2 = controlSupportLevel;
                            provider = provider2;
                        }
                    }
                    if (provider != null)
                    {
                        this.lastControl = control;
                        this.lastProvider = provider;
                        return provider;
                    }
                }
            }
            return this.GetBestDefaultProvider(control);
        }

        private IUiaControlsPropertyProvider GetBestDefaultProvider(ZappyTaskControl control)
        {
            IUiaControlsPropertyProvider provider = null;
            int controlSupportLevel = 0;
            int num2 = -1;
            foreach (IUiaControlsPropertyProvider provider2 in this.defaultProviders)
            {
                controlSupportLevel = provider2.GetControlSupportLevel(control);
                if (controlSupportLevel > num2)
                {
                    num2 = controlSupportLevel;
                    provider = provider2;
                }
            }
            if (provider != null)
            {
                this.lastControl = control;
                this.lastProvider = provider;
            }
            return provider;
        }

        public override int GetControlSupportLevel(ZappyTaskControl uiTaskControl)
        {
            int num = 0;
            if (string.Equals(uiTaskControl.TechnologyName, "UIA", StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }
            if (string.Equals(ZappyTaskService.Instance.GetCoreTechnologyName(uiTaskControl.TechnologyName), "UIA", StringComparison.OrdinalIgnoreCase))
            {
                num = 1;
            }
            return num;
        }

        public override string[] GetPredefinedSearchProperties(Type specializedClass)
        {
            if (specializedClass != typeof(WpfControl))
            {
                return new string[] { ZappyTaskControl.PropertyNames.ControlType, WpfControl.PropertyNames.FrameworkId };
            }
            return new string[] { WpfControl.PropertyNames.FrameworkId };
        }

        public override ZappyTaskPropertyDescriptor GetPropertyDescriptor(ZappyTaskControl uiTaskControl, string propertyName)
        {
            Dictionary<string, ZappyTaskPropertyDescriptor> propertiesMap = this.GetBestControlProvider(uiTaskControl).GetPropertiesMap();
            if (propertiesMap.ContainsKey(propertyName))
            {
                return propertiesMap[propertyName];
            }
            return null;
        }

        public override string GetPropertyForAction(ZappyTaskControl uiTaskControl, ZappyTaskAction action) =>
            this.GetBestControlProvider(uiTaskControl).GetPropertyForAction(action);

        internal override string[] GetPropertyForControlState(ZappyTaskControl uiTaskControl, ControlStates uiState, out bool[] stateValues)
        {
            List<bool> list = new List<bool>();
            List<string> propertyForControlState = this.GetBestControlProvider(uiTaskControl).GetPropertyForControlState(uiState, ref list);
            stateValues = list.ToArray();
            return propertyForControlState.ToArray();
        }

        public override ICollection<string> GetPropertyNames(ZappyTaskControl uiTaskControl) =>
            this.GetBestControlProvider(uiTaskControl).GetPropertiesMap().Keys;

        public override Type GetPropertyNamesClassType(ZappyTaskControl uiTaskControl) =>
            this.GetBestControlProvider(uiTaskControl).GetPropertyClassName();

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object obj2;
            if (TryGetPropertyFromTechnologyElement(uiTaskControl, propertyName, out obj2))
            {
                return obj2;
            }
            IUiaControlsPropertyProvider bestControlProvider = this.GetBestControlProvider(uiTaskControl);
            if (bestControlProvider.IsCommonReadableProperty(propertyName))
            {
                return bestControlProvider.GetPropertyValue(uiTaskControl, propertyName);
            }
            ALUtility.ThrowNotSupportedException(true);
            return null;
        }

        public override Type GetSpecializedClass(ZappyTaskControl uiTaskControl) =>
            this.GetBestControlProvider(uiTaskControl).GetSpecializedClass();

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            IUiaControlsPropertyProvider bestControlProvider = this.GetBestControlProvider(uiTaskControl);
            if (bestControlProvider.IsCommonWritableProperty(propertyName))
            {
                bestControlProvider.SetPropertyValue(uiTaskControl, propertyName, value);
            }
            else
            {
                ALUtility.ThrowNotSupportedException(true);
            }
        }
    }
}