using System.Windows.Forms;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class ConvertActivitiesForStartMenuSearch : ActionFilter
    {
        public ConvertActivitiesForStartMenuSearch() : base("ConvertActivitiesForStartMenuSearch", ZappyTaskActionFilterType.Binary, false, "MiscellaneousAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction action = actions.Peek();
                if (AggregatorUtilities.IsWindows7OrHigher)
                {
                    if (AbsorbActivitiesBeforeLaunchApplication.IsActionOnChildOfStartMenu(action))
                    {
                        if (AbsorbActivitiesBeforeLaunchApplication.IsActionOnListItemOnStartMenu(action, true))
                        {
                            return true;
                        }
                        if (AbsorbActivitiesBeforeLaunchApplication.IsActionOnSearchBoxOnStartMenu(action))
                        {
                            return AggregatorUtilities.IsSendKeysOfEnter(action);
                        }
                    }
                    return false;
                }
                if (AbsorbActivitiesBeforeLaunchApplication.IsActionOnSearchBoxOnStartMenu(action))
                {
                    SetValueAction action2 = action as SetValueAction;
                    return action2 != null && string.IsNullOrEmpty(action2.ValueAsString);
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            if (!AggregatorUtilities.IsWindows7OrHigher)
            {
                actions.Pop();
                return false;
            }
            string text = string.Empty;
            ZappyTaskAction action = actions.Peek();
            TaskActivityElement uIElement = action.ActivityElement;
            SendKeysAction action2 = null;
            bool flag = true;
            if (AggregatorUtilities.IsSendKeysOfEnter(action) && AbsorbActivitiesBeforeLaunchApplication.IsActionOnSearchBoxOnStartMenu(action))
            {
                action2 = action as SendKeysAction;
                text = action2.Text;
                flag = false;
            }
            for (ZappyTaskAction action3 = actions.Peek(1); AbsorbActivitiesBeforeLaunchApplication.IsActionOnChildOfStartMenu(action3) && action3 != null; action3 = AggregatorUtilities.PeekHelper(actions, 1))
            {
                if (action3 is SetValueAction)
                {
                    SetValueAction action6 = action3 as SetValueAction;
                    text = action6.ValueAsString + text;
                }
                if (action3 is SendKeysAction)
                {
                    action2 = action3 as SendKeysAction;
                    text = action2.Text + text;
                }
                if (AbsorbActivitiesBeforeLaunchApplication.IsActionOnSearchBoxOnStartMenu(action3))
                {
                    uIElement = action3.ActivityElement;
                }
                actions.Pop(1);
            }
            action = actions.Pop();
            MouseAction element = new MouseAction(uIElement, MouseButtons.Left, MouseActionType.Click)
            {
                AdditionalInfo = "AggregateIfLaunched"
            };
            actions.Push(element);
            SendKeysAction action5 = new SendKeysAction();
            text = text.Replace("{Up}", string.Empty).Replace("{Down}", string.Empty);
            if (flag)
            {
                text = text.Replace("{Enter}", string.Empty);
            }
            action5.Text = text;
            action5.AdditionalInfo = "AggregateIfLaunched";
            actions.Push(action5);
            if (flag)
            {
                if (!AggregatorUtilities.IsLeftClick(action))
                {
                    action = new MouseAction(action.ActivityElement, MouseButtons.Left, MouseActionType.Click);
                }
                action.AdditionalInfo = "AggregateIfLaunched";
                actions.Push(action);
            }
            return false;
        }
    }
}

