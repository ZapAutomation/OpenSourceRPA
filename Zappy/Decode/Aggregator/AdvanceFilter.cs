using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Properties;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    [Serializable]
    internal class AdvanceFilter : ActionFilter
    {
        private QueryClause[] queryFilter;
        private QueryClause[] queryOutput;

        internal AdvanceFilter(string filterName, string filteringQuery, string outputQuery, ZappyTaskActionFilterType filterType, bool applyTimeout, string groupName) : base(filterName, filterType, applyTimeout, groupName)
        {
            ThrowIfNullOrEmpty("FilteringQuery", filteringQuery);
            filteringQuery = filteringQuery.TrimEnd(' ', '&', '\r', '\n');
            ThrowIfNullOrEmpty("FilteringQuery", filteringQuery);
            string[] separator = { "&" };
            string[] strArray = filteringQuery.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            queryFilter = new QueryClause[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                queryFilter[i] = new QueryClause(this, strArray[i]);
            }
            if (!string.IsNullOrEmpty(outputQuery))
            {
                outputQuery = outputQuery.TrimEnd(' ', '&', '\r', '\n');
                if (!string.IsNullOrEmpty(outputQuery))
                {
                    string[] textArray2 = { "&" };
                    strArray = outputQuery.Split(textArray2, StringSplitOptions.RemoveEmptyEntries);
                    queryOutput = new QueryClause[strArray.Length];
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        queryOutput[j] = new QueryClause(this, strArray[j]);
                    }
                }
            }
            VerifyFilterType(filteringQuery, outputQuery);
        }

        private static void FireFilterEvents(ZappyTaskActionStack actions, ZappyTaskAction originalStepLast, ZappyTaskAction originalStepSecondLast, ZappyTaskAction filteredStepLast, ZappyTaskAction filteredStepSecondLast)
        {
            if (filteredStepSecondLast != null)
            {
                actions.Push(filteredStepSecondLast);
                if (originalStepLast != null && filteredStepSecondLast.Id == originalStepLast.Id)
                {
                    originalStepLast = null;
                }
                else if (originalStepSecondLast != null && filteredStepSecondLast.Id == originalStepSecondLast.Id)
                {
                    originalStepSecondLast = null;
                }
            }
            if (filteredStepLast != null)
            {
                actions.Push(filteredStepLast);
                if (originalStepLast != null && filteredStepLast.Id == originalStepLast.Id)
                {
                    originalStepLast = null;
                }
                else if (originalStepSecondLast != null && filteredStepLast.Id == originalStepSecondLast.Id)
                {
                    originalStepSecondLast = null;
                }
            }
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            ZappyTaskAction stepLast = actions.Peek();
            ZappyTaskAction stepSecondLast = null;
            if (type == ZappyTaskActionFilterType.Binary)
            {
                if (actions.Count < 2)
                {
                    return false;
                }
                stepSecondLast = actions.Peek(1);
            }
            foreach (QueryClause clause in queryFilter)
            {
                if (!clause.IsMatch(stepLast, stepSecondLast))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction stepLast = actions.Pop();
            ZappyTaskAction stepSecondLast = null;
            if (type == ZappyTaskActionFilterType.Binary)
            {
                stepSecondLast = actions.Pop();
            }
            if (queryOutput == null)
            {
                return true;
            }
            ZappyTaskAction recordedStep = null;
            ZappyTaskAction filteredStepLast = null;
            ZappyTaskAction filteredStepSecondLast = null;
            foreach (QueryClause clause in queryOutput)
            {
                if (clause.rhTerms.Length == 1 && clause.rhTerms[0].termStepName != TermStepName.None && clause.rhTerms[0].recordedStepField == RecordedStepFields.All)
                {
                    if (clause.rhTerms[0].termStepName == TermStepName.Last)
                    {
                        recordedStep = stepLast;
                    }
                    else
                    {
                        recordedStep = stepSecondLast;
                    }
                }
                else
                {
                    if (clause.lhTerm.termStepName == TermStepName.Last)
                    {
                        recordedStep = filteredStepLast == null ? stepLast : filteredStepLast;
                    }
                    else if (clause.lhTerm.termStepName == TermStepName.Last2)
                    {
                        recordedStep = filteredStepSecondLast == null ? stepSecondLast : filteredStepSecondLast;
                    }
                    else
                    {
                        ThrowInvalidActionFilterException("Left hand term step name", clause.lhTerm.termStepName.ToString());
                    }
                    string stepFieldValue = string.Empty;
                    foreach (ExpressionTerm term in clause.rhTerms)
                    {
                        stepFieldValue = stepFieldValue + term.Evaluate(stepLast, stepSecondLast);
                    }
                    recordedStep = UpdateStepField(recordedStep, clause.lhTerm.recordedStepField, stepFieldValue);
                }
                if (clause.lhTerm.termStepName == TermStepName.Last)
                {
                    filteredStepLast = recordedStep;
                }
                else if (clause.lhTerm.termStepName == TermStepName.Last2)
                {
                    filteredStepSecondLast = recordedStep;
                }
                else
                {
                    ThrowInvalidActionFilterException("Left hand term step name", clause.lhTerm.termStepName.ToString());
                }
            }
            FireFilterEvents(actions, stepLast, stepSecondLast, filteredStepLast, filteredStepSecondLast);
            return false;
        }

        internal void ThrowIfNullOrEmpty(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                object[] args = { name, parameterName };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.EmptyValueInFilter, args));
            }
        }

        internal void ThrowInvalidActionFilterException(string parameterName, string parameterValue)
        {
            object[] args = { name, parameterName, parameterValue };
            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidValueInFilter, args));
        }

        private ZappyTaskAction UpdateStepField(ZappyTaskAction recordedStep, RecordedStepFields stepField, string stepFieldValue)
        {
            object[] objArray1;
            if (stepField <= RecordedStepFields.ActionType)
            {
                if (stepField <= RecordedStepFields.ElementType)
                {
                    if (stepField - 1 <= RecordedStepFields.ElementClass || stepField == RecordedStepFields.ElementQueryId || stepField == RecordedStepFields.ElementType)
                    {
                        goto Label_0176;
                    }
                }
                else
                {
                    if (stepField == RecordedStepFields.ActionValue)
                    {
                        (recordedStep as ZappyTaskAction).ValueAsString = stepFieldValue;
                        return recordedStep;
                    }
                    if (stepField == RecordedStepFields.ActionName)
                    {
                        recordedStep.ActionName = stepFieldValue;
                        return recordedStep;
                    }
                    if (stepField == RecordedStepFields.ActionType)
                    {
                        ZappyTaskAction action =
                            (ZappyTaskAction)Type.GetType(typeof(ZappyTaskAction).AssemblyQualifiedName.Replace("ZappyTaskAction", stepFieldValue)).GetConstructor(Type.EmptyTypes).Invoke(null);
                        action.ShallowCopy(recordedStep as ZappyTaskAction, false);
                        recordedStep = action;
                        return recordedStep;
                    }
                }
            }
            else if (stepField <= RecordedStepFields.ModifierKeys)
            {
                if (stepField == RecordedStepFields.AdditionalInfo)
                {
                    recordedStep.AdditionalInfo = stepFieldValue;
                    return recordedStep;
                }
                if (stepField == RecordedStepFields.Coordinates)
                {
                    goto Label_0176;
                }
                if (stepField == RecordedStepFields.ModifierKeys)
                {
                    InputAction action2 = recordedStep as InputAction;
                    if (action2 != null)
                    {
                        if (string.IsNullOrEmpty(stepFieldValue))
                        {
                            action2.ModifierKeys = ModifierKeys.None;
                            return recordedStep;
                        }
                        action2.ModifierKeys = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), stepFieldValue);
                    }
                    return recordedStep;
                }
            }
            else if (stepField <= RecordedStepFields.IsTopLevelElement)
            {
                if (stepField == RecordedStepFields.NeedFiltering)
                {
                    bool flag;
                    if (bool.TryParse(stepFieldValue, out flag))
                    {
                        (recordedStep as ZappyTaskAction).NeedFiltering = flag;
                    }
                    return recordedStep;
                }
                if (stepField == RecordedStepFields.IsTopLevelElement)
                {
                    goto Label_0176;
                }
            }
            else
            {
                if (stepField == RecordedStepFields.MouseButton)
                {
                    goto Label_0176;
                }
                if (stepField == RecordedStepFields.All)
                {
                    return recordedStep;
                }
            }
            object[] args = { name, "OutputQuery.RecordedStepFields", stepField };
            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidValueInFilter, args));
        Label_0176:
            objArray1 = new object[] { name, "OutputQuery.RecordedStepFields", stepField };
            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidValueInFilter, objArray1));
        }

        private void VerifyFilterType(string filteringQuery, string outputQuery)
        {
            TermStepName last = TermStepName.Last;
            bool flag = filteringQuery.IndexOf(last.ToString(), StringComparison.OrdinalIgnoreCase) != -1;
            last = TermStepName.Last2;
            bool flag2 = filteringQuery.IndexOf(last.ToString(), StringComparison.OrdinalIgnoreCase) != -1;
            if (!string.IsNullOrEmpty(outputQuery))
            {
                flag = flag || outputQuery.IndexOf((last = TermStepName.Last).ToString(), StringComparison.OrdinalIgnoreCase) != -1;
                flag2 = flag2 || outputQuery.IndexOf((last = TermStepName.Last2).ToString(), StringComparison.OrdinalIgnoreCase) != -1;
            }
            if (!flag)
            {
                ThrowInvalidActionFilterException("FilteringQuery", filteringQuery);
            }
            if (flag2)
            {
                type = ZappyTaskActionFilterType.Binary;
            }
            else
            {
                type = ZappyTaskActionFilterType.Unary;
            }
        }

        [Serializable]
        private class ExpressionTerm
        {
            internal int ancestorCount;
            internal string constantData;
            internal AdvanceFilter filter;
            internal RecordedStepFields recordedStepField;
            private bool refersTopLevelElement;
            internal TermStepName termStepName;

            internal ExpressionTerm(AdvanceFilter parentFilter, string termString)
            {
                filter = parentFilter;
                filter.ThrowIfNullOrEmpty("Expression", termString);
                if (termString.Length > 1 && termString[0] == '"' && termString[termString.Length - 1] == '"')
                {
                    termStepName = TermStepName.None;
                    constantData = termString.Substring(1, termString.Length - 2);
                }
                else
                {
                    char[] separator = { '.' };
                    string[] strArray = termString.Split(separator);
                    if (strArray.Length < 2)
                    {
                        filter.ThrowInvalidActionFilterException("Expression", termString);
                    }
                    try
                    {
                        termStepName = (TermStepName)Enum.Parse(typeof(TermStepName), strArray[0]);
                    }
                    catch (ArgumentException)
                    {
                        filter.ThrowInvalidActionFilterException("Term step name", strArray[0]);
                    }
                    if (termStepName != TermStepName.Last && termStepName != TermStepName.Last2)
                    {
                        filter.ThrowInvalidActionFilterException("Term step name", strArray[0]);
                    }
                    if (strArray.Length > 2)
                    {
                        if (string.Equals(strArray[1], "TopLevelElement", StringComparison.Ordinal))
                        {
                            refersTopLevelElement = true;
                        }
                        else
                        {
                            for (int i = 1; i < strArray.Length - 1; i++)
                            {
                                if (!string.Equals(strArray[i], "Parent", StringComparison.Ordinal))
                                {
                                    filter.ThrowInvalidActionFilterException("Term", strArray[i]);
                                }
                                ancestorCount++;
                            }
                        }
                    }
                    try
                    {
                        recordedStepField = (RecordedStepFields)Enum.Parse(typeof(RecordedStepFields), strArray[strArray.Length - 1]);
                    }
                    catch (ArgumentException)
                    {
                        filter.ThrowInvalidActionFilterException("Recorded Step Field", strArray[strArray.Length - 1]);
                    }
                }
            }

            internal string Evaluate(ZappyTaskAction stepLast, ZappyTaskAction stepSecondLast)
            {
                ZappyTaskAction action;
                if (termStepName == TermStepName.Last)
                {
                    action = stepLast;
                }
                else if (termStepName == TermStepName.Last2)
                {
                    action = stepSecondLast;
                }
                else
                {
                    return constantData;
                }
                TaskActivityElement elementInfo = null;
                if (refersTopLevelElement)
                {
                    elementInfo = action.ActivityElement != null ? FrameworkUtilities.TopLevelElement(action.ActivityElement) : null;
                }
                else
                {
                    elementInfo = MoveToAppropriateAncestor(action.ActivityElement);
                }
                try
                {
                    return EvaluateField(action, elementInfo);
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                    if (ancestorCount == 0)
                    {
                        throw;
                    }
                    return string.Empty;
                }
            }


            private string EvaluateField(ZappyTaskAction recordedStep, TaskActivityElement elementInfo)
            {
                string str = string.Empty;
                if (elementInfo == null && (recordedStepField & (RecordedStepFields.ElementType | RecordedStepFields.ElementQueryId | RecordedStepFields.ElementName | RecordedStepFields.ElementClass)) == recordedStepField)
                {
                    return str;
                }
                switch (recordedStepField)
                {
                    case RecordedStepFields.ActionName:
                        return recordedStep.ActionName;

                    case RecordedStepFields.ActionType:
                        return recordedStep.GetType().Name;

                    case RecordedStepFields.AdditionalInfo:
                        return recordedStep.AdditionalInfo;

                    case RecordedStepFields.ElementClass:
                        return elementInfo.ClassName;

                    case RecordedStepFields.ElementName:
                        return elementInfo.Name;

                    case RecordedStepFields.ElementQueryId:
                        return elementInfo.GetHashCode().ToString(CultureInfo.InvariantCulture);

                    case RecordedStepFields.ElementType:
                        return elementInfo.ControlTypeName;

                    case RecordedStepFields.ActionValue:
                        return recordedStep.ValueAsString;

                    case RecordedStepFields.Coordinates:
                        {
                            MouseAction action = recordedStep as MouseAction;
                            if (action != null)
                            {
                                str = action.Location.ToString();
                            }
                            return str;
                        }
                    case RecordedStepFields.ModifierKeys:
                        {
                            InputAction action5 = recordedStep as InputAction;
                            if (action5 == null)
                            {
                                return str;
                            }
                            if (action5.ModifierKeys == ModifierKeys.None)
                            {
                                return string.Empty;
                            }
                            return action5.ModifierKeys.ToString();
                        }
                    case RecordedStepFields.NeedFiltering:
                        return (recordedStep as ZappyTaskAction).NeedFiltering ? bool.TrueString : bool.FalseString;

                    case RecordedStepFields.IsTopLevelElement:
                        return FrameworkUtilities.IsTopLevelElement(elementInfo).ToString(CultureInfo.InvariantCulture);

                    case RecordedStepFields.MouseButton:
                        {
                            MouseAction action2 = recordedStep as MouseAction;
                            if (action2 != null)
                            {
                                return action2.MouseButton.ToString();
                            }
                            DragAction action3 = recordedStep as DragAction;
                            if (action3 != null)
                            {
                                return action3.MouseButton.ToString();
                            }
                            DragDropAction action4 = recordedStep as DragDropAction;
                            if (action4 != null)
                            {
                                str = action4.MouseButton.ToString();
                            }
                            return str;
                        }
                    case RecordedStepFields.All:
                        return str;
                }
                object[] args = { filter.Name, "FilteringQuery.RecordedStepFields", recordedStepField };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidValueInFilter, args));
            }

            internal TaskActivityElement MoveToAppropriateAncestor(TaskActivityElement elementInfo)
            {
                int num = 0;
                while (num < ancestorCount && elementInfo != null)
                {
                    try
                    {
                        elementInfo = ZappyTaskService.Instance.GetParentFast(elementInfo);
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                        num = 0;
                        break;
                    }
                    num++;
                }
                if (num != ancestorCount)
                {
                    throw new Exception();
                }
                return elementInfo;
            }
        }

        private static class LogicalOperator
        {
            internal const string And = "&";
        }

        [Serializable]
        private class QueryClause
        {
            internal RelationCondition expectedCondition;
            internal AdvanceFilter filter;
            internal ExpressionTerm lhTerm;
            internal ExpressionTerm[] rhTerms;

            internal QueryClause(AdvanceFilter parentFilter, string clause)
            {
                filter = parentFilter;
                char[] separator = { '=' };
                string[] strArray = clause.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length != 2)
                {
                    filter.ThrowInvalidActionFilterException("Query", clause);
                }
                string parameterValue = strArray[0].Trim();
                string str2 = strArray[1].Trim();
                filter.ThrowIfNullOrEmpty("Left hand term", parameterValue);
                filter.ThrowIfNullOrEmpty("Right hand term", str2);
                if (parameterValue[parameterValue.Length - 1] == '!')
                {
                    parameterValue = parameterValue.Substring(0, parameterValue.Length - 1);
                    expectedCondition = RelationCondition.NotEqual;
                }
                else if (parameterValue[parameterValue.Length - 1] == '<')
                {
                    parameterValue = parameterValue.Substring(0, parameterValue.Length - 1);
                    expectedCondition = RelationCondition.RightContains;
                }
                else if (str2.Length > 0 && str2[0] == '>')
                {
                    str2 = str2.Substring(1, str2.Length - 1);
                    expectedCondition = RelationCondition.LeftContains;
                }
                else
                {
                    expectedCondition = RelationCondition.Equal;
                }
                lhTerm = new ExpressionTerm(filter, parameterValue);
                char[] chArray2 = { '+' };
                string[] strArray2 = str2.Split(chArray2, StringSplitOptions.RemoveEmptyEntries);
                rhTerms = new ExpressionTerm[strArray2.Length];
                for (int i = 0; i < strArray2.Length; i++)
                {
                    rhTerms[i] = new ExpressionTerm(filter, strArray2[i]);
                }
            }

            internal bool IsMatch(ZappyTaskAction stepLast, ZappyTaskAction stepSecondLast)
            {
                string str;
                string str2;
                if (rhTerms.Length != 1)
                {
                    filter.ThrowInvalidActionFilterException("Right hand term", rhTerms.Length.ToString(CultureInfo.InvariantCulture));
                }
                try
                {
                    str = lhTerm.Evaluate(stepLast, stepSecondLast);
                    str2 = rhTerms[0].Evaluate(stepLast, stepSecondLast);
                }
                catch (Exception)
                {
                    
                    return false;
                }
                if (str == null && str2 == null)
                {
                    return expectedCondition != RelationCondition.NotEqual;
                }
                if (str == null || str2 == null)
                {
                    return expectedCondition == RelationCondition.NotEqual;
                }
                switch (expectedCondition)
                {
                    case RelationCondition.NotEqual:
                        return !string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);

                    case RelationCondition.LeftContains:
                        return str.IndexOf(str2, StringComparison.CurrentCultureIgnoreCase) != -1;

                    case RelationCondition.RightContains:
                        return str2.IndexOf(str, StringComparison.CurrentCultureIgnoreCase) != -1;
                }
                return string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [Flags]
        private enum RecordedStepFields
        {
            ActionName = 0x20,
            ActionType = 0x40,
            ActionValue = 0x10,
            AdditionalInfo = 0x80,
            All = 0x1fff,
            Coordinates = 0x100,
            ElementClass = 1,
            ElementName = 2,
            ElementQueryId = 4,
            ElementType = 8,
            IsTopLevelElement = 0x800,
            ModifierKeys = 0x200,
            MouseButton = 0x1000,
            NeedFiltering = 0x400
        }

        private enum RelationCondition
        {
            Equal,
            NotEqual,
            LeftContains,
            RightContains
        }

        private enum TermStepName
        {
            Last = 1,
            Last2 = 0,
            None = -1
        }
    }
}

