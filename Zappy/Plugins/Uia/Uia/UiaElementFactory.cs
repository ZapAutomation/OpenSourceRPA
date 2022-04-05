using System;
using System.Windows.Automation;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Plugins.Uia.Uia.WPFControls;
using Zappy.Plugins.Uia.Uia.XAMLControls;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class UiaElementFactory
    {
        private static XamlElement CreateXamlElement(AutomationElement element)
        {
            if (string.Equals(element.Current.ClassName, ControlType.ToggleSwitch.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new XamlToggleSwitch(element);
            }
            return new XamlElement(element);
        }

        internal static UiaElement GetUiaElement(AutomationElement automationElement, bool throwOnError)
        {
            UiaElement element = null;
            if (automationElement != null)
            {
                try
                {
                    if (UiaUtility.IsJupiterFrameWorkId(automationElement))
                    {
                        element = CreateXamlElement(automationElement);
                    }
                    else if (UiaUtility.IsWpfFrameWorkId(automationElement))
                    {
                        element = new WpfElement(automationElement);
                    }
                    else if (string.Equals(automationElement.Current.ClassName, "Internet Explorer_Server", StringComparison.OrdinalIgnoreCase))
                    {
                        element = new HtmlHostElement(automationElement);
                    }
                    else
                    {
                        element = new UiaElement(automationElement);
                    }
                    element.InitializeTechnologyElementOptions();
                }
                catch (Exception exception)
                {
                    CrapyLogger.log.Error(exception);
                    if (throwOnError)
                    {
                        UiaUtility.MapAndThrowException(exception, null, false);
                        throw;
                    }
                }
                return element;
            }
            if (throwOnError)
            {
                throw new ZappyTaskControlNotAvailableException();
            }
            return element;
        }

        internal static UiaElement GetUiaElement(AutomationElement automationElement, bool throwOnError, AutomationElement ceilingElement)
        {
            UiaElement uiaElement = GetUiaElement(automationElement, throwOnError);
            if (uiaElement != null)
            {
                uiaElement.CeilingElement = ceilingElement;
            }
            return uiaElement;
        }
    }
}

