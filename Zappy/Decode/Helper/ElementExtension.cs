using System.Drawing;
using System.Windows.Automation;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.LogManager;
using Zappy.Decode.Screenshot;

namespace Zappy.Decode.Helper
{
  
    public static class ElementExtension
    {
        public static Rectangle GetBoundingRectangle(TaskActivityElement element)
        {
            int left = 0;
            int top = 0;
            int width = 0;
            int height = 0;
            element.GetBoundingRectangle(out left, out top, out width, out height);
            return new Rectangle(left, top, width, height);
        }

        public static Rectangle GetBoundingRectangleForScreenShot(TaskActivityElement element)
        {
                        if (ImageCaptureUtility.ScalingFactor > 1 && element.AutomationElementForScreenshot != null)
            {
                return (Rectangle)element.AutomationElementForScreenshot.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            }
            return GetBoundingRectangle(element);
        }
    }



}
