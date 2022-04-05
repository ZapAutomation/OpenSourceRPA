using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;
using ScrollAmount = Zappy.ActionMap.Enums.ScrollAmount;

namespace Zappy.ActionMap.TaskTechnology
{
#if COMENABLED
    [Guid("630624E3-24CA-4059-8D78-DC5E2710F945"), ComVisible(true)]
#endif
    public abstract class TaskActivityElement : ITaskActivityElement
    {
        public const AccessibleStates AllAccessibleStates = ~AccessibleStates.None;
        private PropertyBag<UITechnologyElementOption> options = new PropertyBag<UITechnologyElementOption>("UITechnologyElementOption");
        private int supportLevel = -1;
        private IList<string> windowTitles;
        private AutomationElement _automationElement = null;

        public abstract void CacheProperties();
        internal static TaskActivityElement Cast(ITaskActivityElement element)
        {
            if (element == null)
            {
                return null;
            }
            TaskActivityElement element2 = element as TaskActivityElement;
            if (element2 == null)
            {
                throw new ZappyTaskException();
            }
            return element2;
        }

        internal void CopyOptionsTo(ITaskActivityElement element)
        {
            if (element != null && this != element)
            {
                foreach (KeyValuePair<UITechnologyElementOption, object> pair in options)
                {
                    element.SetOption(pair.Key, pair.Value);
                }
            }
        }

