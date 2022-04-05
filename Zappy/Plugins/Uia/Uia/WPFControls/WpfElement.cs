using System;
using System.Drawing;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Plugins.Uia.Uia.Utilities;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia.WPFControls
{
    [Serializable]
#if COMENABLED
    [ComVisible(true), Guid("0907D719-BB8F-442D-AA38-FED1072982EA")]
#endif
    public class WpfElement : UiaElement
    {
        public WpfElement(AutomationElement automationElement) : base(automationElement)
        {
        }

        public override object GetOption(UITechnologyElementOption technologyElementOption)
        {
            object option;
            try
            {
                option = base.GetOption(technologyElementOption);
            }
            catch (NotSupportedException)
            {
                if (technologyElementOption == UITechnologyElementOption.GetClickableRectangle && ControlType.TreeItem.NameEquals(ControlTypeName))
                {
                    VirtualizationContext disable = VirtualizationContext.Disable;
                    AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(InnerElement, TreeWalker.RawViewWalker);
                    for (int i = 0; i < 5 && firstChild != null; i++)
                    {
                        if (System.Windows.Automation.ControlType.Text == firstChild.Current.ControlType)
                        {
                            Rectangle automationPropertyValue = UiaUtility.GetAutomationPropertyValue<Rectangle>(firstChild, AutomationElement.BoundingRectangleProperty);
                            return new[] { automationPropertyValue.Left, automationPropertyValue.Top, automationPropertyValue.Width, automationPropertyValue.Height };
                        }
                        firstChild = TreeWalkerHelper.GetNextSibling(firstChild, TreeWalker.RawViewWalker, ref disable);
                    }
                }
                                return null;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return option;
        }

        public override object GetPropertyValue(string propertyName)
        {
            if (string.Equals(propertyName, "SelectedItem", StringComparison.OrdinalIgnoreCase))
            {
                object propertyValue = base.GetPropertyValue(propertyName);
                AutomationElement editElement = null;
                if (propertyValue == null && UiaUtility.IsEditableComboBox(this, ref editElement) && !string.IsNullOrEmpty(Value))
                {
                    return Value;
                }
            }
            return base.GetPropertyValue(propertyName);
        }

        protected override string SanitizeUIAClassName(string uiaClassName) =>
            "Uia." + uiaClassName;

        public override void SetFocus()
        {
            try
            {
                AutomationElement editElement = null;
                if (UiaUtility.IsEditableComboBox(this, ref editElement))
                {
                    UiaTechnologyManager.Instance.LastFocusWasOnComboBox = true;
                    UiaTechnologyManager.Instance.LastFocussedComboBox = InnerElement;
                    editElement.SetFocus();
                }
                else
                {
                    if (ControlTypeName == ControlType.Cell)
                    {
                        AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(InnerElement);
                        if (DataGridUtility.IsElementNotTemplateContentOfCell(firstChild))
                        {
                            AutomationElement focusedElement = AutomationElement.FocusedElement;
                            if (focusedElement == null || !Automation.Compare(focusedElement, firstChild))
                            {
                                firstChild.SetFocus();
                            }
                            return;
                        }
                    }
                    UiaTechnologyManager.Instance.LastFocusWasOnComboBox = false;
                    InnerElement.SetFocus();
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        protected override bool LazyInitializeClassName
        {
            get
            {
                System.Windows.Automation.ControlType automationPropertyValue = UiaUtility.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(InnerElement, AutomationElement.ControlTypeProperty);
                if (automationPropertyValue != System.Windows.Automation.ControlType.DataItem && automationPropertyValue != System.Windows.Automation.ControlType.Custom)
                {
                    return false;
                }
                return base.LazyInitializeClassName;
            }
        }

        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                try
                {
                    if (ControlType.ScrollBar.NameEquals(ControlTypeName))
                    {
                        RangeValuePattern rangeValuePattern = PatternHelper.GetRangeValuePattern(InnerElement);
                        LegacyIAccessiblePattern legacyIAccessiblePattern = PatternHelper.GetLegacyIAccessiblePattern(InnerElement);
                        if (legacyIAccessiblePattern != null && rangeValuePattern != null)
                        {
                            double num = ZappyTaskUtilities.ConvertToType<double>(value);
                            double minimum = rangeValuePattern.Current.Minimum;
                            double a = 100.0 * (num - minimum) / (rangeValuePattern.Current.Maximum - minimum);
                            
                            legacyIAccessiblePattern.SetValue(Math.Round(a).ToString());
                            return;
                        }
                    }
                }
                catch (Exception exception)
                {
                    UiaUtility.MapAndThrowException(exception, this, false);
                    throw;
                }
                base.Value = value;
            }
        }
    }
}

