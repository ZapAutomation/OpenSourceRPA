using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfScrollViewerPropertyProvier : WpfSrollBarPropertyProvider
    {
        public WpfScrollViewerPropertyProvier()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override int GetControlSupportLevel(ZappyTaskControl control)
        {
            string a = null;
            if (control.SearchProperties.Contains(ZappyTaskControl.PropertyNames.ClassName))
            {
                a = control.SearchProperties[ZappyTaskControl.PropertyNames.ClassName];
            }
            else if (control.TechnologyElement != null)
            {
                a = control.TechnologyElement.ClassName;
            }
            if (string.Equals(a, "Uia.ScrollViewer", StringComparison.OrdinalIgnoreCase))
            {
                return base.GetControlSupportLevel(control);
            }
            return 0;
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfPane.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfPane.PropertyNames.VerticalScrollBar, StringComparison.OrdinalIgnoreCase))
            {
                return GetScrollBar(UIControl, OrientationType.Vertical);
            }
            if (string.Equals(propertyName, WpfPane.PropertyNames.HorizontalScrollBar, StringComparison.OrdinalIgnoreCase))
            {
                return GetScrollBar(UIControl, OrientationType.Horizontal);
            }
            return null;
        }

        internal static object GetScrollBar(ZappyTaskControl UIControl, OrientationType orientationType)
        {
            foreach (ZappyTaskControl control in ALUtility.GetDescendantsByControlType(UIControl, UIControl.TechnologyName, ControlType.ScrollBar, -1))
            {
                AutomationElement nativeElement = control.TechnologyElement.NativeElement as AutomationElement;
                if (IsScrollBarOfGivenQrientation(nativeElement, orientationType) && IsScrollBarVisibleToUser(nativeElement))
                {
                    return control;
                }
            }
            return null;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfPane);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.Position, new ZappyTaskPropertyDescriptor(typeof(double), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.MaximumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.MinimumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.Orientation, new ZappyTaskPropertyDescriptor(typeof(OrientationType)));
            return m_ControlSpecificProperties;
        }

        internal static bool IsScrollBarOfGivenQrientation(AutomationElement element, OrientationType orientation) =>
            (((element != null) && (element.Current.ControlType == System.Windows.Automation.ControlType.ScrollBar)) && (element.Current.Orientation == orientation));

        internal static bool IsScrollBarVisibleToUser(AutomationElement element)
        {
            if (((element != null) && element.Current.IsOffscreen) && (element.Current.BoundingRectangle.Width == 0))
            {
                return (element.Current.BoundingRectangle.Height > 0);
            }
            return true;
        }

        public override void SetPropertyValue(ZappyTaskControl UIControl, string propertyName, object value)
        {
            ZappyTaskElementKind horizontalThumb = ZappyTaskElementKind.HorizontalThumb;
            if (((OrientationType)this.GetPropertyValue(UIControl, WpfScrollBar.PropertyNames.Orientation)) == OrientationType.Vertical)
            {
                horizontalThumb = ZappyTaskElementKind.VerticalThumb;
            }
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                double num = ZappyTaskUtilities.ConvertToType<double>(value);
                UIControl.ScreenElement.SetValueAsScrollBar(num.ToString(CultureInfo.InvariantCulture), horizontalThumb);
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                double num2;
                if (!ALUtility.ConvertStringToDouble(ZappyTaskUtilities.ConvertToType<string>(value), out num2))
                {
                    object[] args = new object[] { value, propertyName, UIControl.ControlType };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, args));
                }
                UIControl.ScreenElement.SetValueAsScrollBar(num2.ToString(CultureInfo.CurrentCulture), horizontalThumb);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.Pane;
    }
}

