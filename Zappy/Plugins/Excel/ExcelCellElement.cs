


using System;
using System.Runtime.InteropServices;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using ZappyMessages.ExcelMessages;
using AndCondition = Zappy.ActionMap.HelperClasses.AndCondition;
using IQueryElement = Zappy.ActionMap.Query.IQueryElement;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;
using QueryElement = Zappy.ActionMap.Query.QueryElement;

namespace Zappy.Plugins.Excel
{
                    [ComVisible(true)]
    public sealed class ExcelCellElement : ExcelElement
    {
        #region Simple Properties & Methods

                                public override int ChildIndex
        {
            get { return this.CellInfo.RowIndex * this.CellInfo.ColumnIndex; }
        }

                                public override string ClassName
        {
            get { return "Excel.Cell"; }
        }

                                public override string ControlTypeName
        {
            get
            {
                return ControlType.Cell.Name;
            }
        }

                                public override bool IsLeafNode
        {
            get { return true; }
        }

                                public override string Name
        {
            get
            {
                return this.CellInfo.ToString();
            }
        }

                                public override object NativeElement
        {
                        get { return new object[] { this.WindowHandle, this.CellInfo }; }
        }

                                        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            left = top = width = height = -1;

            ExcelTechnologyUtilities.RECT windowRect;
            if (ExcelTechnologyUtilities.GetWindowRect(this.WindowHandle, out windowRect))
            {
                double[] cellRect = ExcelCommunicator.Instance.GetBoundingRectangle(this.CellInfo);

                                                                left = windowRect.left + ExcelTechnologyUtilities.PointToPixel(cellRect[0], true);
                top = windowRect.top + ExcelTechnologyUtilities.PointToPixel(cellRect[1], false);
                width = ExcelTechnologyUtilities.PointToPixel(cellRect[2], true);
                height = ExcelTechnologyUtilities.PointToPixel(cellRect[3], false);
            }
        }

                                                public override object GetPropertyValue(string propertyName)
        {
                        if (string.Equals(PropertyNames.WorksheetName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.CellInfo.SheetName;
            }
            else if (string.Equals(PropertyNames.RowIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.CellInfo.RowIndex;
            }
            else if (string.Equals(PropertyNames.ColumnIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.CellInfo.ColumnIndex;
            }

            return base.GetPropertyValue(propertyName);
        }

        #endregion

                                                                                        public override IQueryElement QueryId
        {
                        get
            {
                if (this.queryId == null)
                {
                    this.queryId = new QueryElement();
                    this.queryId.Condition = new AndCondition(
                            new PropertyCondition(PropertyNames.ControlType, this.ControlTypeName),
                            new PropertyCondition(PropertyNames.RowIndex, this.CellInfo.RowIndex),
                            new PropertyCondition(PropertyNames.ColumnIndex, this.CellInfo.ColumnIndex));
                    this.queryId.Ancestor = this.Parent;
                                    }

                return this.queryId;
            }
        }

        #region Override for Object Methods

                                                public override bool Equals(ITaskActivityElement element)
        {
            if (base.Equals(element))
            {
                ExcelCellElement otherElement = element as ExcelCellElement;
                if (otherElement != null)
                {
                    return this.CellInfo.Equals(otherElement.CellInfo);
                }
            }

            return false;
        }

                                                public override int GetHashCode()
        {
            return this.CellInfo.GetHashCode();
        }

        #endregion

        #region Advance Methods

                                public override void SetFocus()
        {               Keyboard.SendKeys("{Tab}");
                        ExcelCommunicator.Instance.SetFocus(this.CellInfo);
        }

                                                                                        public override void EnsureVisibleByScrolling(int pointX, int pointY, ref int outpointX, ref int outpointY)
        {
                        ExcelCommunicator.Instance.ScrollIntoView(this.CellInfo);
        }

        #endregion

        #region Internals/Privates

                                internal override TaskActivityElement Parent
        {
            get
            {
                if (this.parent == null)
                {
                    this.parent = this.technologyManager.GetExcelElement(this.WindowHandle, this.CellInfo.Parent);
                }

                return this.parent;
            }
        }

                                                internal override System.Collections.IEnumerator GetChildren(AndCondition condition)
        {
                        return null;
        }

                                internal ExcelCellInfo CellInfo { get; private set; }

                                                        internal ExcelCellElement(IntPtr windowHandle, ExcelCellInfo cellInfo, ExcelTechnologyManager manager)
            : base(windowHandle, manager)
        {
            this.CellInfo = cellInfo;
        }

        #endregion
    }
}
