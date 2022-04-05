


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ExecuteTask.Helpers.Interface;
using ZappyMessages.ExcelMessages;
using AndCondition = Zappy.ActionMap.HelperClasses.AndCondition;
using ILastInvocationInfo = Zappy.ExecuteTask.Helpers.Interface.ILastInvocationInfo;
using IQueryCondition = Zappy.ActionMap.Query.IQueryCondition;
using IUISynchronizationWaiter = Zappy.ActionMap.HelperClasses.IUISynchronizationWaiter;

namespace Zappy.Plugins.Excel
{
                    [ComVisible(true)]
    public sealed class ExcelTechnologyManager : ActionMap.TaskTechnology.UITechnologyManager
    {
        internal ExcelTechnologyManager()
        {
            InitializeTechnologyManagerProperties();
        }

        private void InitializeTechnologyManagerProperties()
        {
            SetTechnologyManagerProperty(UITechnologyManagerProperty.WindowLessTreeSwitchingSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.SearchSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.FilterPropertiesForSearchSupported, false);
                        SetTechnologyManagerProperty(UITechnologyManagerProperty.DoNotGenerateVisibleOnlySearchConfiguration, null);
                        SetTechnologyManagerProperty(UITechnologyManagerProperty.MergeSingleSessionObjects, false);
        }
                                        public override string TechnologyName
        {
            get { return ExcelTechnologyUtilities.ExcelTechnologyName; }
        }

                                                                public override int GetControlSupportLevel(IntPtr windowHandle)
        {
                        return ExcelTechnologyUtilities.IsExcelWorksheetWindow(windowHandle) ?
                   100 : 0;
        }

                                                                public override ITaskActivityElement GetFocusedElement(IntPtr windowHandle)
        {
                                    return GetExcelElement(windowHandle, ExcelCommunicator.Instance.GetFocussedElement());
        }

                                                        public override ITaskActivityElement GetElementFromPoint(int pointX, int pointY)
        {
                        IntPtr windowHandle = ExcelTechnologyUtilities.WindowFromPoint(pointX, pointY);

                                    return GetExcelElement(windowHandle, ExcelCommunicator.Instance.GetElementFromPoint(pointX, pointY));

        }

                                                public override ITaskActivityElement GetElementFromWindowHandle(IntPtr windowHandle)
        {
                                    return GetExcelElement(windowHandle, null);
        }

                                                        public override ITaskActivityElement GetElementFromNativeElement(object nativeElement)
        {
            object[] parts = nativeElement as object[];
            if (parts != null && parts.Length == 2 && parts[0] is IntPtr && parts[1] is ExcelElementInfo)
            {
                                IntPtr windowHandle = (IntPtr)parts[0];
                ExcelElementInfo elementInfo = (ExcelElementInfo)parts[1];
                return GetExcelElement(windowHandle, elementInfo);
            }
            else if (nativeElement is IntPtr)
            {
                                return GetElementFromWindowHandle((IntPtr)nativeElement);
            }

            return null;
        }

                                                                                        public override ITaskActivityElement ConvertToThisTechnology(ITaskActivityElement elementToConvert, out int supportLevel)
        {
            supportLevel = 0;
            if (elementToConvert is ExcelElement)
            {
                                supportLevel = 100;
                return elementToConvert;
            }
            else
            {
                                IntPtr windowHandle = elementToConvert.WindowHandle;
                if (ExcelTechnologyUtilities.IsExcelWorksheetWindow(windowHandle))
                {
                    supportLevel = 100;
                    return GetExcelElement(windowHandle, null);
                }
            }

                        return null;
        }

                                                                                public override string ParseQueryId(string queryElement, out object parsedQueryIdCookie)
        {
                                                IQueryCondition condition = null;
            try
            {
                condition = AndCondition.Parse(queryElement);
            }
            catch (ArgumentException)
            {
            }

            if (condition == null)
            {
                                Debug.Fail("ParseQueryId failed");
                parsedQueryIdCookie = null;
                return queryElement;
            }
            else
            {
                                parsedQueryIdCookie = condition;
                return string.Empty;
            }
        }

                                                                                                                                                                public override bool MatchElement(ITaskActivityElement element, object parsedQueryIdCookie, out bool useEngine)
        {
                        IQueryCondition condition = parsedQueryIdCookie as AndCondition;
            if (condition != null)
            {
                                                                useEngine = false;
                return condition.Match(element);
            }
            else
            {
                useEngine = true;
                return false;
            }
        }

        #region Navigation Methods

                                                        public override ITaskActivityElement GetParent(ITaskActivityElement element)
        {
            ExcelElement excelElement = element as ExcelElement;
            if (excelElement != null)
            {
                return excelElement.Parent;
            }

            return null;
        }

                                                                public override System.Collections.IEnumerator GetChildren(ITaskActivityElement element, object parsedQueryIdCookie)
        {
            ExcelElement excelElement = element as ExcelElement;
            AndCondition condition = parsedQueryIdCookie as AndCondition;
            if (excelElement != null)
            {
                if (condition != null)
                {
                    return excelElement.GetChildren(condition);
                }
                else
                {
                    return new TaskActivityElement[] { }.GetEnumerator();
                }
            }

            return null;
        }

                                                public override ITaskActivityElement GetNextSibling(ITaskActivityElement element)
        {
                        return null;
        }

                                                public override ITaskActivityElement GetPreviousSibling(ITaskActivityElement element)
        {
                        return null;
        }

        #endregion

        #region Search Methods - Not required for this sample.

                                                                                                                                                                        public override object[] Search(object parsedQueryIdCookie, ITaskActivityElement parentElement, int maxDepth)
        {
            throw new NotSupportedException();
        }

                                        public override ILastInvocationInfo GetLastInvocationInfo()
        {
            return null;
        }

                                                                public override void CancelStep()
        {
                    }

        #endregion

        #region Add/Remove Event Methods - Not required for this sample.

                                                                        public override bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            return false;
        }

                                                                public override bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            return false;
        }

                                                        public override bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            return false;
        }

                                                        public override bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            return false;
        }

                                                                        public override IUISynchronizationWaiter GetSynchronizationWaiter(ITaskActivityElement element, ZappyTaskEventType eventType)
        {
            return null;
        }

                                        public override void ProcessMouseEnter(IntPtr handle)
        {
                    }

        #endregion

        #region Initialize/Cleanup Methods - Not required for this sample

                                        public override void StartSession(bool recordingSession)
        {
                    }

                                public override void StopSession()
        {
                    }

        #endregion

        #region Internal/Private

                                                        internal TaskActivityElement GetExcelElement(IntPtr windowHandle, ExcelElementInfo elementInfo)
        {
            if (elementInfo is ExcelCellInfo)
            {
                return new ExcelCellElement(windowHandle, elementInfo as ExcelCellInfo, this);
            }
            else if (elementInfo is ExcelWorksheetInfo)
            {
                return new ExcelWorksheetElement(windowHandle, elementInfo as ExcelWorksheetInfo, this);
            }
            else
            {
                return new ExcelElement(windowHandle, this);
            }
        }

        #endregion
    }
}
