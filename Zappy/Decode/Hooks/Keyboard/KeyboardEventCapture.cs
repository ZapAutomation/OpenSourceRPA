using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Helper;
using Zappy.Decode.Helper.Enums;
using Zappy.Decode.Hooks.LowLevelHookEvent;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.Decode.Screenshot;
using Zappy.Invoker;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.Keyboard
{
    internal sealed class KeyboardEventCapture : LowLevelHookEventCapture
    {
        private static readonly Keys[] AllModifiers = { Keys.LShiftKey, Keys.RShiftKey, Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LWin, Keys.RWin };
        private Dictionary<IntPtr, Dictionary<VirtualKey, VirtualKey>> deadKeyDictionary;
        private static byte[] keyboardState = new byte[0x100];
        private const uint KLF_NOTELLSHELL = 0x80;
        private char? lastHighSurrogate;
        private TaskActivityElement lastKeyActionElement;
        private const int MaxKeyLength = 10;
        private bool skipNextKeyDown;

        public KeyboardEventCapture(IEventCapture accessoryEventCapture) : base(accessoryEventCapture)
        {
            deadKeyDictionary = new Dictionary<IntPtr, Dictionary<VirtualKey, VirtualKey>>();
        }

        private void AddDeadKeys(IntPtr hkl)
        {
            if (hkl != IntPtr.Zero && !deadKeyDictionary.ContainsKey(hkl))
            {
                List<VirtualKey> virtualKeys = new List<VirtualKey>();
                for (uint i = 1; i <= 0x7f; i++)
                {
                    AddVirtualKey(i, true, hkl, virtualKeys);
                }
                for (uint j = 0x60; j <= 0x69; j++)
                {
                    AddVirtualKey(j, false, hkl, virtualKeys);
                }
                AddVirtualKey(110, false, hkl, virtualKeys);
                AddVirtualKey(0x6f, false, hkl, virtualKeys);
                AddVirtualKey(3, false, hkl, virtualKeys);
                Dictionary<VirtualKey, VirtualKey> dictionary = new Dictionary<VirtualKey, VirtualKey>();
                foreach (VirtualKey key in virtualKeys)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (k != 1 && k != 5)
                        {
                            FillKeyState(keyboardState, (ModifierKeys)k);
                            StringBuilder pwszBuff = new StringBuilder(10);
                            if (NativeMethods.ToUnicodeEx(key.VirtualCode, key.ScanCode, keyboardState, pwszBuff, 10, 0, key.Layout) < 0 && !dictionary.ContainsKey(key))
                            {
                                object[] args = { key.VirtualCode, key.ScanCode, key.Layout };
                                
                                dictionary.Add(key, key);
                            }
                        }
                    }
                }
                deadKeyDictionary.Add(hkl, dictionary);
                FillKeyState(keyboardState, ModifierKeys.None);
                ClearKeyboardBuffer(hkl);
            }
        }

        private static void AddVirtualKey(uint code, bool isScanCode, IntPtr hkl, List<VirtualKey> virtualKeys)
        {
            uint num;
            uint num2;
            if (isScanCode)
            {
                num = code;
                num2 = NativeMethods.MapVirtualKeyEx(num, 1, hkl);
            }
            else
            {
                num2 = code;
                num = NativeMethods.MapVirtualKeyEx(num2, 0, hkl);
            }
            if (num2 != 0 && num != 0)
            {
                virtualKeys.Add(new VirtualKey(num2, num, hkl));
            }
        }

        private void CheckForOrphanDrag(KeyboardAction keyAction)
        {
            bool flag = false;
            if (keyAction.ActionType == KeyActionType.KeyDown && mouseButtonState != MouseButtonState.None)
            {
                if (lastMouseAction == null)
                {
                    object[] args = { mouseButtonState };
                    CrapyLogger.log.ErrorFormat("CheckForOrphanDrag: lastMouseAction is null but mouseButtonState was {0}", args);
                    flag = true;
                }
                else if (!IsButtonDown(lastMouseAction.MouseButton))
                {
                    object[] objArray2 = { lastMouseAction.MouseButton };
                    CrapyLogger.log.ErrorFormat("CheckForOrphanDrag: Missed mouse up for the button {0}", objArray2);
                    flag = true;
                }
            }
            if (flag)
            {
                                ErrorAction element = new ErrorAction(Resources.FailedToRecordMouseAction)
                {
                    ContinueOnError = true
                };
                actions.Push(element);
                ClearActionTracking();
            }
        }

        private static void ClearKeyboardBuffer(IntPtr keyboardLayout)
        {
            StringBuilder pwszBuff = new StringBuilder(10);
            uint wScanCode = NativeMethods.MapVirtualKey(Keys.Decimal, NativeMethods.VirtualKeyMapType.VirtualKeyToScanCode);
            int num2 = 0;
            for (int i = 0; i < 10; i++)
            {
                num2 = NativeMethods.ToUnicodeEx(110, wScanCode, keyboardState, pwszBuff, pwszBuff.Capacity, 0, keyboardLayout);
                if (num2 == 1)
                {
                    return;
                }
            }
            object[] args = { num2 };
            CrapyLogger.log.ErrorFormat("ClearKeyboardBuffer: Unable to clear keyboard layout. Last value returned by ToUnicodeEx = {0}", args);
        }

        private static void FillKeyState(byte[] keyState, ModifierKeys modifierKeys)
        {
            keyState[0x10] = (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None ? (byte)0x80 : (byte)0;
            keyState[0x11] = (modifierKeys & ModifierKeys.Control) != ModifierKeys.None ? (byte)0x80 : (byte)0;
            keyState[0x12] = (modifierKeys & ModifierKeys.Alt) != ModifierKeys.None ? (byte)0x80 : (byte)0;
        }

        private bool GetAlwaysTakeSnapShotFlag(KeyboardAction keyAction) =>
            keyAction != null && keyAction.Key == Keys.Enter && keyAction.ActionType == KeyActionType.KeyDown;

        protected override ProcessType GetProcessType(InputAction inputAction, out int processId)
        {
            processId = 0;
            KeyboardAction keyInfo = inputAction as KeyboardAction;


            if (IsHoverKey(keyInfo))
            {
                return ProcessManager.IgnoreAction(Cursor.Position, true, out processId);
            }
            keyInfo.IsGlobalHotkey = IsGlobalHotKey(keyInfo);
            if (keyInfo.IsGlobalHotkey)
            {
                return ProcessType.None;
            }
            return ProcessManager.IgnoreAction(NativeMethods.GetForegroundWindow(), true, out processId);
        }

                        
                        
        private bool IgnoreKey(KeyboardAction keyInfo)
        {
            if (keyInfo.ModifierKeys != ModifierKeys.None)
            {
                foreach (KeyCombination combination in recorderOptions.IgnoreKeys)
                {
                    if (combination.Modifier == keyInfo.ModifierKeys && combination.Key == keyInfo.Key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsGlobalHotKey(KeyboardAction keyInfo)
        {
            if (keyInfo.ModifierKeys != ModifierKeys.None)
            {
                                if (keyInfo.ModifierKeys == ModifierKeys.Windows)
                {
                    return true;
                }

                return HotKeyHandler.Instance.raiseHotKeyEvents(keyInfo, ApplicationSettingProperties.Instance.EnableZappyShortcuts);
            }
            return false;
        }

        private bool IsHoverKey(KeyboardAction keyInfo) =>
            keyInfo.ModifierKeys == recorderOptions.HoverKey.Modifier && keyInfo.Key == recorderOptions.HoverKey.Key;

        protected override bool IsMessageOfInterest(LowLevelHookMessage message, IntPtr lParam)
        {
            if (message != LowLevelHookMessage.KeyDown && message != LowLevelHookMessage.KeyUp && message != LowLevelHookMessage.SysKeyDown)
            {
                return message == LowLevelHookMessage.SysKeyUp;
            }
            return true;
        }

        private static bool IsModifierKeyPress(KeyboardAction keyInfo)
        {
            if (keyInfo.ActionType == KeyActionType.KeyDown && keyInfo.ModifierKeys != ModifierKeys.None)
            {
                foreach (Keys keys in AllModifiers)
                {
                    if ((keyInfo.Key & keys) != Keys.None)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsRepeatOfLastModifierKey(KeyboardAction keyInfo) =>
            !ReferenceEquals(lastModifierKeyPress, null) && keyInfo.ActionType == KeyActionType.KeyDown && lastModifierKeyPress.ModifierKeys == keyInfo.ModifierKeys && lastModifierKeyPress.Key == keyInfo.Key;

        private static bool IsValidKey(KeyboardAction keyInfo)
        {
            if (!StringKeys.IsValidKey(keyInfo.Key))
            {
                return false;
            }
            return keyInfo.Key >= Keys.Back && keyInfo.Key <= Keys.OemClear || keyInfo.Key == Keys.Cancel;
        }

        private KeyboardAction LowLevelCodeToKeyAction(LowLevelHookMessage message, NativeMethods.KeyboardHookStruct keyboardStruct, out bool skip)
        {
            skip = false;
            KeyboardAction action = new KeyboardAction();
            TimeSpan span = new TimeSpan(WallClock.Now.Ticks);
            action.StartTimestamp = action.EndTimestamp = (long)span.TotalMilliseconds;
            action.ActionType = MapMessageToKeyAction(message);
            action.Key = (Keys)keyboardStruct.vkCode;
            action.ModifierKeys = GetModifiers();
            if (action.Key == Keys.LWin || action.Key == Keys.RWin)
            {
                action.ModifierKeys |= ModifierKeys.Windows;
            }
            if (action.Key == Keys.Packet)
            {
                char scanCode = (char)keyboardStruct.scanCode;
                if (char.IsHighSurrogate(scanCode))
                {
                    object[] args = { keyboardStruct.scanCode };
                    
                    lastHighSurrogate = scanCode;
                    skip = true;
                    return action;
                }
                if (char.IsLowSurrogate(scanCode))
                {
                    if (lastHighSurrogate.HasValue)
                    {
                        object[] objArray2 = { keyboardStruct.scanCode };
                        
                        action.KeyValue = char.ConvertFromUtf32(char.ConvertToUtf32(lastHighSurrogate.Value, scanCode));
                        return action;
                    }
                    object[] objArray3 = { keyboardStruct.scanCode };
                    CrapyLogger.log.ErrorFormat("LowLevelCodeToKeyAction: Received lowsurrogate '{0}' without receiving preceding high surrogate. Skipping this key action.", objArray3);
                    skip = true;
                    return action;
                }
                lastHighSurrogate = null;
                action.KeyValue = char.ConvertFromUtf32(keyboardStruct.scanCode);
                return action;
            }
            if (action.ModifierKeys == ModifierKeys.Windows || (action.ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                action.KeyValue = string.Empty;
                return action;
            }
            action.KeyValue = VirtualCodeToString(keyboardStruct.vkCode, (uint)keyboardStruct.scanCode, action.ActionType, ref skip);
            return action;
        }

                                
        
        public void LowLevelHookHandlerInternal(LowLevelHookMessage message, NativeMethods.KeyboardHookStruct keyboardStruct)
        {
            bool skip = false;
                        KeyboardAction keyInfo = LowLevelCodeToKeyAction(message, keyboardStruct, out skip);
                        
            if (!skip && !IsRepeatOfLastModifierKey(keyInfo))
            {
                if (IsModifierKeyPress(keyInfo))
                {
                    lastModifierKeyPress = keyInfo;
                }
                else
                {
                    lastModifierKeyPress = null;
                }
                if (keyInfo.ActionType != KeyActionType.KeyUp)
                {
                    lastKeyActionElement = null;
                }
                if (!IsValidKey(keyInfo))
                {
                    object[] objArray2 = { keyInfo.Key };
                    
                }
                else if (IgnoreKey(keyInfo))
                {
                    object[] objArray3 = { keyInfo.ModifierKeys, keyInfo.Key };
                    
                    RemoveKeysAction element = new RemoveKeysAction
                    {
                        StartTimestamp = keyInfo.StartTimestamp,
                        EndTimestamp = keyInfo.EndTimestamp
                    };
                    actions.Push(element);
                }
                else
                {
                    CheckForOrphanDrag(keyInfo);
                    if (!IsActionOnIgnoredOrTransparentProcess(keyInfo))
                    {
                        if (keyInfo.ActionType == KeyActionType.KeyDown && IsHoverKey(keyInfo))
                        {
                            RecordMouseHover();
                        }
                        else if (keyInfo.ActionType == KeyActionType.KeyUp)
                        {
                            object[] objArray4 = { keyInfo.ActionType };
                            
                            keyInfo.ActivityElement = lastKeyActionElement;
                            actions.Push(keyInfo);
                        }
                        else
                        {
                                                                                    if (!keyInfo.IsGlobalHotkey)
                            {
                                if (ZappyTaskUtilities.IsImageActionLogEnabled && GetAlwaysTakeSnapShotFlag(keyInfo))
                                {
                                    Screen activeScreen = Screen.FromHandle(NativeMethods.GetForegroundWindow());
                                    SnapshotGenerator.Instance.StartSnapshotOfScreenTask(activeScreen);
                                }
                                UpdateActionWithFocusedElement(keyInfo);
                                lastKeyActionElement = keyInfo.ActivityElement;
                            }
                            else
                            {
                                lastKeyActionElement = null;
                            }
                                                        UpdateAndEnqueueKeyboardAction(keyInfo);
                        }
                    }
                }
            }
        }

        private static KeyActionType MapMessageToKeyAction(LowLevelHookMessage message)
        {
            switch (message)
            {
                case LowLevelHookMessage.KeyDown:
                case LowLevelHookMessage.SysKeyDown:
                    return KeyActionType.KeyDown;

                case LowLevelHookMessage.KeyUp:
                case LowLevelHookMessage.SysKeyUp:
                    return KeyActionType.KeyUp;
            }
            throw new ArgumentOutOfRangeException("message");
        }

        private void RecordMouseHover()
        {
            Point position = Cursor.Position;
            MouseAction mouseAction = new MouseAction(MouseButtons.Left, MouseActionType.Hover, position);
            mouseAction.AbsoluteLocation = mouseAction.Location;
            UpdateActionWithCursorElement(mouseAction, position);
                        
            actions.Push(new RemoveKeysAction());
            actions.Push(mouseAction);
        }

        private void UpdateAndEnqueueKeyboardAction(KeyboardAction keyAction)
        {
            ZappyTaskImageInfo info;
            if (keyAction.Key == Keys.Enter && recorderOptions.ImeLanguageList.Count > 0 && AggregatorUtilities.IsActionOnControlType(keyAction, ControlType.Edit))
            {
                int lcidFromWindowHandle = ZappyTaskUtilities.GetLcidFromWindowHandle(keyAction.ActivityElement.WindowHandle);
                if (recorderOptions.ImeLanguageList.Contains(lcidFromWindowHandle))
                {
                    keyAction.Tags.Add(RecorderUtilities.ImeLanguageTag, RecorderUtilities.ImeLanguageTag);
                }
            }
            Point interactionPoint = ZappyTaskUtilities.IsImageActionLogEnabled ? RecorderUtilities.GetDefaultInteractionPoint(keyAction.ActivityElement) : new Point();
            accessoryEventCapture.SetTrackingElement(keyAction.ActivityElement, interactionPoint, GetAlwaysTakeSnapShotFlag(keyAction));
            if (keyAction.ActivityElement != null && (info = SnapshotGenerator.Instance.LastSnapshot) != null && info.ImagePath != null)
            {
                keyAction.ImageEntry = new ZappyTaskImageEntry(info, interactionPoint);
            }
            object[] args = { keyAction, keyAction.ActivityElement };
            
            actions.Push(keyAction);
        }

        private string VirtualCodeToString(int virtualCode, uint scanCode, KeyActionType actionType, ref bool skipCurrentAction)
        {
            uint num;
            string str = string.Empty;
            uint windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out num);
            object[] args = { num, windowThreadProcessId };
                                    IntPtr keyboardLayout = NativeMethods.GetKeyboardLayout(windowThreadProcessId);
            AddDeadKeys(keyboardLayout);
            if (!NativeMethods.GetKeyboardState(keyboardState))
            {
                CrapyLogger.log.ErrorFormat("GetKeyboardState returned false");
                return string.Empty;
            }
            if (deadKeyDictionary.ContainsKey(keyboardLayout) && deadKeyDictionary[keyboardLayout].ContainsKey(new VirtualKey((uint)virtualCode, scanCode, keyboardLayout)))
            {
                skipCurrentAction = true;
                skipNextKeyDown = true;
                return string.Empty;
            }
            if (skipNextKeyDown)
            {
                if (actionType == KeyActionType.KeyDown)
                {
                    skipNextKeyDown = false;
                }
                skipCurrentAction = true;
                return string.Empty;
            }
            StringBuilder pwszBuff = new StringBuilder(10);
            int num3 = NativeMethods.ToUnicodeEx((uint)virtualCode, scanCode, keyboardState, pwszBuff, pwszBuff.Capacity, 0, keyboardLayout);
            if (num3 > 0)
            {
                pwszBuff.Length = num3;
                return pwszBuff.ToString();
            }
            if (num3 < 0)
            {
                object[] objArray2 = { virtualCode, scanCode, keyboardLayout };
                CrapyLogger.log.ErrorFormat("VirtualCodeToString: Received a dead key with vcode={0}, scode={1}, keyboardlayout={2}." + objArray2);
            }
            return str;
        }

        protected override int LowLevelHookId =>
            13;

        internal class VirtualKey
        {
            public VirtualKey(uint virtualCode, uint scanCode, IntPtr layout)
            {
                VirtualCode = virtualCode;
                ScanCode = scanCode;
                Layout = layout;
            }

            public override bool Equals(object obj)
            {
                VirtualKey key = obj as VirtualKey;
                if (key == null)
                {
                    return base.Equals(obj);
                }
                return ScanCode == key.ScanCode && VirtualCode == key.VirtualCode && Layout == key.Layout;
            }

            public override int GetHashCode() =>
                ScanCode.GetHashCode() ^ VirtualCode.GetHashCode() ^ Layout.GetHashCode();

            public IntPtr Layout { get; private set; }

            public uint ScanCode { get; private set; }

            public uint VirtualCode { get; private set; }
        }
    }

}
