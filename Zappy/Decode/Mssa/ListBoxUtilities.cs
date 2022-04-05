using System.Windows.Forms;

namespace Zappy.Decode.Mssa
{
    internal static class ListBoxUtilities
    {
        private const int MaxChildDepth = 0x7d0;

        private static bool GetNameIfSelected(AccWrapper accWrapper, ref string name)
        {
            if (accWrapper.RoleInt == AccessibleRole.ListItem && (accWrapper.State & AccessibleStates.Selected) != AccessibleStates.None)
            {
                name = accWrapper.Name;
                return true;
            }
            return false;
        }

        private static bool IsSupportedEvent(AccessibleEvents accEvent)
        {
            if (accEvent != AccessibleEvents.SystemCaptureEnd && accEvent != AccessibleEvents.Selection && accEvent != AccessibleEvents.SelectionAdd && accEvent != AccessibleEvents.SelectionRemove)
            {
                return accEvent == AccessibleEvents.SelectionWithin;
            }
            return true;
        }

        private static object TryGetCommaSeparatedValues(MsaaElement elementForEvent, out bool result)
        {
            CommaListBuilder builder = new CommaListBuilder();
            int childCount = elementForEvent.AccessibleWrapper.ChildCount;
            result = false;
            if (childCount > 0x7d0)
            {
                return null;
            }
            AccWrapper wrapper = elementForEvent.AccessibleWrapper.Navigate(AccessibleNavigation.FirstChild);
            if (wrapper == null || wrapper.AccessibleObject == null)
            {
                return null;
            }
            string name = null;
            bool flag = false;
            for (int i = 1; i <= childCount; i++)
            {
                if (GetNameIfSelected(new AccWrapper(wrapper.AccessibleObject, i), ref name))
                {
                    flag = true;
                    builder.AddValue(name);
                }
            }
            result = true;
            if (!flag)
            {
                return null;
            }
            return builder.ToString();
        }

        internal static bool TryGetValue(AccessibleEvents accEvent, MsaaElement elementForEvent, ref bool isLastEventGood, object eventArg, out object value)
        {
            value = null;
            if (IsSupportedEvent(accEvent))
            {
                bool flag;
                value = TryGetCommaSeparatedValues(elementForEvent, out flag);
                if (!flag)
                {
                    return false;
                }
                if (accEvent != AccessibleEvents.SystemCaptureEnd)
                {
                    isLastEventGood = true;
                    return true;
                }
                if (isLastEventGood || eventArg != null && !Equals(eventArg, value))
                {
                    isLastEventGood = false;
                    return true;
                }
            }
            return false;
        }
    }
}