using System;
using System.Windows.Automation;

namespace Zappy.Plugins.Uia.Uia.XAMLControls
{
    [Serializable]
#if COMENABLED
    [ComVisible(true), Guid("51806001-0BF1-4209-A386-CE0052C4726A")]
#endif
    public class XamlToggleSwitch : XamlElement
    {
        public XamlToggleSwitch(AutomationElement automationElement) : base(automationElement)
        {
        }

        internal XamlToggleSwitch(AutomationElement automationElement, string controlTypeName) : base(automationElement, controlTypeName)
        {
        }

        public override object GetPropertyValue(string propertyName)
        {
            try
            {
                propertyName = PropertyNames.GetPropertyNameInCorrectCase(propertyName);
                if (propertyName == "DisplayText")
                {
                    return GetToggleSwitchDisplayName();
                }
                return base.GetPropertyValue(propertyName);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        internal virtual string GetToggleSwitchDisplayName()
        {
            AutomationElement firstChild = TreeWalker.RawViewWalker.GetFirstChild(InnerElement);
            return firstChild != null ? firstChild.Current.Name : string.Empty;
        }
    }
}

