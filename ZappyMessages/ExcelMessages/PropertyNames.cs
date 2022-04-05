namespace ZappyMessages.ExcelMessages
{
    /// <summary>
    /// Names of various properties of an Excel cell.
    /// </summary>
    public static class PropertyNames
    {
        public const string ControlType = "ControlType";
        public const string ClassName = "ClassName";
        public const string Name = "Name";
        public const string WorksheetName = "WorksheetName";
        public const string RowIndex = "RowIndex";
        public const string ColumnIndex = "ColumnIndex";

        public const string Enabled = "Enabled";
        public const string Value = "Value";
        public const string Text = "Text";
        public const string WidthInChars = "WidthInChars";
        public const string HeightInPoints = "HeightInPoints";
        public const string Formula = "Formula";
        public const string WrapText = "WrapText";
        public const string CellBackgroundColor = "CellBackgroundColor";
        public const string CellNumberFormat = "CellNumberFormat";
        public const string DeleteExcelRow = "DeleteExcelRow";
        public const string GetPictureInClipboard = "GetPictureInClipboard";
        //public const string DeleteCell= "DeleteCell";
        //Make sure do not change this
        public enum PropertyNameEnum
        {
            Value,
            WidthInChars,
            HeightInPoints,
            Formula,
            WrapText,
            Text,
            CellBackgroundColor,
            CellNumberFormat,
            DeleteExcelRow,
            GetPictureInClipboard
        }
    }
}