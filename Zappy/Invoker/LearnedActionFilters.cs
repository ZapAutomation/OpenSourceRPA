using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Hooks.Window;
using Zappy.Decode.LogManager;
using Zappy.InputData;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Code;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.DataSearch;
using Zappy.ZappyActions.Documents;
using Zappy.ZappyActions.Excel;
using Zappy.ZappyActions.Loops;
using Zappy.ZappyActions.Miscellaneous;
using Zappy.ZappyActions.PDF;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using ZappyMessages.ExcelMessages;

namespace Zappy.Invoker
{
    internal class LearnedActionFilters
    {
        internal List<IZappyAction> createChromeOpenActionCommand(List<IZappyAction> recordedTaskActivities)
        {
            List<IZappyAction> copyList = new List<IZappyAction>();
            bool insertedOpenCommand = false;

            foreach (var action in recordedTaskActivities)
            {
                if (!insertedOpenCommand)
                {
                    if (action is ChromeAction CurAction)
                    {
                        ChromeAction_open openCommand = new ChromeAction_open();
                        openCommand.CommandTarget = new List<string>() { CurAction.ActionUrlTab };
                        openCommand.CommandValue = "";
                        openCommand.ActionUrlTab = CurAction.ActionUrlTab;
                        openCommand.ZappyWindowTitle = CurAction.ZappyWindowTitle;
                        openCommand.Timestamp = CurAction.Timestamp;
                                                                        openCommand.ExeName = "chrome.exe";
                        copyList.Add(openCommand);
                        insertedOpenCommand = true;
                        copyList.Add(CurAction);
                                            }
                    else
                    {
                        copyList.Add(action);
                    }
                }
                else
                    copyList.Add(action);
            }

            return copyList;
        }

        internal List<IZappyAction> removeUnnessaryMouseActions(List<IZappyAction> recordedTaskActivities)
        {
            List<IZappyAction> copyList = new List<IZappyAction>();
            foreach (var action in recordedTaskActivities)
            {
                if (action is MouseAction maction)
                {
                                                            if (!action.ExeName.ToUpper().Equals("EXCEL.EXE"))
                    {
                        if (maction.ZappyWindowTitle != null && maction.ZappyWindowTitle.Equals("Running applications"))
                        { }
                        else
                        {
                            copyList.Add(action);
                        }
                    }
                }
                else
                {
                    copyList.Add(action);
                }
            }
            return copyList;
        }

        internal List<IZappyAction> removeChromeNativeActions(List<IZappyAction> recordedTaskActivities)
        {
            List<IZappyAction> copyList = new List<IZappyAction>();
            foreach (var action in recordedTaskActivities)
            {
                if (action is ZappyTaskAction zaction)
                {
                    if(action is ChromeAction)
                    {
                        copyList.Add(action);
                    }
                    else if (!action.ExeName.ToUpper().Equals("CHROME.EXE"))
                    {                       
                            copyList.Add(action);
                    }               
                }
                else
                {
                    copyList.Add(action);
                }
            }
            return copyList;
        }

        private string getCommandLineArgs(IntPtr windowhandle)
        {
            try
            {
                                Process process = WindowLaunchEventCapture.GetProcess(windowhandle);
                string processCommandLine = WindowLaunchEventCapture.GetProcessCommandLine(process);
                return processCommandLine;
                                                                                            }
            catch
            {
                return "";
            }
        }

