using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using DataGridUtility = Zappy.Plugins.Uia.Uia.Utilities.DataGridUtility;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;
using ScrollAmount = Zappy.ActionMap.Enums.ScrollAmount;

namespace Zappy.Plugins.Uia.Uia
{
    [Serializable]
#if COMENABLED
    [ComVisible(true), Guid("72FF0C88-2BAE-4868-B15C-F7D42975D2AB"), ]
#endif
    public class UiaElement : TaskActivityElement
    {
        private UiaElement ancestorHavingWindowHandle;
        private string automationId;
        private string className;
        private string controlTypeName;
        private bool distinctCheckDone;
        private string friendlyName;
        private bool isCacheMode;
        private bool? isCeilingElement;
        private bool? isPassword;
        private const string LineDownName = "LineDown";
        private const string LineLeftName = "LineLeft";
        private const string LineRightName = "LineRight";
        private const string LineUpName = "LineUp";
        private string name;
        private IntPtr? nativeWindowHandle;
        private const string PageDownName = "PageDown";
        private const string PageLeftName = "PageLeft";
        private const string PageRightName = "PageRight";
        private const string PageUpName = "PageUp";
        private UiaElement parent;
        private const string QidFrameWorkName = ";[UIA]Name='";
        private IQueryElement queryID;
        private int[] runtimeId;
        private string runtimeIdString;
        private const string ScrollBarName = "ScrollBar";
        [NonSerialized]
        private ScrollItemPattern scrollItemPattern;
        [NonSerialized]
        private ScrollPattern scrollPattern;
        private const string SingleQuote = "'";
        private UiaElement supportedTopLevelWindow;
        private const string Thumb = "Thumb";
        [NonSerialized]
        private TaskActivityElement topLevelElement;
        private const string UnInitializedValue = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";

        internal UiaElement(System.Windows.Automation.AutomationElement automationElement)
        {
            this.className = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.name = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.friendlyName = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.automationId = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            if (automationElement == null)
            {
                throw new ArgumentNullException("automationElement");
            }
            this.InnerElement = automationElement;
            this.runtimeId = this.InnerElement.GetRuntimeId();
            System.Windows.Automation.ControlType automationPropertyValue = UiaUtility.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(this.InnerElement, AutomationElement.ControlTypeProperty);
            string uiaClassName = null;
            if (!this.LazyInitializeClassName)
            {
                uiaClassName = this.InitializeClassName();
            }
            this.controlTypeName = UiaUtility.GetControlTypeName(this.InnerElement, automationPropertyValue, uiaClassName);
                                    this.FrameworkId = automationElement.Current.FrameworkId;
        }

        internal UiaElement(System.Windows.Automation.AutomationElement automationElement, string controlType)
        {
            this.className = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.name = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.friendlyName = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            this.automationId = "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1";
            if (automationElement == null)
            {
                throw new ArgumentNullException("automationElement");
            }
            this.InnerElement = automationElement;
            this.controlTypeName = controlType;
        }