        public virtual void SetPropertyValue(string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public virtual void PerformKeyboardAction(string str)
        {
            throw new NotImplementedException();
        }

        public virtual void PerformMouseAction(string action)
        {
            throw new NotImplementedException();
        }


        public abstract void EnsureVisibleByScrolling(int pointX, int pointY, ref int outpointX, ref int outpointY);
        public abstract bool Equals(ITaskActivityElement element);

        public abstract void GetBoundingRectangle(out int left, out int top, out int width, out int height);

        public abstract void GetClickablePoint(out int pointX, out int pointY);
        public abstract object GetNativeControlType(NativeControlTypeKind nativeControlTypeKind);
        public virtual object GetOption(UITechnologyElementOption technologyElementOption)
        {
            if (!options.ContainsKey(technologyElementOption))
            {
                                throw new NotSupportedException();
            }
            return options.GetProperty<object>(technologyElementOption);
        }

        public abstract object GetPropertyValue(string propertyName);
        public abstract string GetQueryIdForRelatedElement(ZappyTaskElementKind relatedElement, object additionalInfo, out int maxDepth);
        public abstract AccessibleStates GetRequestedState(AccessibleStates requestedState);
        public abstract bool GetRightToLeftProperty(RightToLeftKind rightToLeftKind);
        public abstract int GetScrolledPercentage(ScrollDirection scrollDirection, ITaskActivityElement scrollElement);
        internal static object GetTechnologyElementOption(ITaskActivityElement element, UITechnologyElementOption option)
        {
            object obj2 = null;
            try
            {
                obj2 = element.GetOption(option);
            }
            catch (COMException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            return obj2;
        }

        public abstract bool InitializeProgrammaticScroll();
        public abstract void InvokeProgrammaticAction(ProgrammaticActionOption programmaticActionOption);
        internal static bool IsAnyState(ITaskActivityElement element, AccessibleStates[] states)
        {
            AccessibleStates none = AccessibleStates.None;
            for (int i = 0; i < states.Length; i++)
            {
                none |= states[i];
            }
            AccessibleStates requestedState = element.GetRequestedState(none);
            for (int j = 0; j < states.Length; j++)
            {
                if ((requestedState & states[j]) == states[j])
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool? IsInteractable()
        {
            bool? nullable;
            try
            {
                int num;
                int num2;
                int num3;
                int num4;
                GetBoundingRectangle(out num, out num2, out num3, out num4);
                Rectangle rectangle = new Rectangle(num, num2, num3, num4);
                if (rectangle.Width <= 0 || rectangle.Height <= 0)
                {
                    return false;
                }
                try
                {
                    GetClickablePoint(out num, out num2);
                    if (rectangle.Contains(num, num2))
                    {
                        ITaskActivityElement elementFromPoint = TechnologyManager.GetElementFromPoint(num, num2);
                        if (elementFromPoint != null && elementFromPoint.Equals(this))
                        {
                            goto Label_009E;
                        }
                    }
                    return false;
                }
                catch (NotSupportedException)
                {
                    return null;
                }
                catch (NotImplementedException)
                {
                    return null;
                }
            Label_009E:
                nullable = true;
            }
            catch (Exception)
            {
                nullable = false;
            }
            return nullable;
        }

        internal static bool IsState(ITaskActivityElement element, AccessibleStates state) =>
            (element.GetRequestedState(state) & state) == state;

        public abstract void ScrollProgrammatically(ScrollDirection scrollDirection, ScrollAmount scrollAmount);
        public abstract void SetFocus();
        public virtual void SetOption(UITechnologyElementOption technologyElementOption, object optionValue)
        {
            switch (technologyElementOption)
            {
                case UITechnologyElementOption.None:
                    break;

                case UITechnologyElementOption.SetMousePositionInstantly:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.UseWaitForReadyLevelForElementReady:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.PerformEnsureVisible:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.TrustGetValue:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.TrustGetState:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.UseSetFocusForEnsureVisible:
                    options.AddProperty<bool>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.UISynchronizationOptions:
                    options.AddProperty<UISynchronizationOptions>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.WaitForReadyOptions:
                    options.AddProperty<WaitForReadyOptions>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.GetClickableRectangle:
                    options.AddProperty<int[]>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.GetClickablePointFrom:
                    options.AddProperty<GetClickablePointFromOption>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.SetValueAsComboBoxOptions:
                    options.AddProperty<SetValueAsComboBoxOptions>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.SetValueAsEditBoxOptions:
                    options.AddProperty<SetValueAsEditBoxOptions>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.ScrollOptions:
                    options.AddProperty<ScrollOptions>(technologyElementOption, optionValue, true);
                    return;

                case UITechnologyElementOption.ExpandCollapseOptions:
                    options.AddProperty<ExpandCollapseOptions>(technologyElementOption, optionValue, true);
                    break;

                default:
                    return;
            }
        }

        public abstract void WaitForReady();

        public virtual AutomationElement AutomationElement
        {
            get
            {
                if (WindowHandle != IntPtr.Zero && ReferenceEquals(_automationElement, null))
                {
                    _automationElement = AutomationElement.FromHandle(WindowHandle);
                }
                return _automationElement;
            }
        }

        private AutomationElement _automationElementForScreenshot;
        public virtual AutomationElement AutomationElementForScreenshot
        {
            get
            {
                if (!ReferenceEquals(AutomationElement, null))
                {
                    _automationElementForScreenshot = AutomationElement;
                }
                else if(ReferenceEquals(_automationElementForScreenshot, null))
                {
                                                            _automationElementForScreenshot = AutomationElement.FromHandle(WindowHandle);
                }
                return _automationElementForScreenshot;
            }          
        }

        public abstract int ChildIndex { get; }

        public abstract string ClassName { get; }

        public abstract string ControlTypeName { get; }

        public abstract string FriendlyName { get; }

        public abstract bool IsLeafNode { get; }

        public abstract bool IsPassword { get; }

        public abstract bool IsTreeSwitchingRequired { get; }

        public abstract string Name { get; }

        public abstract object NativeElement { get; }


        public abstract IQueryElement QueryId { get; }

        public virtual int SupportLevel
        {
            get =>
                supportLevel;
            set
            {
                supportLevel = value;
            }
        }

        public abstract ITaskActivityElement SwitchingElement { get; set; }

        public abstract UITechnologyManager TechnologyManager { get; }

        public abstract string Framework { get; }

        public abstract TaskActivityElement TopLevelElement { get; set; }

        public abstract string Value { get; set; }

        public abstract IntPtr WindowHandle { get; }

        public virtual IList<string> WindowTitles
        {
            get
            {
                if (windowTitles == null)
                {
                    windowTitles = new List<string>();
                }
                return windowTitles;
            }
        }
    }

}
