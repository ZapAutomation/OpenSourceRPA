


using System;
using System.Runtime.InteropServices;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ExecuteTask.Helpers.Interface;
using ZappyMessages.ExcelMessages;

namespace Zappy.Plugins.Excel
{
                    [ComVisible(true)]
    public sealed class ExcelWorksheetElement : ExcelElement
    {
        #region Simple Properties & Methods

                                public override string ClassName
        {
            get { return "Excel.Sheet"; }
        }

                                public override string ControlTypeName
        {
            get
            {
                return ControlType.Table.Name;
            }
        }

                                public override string Name
        {
            get
            {
                return this.WorksheetInfo.SheetName;
            }
        }

        #endregion

        #region Override for Object Methods

                                                public override bool Equals(ITaskActivityElement element)
        {
            if (base.Equals(element))
            {
                ExcelWorksheetElement otherElement = element as ExcelWorksheetElement;
                if (otherElement != null)
                {
                    return object.Equals(this.WorksheetInfo, otherElement.WorksheetInfo);
                }
            }

            return false;
        }

                                                public override int GetHashCode()
        {
            return this.WorksheetInfo.GetHashCode();
        }

        #endregion

        #region Internals/Privates

                                internal override TaskActivityElement Parent
        {
            get
            {
                if (this.parent == null)
                {
                    this.parent = this.technologyManager.GetExcelElement(this.WindowHandle, null);
                }

                return this.parent;
            }
        }

                                                internal override System.Collections.IEnumerator GetChildren(AndCondition condition)
        {
            int row = 0, column = 0;
            string rowString = condition.GetPropertyValue(PropertyNames.RowIndex) as string;
            string columnString = condition.GetPropertyValue(PropertyNames.ColumnIndex) as string;
            if (int.TryParse(rowString, out row) &&
                int.TryParse(columnString, out column))
            {
                TaskActivityElement cellElement = this.technologyManager.GetExcelElement(this.WindowHandle,
                        new ExcelCellInfo(row, column, this.WorksheetInfo));
                return new TaskActivityElement[] { cellElement }.GetEnumerator();
            }

            return null;
        }

                                internal ExcelWorksheetInfo WorksheetInfo { get; private set; }

                                                        internal ExcelWorksheetElement(IntPtr windowHandle, ExcelWorksheetInfo worksheetInfo, ExcelTechnologyManager manager)
            : base(windowHandle, manager)
        {
            this.WorksheetInfo = worksheetInfo;
        }

        #endregion
    }

}
