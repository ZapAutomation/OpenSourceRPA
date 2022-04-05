using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinList : WinControl
    {
        public WinList() : this(null)
        {
        }

        public WinList(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.List.Name);
        }

        public string[] GetColumnNames()
        {
            if (!IsReportView)
            {
                throw new InvalidOperationException(Resources.NotInReportViewMode);
            }
            WinList headerListOfListView = GetHeaderListOfListView();
            return headerListOfListView?.GetChildren().GetNamesOfControls();
        }

        public string[] GetContent()
        {
            if (GetHeaderListOfListView() == null)
            {
                ZappyTaskControlCollection items = Items;
                if (items != null)
                {
                    return items.GetNamesOfControls();
                }
            }
            else
            {
                List<string> list = new List<string>();
                ZappyTaskControlCollection controls2 = Items;
                if (controls2 != null)
                {
                    foreach (WinListItem item in controls2)
                    {
                        list.AddRange(item.GetColumnValues());
                    }
                    return list.ToArray();
                }
            }
            return null;
        }

        private WinList GetHeaderListOfListView()
        {
            WinWindow parent = new WinWindow(this)
            {
                SearchProperties = { [ZappyTaskControl.PropertyNames.MaxDepth] = "1" }
            };
            WinList list = new WinList(parent)
            {
                SearchProperties = { [ZappyTaskControl.PropertyNames.MaxDepth] = "1" }
            };
            if (list.TryFind())
            {
                return list;
            }
            return null;
        }

        public virtual int[] CheckedIndices
        {
            get =>
                (int[])GetProperty(PropertyNames.CheckedIndices);
            set
            {
                SetProperty(PropertyNames.CheckedIndices, value);
            }
        }

        public virtual string[] CheckedItems
        {
            get =>
                (string[])GetProperty(PropertyNames.CheckedItems);
            set
            {
                SetProperty(PropertyNames.CheckedItems, value);
            }
        }

        public virtual ZappyTaskControlCollection Columns =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Columns);

        public virtual ZappyTaskControl HorizontalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.HorizontalScrollBar);

        public virtual bool IsCheckedList =>
            (bool)GetProperty(PropertyNames.IsCheckedList);

        public virtual bool IsIconView =>
            (bool)GetProperty(PropertyNames.IsIconView);

        public virtual bool IsListView =>
            (bool)GetProperty(PropertyNames.IsListView);

        public virtual bool IsMultipleSelection =>
            (bool)GetProperty(PropertyNames.IsMultipleSelection);

        public virtual bool IsReportView =>
            (bool)GetProperty(PropertyNames.IsReportView);

        public virtual bool IsSmallIconView =>
            (bool)GetProperty(PropertyNames.IsSmallIconView);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        public virtual int[] SelectedIndices
        {
            get =>
                (int[])GetProperty(PropertyNames.SelectedIndices);
            set
            {
                SetProperty(PropertyNames.SelectedIndices, value);
            }
        }

        public virtual string[] SelectedItems
        {
            get =>
                (string[])GetProperty(PropertyNames.SelectedItems);
            set
            {
                SetProperty(PropertyNames.SelectedItems, value);
            }
        }

        public virtual string SelectedItemsAsString
        {
            get =>
                (string)GetProperty(PropertyNames.SelectedItemsAsString);
            set
            {
                SetProperty(PropertyNames.SelectedItemsAsString, value);
            }
        }

        public virtual ZappyTaskControl VerticalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.VerticalScrollBar);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string CheckedIndices = "CheckedIndices";
            public static readonly string CheckedItems = "CheckedItems";
            public static readonly string Columns = "Columns";
            public static readonly string HorizontalScrollBar = "HorizontalScrollBar";
            public static readonly string IsCheckedList = "IsCheckedList";
            public static readonly string IsIconView = "IsIconView";
            public static readonly string IsListView = "IsListView";
            public static readonly string IsMultipleSelection = "IsMultipleSelection";
            public static readonly string IsReportView = "IsReportView";
            public static readonly string IsSmallIconView = "IsSmallIconView";
            public static readonly string Items = "Items";
            public static readonly string SelectedIndices = "SelectedIndices";
            public static readonly string SelectedItems = "SelectedItems";
            public static readonly string SelectedItemsAsString = "SelectedItemsAsString";
            public static readonly string VerticalScrollBar = "VerticalScrollBar";
        }
    }
}

