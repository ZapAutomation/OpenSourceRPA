using System;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;

namespace Zappy.Plugins.Uia.Uia.XAMLControls
{
    [Serializable]
#if COMENABLED
    [ComVisible(true), Guid("51806001-0BF1-4209-A386-CE0052C4726A")]
#endif
    public class XamlElement : UiaElement
    {
        public XamlElement(AutomationElement automationElement) : base(automationElement)
        {
        }

        internal XamlElement(AutomationElement automationElement, string controlTypeName) : base(automationElement, controlTypeName)
        {
        }

        public override object GetPropertyValue(string propertyName)
        {
            object obj2;
            try
            {
                AutomationElement element;
                propertyName = PropertyNames.GetPropertyNameInCorrectCase(propertyName);
                if (propertyName != "CanToggle")
                {
                    if (propertyName == "IsHeaderInteractable")
                    {
                        goto Label_0037;
                    }
                    goto Label_0079;
                }
                return InnerElement.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty);
            Label_0037:
                element = TreeWalkerHelper.GetFirstChild(InnerElement);
                if (element != null && string.Equals(element.Current.AutomationId, "HeaderButton", StringComparison.Ordinal))
                {
                    return true;
                }
                return false;
            Label_0079:
                obj2 = base.GetPropertyValue(propertyName);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, false);
                throw;
            }
            return obj2;
        }

        public override void SetPropertyValueInternal(string propertyName, object value)
        {
            try
            {
                int num;
                int[] numArray;
                string[] strArray;
                double num2;
                ThrowExceptionIfDisabled();
                propertyName = PropertyNames.GetPropertyNameInCorrectCase(propertyName);
                if (propertyName != "Indeterminate")
                {
                    if (propertyName == "SelectedIndex")
                    {
                        goto Label_008B;
                    }
                    if (propertyName == "Expanded")
                    {
                        goto Label_00BF;
                    }
                    if (propertyName == "SelectedIndices")
                    {
                        goto Label_00E3;
                    }
                    if (propertyName == "SelectedItems")
                    {
                        goto Label_00F8;
                    }
                    if (propertyName == "Position")
                    {
                        goto Label_010F;
                    }
                    goto Label_0126;
                }
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                UiaUtility.SetToggleState(InnerElement, flag ? ToggleState.Indeterminate : ToggleState.Off);
                return;
            Label_008B:
                num = ZappyTaskUtilities.ConvertToType<int>(value);
                if (PatternHelper.GetExpandCollapsePattern(InnerElement, true) != null)
                {
                    InvokeProgrammaticAction(ProgrammaticActionOption.Expand);
                }
                UiaUtility.SelectContainerItem(InnerElement, ControlTypeName, num);
                return;
            Label_00BF:
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    InvokeProgrammaticAction(ProgrammaticActionOption.Expand);
                }
                else
                {
                    InvokeProgrammaticAction(ProgrammaticActionOption.Collapse);
                }
                return;
            Label_00E3:
                numArray = value as int[];
                UiaUtility.SelectListBoxItems(InnerElement, numArray);
                return;
            Label_00F8:
                strArray = value as string[];
                UiaUtility.SelectListBoxItems(InnerElement, strArray);
                return;
            Label_010F:
                num2 = ZappyTaskUtilities.ConvertToType<double>(value);
                UiaUtility.SetSliderPostion(InnerElement, num2);
                return;
            Label_0126:
                base.SetPropertyValueInternal(propertyName, value);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, this, true);
                throw;
            }
        }

        public override string Value
        {
            get
            {
                string str = base.Value;
                if (string.Equals(ClassName, "RichEditBox", StringComparison.OrdinalIgnoreCase))
                {
                    int length = str.Length;
                    if (length > 0 && str[length - 1] == '\r')
                    {
                        str = str.Remove(length - 1);
                    }
                }
                return str;
            }
            set
            {
                if (string.Equals(ClassName, "RichEditBox", StringComparison.OrdinalIgnoreCase))
                {
                    AutomationElement firstChild = TreeWalker.RawViewWalker.GetFirstChild(InnerElement);
                    if (firstChild != null && firstChild.Current.ControlType == ControlType.Edit)
                    {
                        ValuePattern valuePattern = PatternHelper.GetValuePattern(firstChild);
                        if (valuePattern == null || GetPatternValue<bool>(valuePattern, ValuePatternIdentifiers.IsReadOnlyProperty))
                        {
                            goto Label_008D;
                        }
                        try
                        {
                            valuePattern.SetValue(value);
                            return;
                        }
                        catch (InvalidOperationException exception)
                        {
                            object[] args = { exception.Message };
                            CrapyLogger.log.ErrorFormat("Uia : Exception in ValuePattern.SetValue : {0}.", args);
                            throw new Exception();
                        }
                    }
                    CrapyLogger.log.ErrorFormat("Uia : Exception in ValuePattern.SetValue : RichEditBox : Child Edit control is null");
                }
            Label_008D:
                base.Value = value;
            }
        }
    }
}

