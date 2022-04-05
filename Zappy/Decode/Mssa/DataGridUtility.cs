using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Decode.Mssa
{
    internal static class DataGridUtility
    {
        internal static MsaaElement cachedDatagridElement;
        internal const string CreateNewRowValue = "(Create New)";
        private const int PixelOffset = 5;
        internal const string TopRowValue = "Top Row";
        internal const string WinformsDatagridTableName = "DataGridView";
        internal const string WinformsEditableControlName = "Editing Control";
        internal const string WinformsEditablePanelName = "Editing Panel";

        internal static void GetDataGridCellFromElement(MsaaZappyPlugin plugin, ref MsaaElement elementForEvent, bool useCache)
        {
            if (useCache && plugin.WinEvent.ValueChangeDataGridCell != null)
            {
                elementForEvent = plugin.WinEvent.ValueChangeDataGridCell;
            }
            else if (elementForEvent != null)
            {
                Rectangle boundingRectangle = elementForEvent.AccessibleWrapper.GetBoundingRectangle();
                if (!MsaaUtility.InvalidRectangle.Contains(boundingRectangle))
                {
                    plugin.WinEvent.ValueChangeDataGridCell = GetElementFromGridSibling(boundingRectangle.Left, boundingRectangle.Top + boundingRectangle.Height / 2, AccessibleNavigation.Next, AccessibleRole.Cell, boundingRectangle, plugin);
                    if (plugin.WinEvent.ValueChangeDataGridCell == null)
                    {
                        plugin.WinEvent.ValueChangeDataGridCell = GetElementFromGridSibling(boundingRectangle.Left + boundingRectangle.Width, boundingRectangle.Top + boundingRectangle.Height / 2, AccessibleNavigation.Previous, AccessibleRole.Cell, boundingRectangle, plugin);
                    }
                    MsaaElement element = null;
                    if (plugin.WinEvent.ValueChangeDataGridCell == null)
                    {
                        element = GetElementFromGridSibling(boundingRectangle.Left + boundingRectangle.Width / 2, boundingRectangle.Top, AccessibleNavigation.Next, AccessibleRole.Row, boundingRectangle, plugin);
                        if (element == null)
                        {
                            element = GetElementFromGridSibling(boundingRectangle.Left + boundingRectangle.Width / 2, boundingRectangle.Top + boundingRectangle.Height, AccessibleNavigation.Previous, AccessibleRole.Row, boundingRectangle, plugin);
                        }
                        if (plugin.WinEvent.LastParentChangeParentAccWrapper != null && plugin.WinEvent.LastParentChangeParentAccWrapper.RoleInt == AccessibleRole.Table)
                        {
                            if (element == null)
                            {
                                element = SearchOverlappingGridElement(new MsaaElement(plugin.WinEvent.LastParentChangeParentAccWrapper), boundingRectangle.Top + boundingRectangle.Height / 2);
                            }
                            if (element != null)
                            {
                                plugin.WinEvent.ValueChangeDataGridCell = SearchOverlappingGridElement(element, boundingRectangle.Left + boundingRectangle.Width / 2);
                            }
                        }
                    }
                    if (plugin.WinEvent.ValueChangeDataGridCell != null)
                    {
                        elementForEvent = plugin.WinEvent.ValueChangeDataGridCell;
                    }
                }
            }
        }

        private static MsaaElement GetElementFromGridSibling(int x, int y, AccessibleNavigation navDir, AccessibleRole role, Rectangle rect, MsaaZappyPlugin plugin)
        {
            MsaaElement element;
            do
            {
                if (role == AccessibleRole.Cell)
                {
                    x = navDir == AccessibleNavigation.Next ? x - 5 : x + 5;
                }
                else
                {
                    y = navDir == AccessibleNavigation.Next ? y - 5 : y + 5;
                }
            }
            while (GetElementFromGridSiblingInternal(x, y, navDir, role, rect, plugin, out element) && element == null);
            return element;
        }

        private static bool GetElementFromGridSiblingInternal(int x, int y, AccessibleNavigation navDir, AccessibleRole role, Rectangle rect, MsaaZappyPlugin plugin, out MsaaElement gridElement)
        {
            MsaaElement parent;
            AccWrapper accessibleWrapper = null;
            gridElement = null;
            if ((parent = plugin.GetElementFromPoint(x, y) as MsaaElement) != null && parent.RoleInt == AccessibleRole.Cell)
            {
                if (role == AccessibleRole.Row)
                {
                    parent = parent.Parent;
                }
                if (parent != null && parent.RoleInt == role)
                {
                    Rectangle boundingRectangle = parent.AccessibleWrapper.GetBoundingRectangle();
                    if (!MsaaUtility.InvalidRectangle.Contains(boundingRectangle))
                    {
                        if (!boundingRectangle.Contains(rect))
                        {
                            accessibleWrapper = parent.AccessibleWrapper.Navigate(navDir);
                            gridElement = accessibleWrapper != null ? new MsaaElement(accessibleWrapper) : null;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsPointInsideGrid(Rectangle rect, AccessibleRole role, int elementPoint)
        {
            int num = role == AccessibleRole.Table ? rect.Top : rect.Left;
            int num2 = role == AccessibleRole.Table ? rect.Height : rect.Width;
            return num < elementPoint && num + num2 > elementPoint;
        }

        private static MsaaElement SearchOverlappingGridElement(MsaaElement element, int visiblePoint)
        {
            if (element.RoleInt == AccessibleRole.Row || element.RoleInt == AccessibleRole.Table)
            {
                AccWrapper wrapper;
                Rectangle invalidRectangle = MsaaUtility.InvalidRectangle;
                AccChildrenEnumerator enumerator = new AccChildrenEnumerator(element.AccessibleWrapper);
                while ((wrapper = enumerator.GetNextChild(true)) != null)
                {
                    if (element.RoleInt == AccessibleRole.Table && wrapper.RoleInt == AccessibleRole.Row || element.RoleInt == AccessibleRole.Row && wrapper.RoleInt == AccessibleRole.Cell)
                    {
                        break;
                    }
                }
                if (wrapper == null)
                {
                    return null;
                }
                int num = -1;
                int num2 = -1;
                invalidRectangle = wrapper.GetBoundingRectangle();
                if (MsaaUtility.InvalidRectangle.Contains(invalidRectangle))
                {
                    return null;
                }
                num = element.RoleInt == AccessibleRole.Table ? invalidRectangle.Top : invalidRectangle.Left;
                num2 = element.RoleInt == AccessibleRole.Table ? invalidRectangle.Height : invalidRectangle.Width;
                int num3 = (visiblePoint - num) / num2;
                wrapper = null;
                invalidRectangle = MsaaUtility.InvalidRectangle;
                if (num3 != 0)
                {
                    while ((wrapper = enumerator.GetNextChild(true)) != null)
                    {
                        if (--num3 == 0)
                        {
                            invalidRectangle = wrapper.GetBoundingRectangle();
                            break;
                        }
                    }
                }
                else
                {
                    invalidRectangle = wrapper.GetBoundingRectangle();
                }
                if (!MsaaUtility.InvalidRectangle.Contains(invalidRectangle))
                {
                    if (IsPointInsideGrid(invalidRectangle, element.RoleInt, visiblePoint))
                    {
                        return new MsaaElement(wrapper);
                    }
                    int num4 = element.RoleInt == AccessibleRole.Table ? invalidRectangle.Top : invalidRectangle.Left;
                    int num5 = element.RoleInt == AccessibleRole.Table ? invalidRectangle.Height : invalidRectangle.Width;
                    AccessibleNavigation navDir = num4 > visiblePoint ? AccessibleNavigation.Previous : AccessibleNavigation.Next;
                    while ((wrapper = wrapper.Navigate(navDir)) != null)
                    {
                        invalidRectangle = wrapper.GetBoundingRectangle();
                        if (!MsaaUtility.InvalidRectangle.Contains(invalidRectangle) && IsPointInsideGrid(invalidRectangle, element.RoleInt, visiblePoint))
                        {
                            return new MsaaElement(wrapper);
                        }
                    }
                }
            }
            return null;
        }

        internal static void SetSwitchingElement(MsaaElement element)
        {
            if (element != null && element.EqualsIgnoreContainer(cachedDatagridElement) && element.SwitchingElement == null && cachedDatagridElement.SwitchingElement != null)
            {
                element.SwitchingElement = cachedDatagridElement.SwitchingElement;
            }
        }
    }
}