using System;
using System.ComponentModel;
using System.Globalization;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets Actions of Excel Send Keys")]
    public class ExcelSendKeysAction : SendKeysAction, IExcelAction
    {
        public ExcelSendKeysAction()
        {
            UseFixedRow = true;
        }
                
        [Description("RowIndex of excel")]
        [Category("Input")]
        public DynamicProperty<int> RowIndex { get; set; }

        [Description("Column index of excel")]
        [Category("Input")]
        public DynamicProperty<int> ColumnIndex { get; set; }

        [Description("Sheet name of excel")]
        [Category("Input")]
        public DynamicProperty<string> SheetName { get; set; }

        [Description("WorkbookName")]
        [Category("Input")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Description("Formula for to get the Excel Send key action")]
        [Category("Optional")]
        public DynamicTextProperty Formula { get; set; }

        [Description("Gets Excel Property Type")]
        [Category("Optional")]
        public PropertyNames.PropertyNameEnum ExcelPropertyType { get; set; }

                        [Category("Optional")]
        public bool UseFixedRow { get; set; }



                                                public ExcelSendKeysAction(SendKeysAction action) : this()
        {
            this.SelfGuid = action.SelfGuid;
            this.Id = action.Id;
            this.NextGuid = action.NextGuid;
            this.ExeName = action.ExeName;
            this.Timestamp = action.Timestamp;
            this.ZappyWindowTitle = action.ZappyWindowTitle;
            this.ModifierKeys = action.ModifierKeys;
            this.Text = action.Text;

            ExcelPropertyType = PropertyNames.PropertyNameEnum.Value;

            if (action.ActivityElement.TopLevelElement != null)
            {
                TopLevelWindowHandle = action.ActivityElement.TopLevelElement.WindowHandle;
                ZappyWindowTitle = action.ActivityElement.TopLevelElement.Name;
            }

            ElementWindowHandle = action.ActivityElement.WindowHandle;
            ExcelCellInfo ExcelCellInfo = (action.ActivityElement as ExcelCellElement)?.CellInfo;
                        if (ExcelCellInfo != null)
            {
                this.RowIndex = ExcelCellInfo.RowIndex;                  this.ColumnIndex = ExcelCellInfo.ColumnIndex;                  this.WorkbookName = ExcelCellInfo.WorkbookName;
                this.SheetName = ExcelCellInfo.SheetName;
            }
        }

        
                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            int _RowIndex = this.RowIndex;
            string text = Text.Value;
            if (!UseFixedRow)
            {
                int i = 0;

                        Retry:
                ExcelCellInfo focusedExcelCellInfo = ExcelCommunicator.Instance.GetFocussedElement() as ExcelCellInfo;
                if (focusedExcelCellInfo != null)
                {
                    if (this.EqualSheets(focusedExcelCellInfo))
                        _RowIndex = focusedExcelCellInfo.RowIndex;
                    else
                    {
                                                ExcelCommunicator.Instance.
                            ActivateWorksheet(ExcelHelper.CreateExcelCellInfo(_RowIndex, ColumnIndex, SheetName, WorkbookName));
                        if (i < 1)
                        {
                            i++;
                            goto Retry;
                        }
                        throw new Exception
                            ("ERR: EX315 Unable to activate worksheet " + SheetName + ":" + WorkbookName);
                    }
                }
                else
                {
                    if (i < 5)
                    {
                        i++;
                        goto Retry;
                    }
                    throw new Exception("ERR: E345 Unable to get focused cell for loop iteration");
                }
            }

            ExcelCellInfo cellInfo = ExcelHelper.CreateExcelCellInfo(_RowIndex, ColumnIndex, SheetName, WorkbookName);

                                    string clipBoardChar = string.Empty;
            if (this.ModifierKeys == ModifierKeys.Control)
            {
                if (text.Equals("c"))
                {
                                        string copiedValue =
                        ExcelCommunicator.Instance.GetCellProperty(cellInfo, PropertyNames.PropertyNameEnum.Text.ToString()).ToString();
                    if (copiedValue != null) CommonProgram.SetTextInClipboard(copiedValue);
                }
                else if (text.Equals("v"))
                {
                    if (!CommonProgram.GetTextFromClipboard(out clipBoardChar))
                        throw new Exception("Unable to read clipboard contents!");
                                        ExcelCommunicator.Instance.SetCellProperty(cellInfo, ExcelPropertyType.ToString(), clipBoardChar);
                                    }
                else
                {
                    throw new Exception("Not Supported Command");
                }
            }
            else if (text.Contains(CrapyConstants.pasteChar))
            {
                if (!CommonProgram.GetTextFromClipboard(out clipBoardChar))
                    throw new Exception("Unable to read clipboard contents!");
                text = text.Replace(CrapyConstants.pasteChar, clipBoardChar);
                ExcelCommunicator.Instance.SetCellProperty(cellInfo, ExcelPropertyType.ToString(), text);
            }
            else
            {
                if (ExcelPropertyType == PropertyNames.PropertyNameEnum.Formula)
                {
                    ExcelCommunicator.Instance.SetCellProperty(cellInfo, ExcelPropertyType.ToString(), Formula.Value);
                }
                else
                {
                    ExcelCommunicator.Instance.SetCellProperty(cellInfo, ExcelPropertyType.ToString(), Text.Value);
                }
                                            }
        }


                                public void refreshAndgetCellValueAndFormula()
        {
            try
            {
                string _Text = Text;

                if (this.ModifierKeys == ModifierKeys.None)
                {
                    ExcelCellInfo cellInfo = ExcelHelper.CreateExcelCellInfo(RowIndex, ColumnIndex, SheetName, WorkbookName);

                    if (_Text.StartsWith("="))
                    {
                        this.Formula =
                            ExcelCommunicator.Instance.GetCellProperty(cellInfo,
                                PropertyNames.Formula) as string;
                        ExcelPropertyType = PropertyNames.PropertyNameEnum.Formula;
                    }
                    else
                    {
                        this.Text =
                            ExcelCommunicator.Instance.GetCellProperty(cellInfo,
                                PropertyNames.Text) as string;
                    }
                                                                            }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

                                                        public bool EqualSheets(ExcelCellInfo other)
        {
            if (other != null)
            {
                return SheetName == other.SheetName && WorkbookName == other.WorkbookName;
            }

            return false;
        }

                                                                public bool EqualCellInfo(ExcelSendKeysAction other)
        {
            if (other != null)
            {
                return RowIndex.Value == other.RowIndex.Value && ColumnIndex.Value == other.ColumnIndex.Value
                        && string.Equals(SheetName.Value, other.SheetName.Value) && string.Equals(WorkbookName.Value,
                                                                  other.WorkbookName.Value);
            }

            return false;
        }

                                                public string ToDisplayString()
        {
            if (UseFixedRow)
                return string.Format(CultureInfo.InvariantCulture, "{0}[{1}, {2}]", SheetName, RowIndex, ColumnIndex);
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}[{1}, {2}]", SheetName, "FocusedRow", ColumnIndex);

            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " SheetName:" + this.SheetName + " Excel File location:" + this.WorkbookName + " RowIndex: " + this.RowIndex + " ColumnIndex: " + ColumnIndex;
        }
    }
}