                internal List<IZappyAction> getPDFSearchActivity(
            List<IZappyAction> recordedTaskActivities)
        {
            List<IZappyAction> copyList = new List<IZappyAction>();
                        bool RecordFirstAdobeMouseAction = true;
            var firstPDFAction = true;
            string pdftextValue = string.Empty;
            PDFToTextSharp pdfToText = new PDFToTextSharp();
            TextNormalization textNormalization = new TextNormalization();
            OpenFileDialogAction openFileDialogAction = new OpenFileDialogAction();
                        foreach (var action in recordedTaskActivities)
            {
                if (action is SendKeysAction zaction)
                {
                    try
                    {
                        if (zaction.ExeName.ToUpper().Equals("ACRORD32.EXE")
                            && zaction.ModifierKeys == ModifierKeys.Control
                            && zaction.Text.Equals("c"))
                        {
                            string ClipboardData = zaction.ClipboardData;
                            if (firstPDFAction)
                            {
                                if (zaction.ActivityElement != null && zaction.ActivityElement.TopLevelElement != null)
                                {
                                    zaction.TopLevelWindowHandle = zaction.ActivityElement.TopLevelElement.WindowHandle;
                                }
                                string cmdLinePath = getCommandLineArgs(zaction.TopLevelWindowHandle);
                                                                var splittedPath = cmdLinePath.Split('"');
                                string pdfFilePath = string.Empty;
                                if (splittedPath.Length > 2)
                                {
                                                                        pdfFilePath = splittedPath[splittedPath.Length - 2];
                                    pdfFilePath.Trim('"');
                                }
                                                                                                pdfToText.InputFilePath = pdfFilePath;
                                pdfToText.Invoke(null, null);
                                pdftextValue = pdfToText.OutputString;
                                pdfToText.OutputString = "";
                                pdfToText.InputFilePath.DymanicKey = ZappyExecutionContext.GetKey(openFileDialogAction.SelfGuid,
                                nameof(openFileDialogAction.FilePath));
                                textNormalization.TextValues = pdftextValue;
                                textNormalization.Invoke(null, null);
                                pdftextValue = textNormalization.NoramalizeText;
                                                                textNormalization.NoramalizeText = "";
                                textNormalization.TextValues = new DynamicProperty<string>();
                                textNormalization.TextValues.DymanicKey = ZappyExecutionContext.GetKey(pdfToText.SelfGuid,
                                nameof(pdfToText.OutputString));
                                copyList.Add(openFileDialogAction);
                                copyList.Add(pdfToText);
                                copyList.Add(textNormalization);
                                firstPDFAction = false;
                            }

                            StringDataSearch stringDataSearch = new StringDataSearch();
                                                                                    stringDataSearch.SourceText = new DynamicProperty<string>();
                            stringDataSearch.SourceText.DymanicKey = ZappyExecutionContext.GetKey(textNormalization.SelfGuid,
                                nameof(textNormalization.NoramalizeText));
                                                        stringDataSearch.SearchText = generateSearchStringAction(pdftextValue, ClipboardData);
                            stringDataSearch.PauseTimeAfterAction = 500;
                            stringDataSearch.SaveResultToClipboard = true;
                            stringDataSearch.DisplayName = "Get " + stringDataSearch.SearchText;
                                                                                                                                            copyList.Add(stringDataSearch);
                                                    }
                        else
                        {
                            copyList.Add(action);
                        }
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                        copyList.Add(action);
                    }
                        
                }
                else if (action is MouseAction maction && maction.ExeName.ToUpper().Equals("ACRORD32.EXE"))
                {
                                      if(RecordFirstAdobeMouseAction)
                    {
                        maction.ContinueOnError = true;
                        copyList.Add(action);
                        RecordFirstAdobeMouseAction = false;
                    }
                }                
                else
                {
                    copyList.Add(action);
                }
            }
            return copyList;
        }


                private string generateSearchStringAction(string PDFText, string ClipboardData)
        {
            string[] StringArray = PDFText.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < StringArray.Length; i++)
            {
                int indexMatch = StringArray[i].IndexOf(ClipboardData);
                if (indexMatch >= 0)
                {
                    return StringArray[i].Substring(0, indexMatch);
                                    }
            }
            return "";
        }
            


            internal List<IZappyAction> removeExcelEmptyActionsAndAddSetFocus(
            List<IZappyAction> recordedTaskActivities)
        {
            bool recognisedLoop = false;
            List<IZappyAction> copyList = new List<IZappyAction>();
            ExcelSendKeysAction firstExcelAction = null;

            foreach (var action in recordedTaskActivities)
            {
                if (action is ExcelSendKeysAction eaction)
                {
                    if (firstExcelAction == null)
                        firstExcelAction = eaction;
                    if (string.IsNullOrEmpty(eaction.Text) && string.IsNullOrEmpty(eaction.Formula))
                    {
                                            }
                    else
                    {
                        copyList.Add(action);
                    }
                }
                else
                {
                    copyList.Add(action);
                }
            }

            if (firstExcelAction != null)
            {
                ExcelCellInfo focusedCellInfo = ExcelCommunicator.Instance.GetFocussedElement() as ExcelCellInfo;
                if (focusedCellInfo != null && firstExcelAction.EqualSheets(focusedCellInfo)
                                            && firstExcelAction.ColumnIndex == focusedCellInfo.ColumnIndex
                                            && focusedCellInfo.RowIndex == (firstExcelAction.RowIndex + 1))
                {
                                        recognisedLoop = true;
                    ExcelSetFocusedCell excelSetFocusedCell = new ExcelSetFocusedCell();
                    excelSetFocusedCell.ExcelCellInfo = focusedCellInfo;
                    excelSetFocusedCell.RowOffset = 1;
                    copyList.Add(excelSetFocusedCell);
                }
            }

            if (recognisedLoop)
            {
                foreach (var action in copyList)
                {
                    if (action is ExcelSendKeysAction eaction)
                    {
                        eaction.UseFixedRow = false;
                    }
                }
            }
            return copyList;
        }


