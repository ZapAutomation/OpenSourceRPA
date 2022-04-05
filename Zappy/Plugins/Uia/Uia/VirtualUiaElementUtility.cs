using System;
using System.Drawing;
using System.Windows.Automation;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia
{

    internal static class VirtualUiaElementUtility
    {
        public static bool ElementAreEqual(ITaskActivityElement firstElement, ITaskActivityElement secondElement)
        {
            try
            {
                if (secondElement is UiaElement && firstElement is UiaElement && firstElement.Name != null && firstElement.Name.Equals(secondElement.Name) && ControlType.NameComparer.Equals(firstElement.ControlTypeName, secondElement.ControlTypeName))
                {
                    Rectangle automationPropertyValue = UiaUtility.GetAutomationPropertyValue<Rectangle>((firstElement as UiaElement).InnerElement, AutomationElement.BoundingRectangleProperty);
                    Rectangle rectangle2 = UiaUtility.GetAutomationPropertyValue<Rectangle>(secondElement.NativeElement as AutomationElement, AutomationElement.BoundingRectangleProperty);
                    return automationPropertyValue.Equals(rectangle2);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            return false;
        }

        internal static bool NeedVirtualizedChildren(UiaElement uiaElement)
        {
            if (uiaElement != null && !ControlType.Menu.NameEquals(uiaElement.ControlTypeName))
            {
                try
                {
                    if (PatternHelper.GetVirtualizedItemPattern(uiaElement.InnerElement) is VirtualizedItemPattern)
                    {
                        UiaUtility.RealizeElement(uiaElement.InnerElement);
                        ScrollItemPattern scrollItemPattern = PatternHelper.GetScrollItemPattern(uiaElement.InnerElement, true);
                        if (scrollItemPattern != null)
                        {
                            scrollItemPattern.ScrollIntoView();
                        }
                    }
                }
                catch (ElementNotAvailableException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                if (PatternHelper.GetItemContainerPattern(uiaElement.InnerElement) != null)
                {
                    if (uiaElement.ControlTypeName == ControlType.Row)
                    {
                        try
                        {
                            ScrollItemPattern pattern3 = PatternHelper.GetScrollItemPattern(uiaElement.InnerElement, true);
                            if (pattern3 != null)
                            {
                                pattern3.ScrollIntoView();
                            }
                        }
                        catch (ElementNotAvailableException)
                        {
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

