using System;
using System.IO;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Excel;

namespace Zappy.Invoker
{
    public static class DisplayHelper
    {
        public static string getFrmZappyActionDisplayName(IZappyAction currentAction, bool keyVal = false)
        {
            string elementname = GetActionElementName(currentAction);
            if (!string.IsNullOrEmpty(elementname))
            {
                elementname = " (" + elementname + ")";
            }
            return NodeTextHelper(currentAction, keyVal) + elementname;
        }

                                
                public static void buildfrmZappyDisplay(ZappyTask taskToPlay, TreeNode TaskNode)
        {
            try
            {
                TreeNode _ActionNode;
                for (int j = 0; j < taskToPlay.ExecuteActivities.Count; j++)
                {
                    IZappyAction currentAction = taskToPlay.ExecuteActivities.Activities[j];
                                                                                                    if (currentAction is StartNodeAction || currentAction is EndNodeAction)
                    {
                                            }
                    else
                    {
                        string actionValue = ActionValueHelper(currentAction);
                        int icon = IconValueHelper(currentAction);
                        _ActionNode =
                            TaskNode.Nodes.Add(getFrmZappyActionDisplayName(currentAction));
                        _ActionNode.Tag = currentAction;
                        _ActionNode.ToolTipText = GetActionElementWindow(currentAction) + actionValue;                         _ActionNode.ImageIndex = icon;
                    }

                }
            }
            catch
            {
                TaskNode.Nodes.Clear();
            }
        }


        public static string ActionValueHelper(IZappyAction action, bool newLine = true)
        {
            string actionValue = "";
            if (action is ZappyTaskAction)
            {
                ZappyTaskAction curAction = action as ZappyTaskAction;
                actionValue = curAction.ValueAsString;
            }

            SendKeysAction saction = action as SendKeysAction;
            if (saction != null)
            {
                if (saction.ModifierKeys == ModifierKeys.Control)
                {
                    if (actionValue.Equals("c"))
                    {
                        actionValue = "${Copy}";
                    }
                    else if (actionValue.Equals("v"))
                    {
                        actionValue = "${Paste}";
                    }
                    else if (actionValue.Equals("a"))
                    {
                        actionValue = "${SelectAll}";
                    }
                }
            }
            else if (action is LaunchApplicationAction)
            {
                actionValue = "Application: " +
                              Path.GetFileName((action as LaunchApplicationAction)
                                  .FileName);
            }
            else if (action.GetType() == typeof(MouseAction))
            {
                actionValue = string.Empty;             }
            else if (action is ChromeActionKeyboard)
            {
                ChromeActionKeyboard curAction = action as ChromeActionKeyboard;
                actionValue = curAction.CommandValue;
            }
            else if (action is ChromeAction)
            {
                ChromeAction curAction = action as ChromeAction;
                if (curAction.CommandName.StartsWith("open"))
                {

                    actionValue = TrimChromeUrl(curAction);                 }
            }
            else if (action is ZappyTaskAction)
                actionValue = (action as ZappyTaskAction).ValueAsString;

            if (newLine && (actionValue != string.Empty))
            {
                actionValue = "\n" + actionValue;
            }

            return actionValue;
        }

                                
                                
        public static int IconValueHelper(IZappyAction action)
        {
            int _Icon = 0;
            SendKeysAction saction = action as SendKeysAction;
            if (saction != null)
            {
                if ((saction).IsActionOnProtectedElement())
                {
                    _Icon = 1;
                }
                else
                {
                    _Icon = 6;
                }
            }
                        
                        else if (action is ChromeAction)
            {
                _Icon = 8;
                                string tempText = (action as ChromeAction).CommandName;
                if (tempText.StartsWith("click"))
                {
                    _Icon = 2;
                }
                else if (tempText.StartsWith("type") || tempText.StartsWith("sendKeys"))
                {
                    _Icon = 6;
                }
                else if (tempText.StartsWith("double"))
                {
                    _Icon = 3;
                }
                else if (tempText.StartsWith("mouse"))
                {
                    _Icon = 5;
                }

            }

            else if (action.GetType() == typeof(MouseAction))
            {
                MouseAction _Mouse = action as MouseAction;
                if (_Mouse.ActionType == MouseActionType.Click)
                {
                    if (_Mouse.MouseButton == MouseButtons.Left)
                        _Icon = 2;
                    else if (_Mouse.MouseButton == MouseButtons.Right)
                        _Icon = 4;
                }
                else if (_Mouse.ActionType == MouseActionType.DoubleClick)
                    _Icon = 3;
                else
                    _Icon = 5;
            }
            return _Icon;
        }

