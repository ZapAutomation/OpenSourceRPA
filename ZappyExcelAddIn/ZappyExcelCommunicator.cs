using Microsoft.Office.Interop.Excel;
using System;
using System.Drawing;
using System.ServiceModel;
using System.Threading;
using ZappyMessages;
using ZappyMessages.ExcelMessages;
using ZappyMessages.Helpers;
using ZappyMessages.Logger;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace ZappyExcelAddIn
{
    /// <summary>
    /// Implementation of IExcelUITestCommunication which provides information
    /// to the ExcelExtension (loaded in the Coded UI Test process) from the
    /// ExcelAddin (loaded in the Excel process) via .NET Remoting.
    /// </summary>
    internal class ZappyExcelCommunicator : IExcelZappyTaskCommunication
    {
        public const string StringArrayDelemiter = "^^";

        private PubSubClient _Client;
        private Worksheet _PreviousWorksheet;

        private int excelCommunicatorCallCount = 0;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ZappyExcelCommunicator()
        {
            if (ThisAddIn.Instance == null || ThisAddIn.Instance.Application == null)
            {
                throw new InvalidOperationException();
            }

            ConsoleLogger.Info("Plugin Loaded");
            EndpointAddress _RemoteAddress = new EndpointAddress(ZappyMessagingConstants.EndpointLocationZappyService);
            _Client = new PubSubClient("PlaybackHelper", _RemoteAddress, new int[] { PubSubTopicRegister.ZappyExcelRequest, PubSubTopicRegister.ZappyPlaybackHelper2ExcelRequest });
            _Client.DataPublished += _Client_DataPublished;
            // Cache the Excel application of this addin.
            this.application = ThisAddIn.Instance.Application;
            //this.application.SheetActivate += Application_SheetActivate;
            //_PreviousWorksheet = ((Worksheet)application.ActiveSheet);
            //_PreviousWorksheet.SelectionChange += activeSheet_SelectionChange;
        }

        private void Application_SheetActivate(object Sh)
        {
            if (_PreviousWorksheet != null)
                _PreviousWorksheet.SelectionChange -= activeSheet_SelectionChange;
            _PreviousWorksheet = Sh as Worksheet;
            _PreviousWorksheet.SelectionChange += activeSheet_SelectionChange;
        }

        void activeSheet_SelectionChange(Range Target)
        {
            ConsoleLogger.Info(GetFocussedElement().ToString());
        }

        private void _Client_DataPublished(PubSubClient client, int arg1, string arg2)
        {
            //arg2 = StringCipher.Decrypt(arg2, ZappyMessagingConstants.MessageKey);

            //{message:"GetElementFromPoint"; param1:"156"; param2:"150"}
            //
            Tuple<int, ExcelRequest, string> _Request = ZappySerializer.DeserializeObject<Tuple<int, ExcelRequest, string>>(arg2);
            string _result = null;

        REDO:
            bool _RedoRequired = false;

            try
            {

                switch (_Request.Item2)
                {
                    case ExcelRequest.GetElementFromPoint:
                        System.Drawing.Point _pt = ZappySerializer.DeserializeObject<System.Drawing.Point>(_Request.Item3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, ExcelElementInfo>(_Request.Item1, GetElementFromPoint(_pt.X, _pt.Y)));
                        break;
                    case ExcelRequest.GetFocussedElement:
                        _result = ZappySerializer.SerializeObject(new Tuple<int, ExcelElementInfo>(_Request.Item1, GetFocussedElement()));
                        break;
                    case ExcelRequest.GetBoundingRectangle:
                        ExcelCellInfo cellInfo = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, double[]>(_Request.Item1, GetBoundingRectangle(cellInfo)));
                        break;
                    case ExcelRequest.GetCellProperty:
                        string[] _Parts = _Request.Item3.Split(new string[] { StringArrayDelemiter }, StringSplitOptions.RemoveEmptyEntries);
                        ExcelCellInfo cellInfo1 = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Parts[0]);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, object>(_Request.Item1, GetCellProperty(cellInfo1, _Parts[1])));
                        break;
                    case ExcelRequest.SetFocus:
                        ExcelCellInfo cellInfo2 = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        SetFocus(cellInfo2);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));

                        break;
                    case ExcelRequest.ScrollIntoView:
                        ExcelCellInfo cellInfo3 = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        ScrollIntoView(cellInfo3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    case ExcelRequest.SetCellProperty:
                        Tuple<ExcelCellInfo, string, object> _PropertyRequest = ZappySerializer.DeserializeObject<Tuple<ExcelCellInfo, string, object>>(_Request.Item3);
                        SetCellProperty(_PropertyRequest.Item1, _PropertyRequest.Item2, _PropertyRequest.Item3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    case ExcelRequest.RunExcelCustomAction:
                        Tuple<ExcelCellInfo, string, object> _PropertyRequest2 = ZappySerializer.DeserializeObject<Tuple<ExcelCellInfo, string, object>>(_Request.Item3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, object>(_Request.Item1, RunExcelCustomAction(_PropertyRequest2.Item1, _PropertyRequest2.Item2, _PropertyRequest2.Item3)));
                        break;
                    case ExcelRequest.SortRange:
                        Tuple<ExcelCellInfo, string, int> _PropertyRequestsort = ZappySerializer.DeserializeObject<Tuple<ExcelCellInfo, string, int>>(_Request.Item3);
                        SortExcelRange(_PropertyRequestsort.Item1, _PropertyRequestsort.Item2, _PropertyRequestsort.Item3);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    //case ExcelRequest.ExcelCustomActivities:
                    //    Tuple<int, string, object> _PropertyRequest3 = ZappySerializer.DeserializeObject<Tuple<int, string, object>>(_Request.Item3);
                    //    _result = ZappySerializer.SerializeObject(new Tuple<int, object>(_Request.Item1, ExcelCustomActivities(_PropertyRequest3.Item1, _PropertyRequest3.Item2, _PropertyRequest3.Item3)));
                    //    break;
                    case ExcelRequest.ActivateWorksheet:
                        ExcelCellInfo cellInfow = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        ActivateWorksheet(cellInfow);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    case ExcelRequest.SaveWorkbook:
                        ExcelCellInfo cellInfowsave = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        SaveWorkbook(cellInfowsave);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    case ExcelRequest.SaveWorkbookAs:
                        ExcelCellInfo cellInfowsaveas = ZappySerializer.DeserializeObject<ExcelCellInfo>(_Request.Item3);
                        SaveWorkbookAs(cellInfowsaveas);
                        _result = ZappySerializer.SerializeObject(new Tuple<int, string>(_Request.Item1, "completed"));
                        break;
                    default:
                        break;
                }
            }
            catch (System.Runtime.InteropServices.COMException cex)
            {
                if ((uint)cex.ErrorCode == 0x800AC472)
                    _RedoRequired = true;
                else
                {
                    ConsoleLogger.Error(cex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
            }

            if (_RedoRequired)
            {
                Thread.Sleep(100);
                goto REDO;

            }

            if (!string.IsNullOrEmpty(_result))
                _Client.Publish(GetResponseChannel(arg1), _result);

            excelCommunicatorCallCount = excelCommunicatorCallCount + 1;
            if (excelCommunicatorCallCount > 1000)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                excelCommunicatorCallCount = 0;
            }

        }

        public object RunExcelCustomAction(ExcelCellInfo cellInfo, string propertyName, object propertyValue)
        {
            //TODO: Move this to excel Get Property - everything except find text in Excel
            if (propertyName == ExcelCustomPropertyNames.FindSheetName)
            {
                cellInfo.SheetName = (this.application.Workbooks[cellInfo.WorkbookName].Worksheets[1] as Worksheet).Name;
                ConsoleLogger.Info(cellInfo.SheetName);
                return cellInfo;
            }
            if (propertyName == ExcelCustomPropertyNames.CloseWorkbook)
            {
                Workbook wb = this.application.Workbooks[cellInfo.WorkbookName];
                wb.Close();
                return true;
            }
            Worksheet ws = GetWorksheet(cellInfo);
            if (propertyName == ExcelCustomPropertyNames.FitExcelWorkSheet)
            {
                ws.Columns.AutoFit();
                return cellInfo;
            }          
            //return new ExcelCellInfo(cell.Row, cell.Column, new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name));           
            if (propertyName == ExcelCustomPropertyNames.FindTextInSheet)
            {
                Range cells = ws.Cells;
                Range match = cells.Find(propertyValue);
                if (match != null)
                    return new ExcelCellInfo(match.Row, match.Column, new ExcelWorksheetInfo(cellInfo.SheetName, cellInfo.WorkbookName));
                //string matchAdd = match != null ? match.Address : null;
                return null;
            }
            if (propertyName == ExcelCustomPropertyNames.RunMacro)
            {
                this.application.Run(propertyValue);
                //string matchAdd = match != null ? match.Address : null;
                return new ExcelCellInfo(1, 1, new ExcelWorksheetInfo("Macro Run Completed", "")); ;
            }
            else if (propertyName == ExcelCustomPropertyNames.FindLastCellRow)
            {
                int lastUsedRow = ws.Cells.Find("*", System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    XlSearchOrder.xlByRows, XlSearchDirection.xlPrevious,
                    false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

                int lastUsedColumn = ws.Cells.Find("*", System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    XlSearchOrder.xlByColumns, XlSearchDirection.xlPrevious,
                    false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;
                return new
                    ExcelCellInfo(lastUsedRow, lastUsedColumn, new ExcelWorksheetInfo(cellInfo.SheetName, cellInfo.WorkbookName));
            }
            return null;
        }

        //public object ExcelCustomActivities(int objectTosearch, string propertyName, object propertyValue)
        //{

        //    //return new ExcelCellInfo(cell.Row, cell.Column, new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name));
        //    return null;
        //}


        public void ActivateWorksheet(ExcelCellInfo cellInfo)
        {
            Worksheet ws = GetWorksheet(cellInfo);
            ws.Activate();
        }

        public void SaveWorkbook(ExcelCellInfo cellInfo)
        {
            Workbook ws = this.application.Workbooks[cellInfo.WorkbookName];
            ws.Save();
        }

        public void SaveWorkbookAs(ExcelCellInfo cellInfo)
        {
            Workbook ws = this.application.Workbooks[cellInfo.WorkbookName];
            ws.SaveAs(cellInfo.SheetName);
        }

        int GetResponseChannel(int RequestChannel)
        {
            return RequestChannel + 1;
        }

        #region IExcelUITestCommunication Implementation

        /// <summary>
        /// Gets an Excel UI element at the given screen location. 
        /// </summary>
        /// <param name="x">The x-coordinate of the location.</param>
        /// <param name="y">The y-coordinate of the location.</param>
        /// <returns>The Excel UI element info.</returns>
        public ExcelElementInfo GetElementFromPoint(int x, int y)
        {
            ExcelElementInfo excelElementInfo = null;
            // Use Excel's Object Model to get the required.
            Worksheet ws = this.application.ActiveSheet as Worksheet;
            Workbook excelWorkbook = this.application.ActiveWorkbook as Workbook;
            //excelWorkbook.Name;
            if (ws != null && this.application.ActiveWindow != null)
            {
                //string workSheetNameAndWindowName = ws.Name + "^" + excelWorkbook.Name;
                Range cellAtPoint = this.application.ActiveWindow.RangeFromPoint(x, y) as Range;

                if (cellAtPoint != null)
                {
                    excelElementInfo = new ExcelCellInfo(cellAtPoint.Row, cellAtPoint.Column, new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name));
                }
                else
                {
                    excelElementInfo = new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name);
                }
            }
#if DEBUG
            ConsoleLogger.Info("SheetName " + ws.Name + " WorkbookName " + excelWorkbook.Name);
#endif
            return excelElementInfo;
        }

        /// <summary>
        /// Gets the Excel UI element current under keyboard focus.
        /// </summary>
        /// <returns>The Excel UI element info.</returns>
        public ExcelElementInfo GetFocussedElement()
        {
            // Use Excel's Object Model to get the required.
            Worksheet ws = this.application.ActiveSheet as Worksheet;
            Workbook excelWorkbook = this.application.ActiveWorkbook as Workbook;

            if (ws != null)
            {
                //string workSheetNameAndWindowName = ws.Name + "^" + excelWorkbook.Name;

                Range cell = this.application.ActiveCell;
                if (cell != null)
                {
                    return new ExcelCellInfo(cell.Row, cell.Column, new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name));
                }
                else
                {
                    return new ExcelWorksheetInfo(ws.Name, excelWorkbook.Name);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the bounding rectangle of the Excel cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns>The bounding rectangle as an array.
        /// The values are relative to the parent window and in Points (instead of Pixels).</returns>
        public double[] GetBoundingRectangle(ExcelCellInfo cellInfo)
        {
            // Use Excel's Object Model to get the required.
            double[] rect = new double[4];
            rect[0] = rect[1] = rect[2] = rect[3] = -1;

            Range cell = GetCell(cellInfo);
            if (cell != null)
            {
                const double xOffset = 25.6; // The constant width of row name column.
                const double yOffset = 36.0; // The constant height of column name row.
                rect[0] = (double)cell.Left + xOffset;
                rect[1] = (double)cell.Top + yOffset;
                rect[2] = (double)cell.Width;
                rect[3] = (double)cell.Height;

                Range visibleRange = this.application.ActiveWindow.VisibleRange;
                if (visibleRange != null)
                {
                    rect[0] -= (double)visibleRange.Left;
                    rect[1] -= (double)visibleRange.Top;
                }
            }

            return rect;
        }

        /// <summary>
        /// Sets focus on a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        public void SetFocus(ExcelCellInfo cellInfo)
        {
            // Use Excel's Object Model to get the required.
            Worksheet ws = GetWorksheet(cellInfo);
            if (ws != null)
            {
                // There could be some other cell under editing. Exit that mode.
                ExitPreviousEditing(ws);
                //TODO - fix causes blur with multipe sheets in excel
                ws.Activate();
                while (ws.Rows[cellInfo.RowIndex].Hidden)
                    cellInfo.RowIndex++;
                Range cell = GetCell(cellInfo);

                if (cell != null)
                {
                    cell.Activate();
                }
            }
        }

        /// <summary>
        /// Scrolls a given cell into view.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        public void ScrollIntoView(ExcelCellInfo cellInfo)
        {
            // Use Excel's Object Model to get the required.
            Worksheet ws = GetWorksheet(cellInfo);
            if (ws != null)
            {
                ws.Activate();
                double[] rect = this.GetBoundingRectangle(cellInfo);
                this.application.ActiveWindow.ScrollIntoView((int)rect[0], (int)rect[1], (int)rect[2], (int)rect[3]);
            }
        }

        /// <summary>
        /// Gets the property of a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public object GetCellProperty(ExcelCellInfo cellInfo, string propertyName)
        {
            // Use Excel's Object Model to get the required.
            Range cell = GetCell(cellInfo);
            if (cell == null)
            {
                throw new InvalidOperationException();
            }

            switch (propertyName)
            {
                case PropertyNames.Enabled:
                    return true; // TODO - Needed to add support for "locked" cells.

                case PropertyNames.Value:
                    return cell.Value ?? string.Empty;

                case PropertyNames.Text:
                    return cell.Text ?? string.Empty;

                case PropertyNames.WidthInChars:
                    return cell.ColumnWidth;
                case PropertyNames.GetPictureInClipboard:
                    return cell.CopyPicture(XlPictureAppearance.xlScreen, XlCopyPictureFormat.xlPicture);
                case PropertyNames.HeightInPoints:
                    return cell.RowHeight;
                case PropertyNames.CellBackgroundColor:
                    //if(XlRgbColor.TryParse(propertyValue as string, true, out XlRgbColor color))
                    return cell.Interior.Color;
                case PropertyNames.Formula:
                    if (cell.HasFormula)
                        return cell.Formula;
                    return string.Empty;
                case PropertyNames.WrapText:
                    return cell.WrapText;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the property of a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public bool SetCellProperty(ExcelCellInfo cellInfo, string propertyName, object propertyValue)
        {
            // Use Excel's Object Model to get the required.
            Range cell = GetCell(cellInfo);
            if (cell == null)
            {
                throw new InvalidOperationException();
            }

            switch (propertyName)
            {
                case PropertyNames.Value:
                    cell.Value = propertyValue;
                    break;
                case PropertyNames.CellBackgroundColor:
                    //if(XlRgbColor.TryParse(propertyValue as string, true, out XlRgbColor color))
                    if (int.TryParse(propertyValue.ToString(), out int argbVal))
                    {
                        cell.Interior.Color = Color.FromArgb(argbVal);
                    }
                    else
                    {
                        cell.Interior.Color = Color.FromName(propertyValue.ToString());
                    }
                    break;
                case PropertyNames.WidthInChars:
                    cell.ColumnWidth = propertyValue;
                    break;

                case PropertyNames.HeightInPoints:
                    cell.RowHeight = propertyValue;
                    break;

                case PropertyNames.Formula:
                    cell.Formula = propertyValue;
                    break;

                case PropertyNames.WrapText:
                    cell.WrapText = propertyValue;
                    break;
                case PropertyNames.Text:
                    //int _RowIndex = application.ActiveCell.Row;
                    //while ((application.ActiveSheet as Worksheet).Rows[++_RowIndex].Hidden)
                    //    ;
                    //(application.ActiveSheet as Worksheet).Rows[_RowIndex].Activate();
                    //(new Range("A" + application.ActiveCell.Row + 1)).Select();
                    //return;
                    application.WindowState = XlWindowState.xlMinimized; // -4140
                    application.WindowState = XlWindowState.xlMaximized; // -4137
                    application.ActiveWindow.Activate();
                    application.ScreenUpdating = false;
                    SetFocus(cellInfo);
                    application.Application.SendKeys(propertyValue, true);
                    application.ScreenUpdating = true;
                    break;
                case PropertyNames.CellNumberFormat:
                    cell.NumberFormat = propertyValue;
                    break;
                case PropertyNames.DeleteExcelRow:
                    cell.EntireRow.Delete(Type.Missing);
                    break;
                //case PropertyNames.DeleteCell:
                //    cell.Value = 00 ;
                //    //cell.
                //    break;

                default:
                    throw new NotSupportedException();
            }

            return true;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gets the Range (cell) from the cell info.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns>The Range.</returns>
        private Range GetCell(ExcelCellInfo cellInfo)
        {
            Range cell = null;
            Worksheet ws = GetWorksheet(cellInfo);
            if (ws != null)
            {
                if (cellInfo.ExcelRange != null && cellInfo.ExcelRange.UseRange)
                {
                    //Excel.Range newRng = excelApp.get_Range(ws.Cells[1, 1], ws.Cells[4, 5]);
                    cell = this.application.Range
                        [ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex],
                        ws.Cells[cellInfo.ExcelRange.RangeEndRowIndex, cellInfo.ExcelRange.RangeEndColumnIndex]];
                    //ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex] as Range;
                }
                else
                    cell = ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex] as Range;
            }

            return cell;
        }


        public bool SortExcelRange(ExcelCellInfo cellInfo, string sortingOrder, int columnToSort)
        {
            Range rangeCells = null;
            Worksheet ws = GetWorksheet(cellInfo);
            if (ws != null)
            {
                if (cellInfo.ExcelRange != null && cellInfo.ExcelRange.UseRange)
                {
                    //Excel.Range newRng = excelApp.get_Range(ws.Cells[1, 1], ws.Cells[4, 5]);
                    rangeCells = this.application.Range
                        [ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex],
                        ws.Cells[cellInfo.ExcelRange.RangeEndRowIndex, cellInfo.ExcelRange.RangeEndColumnIndex]];
                    //ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex] as Range;
                }
                else
                    rangeCells = ws.Cells[cellInfo.RowIndex, cellInfo.ColumnIndex] as Range;
            }
            XlSortOrder order;
            if (sortingOrder == ExcelSortOrder.Ascending.ToString())
            {
                order = XlSortOrder.xlAscending;
            }
            else
            {
                order = XlSortOrder.xlDescending;
            }
            rangeCells.Sort(rangeCells.Columns[columnToSort], order);
            return true;
        }

        /// <summary>
        /// Gets the Worksheet from the worksheet info.
        /// </summary>
        /// <param name="sheetInfo">The worksheet info.</param>
        /// <returns>The Worksheet.</returns>
        private Worksheet GetWorksheet(ExcelCellInfo sheetInfo)
        {
            //string[] sheets = sheetInfo.SheetName.Split('^');
            //this.application.Workbooks[sheetInfo.WorkbookName].ChangeFileAccess(XlFileAccess.xlReadWrite);
            //this.application.Workbooks[sheetInfo.WorkbookName].RefreshAll();
            //this.application.Workbooks[sheetInfo.WorkbookName].Unprotect();
            try
            {
                return this.application.Workbooks[sheetInfo.WorkbookName].Worksheets[sheetInfo.SheetName] as Worksheet;
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Exit editing mode for any previous cell.
        /// </summary>
        /// <param name="ws">The worksheet.</param>
        private void ExitPreviousEditing(Worksheet ws)
        {
            this.application.ActiveCell.Next.Activate();
            this.application.ActiveCell.Previous.Activate();
            return;
            // TODO - This is hack. Need to find better way to do this.
            // The current logic is to activate another worksheet which fails
            // if there is only one worksheet in the workbook.
            Worksheet otherSheet = ws.Next as Worksheet;
            if (otherSheet == null)
            {
                otherSheet = ws.Previous as Worksheet;
            }

            if (otherSheet != null)
            {
                otherSheet.Activate();
            }
        }

        /// <summary>
        /// The Excel application.
        /// </summary>
        private Application application;

        #endregion
    }

}
