using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace ZappyMessages.ExcelMessages
{
    /// <summary>
    /// Class for Excel cell info.
    /// </summary>
    [Serializable]
    public class ExcelCellInfo : ExcelElementInfo
    {
        public ExcelCellInfo()
        {

            SheetName = "";
            WorkbookName = "";
        }
        /// <summary>
        /// Creates a new ExcelCellInfo with the given info.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="parent">The parent worksheet.</param>
        public ExcelCellInfo(int rowIndex, int columnIndex, ExcelWorksheetInfo parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Parent = parent;
            SheetName = parent.SheetName;
            WorkbookName = parent.WorkbookName;
        }

        /// <summary>
        /// Gets or sets the row index of the cell.
        /// </summary>
        public int RowIndex { get; set; }
        /// <summary>
        /// Gets or sets the column index of the cell.
        /// </summary>
        public int ColumnIndex { get; set; }

        public ExcelRangeInfo ExcelRange { get; set; }
        /// <summary>
        /// Gets or sets the parent worksheet of the cell.
        /// </summary>
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        [Browsable(false)]
        [XmlIgnore]
        [JsonIgnore]
        public ExcelWorksheetInfo Parent { get; set; }

        public string SheetName { get; set; }
        public string WorkbookName { get; set; }
        #region Object Overrides

        // Helpful in debugging - Displays in UI
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}, {2}]", SheetName, RowIndex, ColumnIndex);
        }

        // Needed to find out if two objects are same or not.
        public override bool Equals(object obj)
        {
            ExcelCellInfo other = obj as ExcelCellInfo;
            if (other != null)
            {
                return RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex
                                                  && SheetName == other.SheetName && WorkbookName == other.WorkbookName;
            }

            return false;
        }

        public bool EqualSheets(object obj)
        {
            ExcelCellInfo other = obj as ExcelCellInfo;
            if (other != null)
            {
                return SheetName == other.SheetName && WorkbookName == other.WorkbookName;
            }

            return false;
        }

        // Good practice to override this when overriding Equals.
        public override int GetHashCode()
        {
            return RowIndex.GetHashCode() ^ ColumnIndex.GetHashCode() ^ SheetName.GetHashCode();
        }

        #endregion
    }
}