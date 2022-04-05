using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class ConvertActionOnWinformDatagrid : ActionFilter
    {
        public ConvertActionOnWinformDatagrid() : base("ConvertActionOnWinformDatagrid", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        private static TaskActivityElement GetAncestorDatagridTable(TaskActivityElement element)
        {
                        {
                for (int i = 0; i < 5; i++)
                {
                    element = RecorderUtilities.GetParent(element);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }
            return element;
        }

        private static bool IsHorizontalKeyNavigation(ZappyTaskAction zlastAction)
        {
            if (zlastAction is ZappyTaskAction)
            {
                ZappyTaskAction lastAction = zlastAction as ZappyTaskAction;
                if (!lastAction.ValueAsString.EndsWith("{Tab}", StringComparison.OrdinalIgnoreCase) && !lastAction.ValueAsString.EndsWith("{Left}", StringComparison.OrdinalIgnoreCase))
                {
                    return lastAction.ValueAsString.EndsWith("{Right}", StringComparison.OrdinalIgnoreCase);
                }
                return true;
            }
            return false;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            ZappyTaskAction lastAction = actions.Peek();
            ZappyTaskAction secondLastAction = actions.Peek(1);
            TaskActivityElement uIElement = lastAction.ActivityElement;
            if (uIElement == null || !AggregatorUtilities.MatchesWinformsEditingControl(uIElement.Name) && !IsNavigationOnDotNetDataGrid(lastAction, secondLastAction) && !AggregatorUtilities.IsKeyNavigationAction(lastAction))
            {
                return false;
            }
            return AggregatorUtilities.IsActionOnControlType(secondLastAction, ControlType.Cell);
        }

        private static bool IsNavigationOnDotNetDataGrid(ZappyTaskAction lastAction, ZappyTaskAction secondLastAction)
        {
            ITaskActivityElement uIElement = lastAction.ActivityElement;
            ITaskActivityElement grandParentUsingQid = AggregatorUtilities.GetGrandParentUsingQid(secondLastAction);
            bool flag = false;
            if (grandParentUsingQid != null && ControlType.Table.NameEquals(grandParentUsingQid.ControlTypeName) && !string.Equals(grandParentUsingQid.Name, "DataGridView", StringComparison.OrdinalIgnoreCase) && AggregatorUtilities.IsKeyNavigationAction(lastAction))
            {
                flag = true;
            }
            return flag;
        }

        private static bool IsNavigationOnListItem(ZappyTaskAction lastAction) =>
            ControlType.ListItem.NameEquals(lastAction.ActivityElement.ControlTypeName) && lastAction.ValueAsString != null && IsHorizontalKeyNavigation(lastAction);

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ITaskActivityElement ancestorDatagridTable;
            ZappyTaskAction action = actions.Peek(0);
            ZappyTaskAction action2 = actions.Peek(1);
            ITaskActivityElement grandParentUsingQid = AggregatorUtilities.GetGrandParentUsingQid(action2);
            if (AggregatorUtilities.MatchesWinformsEditingControl(action.ActivityElement.Name) && ControlType.Text.NameEquals(action.ActivityElement.ControlTypeName))
            {
                ancestorDatagridTable = GetAncestorDatagridTable(action.ActivityElement);
            }
            else
            {
                ancestorDatagridTable = AggregatorUtilities.GetGrandParentUsingQid(action);
            }
            bool flag = ancestorDatagridTable == null && grandParentUsingQid != null && ControlType.Table.NameEquals(grandParentUsingQid.ControlTypeName) && !string.Equals(grandParentUsingQid.Name, "DataGridView", StringComparison.OrdinalIgnoreCase) && AggregatorUtilities.IsKeyNavigationAction(action);
            if ((ancestorDatagridTable != null && grandParentUsingQid != null && ancestorDatagridTable.Equals(grandParentUsingQid)) | flag && ControlType.Table.NameEquals(grandParentUsingQid.ControlTypeName) && !ShouldNotConvertAction(action, action2) || IsNavigationOnListItem(action))
            {
                actions.Pop(0);
                SetValueAction action3 = action as SetValueAction;
                if (action3 != null)
                {
                    if (ControlType.ComboBox.NameEquals(action.ActivityElement.ControlTypeName) && action2 is SendKeysAction && ControlType.Cell.NameEquals(action2.ActivityElement.ControlTypeName))
                    {
                        action3.PreferEdit = true;
                    }
                    action3.AdditionalInfo = "Aggregated";
                }
                action.ActivityElement = action2.ActivityElement;
                ZappyTaskAction element = action;
                actions.Push(element);
            }
            return false;
        }

        private static bool ShouldNotConvertAction(ZappyTaskAction lastAction, ZappyTaskAction inputAction) =>
            AggregatorUtilities.IsKeyNavigationAction(inputAction) && (!ControlType.ComboBox.NameEquals(lastAction.ActivityElement.ControlTypeName) || IsHorizontalKeyNavigation(inputAction)) || ControlType.DateTimePicker.NameEquals(lastAction.ActivityElement.ControlTypeName);
    }
}

