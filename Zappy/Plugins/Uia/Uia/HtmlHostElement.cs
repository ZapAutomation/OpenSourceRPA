using System.Windows.Automation;

namespace Zappy.Plugins.Uia.Uia
{
#if COMENABLED
    [ComVisible(true), Guid("51806001-0BF1-4209-A386-CE0052C4726A")]
#endif
    public class HtmlHostElement : UiaElement
    {
        public HtmlHostElement(AutomationElement automationElement) : base(automationElement)
        {
        }

        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            base.GetBoundingRectangle(out left, out top, out width, out height);
            if (left == -1 && top == -1 && width == -1 && height == -1 && Parent != null)
            {
                Parent.GetBoundingRectangle(out left, out top, out width, out height);
            }
        }
    }
}

