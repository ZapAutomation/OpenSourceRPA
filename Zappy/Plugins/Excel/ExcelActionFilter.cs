using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ZappyActions.Excel;


namespace Zappy.Plugins.Excel
{
                internal class ExcelActionFilter : ZappyTaskActionFilter
    {
        #region Simple Properties

                                                        public override bool ApplyTimeout
        {
            get { return true; }
        }

                                        public override ZappyTaskActionFilterCategory Category
        {
            get { return ZappyTaskActionFilterCategory.PostSimpleToCompoundActionConversion; }
        }

                                public override bool Enabled
        {
            get { return true; }
        }

                                public override ZappyTaskActionFilterType FilterType
        {
            get { return ZappyTaskActionFilterType.Binary; }
        }

                                        public override string Group
        {
            get { return "ExcelActionFilters"; }
        }

                                        public override string Name
        {
            get { return "ExcelActionFilter"; }
        }

        #endregion

                                                                public static string[] removeSpecialChar =
        {
            "{Enter}", "{Tab}", "{LMenu}", "{RMenu}", "{LShiftKey}", "{RShiftKey}", "{LControlKey}", "{RControlKey}",
            "{Back}", "{Home}",
            "{Right}", "{Left}",
            "{Up}",
            "{Down}"
        };
        public override bool ProcessRule(ZappyTaskActionStack actionStack)
        {
                        SendKeysAction lastAction = actionStack.Peek() as SendKeysAction;

            
                                                                                                            
            ExcelSendKeysAction excelSecondLastAction = actionStack.Peek(1) as ExcelSendKeysAction;
            if ((lastAction != null) && lastAction.ActivityElement != null && lastAction.ActivityElement is ExcelCellElement)
            {
                actionStack.Pop();
                foreach (string item in removeSpecialChar)
                {
                    if (lastAction.Text.Value.Equals(item))                    {
                        return true;
                    }
                }
                if (string.IsNullOrEmpty(lastAction.Text.Value))
                    return true;
                ExcelSendKeysAction excelLastAction = new ExcelSendKeysAction(lastAction);
                                if (excelSecondLastAction != null &&
                    excelSecondLastAction.EqualCellInfo(excelLastAction) &&
                    excelSecondLastAction.ModifierKeys == ModifierKeys.None &&
                    excelLastAction.ModifierKeys == ModifierKeys.None)
                {
                                        excelSecondLastAction.Text.Value = excelSecondLastAction.Text.Value + excelLastAction.Text.Value;
                }
                else
                {
                    if (excelSecondLastAction != null) excelSecondLastAction.refreshAndgetCellValueAndFormula();
                    actionStack.Push(excelLastAction);
                }
                return true;
            }
            else
            {
                if (excelSecondLastAction != null) excelSecondLastAction.refreshAndgetCellValueAndFormula();
            }
            return false;
        }


                                                                
                                                                    }
}
