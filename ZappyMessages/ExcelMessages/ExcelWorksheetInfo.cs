using System;

namespace ZappyMessages.ExcelMessages
{
    /// <summary>
    /// Class for Excel worksheet info.
    /// </summary>
    [Serializable]
    public class ExcelWorksheetInfo : ExcelElementInfo
    {
        public ExcelWorksheetInfo()
        {

        }
        /// <summary>
        /// Creates a new ExcelWorksheetInfo with the given worksheet name.
        /// </summary>
        /// <param name="sheetName">The name of the worksheet.</param>
        public ExcelWorksheetInfo(string sheetName, string workbookName)
        {
            if (sheetName == null) throw new ArgumentNullException("sheetName");
            SheetName = sheetName;
            WorkbookName = workbookName;
        }


        /// <summary>
        /// Gets or sets the name of the worksheet.
        /// </summary>
        public string SheetName { get; set; }
        public string WorkbookName { get; set; }

        #region Object Overrides

        // Helpful in debugging.
        public override string ToString()
        {
            return SheetName;
        }

        // Needed to find out if two objects are same or not.
        public override bool Equals(object obj)
        {
            ExcelWorksheetInfo other = obj as ExcelWorksheetInfo;
            if (other != null)
            {

                return string.Equals(SheetName + WorkbookName, other.SheetName + other.WorkbookName, StringComparison.Ordinal);
            }

            return false;
        }

        // Good practice to override this when overriding Equals.
        public override int GetHashCode()
        {
            return SheetName.GetHashCode();
        }

        #endregion
    }
}