        internal void useChromeXPath(List<IZappyAction> recordedTaskActivities)
        {
            foreach (var action in recordedTaskActivities)
            {
                if (action is ChromeAction ca)
                    ca.TargetToSend = ChromeAction.ChromeTarget.Xpath;
            }
        }

        internal void insertPauseChromeActions(List<IZappyAction> recordedTaskActivities)
        {
            foreach (var action in recordedTaskActivities)
            {
                if (action is ChromeAction ca)
                    ca.PauseTimeAfterAction = 1000;
            }
        }

        internal static ZappyTask excelLoopFromFocusedRowToLastRow
            (ZappyTask uitaskSelectedTasks, StartNodeAction startNodeAction, ForLoopStartAction forLoopStartAction)
        {
            ExcelSendKeysAction excelSendKeysAction = null;
                        foreach (var action in uitaskSelectedTasks.ExecuteActivities.Activities)
            {
                if (action is ExcelSendKeysAction eaction)
                {
                    if (excelSendKeysAction == null)
                        excelSendKeysAction = eaction;
                    eaction.UseFixedRow = true;
                    eaction.RowIndex = new DynamicProperty<int>();
                    eaction.RowIndex.DymanicKey = ZappyExecutionContext.GetKey(forLoopStartAction.SelfGuid,
                        nameof(forLoopStartAction.LoopIntValue));
                }
            }
            if (excelSendKeysAction != null)
            {
                ExcelGetLastRowColumn excelGetLastRowColumn = new ExcelGetLastRowColumn();
                excelGetLastRowColumn.SheetName = excelSendKeysAction.SheetName;
                excelGetLastRowColumn.WorkbookName = excelSendKeysAction.WorkbookName;
                                ExcelGetFocusedCell excelGetFocusedCell = new ExcelGetFocusedCell();
                excelGetFocusedCell.ExcelCellInfo = new ExcelCellInfo(0, 0,
                    new ExcelWorksheetInfo(excelSendKeysAction.SheetName, excelSendKeysAction.WorkbookName));
                forLoopStartAction.InitialValue = new DynamicProperty<int>();
                forLoopStartAction.InitialValue.DymanicKey = ZappyExecutionContext.GetKey(excelGetFocusedCell.SelfGuid, nameof(excelGetFocusedCell.FocusedRow));
                forLoopStartAction.FinalValue = new DynamicProperty<int>();
                forLoopStartAction.FinalValue.DymanicKey = ZappyExecutionContext.GetKey(excelGetLastRowColumn.SelfGuid, nameof(excelGetLastRowColumn.lastUsedRow));
                startNodeAction.NextGuid = excelGetFocusedCell.SelfGuid;
                uitaskSelectedTasks.ExecuteActivities.Activities.Insert(uitaskSelectedTasks.ExecuteActivities.Activities.IndexOf(startNodeAction) + 1, excelGetFocusedCell);
                uitaskSelectedTasks.ExecuteActivities.Activities.Insert(uitaskSelectedTasks.ExecuteActivities.Activities.IndexOf(startNodeAction) + 2, excelGetLastRowColumn);
                uitaskSelectedTasks.ExecuteActivities.Activities.Insert(uitaskSelectedTasks.ExecuteActivities.Activities.IndexOf(startNodeAction) + 3, forLoopStartAction);
            }
            else
            {
                uitaskSelectedTasks = null;
                            }
            return uitaskSelectedTasks;
        }
    }
}
