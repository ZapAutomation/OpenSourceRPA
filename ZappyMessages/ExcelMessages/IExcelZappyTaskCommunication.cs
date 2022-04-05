namespace ZappyMessages.ExcelMessages
{
    /// <summary>
    /// This interface is used by the ExcelExtension
    /// to communicate with the ExcelAddin (loaded in the Excel process) via .NET Remoting.
    /// </summary>
    public interface IExcelZappyTaskCommunication
    {
        /// <summary>
        /// Gets an Excel UI element at the given screen location. 
        /// </summary>
        /// <param name="x">The x-coordinate of the location.</param>
        /// <param name="y">The y-coordinate of the location.</param>
        /// <returns>The Excel UI element info.</returns>
        ExcelElementInfo GetElementFromPoint(int x, int y);

        /// <summary>
        /// Gets the Excel UI element current under keyboard focus.
        /// </summary>
        /// <returns>The Excel UI element info.</returns>
        ExcelElementInfo GetFocussedElement();

        /// <summary>
        /// Gets the bounding rectangle of the Excel cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns>The bounding rectangle as an array.
        /// The values are relative to the parent window and in Points (instead of Pixels).</returns>
        double[] GetBoundingRectangle(ExcelCellInfo cellInfo);

        /// <summary>
        /// Sets focus on a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        void SetFocus(ExcelCellInfo cellInfo);

        /// <summary>
        /// Scrolls a given cell into view.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        void ScrollIntoView(ExcelCellInfo cellInfo);

        /// <summary>
        /// Gets the property of a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        object GetCellProperty(ExcelCellInfo cellInfo, string propertyName);

        /// <summary>
        /// Sets the property of a given cell.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        bool SetCellProperty(ExcelCellInfo cellInfo, string propertyName, object propertyValue);
        object RunExcelCustomAction(ExcelCellInfo cellInfo, string propertyName, object propertyValue);

        void ActivateWorksheet(ExcelCellInfo cellInfo);

        void SaveWorkbook(ExcelCellInfo cellInfo);
        void SaveWorkbookAs(ExcelCellInfo cellInfo);

        bool SortExcelRange(ExcelCellInfo cellInfo, string sortingOrder, int columnToSort);
    }
}