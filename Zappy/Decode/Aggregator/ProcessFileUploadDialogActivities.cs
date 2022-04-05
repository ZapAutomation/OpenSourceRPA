using System;
using System.Runtime.InteropServices;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class ProcessFileUploadDialogActivities : ActionFilter
    {
        private const string DialogClassName = "#32770";
        private const string FileUploadParentClassName = "Alternate Modal Top Most";
        private const string IEClassName = "IEFrame";

        public ProcessFileUploadDialogActivities() : base("ProcessFileUploadDialogActivities", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }


        private static T GetOwnerWindowProperty<T>(TaskActivityElement topLevelElement, string propertyName)
        {
            T propertyValue = default(T);
            try
            {
                if (topLevelElement != null)
                {
                    propertyValue = (T)topLevelElement.GetPropertyValue(propertyName);
                }
            }
            catch (COMException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            catch (NotSupportedException exception2)
            {
                CrapyLogger.log.Error(exception2);
            }
            catch (NotImplementedException exception3)
            {
                CrapyLogger.log.Error(exception3);
            }
            return propertyValue;
        }

        private static bool IsElementFileUploadDialog(TaskActivityElement topLevelElement) =>
            topLevelElement != null && string.Equals(topLevelElement.ClassName, "#32770", StringComparison.Ordinal) && string.Equals(topLevelElement.Name, LocalizedSystemStrings.Instance.IEFileUploadDialogTitle, StringComparison.Ordinal) && MatchClassName(topLevelElement);

        private static bool IsElementFileUploadDialogChild(TaskActivityElement topLevelElement) =>
            topLevelElement != null && string.Equals(topLevelElement.ClassName, "#32770", StringComparison.Ordinal) && string.Equals(GetOwnerWindowProperty<string>(topLevelElement, "OwnerWindowText"), LocalizedSystemStrings.Instance.IEFileUploadDialogTitle, StringComparison.Ordinal);

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            TaskActivityElement uIElement = actions.Peek().ActivityElement;
            TaskActivityElement topLevelElement = uIElement == null ? null : FrameworkUtilities.TopLevelElement(uIElement);
            if (!IsElementFileUploadDialog(topLevelElement))
            {
                return IsElementFileUploadDialogChild(topLevelElement);
            }
            return true;
        }

        private static bool MatchClassName(TaskActivityElement topLevelElement)
        {
            string ownerWindowProperty = GetOwnerWindowProperty<string>(topLevelElement, "OwnerWindowClassName");
            if (!ZappyTaskUtilities.IsIEWindowClassName(ownerWindowProperty))
            {
                return string.Equals(ownerWindowProperty, "Alternate Modal Top Most", StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            TaskActivityElement topLevelElement = FrameworkUtilities.TopLevelElement(actions.Peek().ActivityElement);
            if (IsElementFileUploadDialogChild(topLevelElement))
            {
                actions.Pop();
                return false;
            }
            string ownerWindowProperty = GetOwnerWindowProperty<string>(topLevelElement, "OwnerWindowText");
            if (ownerWindowProperty == null)
            {
                
                return false;
            }
            int nth = 1;
            while (nth < actions.Count)
            {
                TaskActivityElement uIElement = actions.Peek(nth).ActivityElement;
                TaskActivityElement element = uIElement == null ? null : FrameworkUtilities.TopLevelElement(uIElement);
                if (element == null || !string.Equals(AggregatorUtilities.GetNativeWindowText(element), ownerWindowProperty, StringComparison.Ordinal))
                {
                    CrapyLogger.log.Error("ProcessFileUploadDialogActivities: An action on file upload dialog was not preceeded by action on owner IE window");
                    return false;
                }
                if (AggregatorUtilities.IsActionOnControlType(actions.Peek(nth), ControlType.FileInput))
                {
                    break;
                }
                nth++;
            }
            if (nth < actions.Count)
            {
                for (int i = 0; i < nth; i++)
                {
                    actions.Pop();
                }
                ZappyTaskAction source = actions.Pop();
                ZappyTaskAction action2 = new SetValueAction();
                (action2 as ZappyTaskAction).ShallowCopy(source, false);
                action2.AdditionalInfo = "Aggregated";
                action2.ValueAsString = source.ActivityElement.Value;
                actions.Push(action2);
            }
            return false;
        }
    }
}