        public static string NodeTextHelper(IZappyAction action, bool keyVal = false)
        {
            string _NodeText = String.Empty;
            SendKeysAction saction = action as SendKeysAction;
            if (saction != null)
            {
                if (keyVal)
                {
                    _NodeText = "Type";
                }
                else if ((saction).IsActionOnProtectedElement())
                {
                    _NodeText = "***";
                }
                else
                {
                    _NodeText = (saction).ValueAsString.Length > 3 ?
                        (saction).ValueAsString.Substring(0, 3) + ".." : (saction).ValueAsString;
                }
            }
            else if (action.GetType() == typeof(MouseAction))
            {
                MouseAction _Mouse = action as MouseAction;
                if (_Mouse.ActionType == MouseActionType.DoubleClick)
                    _NodeText = "Double Click";

                else if (keyVal)
                {
                    if (_Mouse.ActionType == MouseActionType.Click && _Mouse.MouseButton == MouseButtons.Left)
                        _NodeText = "Click";
                    else
                    {
                        _NodeText = (action as MouseAction).ActionName;

                    }

                }
                else
                {
                    _NodeText = "";
                }
            }
            else if (action.GetType() == typeof(LaunchApplicationAction))
            {
                _NodeText = "Launch";
            }
                                                            else if (action is ChromeAction)
            {
                                string tempText = (action as ChromeAction).CommandName;
                if (tempText.StartsWith("click"))
                {
                    if (keyVal)
                    {
                        _NodeText = "Click";
                    }
                    else
                        _NodeText = "";
                }
                else if (tempText.StartsWith("type") || tempText.StartsWith("sendKeys"))
                {
                                                                                                    if (keyVal)
                    {
                        _NodeText = "Text";
                    }
                    else
                    {
                        string actionValue = (action as ChromeAction).CommandValue;
                        _NodeText = actionValue.Length > 3 ? actionValue.Substring(0, 3) + ".." : actionValue;
                    }

                                        

                }
                else if (tempText.StartsWith("double"))
                {
                    _NodeText = "Double Click";
                }
                else if (tempText.StartsWith("open"))
                {
                    _NodeText = "Navigate";
                }
                else if (tempText.StartsWith("mouse"))
                {
                    _NodeText = "Mouse hover";
                }
                else
                {
                    _NodeText = tempText;
                }

            }
            else
            {
                HelperFunctions.TypeToHumanizedString.TryGetValue(action.GetType(), out _NodeText);
            }
            return _NodeText;
        }

        public static string GetActionElementName(IZappyAction Action)
        {
            try
            {
                if (Action is ChromeAction)
                {
                    try
                    {
                                                ChromeAction curAction = (Action as ChromeAction);
                        
                        string idText;
                        if (curAction.CommandName.StartsWith("open"))
                        {
                            idText = GetChromeMainUrl(curAction);
                        }
                        else
                        {
                            idText = (Action as ChromeAction).CommandTarget[0];
                        }

                        return idText;
                    }
                    catch
                    {
                        return "Navigate";
                    }

                }
                else if (Action is ExcelSendKeysAction eaction)
                {
                    return eaction.ToDisplayString();
                }
                else if (Action is ZappyTaskAction _Action)
                {
                    return GetWindowElement(_Action);


                }
                else if (Action is RunZappyTaskFromFile runZappyTask)
                {
                    if (runZappyTask.LoadFromFilePath != null) return Path.GetFileName(runZappyTask.LoadFromFilePath);
                    return string.Empty;
                }
                else if (Action is VariableNodeAction variableNodeAction)
                {
                    return variableNodeAction.VariableName;
                }
                else return string.Empty;
            }
            catch (Exception)
            {
                
                return string.Empty;
            }

        }

        public static string GetChromeMainUrl(ChromeAction action)
        {
            return new Uri(TrimChromeUrl(action)).Host;
        }

        public static string GetWindowElement(ZappyTaskAction _Action)
        {
            
                        
            if (!string.IsNullOrEmpty(_Action.TaskActivityIdentifier))
            {
                string[] UIObjSplit = _Action.TaskActivityIdentifier.Split('.');
                return UIObjSplit[UIObjSplit.Length - 1];

            }
            if (_Action.ActivityElement != null)
            {
                if (!string.IsNullOrEmpty(_Action.ActivityElement.FriendlyName))
                    return _Action.ActivityElement.FriendlyName;
                if (!string.IsNullOrEmpty(_Action.ActivityElement.Name))
                    return _Action.ActivityElement.Name;
                return _Action.ActivityElement.ClassName;
            }
            return string.Empty;
        }

        private static string TrimChromeUrl(ChromeAction action)
        {
            return action.ActionUrlTab.Trim('\"').Trim('\\').Trim('\"');
        }

        public static string GetActionElementWindow(IZappyAction Action)
        {
            try
            {
                if (Action is ChromeAction)
                {
                    string actionValue = TrimChromeUrl(Action as ChromeAction);
                    return actionValue.Length > 30 ? actionValue.Substring(0, 30) + ".." : actionValue;
                }
                else if (Action is ExcelSendKeysAction eaction)
                {
                    return eaction.WorkbookName;
                }
                else if (Action is ZappyTaskAction _Action)
                {
                    if (_Action.WindowIdentifier?.TopLevelWindows != null)
                        return _Action.WindowIdentifier.TopLevelWindows[0].FriendlyName;

                                                                                                                                            
                                                                                return string.Empty;
                }
                else return string.Empty;
            }
            catch (Exception)
            {
                
                return string.Empty;
            }

        }


    }
}
