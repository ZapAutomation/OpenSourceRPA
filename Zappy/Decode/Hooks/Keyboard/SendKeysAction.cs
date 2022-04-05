
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.Invoker;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;


namespace Zappy.Decode.Hooks.Keyboard
{
    [Serializable]
    public class SendKeysAction : InputAction, IUserTextAction
    {
                private const char closingEscapeSequence = '}';
                private bool hasNonPrintableChars;
        private const char openingEscapeSequence = '{';

        private DynamicTextProperty _text = string.Empty;

        public SendKeysAction()
        {
        }

        public SendKeysAction(TaskActivityElement element) : base(element)
        {
        }

        [Category("Optional")] public int DelayBetweenKeyboardCharacters { get; set; }

        [Category("Input")]

        public bool IsActionOnProtectedElement()
        {
            string y = null;
            try
            {
                y = Text;
            }
            catch (Exception)
            {
                return true;
            }
            return (ActivityElement != null && ActivityElement.IsPassword)                 && !StringKeys.Comparer.Equals("{Tab}", y)
                && !StringKeys.Comparer.Equals("{Enter}", y) && !StringKeys.Comparer.Equals("{Escape}", y);
        }

        [XmlIgnore]
        [Browsable(false)]
        public override string AdditionalInfo
        {
            get
            {
                if (!hasNonPrintableChars)
                {
                    return "Printable";
                }
                string additionalInfo = base.AdditionalInfo;
                if (string.IsNullOrEmpty(additionalInfo))
                {
                    additionalInfo = "NonPrintable";
                }
                return additionalInfo;
            }
            set
            {
                base.AdditionalInfo = value;
                if (string.Equals(value, "Printable", StringComparison.OrdinalIgnoreCase))
                {
                    hasNonPrintableChars = false;
                }
                else
                {
                    hasNonPrintableChars = true;
                }
                NotifyPropertyChanged("AdditionalInfo");
            }
        }
        [Category("Input")]

        public DynamicTextProperty Text
        {
            get => _text; set
            {
                _text = value;
                setClipboardData();
            }
        }

        [Browsable(false)]
        public string ClipboardData { get; set; }

        [Category("Internals")]
        public KeyboardActionTypeEnum KeyboardActionType { get; set; }


        [XmlIgnore, System.ComponentModel.Browsable(false)]
        public override string ValueAsString
        {
            get =>
                Text;
            set
            {
                Text.Value = value;
                                                                            }
        }

                private void setClipboardData()
        {
            try
            {
                if (this.ModifierKeys == ModifierKeys.Control)
                {
                    if (Text.Value == "c")
                    {
                        KeyboardActionType = KeyboardActionTypeEnum.Copy;
                        this.ClipboardData = Clipboard.GetText();
                    }
                    else if(Text.Value == "v")
                    {
                        KeyboardActionType = KeyboardActionTypeEnum.Paste;
                        this.ClipboardData = Clipboard.GetText();
                    }
                }
            }
            catch
            {
            }
        }

        public void BeforeMLSerialization()
        {
            if (Text != null)
                Text.BeforeMLSerialization();
        }

        public void AfterMLSerialization()
        {
            if (Text != null)
                Text.AfterMLSerialization();
        }

