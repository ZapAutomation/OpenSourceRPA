


using Accessibility;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using ZappyMessages.ExcelMessages;

namespace Zappy.Plugins.Excel
{
                    [ComVisible(true)]
    public class ExcelElement : TaskActivityElement
    {
                                                internal ExcelElement(IntPtr windowHandle, ExcelTechnologyManager manager)
        {
            this.windowHandle = windowHandle;
            this.technologyManager = manager;
        }

        #region Properties

                                        public override string Framework
        {
            get { return ExcelTechnologyUtilities.ExcelTechnologyName; }
        }

                                public override UITechnologyManager TechnologyManager
        {
            get { return this.technologyManager; }
        }

                                public override string ClassName
        {
            get { return ExcelTechnologyUtilities.ExcelClassName; }
        }

                                public override IntPtr WindowHandle
        {
            get { return this.windowHandle; }
        }

                                public override int ChildIndex
        {
            get { return 0; }
        }

                                public override string ControlTypeName
        {
            get { return ControlType.Window.Name; }
        }

                                        public override string FriendlyName
        {
            get { return this.Name; }
        }

                                public override bool IsLeafNode
        {
            get { return false; }
        }

                                public override bool IsPassword
        {
            get { return false; }
        }

                                                                                public override bool IsTreeSwitchingRequired
        {
            get { return false; }
        }

                                public override string Name
        {
            get { return ExcelTechnologyUtilities.GetWindowText(this.WindowHandle); }
        }

                                        public override object NativeElement
        {
            get { return this.WindowHandle; }
        }

                                public override string Value
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

                                        public override ITaskActivityElement SwitchingElement
        {
                        get;
            set;
        }

                                                        public override TaskActivityElement TopLevelElement
        {
                        get;
            set;
        }

                                                                                        public override IQueryElement QueryId
        {
                        get
            {
                if (this.queryId == null)
                {
                    this.queryId = new QueryElement();
                    this.queryId.Condition = new AndCondition(
                            new PropertyCondition(PropertyNames.ControlType, this.ControlTypeName),
                            new PropertyCondition(PropertyNames.Name, this.Name));
                }

                return this.queryId;
            }
        }

        #endregion

        #region Methods

                                                        public override void CacheProperties()
        {
                                    object dummy = this.QueryId;
            dummy = this.Parent;
        }

                                                                        public override object GetNativeControlType(NativeControlTypeKind nativeControlTypeKind)
        {
            if (nativeControlTypeKind == NativeControlTypeKind.AsString)
            {
                return this.ControlTypeName;
            }

            return null;
        }

                                                        public override bool GetRightToLeftProperty(RightToLeftKind rightToLeftKind)
        {
                        return false;
        }

                                        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            left = top = width = height = -1;

            ExcelTechnologyUtilities.RECT rect;
            if (ExcelTechnologyUtilities.GetWindowRect(this.WindowHandle, out rect))
            {
                left = rect.left;
                top = rect.top;
                width = rect.right - rect.left;
                height = rect.bottom - rect.top;
            }
        }

                                                                public override AccessibleStates GetRequestedState(AccessibleStates requestedState)
        {
            IAccessible accessible = ExcelTechnologyUtilities.AccessibleObjectFromWindow(this.WindowHandle);
            object state = accessible.accState;
            if (state is AccessibleStates)
            {
                return (AccessibleStates)state;
            }

            return AccessibleStates.Default;
        }

                                                public override object GetPropertyValue(string propertyName)
        {
                        if (string.Equals(PropertyNames.ControlType, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.ControlTypeName;
            }
            else if (string.Equals(PropertyNames.ClassName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.ClassName;
            }
            else if (string.Equals(PropertyNames.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.Name;
            }

            throw new NotSupportedException();
        }

        #endregion

        #region Override for Object Methods

                                                public override bool Equals(object obj)
        {
            return this.Equals(obj as ITaskActivityElement);
        }

                                                public override bool Equals(ITaskActivityElement element)
        {
            try
            {
                return string.Equals(this.Name, element.Name, StringComparison.Ordinal);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                return false;
            }
        }

                                                public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

                                        public override string ToString()
        {
                        return this.Name;
        }

        #endregion

        #region Advanced Methods - Not supported in this base class

                                                                                                public override void GetClickablePoint(out int pointX, out int pointY)
        {
            throw new NotSupportedException();
        }

                                                public override bool InitializeProgrammaticScroll()
        {
            return false;
        }

                                                        public override void ScrollProgrammatically(ScrollDirection scrollDirection, ScrollAmount scrollAmount)
        {
                        throw new NotSupportedException();
        }

                                                                public override int GetScrolledPercentage(ScrollDirection scrollDirection, ITaskActivityElement scrollElement)
        {
                        throw new NotSupportedException();
        }

                                                                                        public override void EnsureVisibleByScrolling(int pointX, int pointY, ref int outpointX, ref int outpointY)
        {
            throw new NotSupportedException();
        }

                                                                        public override string GetQueryIdForRelatedElement(ZappyTaskElementKind relatedElement, object additionalInfo, out int maxDepth)
        {
            throw new NotSupportedException();
        }

                                        public override void InvokeProgrammaticAction(ProgrammaticActionOption programmaticActionOption)
        {
            throw new NotSupportedException();
        }

                                public override void SetFocus()
        {
                    }

                                                                public override void WaitForReady()
        {
                    }

        #endregion

        #region Internals/Privates

                                                internal virtual System.Collections.IEnumerator GetChildren(AndCondition condition)
        {
            string sheetName = condition.GetPropertyValue(PropertyNames.Name) as string;
            if (!string.IsNullOrEmpty(sheetName))
            {
                                TaskActivityElement sheetElement = this.technologyManager.GetExcelElement(
                    this.WindowHandle,
                    new ExcelWorksheetInfo(sheetName, ""));

                                return new TaskActivityElement[] { sheetElement }.GetEnumerator();
            }

            return null;
        }

                                internal virtual TaskActivityElement Parent
        {
            get { return parent; }
        }

                                protected IntPtr windowHandle;

                                protected ExcelTechnologyManager technologyManager;

                                protected IQueryElement queryId;

                                protected TaskActivityElement parent;

        #endregion
    }
}