        private void AddSearchConfigurations()
        {
            List<string> configList = new List<string>();
            if (this.queryID.SearchConfigurations != null)
            {
                configList.AddRange(this.queryID.SearchConfigurations);
            }
            if (!this.distinctCheckDone && (this.queryID.Condition != null))
            {
                this.distinctCheckDone = true;
                if (this.queryID.Ancestor != null)
                {
                    if (ExtensionUtilities.IsQueryConditionSubSetOfParent(this.Framework, this.queryID.Condition.Conditions, this.queryID.Ancestor) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.DisambiguateChild))
                    {
                        configList.Add(SearchConfiguration.DisambiguateChild);
                    }
                }
                else if (Equals(this.TopLevelElement, this.Parent))
                {
                    if (ExtensionUtilities.IsQueryConditionSubSetOfParent(this.Framework, this.queryID.Condition.Conditions, this.TopLevelElement) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.DisambiguateChild))
                    {
                        configList.Add(SearchConfiguration.DisambiguateChild);
                    }
                }
                else
                {
                    this.distinctCheckDone = false;
                }
            }
            if (((ControlType.MenuItem.NameEquals(this.ControlTypeName) || ControlType.TreeItem.NameEquals(this.ControlTypeName)) || (ControlType.CheckBoxTreeItem.NameEquals(this.ControlTypeName) || ControlType.Menu.NameEquals(this.ControlTypeName))) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.ExpandWhileSearching))
            {
                configList.Add(SearchConfiguration.ExpandWhileSearching);
            }
            if (ControlType.Row.NameEquals(this.ControlTypeName) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.AlwaysSearch))
            {
                configList.Add(SearchConfiguration.AlwaysSearch);
            }
            if (configList.Count > 0)
            {
                this.queryID.SearchConfigurations = configList.ToArray();
            }
            else
            {
                this.queryID.SearchConfigurations = null;
            }
        }

        internal void BringControlToView()
        {
            if (this.ShouldBringControlToView)
            {
                System.Windows.Automation.AutomationElement element;
                UiaUtility.GetFirstScrollableParent(this.InnerElement, out element);
                if (element != null)
                {
                    UiaUtility.BringControlToView(element, true);
                }
                UiaUtility.BringControlToView(this.InnerElement, false);
            }
        }

        public override void CacheProperties()
        {
            this.isCacheMode = true;
            try
            {
                object parent = this.Parent;
                parent = this.ClassName;
                parent = this.QueryId;
                parent = this.WindowHandle;
                parent = this.TopLevelElement;
                parent = this.Name;
                parent = this.IsPassword;
                parent = this.FriendlyName;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        public override void EnsureVisibleByScrolling(int pointX, int pointY, ref int outPointX, ref int outPointY)
        {
            try
            {
                if (this.scrollItemPattern == null)
                {
                    this.scrollItemPattern = PatternHelper.GetScrollItemPattern(this.InnerElement, true);
                }
                if (this.scrollItemPattern != null)
                {
                    UiaUtility.RealizeElement(this.InnerElement);
                    this.scrollItemPattern.ScrollIntoView();
                }
                else if (this.Parent != null)
                {
                    System.Windows.Automation.AutomationElement innerElement = this.Parent.InnerElement;
                    for (int i = 0; (i < 3) && (innerElement != null); i++)
                    {
                        ScrollItemPattern scrollItemPattern = PatternHelper.GetScrollItemPattern(innerElement, true);
                        if (scrollItemPattern != null)
                        {
                            UiaUtility.RealizeElement(innerElement);
                            scrollItemPattern.ScrollIntoView();
                            break;
                        }
                        innerElement = TreeWalkerHelper.GetParent(innerElement);
                    }
                }
                this.BringControlToView();
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        public override bool Equals(ITaskActivityElement element)
        {
            bool flag = this.EqualsInternal(element);
            if (!flag && this.IsVirtualElement)
            {
                return VirtualUiaElementUtility.ElementAreEqual(this, element);
            }
            return flag;
        }

        public override bool Equals(object obj)
        {
            ITaskActivityElement element = obj as ITaskActivityElement;
            return this.Equals(element);
        }

        private bool EqualsInternal(ITaskActivityElement element)
        {
            if (this == element)
            {
                return true;
            }
            UiaElement element2 = UiaUtility.TransformUiaElement(element);
            if (element2 == null)
            {
                return false;
            }
            return Automation.Compare(this.runtimeId, element2.runtimeId);
        }

        protected virtual T GetAutomationPropertyValue<T>(AutomationProperty automationProperty) =>
            UiaUtility.GetAutomationPropertyValue<T>(this.InnerElement, automationProperty);

        internal Rectangle GetBoundingRectangle() =>
            this.GetAutomationPropertyValue<Rectangle>(AutomationElement.BoundingRectangleProperty);

        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            int num;
            height = num = -1;
            width = num;
            left = top = num;
            Rectangle empty = Rectangle.Empty;
            try
            {
                empty = this.GetAutomationPropertyValue<Rectangle>(AutomationElement.BoundingRectangleProperty);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            if (empty != Rectangle.Empty)
            {
                left = empty.Left;
                top = empty.Top;
                width = empty.Width;
                height = empty.Height;
            }
        }

        public override void GetClickablePoint(out int pointX, out int pointY)
        {
            throw new NotSupportedException();
        }

        private AccessibleStates GetExpandCollapseState()
        {
            AccessibleStates none = AccessibleStates.None;
            ExpandCollapsePattern expandCollapsePattern = PatternHelper.GetExpandCollapsePattern(this.InnerElement, false);
            if (expandCollapsePattern != null)
            {
                switch (this.GetPatternValue<ExpandCollapseState>(expandCollapsePattern, ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty))
                {
                    case ExpandCollapseState.Expanded:
                        return (none | AccessibleStates.Expanded);

                    case ExpandCollapseState.Collapsed:
                        return (none | AccessibleStates.Collapsed);
                }
            }
            return none;
        }

        private AccessibleStates GetExpanderState()
        {
            AccessibleStates none = AccessibleStates.None;
            UiaElement uiaElement = UiaElementFactory.GetUiaElement(TreeWalkerHelper.GetFirstChild(this.InnerElement), false);
            if (uiaElement == null)
            {
                return none;
            }
            TogglePattern togglePattern = PatternHelper.GetTogglePattern(uiaElement.InnerElement);
            if (togglePattern == null)
            {
                return none;
            }
            if (((ToggleState)uiaElement.GetPatternValue<ToggleState>(togglePattern, TogglePatternIdentifiers.ToggleStateProperty)) == ToggleState.On)
            {
                return (none | AccessibleStates.Expanded);
            }
            return (none | AccessibleStates.Collapsed);
        }

        public override int GetHashCode()
        {
            if (this.runtimeId.Length == 0)
            {
                return -1;
            }
            int hashCode = this.runtimeId[0].GetHashCode();
            for (int i = 1; i < this.runtimeId.Length; i++)
            {
                hashCode ^= this.runtimeId[i].GetHashCode();
            }
            return hashCode;
        }

        public override object GetNativeControlType(NativeControlTypeKind nativeControlTypeKind)
        {
            if (nativeControlTypeKind == NativeControlTypeKind.AsString)
            {
                try
                {
                    return this.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(AutomationElement.ControlTypeProperty).ProgrammaticName;
                }
                catch (Exception exception)
                {
                    UiaUtility.MapAndThrowException(exception, this, false);
                    throw;
                }
            }
            return null;
        }

        public override object GetOption(UITechnologyElementOption technologyElementOption)
        {
            object option;
            try
            {
                option = base.GetOption(technologyElementOption);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return option;
        }

        protected virtual T GetPatternValue<T>(BasePattern pattern, AutomationProperty automationProperty) =>
            PatternHelper.GetPatternValue<T>(pattern, automationProperty);

        public override object GetPropertyValue(string propertyName)
        {
            object attributeValue = null;
            try
            {
                System.Windows.Automation.AutomationElement element;
                bool flag;
                ExpandCollapsePattern pattern2;
                List<string> list;
                List<int> list2;
                List<string> list3;
                List<int> list4;
                SelectionPattern pattern3;
                propertyName = PropertyNames.GetPropertyNameInCorrectCase(propertyName);
                                {
                                        if (propertyName == "SelectedItems")
                    {
                        goto Label_075D;
                    }
                    
                                        if (propertyName == "SelectedIndices")
                    {
                        goto Label_077B;
                    }
                    
                                        if (propertyName == "AutomationId")
                    {
                        return this.AutomationId;
                    }
                    
                                        if (propertyName == "ClassName")
                    {
                        return this.ClassName;
                    }
                    
                                        if (propertyName == "SelectionText")
                    {
                        return UiaUtility.GetSelectionText(this.InnerElement);
                    }
                    
                                        if (propertyName == "Name")
                    {
                        return this.Name;
                    }
                    
                                        if (propertyName == "HelpText")
                    {
                        return this.GetAutomationPropertyValue<string>(AutomationElement.HelpTextProperty);
                    }
                    
                                        if (propertyName == "ReadOnly")
                    {
                        return this.IsReadOnly();
                    }
                    
                                        if (propertyName == "AcceleratorKey")
                    {
                        return this.GetAutomationPropertyValue<string>(AutomationElement.AcceleratorKeyProperty);
                    }
                    
                                        if (propertyName == "Date")
                    {
                        goto Label_0626;
                    }
                    
                                        if (propertyName == "LabeledBy")
                    {
                        goto Label_0522;
                    }
                    
                                        if (propertyName == "Checked")
                    {
                        return (UiaUtility.GetToggleState(this.InnerElement) == ToggleState.On);
                    }
                    
                                        if (propertyName == "IsMultipleSelection")
                    {
                        goto Label_07B1;
                    }
                    
                                        if (propertyName == "ControlType")
                    {
                        return this.ControlTypeName;
                    }
                    
                                        if (propertyName == "IsGroupedTable")
                    {
                        return GridGroupTreeWalker.IsGroupedGrid(this);
                    }
                    
                                        if (propertyName == "Expanded")
                    {
                        goto Label_067B;
                    }
                    
                                        if (propertyName == "SelectedItem")
                    {
                        goto Label_0700;
                    }
                    
                                        if (propertyName == "SelectedDates")
                    {
                        goto Label_06AE;
                    }
                    
                                        if (propertyName == "Indeterminate")
                    {
                        return (UiaUtility.GetToggleState(this.InnerElement) == ToggleState.Indeterminate);
                    }
                    
                                        if (propertyName == "Font")
                    {
                        goto label_break;
                    }
                    
                                        if (propertyName == "SelectedIndex")
                    {
                        goto Label_072C;
                    }
                    
                                        if (propertyName == "Enabled")
                    {
                        return this.GetAutomationPropertyValue<bool>(AutomationElement.IsEnabledProperty);
                    }
                    
                                        if (propertyName == "Value")
                    {
                        return this.Value;
                    }
                    
                                        if (propertyName == "ColumnHeader")
                    {
                        System.Windows.Automation.AutomationElement element2;
                        return DataGridUtility.GetColumnHeaderString(this, out element2);
                    }
                    
                                        if (propertyName == "AccessKey")
                    {
                        return this.GetAutomationPropertyValue<string>(AutomationElement.AccessKeyProperty);
                    }
                    
                                        if (propertyName == "ItemStatus")
                    {
                        return this.GetAutomationPropertyValue<string>(AutomationElement.ItemStatusProperty);
                    }
                    
                                        if (propertyName == "FrameworkId")
                    {
                        return this.InnerElement.Current.FrameworkId;
                    }
                    
                                        if (propertyName == "ColumnIndex")
                    {
                        goto Label_0573;
                    }
                    
                                        if (propertyName == "Selected")
                    {
                        return UiaUtility.GetSelectionItemState(this.InnerElement);
                    }
                    
                                        if (propertyName == "NativeControlType")
                    {
                        return this.GetNativeControlType(NativeControlTypeKind.AsString);
                    }
                    
                                        goto Label_07E5;
                }
            label_break:
                TextPattern textPattern = PatternHelper.GetTextPattern(this.InnerElement);
                if ((textPattern != null) && (textPattern.DocumentRange != null))
                {
                    attributeValue = textPattern.DocumentRange.GetAttributeValue(TextPattern.FontNameAttribute);
                }
                return attributeValue;
            Label_0522:
                element = this.GetAutomationPropertyValue<System.Windows.Automation.AutomationElement>(AutomationElement.LabeledByProperty);
                if (element != null)
                {
                    attributeValue = element.Current.Name;
                }
                return attributeValue;
            Label_0573:
                attributeValue = DataGridUtility.TryGetColumnNumber(this, out flag);
                if (flag)
                {
                    return attributeValue;
                }
                throw new NotSupportedException();
            Label_0626:
                if (ControlType.DatePicker.NameEquals(this.ControlTypeName))
                {
                    DateTime time;
                    ValuePattern valuePattern = PatternHelper.GetValuePattern(this.InnerElement);
                    if ((valuePattern != null) && ZappyTaskUtilities.TryGetShortDateAndLongTime(valuePattern.Current.Value, out time))
                    {
                        attributeValue = time;
                    }
                    return attributeValue;
                }
                throw new NotSupportedException();
            Label_067B:
                pattern2 = PatternHelper.GetExpandCollapsePattern(this.InnerElement, false);
                if (pattern2 != null)
                {
                    attributeValue = pattern2.Current.ExpandCollapseState == ExpandCollapseState.Expanded;
                }
                return attributeValue;
            Label_06AE:
                if (ControlType.Calendar.NameEquals(this.ControlTypeName))
                {
                    SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(this.InnerElement);
                    if (selectionPattern != null)
                    {
                        List<System.Windows.Automation.AutomationElement> selectedAutomationElements = null;
                        List<string> selectedItemsUsingPattern = UiaUtility.GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                        if (selectedItemsUsingPattern != null)
                        {
                            attributeValue = selectedItemsUsingPattern.ToArray();
                        }
                    }
                    return attributeValue;
                }
                throw new NotSupportedException();
            Label_0700:
                list = UiaUtility.GetSelectedItems(this.InnerElement);
                return (((list != null) && (list.Count == 1)) ? list[0] : null);
            Label_072C:
                list2 = UiaUtility.GetSelectedListItemIndices(this.InnerElement);
                return (((list2 != null) && (list2.Count == 1)) ? list2[0] : -1);
            Label_075D:
                list3 = UiaUtility.GetSelectedItems(this.InnerElement);
                return list3?.ToArray();
            Label_077B:
                list4 = UiaUtility.GetSelectedListItemIndices(this.InnerElement);
                return list4?.ToArray();
            Label_07B1:
                pattern3 = PatternHelper.GetSelectionPattern(this.InnerElement);
                if (pattern3 != null)
                {
                    return pattern3.Current.CanSelectMultiple;
                }
                return false;
            Label_07E5:
                throw new NotSupportedException();
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return attributeValue;
        }

        public override string GetQueryIdForRelatedElement(ZappyTaskElementKind relatedElement, object additionalInfo, out int maxDepth)
        {
            maxDepth = -1;
            switch (relatedElement)
            {
                case ZappyTaskElementKind.VerticalScrollBar:
                    return this.GetScrollComponentQid("VerticalScrollBar");

                case ZappyTaskElementKind.HorizontalScrollBar:
                    return this.GetScrollComponentQid("HorizontalScrollBar");

                case ZappyTaskElementKind.LineUp:
                    return this.GetScrollComponentQid("LineUp");

                case ZappyTaskElementKind.LineDown:
                    return this.GetScrollComponentQid("LineDown");

                case ZappyTaskElementKind.PageUp:
                    return this.GetScrollComponentQid("PageUp");

                case ZappyTaskElementKind.PageDown:
                    return this.GetScrollComponentQid("PageDown");

                case ZappyTaskElementKind.VerticalThumb:
                case ZappyTaskElementKind.HorizontalThumb:
                    {
                        if (this.ControlTypeName == ControlType.Table)
                        {
                            throw new NotSupportedException();
                        }
                        object[] args = new object[] { ControlType.Indicator.Name };
                        return string.Format(CultureInfo.InvariantCulture, ";[UIA]ControlType='{0}'", args);
                    }
                case ZappyTaskElementKind.ColumnLeft:
                    return this.GetScrollComponentQid("LineLeft");

                case ZappyTaskElementKind.ColumnRight:
                    return this.GetScrollComponentQid("LineRight");

                case ZappyTaskElementKind.PageLeft:
                    return this.GetScrollComponentQid("PageLeft");

                case ZappyTaskElementKind.PageRight:
                    return this.GetScrollComponentQid("PageRight");

                case ZappyTaskElementKind.Child:
                    if (!(additionalInfo is string))
                    {
                        CrapyLogger.log.Error("UIA::GetQueryIdForRelatedElement unexpected type for additionalInfo. Expected type is string.");
                        throw new ArgumentException("UIA::GetQueryIdForRelatedElement unexpected type for additionalInfo. Expected type is string.");
                    }
                    maxDepth = -1;
                    return this.GetQueryIdOfChild(additionalInfo as string);
            }
            throw new NotSupportedException();
        }

        private string GetQueryIdOfChild(string childName)
        {
            if ((UiaTechnologyManager.Instance != null) && ControlType.ComboBox.NameEquals(this.controlTypeName))
            {
                UiaTechnologyManager.Instance.LastAccessedComboBox = this;
            }
            string str = PropertyCondition.Escape(childName);
            str = ";[UIA]Name='" + str + "'";
            if (ControlType.ComboBox.NameEquals(this.ControlTypeName))
            {
                str = str + " && ControlType = '" + ControlType.ListItem.Name + "'";
            }
            return str;
        }

        public override AccessibleStates GetRequestedState(AccessibleStates requestedState)
        {
            this.WaitForReady();
            AccessibleStates state = AccessibleStates.Default;
            try
            {
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Unavailable))
                {
                    if (!this.GetAutomationPropertyValue<bool>(AutomationElement.IsEnabledProperty))
                    {
                        state |= AccessibleStates.Unavailable;
                    }
                    else if (this.controlTypeName == ControlType.RowHeader)
                    {
                        System.Windows.Automation.AutomationElement parent = TreeWalkerHelper.GetParent(this.InnerElement);
                        if ((this.Parent != null) && !UiaUtility.MatchAutomationElement(this.Parent.InnerElement, parent))
                        {
                            state |= AccessibleStates.Unavailable;
                            this.Parent = null;
                        }
                    }
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Focused) && this.GetAutomationPropertyValue<bool>(AutomationElement.HasKeyboardFocusProperty))
                {
                    state |= AccessibleStates.Focused;
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Focusable) && this.GetAutomationPropertyValue<bool>(AutomationElement.IsKeyboardFocusableProperty))
                {
                    state |= AccessibleStates.Focusable;
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Offscreen) && this.GetAutomationPropertyValue<bool>(AutomationElement.IsOffscreenProperty))
                {
                    state |= AccessibleStates.Offscreen;
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Collapsed | AccessibleStates.Expanded) && ControlType.Expander.NameEquals(this.ControlTypeName))
                {
                    state |= this.GetExpanderState();
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Selectable | AccessibleStates.Checked | AccessibleStates.Selected))
                {
                    state |= this.GetSelectionState();
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Collapsed | AccessibleStates.Expanded))
                {
                    state |= this.GetExpandCollapseState();
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.Indeterminate | AccessibleStates.Checked | AccessibleStates.Pressed))
                {
                    state = this.GetToggleState(state);
                }
                if (UiaUtility.IsAccessibleStateRequested(requestedState, AccessibleStates.ReadOnly) && this.IsReadOnly())
                {
                    state |= AccessibleStates.ReadOnly;
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                state = AccessibleStates.Unavailable;
            }
            catch (ElementNotAvailableException)
            {
                state = AccessibleStates.Unavailable;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            if (state != AccessibleStates.Default)
            {
                state &= ~AccessibleStates.Default;
            }
            return state;
        }

        public override bool GetRightToLeftProperty(RightToLeftKind rightToLeftKind)
        {
            bool flag2;
            try
            {
                bool isRightToLeft = false;
                CultureInfo automationPropertyValue = this.GetAutomationPropertyValue<CultureInfo>(AutomationElement.CultureProperty);
                if (automationPropertyValue != null)
                {
                    if (rightToLeftKind != RightToLeftKind.Text)
                    {
                        if (rightToLeftKind == RightToLeftKind.Layout)
                        {
                            isRightToLeft = automationPropertyValue.TextInfo.IsRightToLeft;
                        }
                    }
                    else
                    {
                        isRightToLeft = automationPropertyValue.TextInfo.IsRightToLeft;
                    }
                }
                flag2 = isRightToLeft;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return flag2;
        }

        private string GetScrollComponentQid(string name)
        {
            if ((this.ControlTypeName == ControlType.Table) || (this.ControlTypeName == ControlType.Row))
            {
                throw new NotSupportedException();
            }
            if (name.Equals("HorizontalScrollBar", StringComparison.OrdinalIgnoreCase) || name.Equals("VerticalScrollBar", StringComparison.OrdinalIgnoreCase))
            {
                object[] objArray1 = new object[] { ControlType.ScrollBar.Name, name };
                return string.Format(CultureInfo.InvariantCulture, ";[UIA]ControlType='{0}' && AutomationId='{1}'", objArray1);
            }
            object[] args = new object[] { name };
            return string.Format(CultureInfo.InvariantCulture, ";[UIA]AutomationId='{0}'", args);
        }

        public override int GetScrolledPercentage(ScrollDirection scrollDirection, ITaskActivityElement scrollElement)
        {
            int num2;
            try
            {
                int horizontalScrollPercent = -1;
                if (this.scrollPattern != null)
                {
                    if (scrollDirection == ScrollDirection.HorizontalScroll)
                    {
                        horizontalScrollPercent = (int)this.scrollPattern.Current.HorizontalScrollPercent;
                    }
                    else if (scrollDirection == ScrollDirection.VerticalScroll)
                    {
                        horizontalScrollPercent = (int)this.scrollPattern.Current.VerticalScrollPercent;
                    }
                }
                num2 = horizontalScrollPercent;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return num2;
        }

        private AccessibleStates GetSelectionState()
        {
            AccessibleStates none = AccessibleStates.None;
            SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(this.InnerElement, false);
            if (selectionItemPattern == null)
            {
                return none;
            }
            if (selectionItemPattern.Current.IsSelected)
            {
                none |= AccessibleStates.Selected;
                if (ControlType.RadioButton.NameEquals(this.controlTypeName))
                {
                    none |= AccessibleStates.Checked;
                }
                return none;
            }
            return (none | AccessibleStates.Selectable);
        }

        private AccessibleStates GetToggleState(AccessibleStates state)
        {
            TogglePattern togglePattern = PatternHelper.GetTogglePattern(this.InnerElement);
            if (togglePattern != null)
            {
                ToggleState patternValue = this.GetPatternValue<ToggleState>(togglePattern, TogglePatternIdentifiers.ToggleStateProperty);
                if (ControlType.ToggleButton.NameEquals(this.controlTypeName) || ControlType.SemanticZoom.NameEquals(this.controlTypeName))
                {
                    state &= ~AccessibleStates.Default;
                    switch (patternValue)
                    {
                        case ToggleState.On:
                            state |= AccessibleStates.Pressed;
                            return state;

                        case ToggleState.Indeterminate:
                            state |= AccessibleStates.Indeterminate;
                            break;
                    }
                    return state;
                }
                if (patternValue == ToggleState.On)
                {
                    state |= AccessibleStates.Checked;
                    return state;
                }
                if (patternValue == ToggleState.Indeterminate)
                {
                    state |= AccessibleStates.Indeterminate;
                }
            }
            return state;
        }

        internal bool HasValidAutomationId()
        {
            if (string.IsNullOrEmpty(this.AutomationId))
            {
                return false;
            }
            if (!(this.NativeWindowHandle == IntPtr.Zero))
            {
                return !string.Equals(this.AutomationId, this.NativeWindowHandle.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        private string InitializeClassName()
        {
            string str2;
            try
            {
                string automationPropertyValue = null;
                automationPropertyValue = UiaUtility.GetAutomationPropertyValue<string>(this.InnerElement, AutomationElement.ClassNameProperty);
                if (!string.IsNullOrEmpty(automationPropertyValue))
                {
                    this.className = this.SanitizeUIAClassName(automationPropertyValue);
                }
                else
                {
                    this.className = string.Empty;
                }
                str2 = automationPropertyValue;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return str2;
        }

        public override bool InitializeProgrammaticScroll()
        {
            try
            {
                if (this.scrollPattern == null)
                {
                    this.scrollPattern = PatternHelper.GetScrollPattern(this.InnerElement);
                }
                if ((this.scrollPattern == null) && (this.scrollItemPattern == null))
                {
                    this.scrollItemPattern = PatternHelper.GetScrollItemPattern(this.InnerElement, true);
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            if (this.scrollPattern == null)
            {
                return (this.scrollItemPattern != null);
            }
            return true;
        }

        internal void InitializeTechnologyElementOptions()
        {
            this.SetOption(UITechnologyElementOption.TrustGetValue, true);
            this.SetOption(UITechnologyElementOption.TrustGetState, true);
            if ((UiaTechnologyManager.Instance != null) && !UiaTechnologyManager.Instance.IsRecordingSession)
            {
                this.SetOption(UITechnologyElementOption.WaitForReadyOptions, WaitForReadyOptions.EnablePlaybackWaitForReady);
                if (ControlType.Menu.NameEquals(this.ControlTypeName) || ControlType.MenuItem.NameEquals(this.ControlTypeName))
                {
                    this.SetOption(UITechnologyElementOption.UISynchronizationOptions, UISynchronizationOptions.DisableKeyboardSynchronization);
                }
                else if (ControlType.ComboBox.NameEquals(this.ControlTypeName))
                {
                    this.SetOption(UITechnologyElementOption.SetValueAsComboBoxOptions, (SetValueAsComboBoxOptions)0x1186);
                }
            }
        }

        public override void InvokeProgrammaticAction(ProgrammaticActionOption programmaticActionOption)
        {
            try
            {
                SelectionItemPattern pattern;
                SelectionItemPattern pattern2;
                SelectionItemPattern pattern3;
                ExpandCollapsePattern pattern4;
                ExpandCollapsePattern pattern5;
                if (programmaticActionOption <= ProgrammaticActionOption.DefaultAction)
                {
                    if (programmaticActionOption == ProgrammaticActionOption.TakeSelection)
                    {
                        goto Label_00FD;
                    }
                    if (programmaticActionOption == ProgrammaticActionOption.RemoveSelection)
                    {
                        goto Label_0115;
                    }
                    if (programmaticActionOption == ProgrammaticActionOption.DefaultAction)
                    {
                        goto Label_0049;
                    }
                }
                else
                {
                    if (programmaticActionOption == ProgrammaticActionOption.Expand)
                    {
                        goto Label_012D;
                    }
                    if (programmaticActionOption == ProgrammaticActionOption.Collapse)
                    {
                        goto Label_0145;
                    }
                    if (programmaticActionOption == ProgrammaticActionOption.MakeVisibleAndSelect)
                    {
                        goto Label_00CE;
                    }
                }
                throw new NotSupportedException();
            Label_0049:
                if (!ControlType.Button.NameEquals(this.ControlTypeName) || ((!string.Equals(this.AutomationId, "PageRight", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AutomationId, "PageLeft", StringComparison.OrdinalIgnoreCase)) && (!string.Equals(this.AutomationId, "PageUp", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AutomationId, "PageDown", StringComparison.OrdinalIgnoreCase))))
                {
                    throw new NotSupportedException();
                }
                InvokePattern invokePattern = PatternHelper.GetInvokePattern(this.InnerElement, true);
                if (invokePattern != null)
                {
                    invokePattern.Invoke();
                }
                return;
            Label_00CE:
                pattern = PatternHelper.GetSelectionItemPattern(this.InnerElement, true);
                if (pattern != null)
                {
                    try
                    {
                        pattern.Select();
                    }
                    catch (ElementNotAvailableException)
                    {
                        UiaUtility.RealizeElement(this.InnerElement);
                        pattern.Select();
                    }
                }
                return;
            Label_00FD:
                pattern2 = PatternHelper.GetSelectionItemPattern(this.InnerElement, true);
                if (pattern2 != null)
                {
                    pattern2.Select();
                }
                return;
            Label_0115:
                pattern3 = PatternHelper.GetSelectionItemPattern(this.InnerElement, true);
                if (pattern3 != null)
                {
                    pattern3.RemoveFromSelection();
                }
                return;
            Label_012D:
                pattern4 = PatternHelper.GetExpandCollapsePattern(this.InnerElement, true);
                if (pattern4 != null)
                {
                    pattern4.Expand();
                }
                return;
            Label_0145:
                pattern5 = PatternHelper.GetExpandCollapsePattern(this.InnerElement, true);
                if (pattern5 != null)
                {
                    pattern5.Collapse();
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        protected bool IsReadOnly()
        {
            try
            {
                ValuePattern valuePattern = PatternHelper.GetValuePattern(this.InnerElement);
                if (valuePattern != null)
                {
                    return this.GetPatternValue<bool>(valuePattern, ValuePatternIdentifiers.IsReadOnlyProperty);
                }
                TextPattern textPattern = PatternHelper.GetTextPattern(this.InnerElement);
                if (textPattern != null)
                {
                    return ZappyTaskUtilities.ConvertToType<bool>(textPattern.DocumentRange.GetAttributeValue(TextPattern.IsReadOnlyAttribute));
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (ElementNotAvailableException)
            {
            }
            return false;
        }

        internal UiaElement MorphIntoScrollBar()
        {
            this.CacheProperties();
            this.name = "ScrollBar";
            this.friendlyName = "ScrollBar";
            this.controlTypeName = ControlType.ScrollBar.Name;
            this.className = "Uia.ScrollBar";
            this.runtimeId[this.runtimeId.Length - 1]++;
            return this;
        }

        private bool PreferLabeledByForFriendlyName()
        {
            if (!ControlType.Edit.NameEquals(this.ControlTypeName) && !ControlType.ComboBox.NameEquals(this.ControlTypeName))
            {
                return ControlType.List.NameEquals(this.ControlTypeName);
            }
            return true;
        }

        protected virtual string SanitizeUIAClassName(string uiaClassName) =>
            uiaClassName;

        public override void ScrollProgrammatically(ScrollDirection srollDirection, ScrollAmount scrollAmount)
        {
            try
            {
                if (this.scrollPattern != null)
                {
                    object[] args = new object[] { srollDirection, scrollAmount };
                    
                    if (srollDirection == ScrollDirection.HorizontalScroll)
                    {
                        this.scrollPattern.ScrollHorizontal(UiaUtility.GetScrollAmount(scrollAmount));
                    }
                    else if (srollDirection == ScrollDirection.VerticalScroll)
                    {
                        this.scrollPattern.ScrollVertical(UiaUtility.GetScrollAmount(scrollAmount));
                    }
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        public override void SetFocus()
        {
            try
            {
                this.InnerElement.SetFocus();
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
        }

        public sealed override void SetPropertyValue(string propertyName, object value)
        {
            try
            {
                int num2;
                int num3;
                int num4;
                int pointX = num2 = num3 = num4 = -1;
                this.EnsureVisibleByScrolling(pointX, num2, ref num3, ref num4);
            }
            catch (Exception exception)
            {
                object[] args = new object[] { exception.Message };
                CrapyLogger.log.ErrorFormat("Uia.SetPropertyValue: Error in EnsureVisible {0}, ignoring and setting the value", args);
            }
            this.SetPropertyValueInternal(propertyName, value);
        }

        public virtual void SetPropertyValueInternal(string propertyName, object value)
        {
            try
            {
                bool flag2;
                string str;
                this.ThrowExceptionIfDisabled();
                propertyName = PropertyNames.GetPropertyNameInCorrectCase(propertyName);
                if (propertyName != "Checked")
                {
                    if (propertyName != "Value")
                    {
                        if (propertyName != "Selected")
                        {
                            if (propertyName != "SelectedItem")
                            {
                                throw new NotSupportedException();
                            }
                            goto Label_0088;
                        }
                        goto Label_0073;
                    }
                }
                else
                {
                    bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                    UiaUtility.SetToggleState(this.InnerElement, flag ? ToggleState.On : ToggleState.Off);
                    return;
                }
                this.ThrowExceptionIfReadOnly();
                this.Value = ZappyTaskUtilities.ConvertToType<string>(value);
                return;
            Label_0073:
                flag2 = ZappyTaskUtilities.ConvertToType<bool>(value);
                UiaUtility.SetSelectionItemState(this.InnerElement, flag2);
                return;
            Label_0088:
                str = ZappyTaskUtilities.ConvertToType<string>(value);
                if (PatternHelper.GetExpandCollapsePattern(this.InnerElement, true) != null)
                {
                    this.InvokeProgrammaticAction(ProgrammaticActionOption.Expand);
                }
                UiaUtility.SelectContainerItem(this.InnerElement, this.controlTypeName, str);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, true);
                throw;
            }
        }

        private bool ShouldRefresh<T>(T? propertyValue) where T : struct
        {
            if (this.isCacheMode)
            {
                return !propertyValue.HasValue;
            }
            return true;
        }

        private bool ShouldRefresh(string propertyValue)
        {
            if (this.isCacheMode)
            {
                return string.Equals(propertyValue, "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1", StringComparison.Ordinal);
            }
            return true;
        }

        internal void ThrowExceptionIfDisabled()
        {
            if ((null != this.InnerElement) && !this.InnerElement.Current.IsEnabled)
            {
                throw new Exception();
            }
        }

        internal void ThrowExceptionIfReadOnly()
        {
            if (this.IsReadOnly())
            {
                throw new Exception();
            }
        }

        public override string ToString()
        {
            if (this.runtimeIdString == null)
            {
                this.runtimeIdString = UiaUtility.ArrayToString(this.runtimeId);
            }
            object[] args = new object[] { this.name, this.controlTypeName, this.automationId, this.runtimeIdString };
            return string.Format(CultureInfo.InvariantCulture, "Name [{0}], ControlType [{1}], AutomationId [{2}], RuntimeId [{3}]", args);
        }

        public override void WaitForReady()
        {
            UiaUtility.WaitForReady(this);
        }

        internal UiaElement AncestorHavingWindowHandle
        {
            get
            {
                if (this.ancestorHavingWindowHandle == null)
                {
                    if (this.NativeWindowHandle != IntPtr.Zero)
                    {
                        return this;
                    }
                    System.Windows.Automation.AutomationElement innerElement = this.InnerElement;
                    do
                    {
                        if ((UiaTechnologyManager.Instance != null) && UiaTechnologyManager.Instance.MenuItemStitchMap.ContainsKey(innerElement))
                        {
                            System.Windows.Automation.AutomationElement element2;
                            UiaTechnologyManager.Instance.MenuItemStitchMap.TryGetValue(innerElement, out element2);
                            if (element2 != null)
                            {
                                innerElement = element2;
                            }
                            break;
                        }
                        if (this.parent != null)
                        {
                            this.ancestorHavingWindowHandle = this.parent.AncestorHavingWindowHandle;
                            innerElement = null;
                            break;
                        }
                        innerElement = TreeWalkerHelper.GetParent(innerElement);
                        if (innerElement == null)
                        {
                            CrapyLogger.log.Error("UIA: No ancestor with window handle");
                            break;
                        }
                    }
                    while (UiaUtility.GetNativeWindowHandle(innerElement) == 0);
                    if (innerElement != null)
                    {
                        this.ancestorHavingWindowHandle = UiaElementFactory.GetUiaElement(innerElement, true);
                    }
                }
                return this.ancestorHavingWindowHandle;
            }
        }

        public override System.Windows.Automation.AutomationElement AutomationElement =>
            this.InnerElement;

        internal string AutomationId
        {
            get
            {
                if (this.ShouldRefresh(this.automationId))
                {
                    try
                    {
                        this.automationId = UiaUtility.GetAutomationId(this.InnerElement);
                    }
                    catch (Exception exception)
                    {
                        UiaUtility.MapAndThrowException(exception, this, false);
                        throw;
                    }
                }
                return this.automationId;
            }
        }

        internal System.Windows.Automation.AutomationElement CeilingElement { get; set; }

        public override int ChildIndex
        {
            get
            {
                int num2;
                try
                {
                    object[] args = new object[] { this };
                    
                    int num = -1;
                    System.Windows.Automation.AutomationElement innerElement = this.InnerElement;
                    VirtualizationContext unknown = VirtualizationContext.Unknown;
                    while (innerElement != null)
                    {
                        num++;
                        innerElement = TreeWalkerHelper.GetPreviousSibling(innerElement, ref unknown);
                    }
                    num2 = num;
                }
                catch (Exception exception)
                {
                    UiaUtility.MapAndThrowException(exception, this, false);
                    throw;
                }
                return num2;
            }
        }

        public override string ClassName
        {
            get
            {
                if (string.Equals(this.className, "UnitializedCB3702D1-14B6-4001-8BC7-CD4C22C18BE1", StringComparison.Ordinal))
                {
                    this.InitializeClassName();
                }
                return this.className;
            }
        }

        public override string ControlTypeName =>
            this.controlTypeName;

        internal string FrameworkId { get; private set; }

        public override string FriendlyName
        {
            get
            {
                if (this.ShouldRefresh(this.friendlyName))
                {
                    this.friendlyName = string.Empty;
                    if (UiaUtility.IsWpfWindow(this))
                    {
                        if (this.Equals(this.TopLevelElement))
                        {
                            this.friendlyName = this.Name;
                        }
                        if (string.IsNullOrEmpty(this.friendlyName))
                        {
                            this.friendlyName = Resources.WpfString;
                        }
                    }
                    else if (this.ControlTypeName == ControlType.Cell)
                    {
                        this.friendlyName = this.Value;
                        if (string.IsNullOrEmpty(this.friendlyName))
                        {
                            this.friendlyName = this.Name;
                        }
                    }
                    else
                    {
                        if (this.PreferLabeledByForFriendlyName())
                        {
                            this.friendlyName = this.GetPropertyValue("LabeledBy") as string;
                        }
                        if (string.IsNullOrEmpty(this.friendlyName))
                        {
                            if (!string.IsNullOrEmpty(this.Name) && !DataBoundControlIssue.HasNameIssue(this.InnerElement))
                            {
                                this.friendlyName = this.Name;
                            }
                            else if (this.HasValidAutomationId())
                            {
                                this.friendlyName = this.AutomationId;
                            }
                            else
                            {
                                this.friendlyName = this.GetPropertyValue("HelpText") as string;
                            }
                        }
                    }
                    if ((this.friendlyName == null) && ControlType.TitleBar.NameEquals(this.ControlTypeName))
                    {
                        this.friendlyName = this.Value;
                    }
                    this.friendlyName = ExtensionUtilities.SanitizeFriendlyName(this.friendlyName);
                }
                return this.friendlyName;
            }
        }

        internal virtual System.Windows.Automation.AutomationElement InnerElement { get; set; }

        internal bool IsBoundaryForHostedControl
        {
            get
            {
                bool flag = false;
                if (this.SwitchingElement == null)
                {
                    return flag;
                }
                if (string.Equals(this.Framework, this.SwitchingElement.Framework, StringComparison.OrdinalIgnoreCase))
                {
                    return this.Equals(this.SwitchingElement);
                }
                return true;
            }
        }

        internal bool IsCeilingElement
        {
            get
            {
                if (!this.isCeilingElement.HasValue || !this.isCeilingElement.HasValue)
                {
                    if ((this.InnerElement == null) || (this.CeilingElement == null))
                    {
                        return false;
                    }
                    this.isCeilingElement = false;
                    if (Automation.Compare(this.InnerElement, this.CeilingElement))
                    {
                        this.isCeilingElement = true;
                    }
                }
                return this.isCeilingElement.Value;
            }
        }

        public override bool IsLeafNode
        {
            get
            {
                bool flag;
                try
                {
                    object[] args = new object[] { this };
                    
                    System.Windows.Automation.AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(this.InnerElement);
                    flag = (firstChild == null) || Automation.Compare(this.InnerElement, firstChild);
                }
                catch (Exception exception)
                {
                    UiaUtility.MapAndThrowException(exception, this, false);
                    throw;
                }
                return flag;
            }
        }

        public override bool IsPassword
        {
            get
            {
                if (this.ShouldRefresh<bool>(this.isPassword))
                {
                    try
                    {
                        this.isPassword = new bool?(this.GetAutomationPropertyValue<bool>(AutomationElement.IsPasswordProperty));
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                        if (ControlType.Edit.NameEquals(this.controlTypeName))
                        {
                            this.isPassword = true;
                        }
                        else
                        {
                            this.isPassword = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        UiaUtility.MapAndThrowException(exception, this, false);
                        throw;
                    }
                }
                return this.isPassword.Value;
            }
        }

        public override bool IsTreeSwitchingRequired =>
            string.Equals(this.ClassName, "Internet Explorer_Server", StringComparison.OrdinalIgnoreCase);

        internal bool IsVirtualElement { get; private set; }

        protected virtual bool LazyInitializeClassName =>
            (PatternHelper.GetVirtualizedItemPattern(this.InnerElement) != null);

        public override string Name
        {
            get
            {
                if (this.ShouldRefresh(this.name))
                {
                    string a = null;
                    try
                    {
                        a = this.InnerElement.Current.Name;
                    }
                    catch (ElementNotAvailableException)
                    {
                    }
                    catch (Exception exception)
                    {
                        UiaUtility.MapAndThrowException(exception, this, false);
                        throw;
                    }
                    if (PluginUtilities.CheckForValueAndNameEquals(this.ControlTypeName))
                    {
                        string b = null;
                        try
                        {
                            b = this.Value;
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                        catch (ZappyTaskException)
                        {
                        }
                        if (string.Equals(a, b, StringComparison.Ordinal))
                        {
                            a = null;
                        }
                    }
                    this.name = a;
                }
                if ((this.name == null) && ControlType.TitleBar.NameEquals(this.ControlTypeName))
                {
                    this.name = this.Value;
                }
                return this.name;
            }
        }

        public override object NativeElement =>
            this.InnerElement;

        internal IntPtr NativeWindowHandle
        {
            get
            {
                if (!this.nativeWindowHandle.HasValue)
                {
                    this.nativeWindowHandle = new IntPtr?((IntPtr)this.GetAutomationPropertyValue<int>(AutomationElement.NativeWindowHandleProperty));
                }
                return this.nativeWindowHandle.Value;
            }
            set
            {
                this.nativeWindowHandle = new IntPtr?(value);
            }
        }

        internal virtual UiaElement Parent
        {
            get
            {
                if ((this.parent == null) && !UiaUtility.IsBoundaryElement(this))
                {
                    object[] args = new object[] { this };
                    
                    System.Windows.Automation.AutomationElement parent = TreeWalkerHelper.GetParent(this.InnerElement);
                    if (parent == null)
                    {
                        return null;
                    }
                    this.parent = UiaElementFactory.GetUiaElement(parent, true, this.CeilingElement);
                }
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        public override IQueryElement QueryId
        {
            get
            {
                if (this.queryID == null)
                {
                    try
                    {
                        this.queryID = QueryIdHelper.GetQueryId(this);
                        if ((this.SupportedTopLevelWindow != null) && !UiaUtility.IsChildOfDesktop(this.SupportedTopLevelWindow))
                        {
                            ITaskActivityElement ancestor = this;
                            while (ancestor.QueryId.Ancestor != null)
                            {
                                ancestor = ancestor.QueryId.Ancestor;
                            }
                            if (!Equals(ancestor.NativeElement, this.SupportedTopLevelWindow.NativeElement))
                            {
                                object[] args = new object[] { this.supportedTopLevelWindow };
                                
                                ancestor.QueryId.Ancestor = this.SupportedTopLevelWindow;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        UiaUtility.MapAndThrowException(exception, this, false);
                        throw;
                    }
                }
                if (this.queryID != null)
                {
                    this.AddSearchConfigurations();
                }
                return this.queryID;
            }
        }

        internal IQueryElement QueryIdInternal
        {
            get
            {
                return this.QueryId;
            }
            set
            {
                this.queryID = value;
            }
        }

        private bool ShouldBringControlToView
        {
            get
            {
                if (!UiaUtility.IsElementOffScreen(this.InnerElement))
                {
                    return false;
                }
                if (!UiaUtility.IsImmersiveApplicationWindow(this))
                {
                    return UiaUtility.IsWebView(this);
                }
                return true;
            }
        }

        internal UiaElement SupportedTopLevelWindow
        {
            get
            {
                if (this.supportedTopLevelWindow == null)
                {
                    try
                    {
                        IntPtr windowHandle = this.WindowHandle;
                        IntPtr zero = IntPtr.Zero;
                        while (UiaUtility.IsWindowOfSupportedTechnology(windowHandle, false))
                        {
                            zero = windowHandle;
                            if (UiaUtility.IsPopUpWithTitleBar(zero))
                            {
                                break;
                            }
                            System.Windows.Automation.AutomationElement parent = AutomationElement.FromHandle(windowHandle);
                            int nativeWindowHandle = 0;
                            while (nativeWindowHandle == 0)
                            {
                                parent = TreeWalkerHelper.GetParent(parent);
                                if (parent == null)
                                {
                                    break;
                                }
                                nativeWindowHandle = parent.Current.NativeWindowHandle;
                            }
                            windowHandle = (IntPtr)nativeWindowHandle;
                        }
                        if (zero != IntPtr.Zero)
                        {
                            System.Windows.Automation.AutomationElement automationElement = AutomationElement.FromHandle(zero);
                            this.supportedTopLevelWindow = UiaElementFactory.GetUiaElement(automationElement, true);
                        }
                    }
                    catch (Exception exception)
                    {
                        UiaUtility.MapAndThrowException(exception, null, false);
                        throw;
                    }
                }
                return this.supportedTopLevelWindow;
            }
        }

        public override ITaskActivityElement SwitchingElement { get; set; }

        public override UITechnologyManager TechnologyManager =>
            UiaTechnologyManager.Instance;

        public override string Framework =>
            "UIA";

        public override TaskActivityElement TopLevelElement
        {
            get
            {
                if (((this.topLevelElement == null) && (this.SupportedTopLevelWindow != null)) && UiaUtility.IsChildOfDesktop(this.SupportedTopLevelWindow))
                {
                    this.topLevelElement = this.SupportedTopLevelWindow;
                }
                return this.topLevelElement;
            }
            set
            {
                this.topLevelElement = value;
            }
        }

        public override string Value
        {
            get
            {
                string patternValue = null;
                try
                {
                    if (this.ControlTypeName == ControlType.Row.Name)
                    {
                        bool isPartialValue = false;
                        return DataGridUtility.GetRowValue(this, out isPartialValue);
                    }
                    ValuePattern valuePattern = PatternHelper.GetValuePattern(this.InnerElement);
                    if (valuePattern != null)
                    {
                        patternValue = this.GetPatternValue<string>(valuePattern, ValuePatternIdentifiers.ValueProperty);
                    }
                    if (patternValue == null)
                    {
                        TextPattern textPattern = PatternHelper.GetTextPattern(this.InnerElement);
                        if (textPattern != null)
                        {
                            patternValue = PatternHelper.GetText(textPattern);
                        }
                    }
                    if (patternValue == null)
                    {
                        RangeValuePattern rangeValuePattern = PatternHelper.GetRangeValuePattern(this.InnerElement);
                        if (rangeValuePattern != null)
                        {
                            patternValue = Math.Round(this.GetPatternValue<double>(rangeValuePattern, RangeValuePatternIdentifiers.ValueProperty), 2).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    throw new UnauthorizedAccessException();
                }
                catch (ElementNotAvailableException exception)
                {
                    if ((string.Equals(exception.Source, "UiaComWrapper", StringComparison.OrdinalIgnoreCase) && (exception.InnerException != null)) && (exception.InnerException is InvalidOperationException))
                    {
                        throw new UnauthorizedAccessException();
                    }
                    UiaUtility.MapAndThrowException(exception, this, false);
                    throw;
                }
                catch (Exception exception2)
                {
                    UiaUtility.MapAndThrowException(exception2, this, false);
                    throw;
                }
                return patternValue;
            }
            set
            {
                try
                {
                    double num;
                    this.ThrowExceptionIfDisabled();
                    this.ThrowExceptionIfReadOnly();
                    ValuePattern valuePattern = PatternHelper.GetValuePattern(this.InnerElement);
                    if ((valuePattern != null) && !this.GetPatternValue<bool>(valuePattern, ValuePatternIdentifiers.IsReadOnlyProperty))
                    {
                        try
                        {
                            valuePattern.SetValue(value);
                            return;
                        }
                        catch (InvalidOperationException exception)
                        {
                            object[] args = new object[] { exception.Message };
                            CrapyLogger.log.ErrorFormat("Uia : Exception in ValuePattern.SetValue : {0}.", args);
                            throw new Exception();
                        }
                    }
                    RangeValuePattern rangeValuePattern = PatternHelper.GetRangeValuePattern(this.InnerElement);
                    if (((rangeValuePattern == null) || this.GetPatternValue<bool>(rangeValuePattern, RangeValuePatternIdentifiers.IsReadOnlyProperty)) || !double.TryParse(value, out num))
                    {
                        throw new NotSupportedException();
                    }
                    rangeValuePattern.SetValue(num);
                }
                catch (Exception exception2)
                {
                    UiaUtility.MapAndThrowException(exception2, this, false);
                    throw;
                }
            }
        }

        public override IntPtr WindowHandle
        {
            get
            {
                if (!ZappyTaskUtilities.IsPhone)
                {
                    if (this.NativeWindowHandle != IntPtr.Zero)
                    {
                        return this.NativeWindowHandle;
                    }
                    if (this.AncestorHavingWindowHandle != null)
                    {
                        return this.AncestorHavingWindowHandle.NativeWindowHandle;
                    }
                }
                return IntPtr.Zero;
            }
        }
    }
}