        public void Cleanup()
        {
            if (Text != null)
                Text.Cleanup();
        }
        internal override void ShallowCopy(ZappyTaskAction source, bool isSeparateAction)
        {
            base.ShallowCopy(source, isSeparateAction);
            KeyboardAction action = source as KeyboardAction;
            if (isSeparateAction)
            {
                SendKeysAction action2 = source as SendKeysAction;
                if (action2 != null)
                {
                    ValueAsString = action2.ValueAsString;
                                    }
            }
            if (action != null)
            {
                if (action.IsControl())
                {
                    Text = StringKeys.KeyToText(action.Key);
                    hasNonPrintableChars = true;
                }
                else if (KeysEncoder.IsKeyWithAmbiguousUnicode(action.Key))
                {
                    Text = StringKeys.KeyToText(action.Key);
                    hasNonPrintableChars = false;
                }
                else
                {
                    Text = KeysEncoder.EscapeIfSpecialChar(action.KeyValue);
                    hasNonPrintableChars = false;
                }
            }
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            updateWindowAndTaskActivityIdentifier();

            ZappyTaskControl uITaskControl =
                UIActionInterpreter.GetZappyTaskControl(this.TaskActivityIdentifier, WindowIdentifier);
            
                        
            string text = Text.Value;

            if (string.IsNullOrEmpty(text))
                return;

            if (text.Contains(CrapyConstants.pasteChar))
            {
                string _BeforeReplace = text;
                string clipBoardChar = string.Empty;
                if (CommonProgram.GetTextFromClipboard(out clipBoardChar))
                    text = text.Replace(CrapyConstants.pasteChar, clipBoardChar);
                else
                    throw new Exception("Unable to read clipboard contents!");

                            }
            List<string> windowTitles = new List<string>();
            int size = 0;
            if (!ApplicationSettingProperties.Instance.EnableComPlayback)
            {
                if (WindowIdentifier.TopLevelWindows[0].Condition.GetPropertyValue("FrameworkId") as string == "WPF")
                {
                    IntPtr WindowHandle =
                        FocusElement.GetMainWindowHandle(WindowIdentifier.TopLevelWindows[0].WindowTitles[0]);

                    ITaskActivityElement focusElement =
                        FocusElement.GetFocusELement(WindowIdentifier, WindowHandle);
                    FocusElement.ShowIfMinimized(WindowHandle);
                    NativeMethods.SetForegroundWindow(WindowHandle);
                    focusElement.SetFocus();
                }
                else
                {
                    windowTitles = FocusElement.GetWindowTitles(WindowIdentifier);
                    size = windowTitles.Count;
                }
            }
            int result = 0;
            if (uITaskControl == null)
            {
                if (ApplicationSettingProperties.Instance.EnableComPlayback)
                    ExecuteTask.Helpers.Keyboard.SendKeys(text, this.ModifierKeys, false);                 else
                {
                    result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.sendKeys(text, Convert.ToInt32(this.ModifierKeys),
                        windowTitles.ToArray(), size, DelayBetweenKeyboardCharacters);
                    if (result != 0)
                        throw new Exception("Could not find " + windowTitles[result - 1]);
                }
            }
            else
            {
                if (ApplicationSettingProperties.Instance.EnableComPlayback)
                    ExecuteTask.Helpers.Keyboard.SendKeys(uITaskControl, text, this.ModifierKeys,
                        false);                 else
                {
                    result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.sendKeys(text, Convert.ToInt32(this.ModifierKeys),
                        windowTitles.ToArray(), size, DelayBetweenKeyboardCharacters);
                    if (result != 0)
                        throw new Exception("Could not find " + windowTitles[result - 1]);
                }

            }
        }

        private static class KeysEncoder
        {
            private static readonly Keys[] keysWithAmbiguousUnicode = InitializeKeysWithAmbiguousUnicode();
            private static readonly Regex regexForEscapedString = new Regex("{(?<EscapedString>.[^}{]*)}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            private static readonly char[] specialChars = InitializeSpecialChars();

            public static string EscapeIfSpecialChar(string input)
            {
                if (input.Length == 1 && Array.BinarySearch(specialChars, input[0]) >= 0)
                {
                    object[] args = { input[0] };
                    return string.Format(CultureInfo.InvariantCulture, "{{{0}}}", args);
                }
                return input;
            }


            private static Keys[] InitializeKeysWithAmbiguousUnicode()
            {
                Keys[] array = { Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Multiply, Keys.Add, Keys.Subtract, Keys.Decimal, Keys.Divide };
                Array.Sort(array);
                return array;
            }

            private static char[] InitializeSpecialChars()
            {
                char[] array = { '+', '^', '%', '#', '~', '(', ')', '{', '}', '[', ']' };
                Array.Sort(array);
                return array;
            }

            public static bool IsKeyWithAmbiguousUnicode(Keys key) =>
                Array.BinarySearch(keysWithAmbiguousUnicode, key) >= 0;
        }

        public override string AuditInfo()
        {
            string text = string.Empty;
            if (this.ModifierKeys == ModifierKeys.Control)
            {
                if (Text.Equals("c"))
                {
                    text = "${Copy}";
                }
                else if (Text.Equals("v"))
                {
                    text = "${Paste}";
                }
                else if (Text.Equals("a"))
                {
                    text = "${SelectAll}";
                }

            }
            else
            {

                                text = this.Text.Value.Length > 3 ? this.Text.Value.Substring(0, 3) + ".." : this.Text.Value; ;
            }

            return ActionName + " Text:" + text;
        }

    }
}