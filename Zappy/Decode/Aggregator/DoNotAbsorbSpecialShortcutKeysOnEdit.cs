using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.Decode.Aggregator
{
    internal class DoNotAbsorbSpecialShortcutKeysOnEdit : ActionFilter
    {
        private static Dictionary<string, string> nonSpecialAltShortcuts;
        private static Dictionary<string, string> nonSpecialCtrlShortcuts;
        private static Dictionary<string, string> specialNoneShortcuts;

        public DoNotAbsorbSpecialShortcutKeysOnEdit() : base("DoNotAbsorbSpecialShortcutKeysOnEdit", ZappyTaskActionFilterType.Unary, false, "SetValueAggregators")
        {
            InitializeDictionaries();
        }

        private static void InitializeDictionaries()
        {
            if (specialNoneShortcuts == null)
            {
                specialNoneShortcuts = new Dictionary<string, string>(StringComparer.Ordinal);
                nonSpecialAltShortcuts = new Dictionary<string, string>(StringComparer.Ordinal);
                nonSpecialCtrlShortcuts = new Dictionary<string, string>(StringComparer.Ordinal);
                specialNoneShortcuts.Add("{Print}", "{Print}");
                specialNoneShortcuts.Add("{Execute}", "{Execute}");
                specialNoneShortcuts.Add("{PrintScreen}", "{PrintScreen}");
                specialNoneShortcuts.Add("{Help}", "{Help}");
                specialNoneShortcuts.Add("{Apps}", "{Apps}");
                specialNoneShortcuts.Add("{Sleep}", "{Sleep}");
                specialNoneShortcuts.Add("{F1}", "{F1}");
                specialNoneShortcuts.Add("{F2}", "{F2}");
                specialNoneShortcuts.Add("{F3}", "{F3}");
                specialNoneShortcuts.Add("{F4}", "{F4}");
                specialNoneShortcuts.Add("{F5}", "{F5}");
                specialNoneShortcuts.Add("{F6}", "{F6}");
                specialNoneShortcuts.Add("{F7}", "{F7}");
                specialNoneShortcuts.Add("{F8}", "{F8}");
                specialNoneShortcuts.Add("{F9}", "{F9}");
                specialNoneShortcuts.Add("{F10}", "{F10}");
                specialNoneShortcuts.Add("{F11}", "{F11}");
                specialNoneShortcuts.Add("{F12}", "{F12}");
                specialNoneShortcuts.Add("{F13}", "{F13}");
                specialNoneShortcuts.Add("{F14}", "{F14}");
                specialNoneShortcuts.Add("{F15}", "{F15}");
                specialNoneShortcuts.Add("{F16}", "{F16}");
                specialNoneShortcuts.Add("{F17}", "{F17}");
                specialNoneShortcuts.Add("{F18}", "{F18}");
                specialNoneShortcuts.Add("{F19}", "{F19}");
                specialNoneShortcuts.Add("{F20}", "{F20}");
                specialNoneShortcuts.Add("{F21}", "{F21}");
                specialNoneShortcuts.Add("{F22}", "{F22}");
                specialNoneShortcuts.Add("{F23}", "{F23}");
                specialNoneShortcuts.Add("{F24}", "{F24}");
                specialNoneShortcuts.Add("{BrowserRefresh}", "{BrowserRefresh}");
                specialNoneShortcuts.Add("{BrowserStop}", "{BrowserStop}");
                specialNoneShortcuts.Add("{BrowserSearch}", "{BrowserSearch}");
                specialNoneShortcuts.Add("{BrowserFavorites}", "{BrowserFavorites}");
                specialNoneShortcuts.Add("{VolumeMute}", "{VolumeMute}");
                specialNoneShortcuts.Add("{VolumeDown}", "{VolumeDown}");
                specialNoneShortcuts.Add("{VolumeUp}", "{VolumeUp}");
                specialNoneShortcuts.Add("{MediaNextTrack}", "{MediaNextTrack}");
                specialNoneShortcuts.Add("{MediaPreviousTrack}", "{MediaPreviousTrack}");
                specialNoneShortcuts.Add("{MediaStop}", "{MediaStop}");
                specialNoneShortcuts.Add("{MediaPlayPause}", "{MediaPlayPause}");
                specialNoneShortcuts.Add("{LaunchMail}", "{LaunchMail}");
                specialNoneShortcuts.Add("{SelectMedia}", "{SelectMedia}");
                specialNoneShortcuts.Add("{LaunchApplication1}", "{LaunchApplication1}");
                specialNoneShortcuts.Add("{LaunchApplication2}", "{LaunchApplication2}");
                specialNoneShortcuts.Add("{Play}", "{Play}");
                nonSpecialAltShortcuts.Add("{Back}", "{Back}");
                nonSpecialAltShortcuts.Add("{Home}", "{Home}");
                nonSpecialAltShortcuts.Add("{Left}", "{Left}");
                nonSpecialAltShortcuts.Add("{Up}", "{Up}");
                nonSpecialAltShortcuts.Add("{Right}", "{Right}");
                nonSpecialAltShortcuts.Add("{Down}", "{Down}");
                nonSpecialCtrlShortcuts.Add("{PageUp}", "{PageUp}");
                nonSpecialCtrlShortcuts.Add("{PageDown}", "{PageDown}");
                nonSpecialCtrlShortcuts.Add("{End}", "{End}");
                nonSpecialCtrlShortcuts.Add("{Home}", "{Home}");
                nonSpecialCtrlShortcuts.Add("{Left}", "{Left}");
                nonSpecialCtrlShortcuts.Add("{Up}", "{Up}");
                nonSpecialCtrlShortcuts.Add("{Right}", "{Right}");
                nonSpecialCtrlShortcuts.Add("{Down}", "{Down}");
                nonSpecialCtrlShortcuts.Add("{Insert}", "{Insert}");
                nonSpecialCtrlShortcuts.Add("{Delete}", "{Delete}");
                nonSpecialCtrlShortcuts.Add("a", "a");
                nonSpecialCtrlShortcuts.Add("c", "c");
                nonSpecialCtrlShortcuts.Add("v", "v");
                nonSpecialCtrlShortcuts.Add("x", "x");
                nonSpecialCtrlShortcuts.Add("y", "y");
                nonSpecialCtrlShortcuts.Add("z", "z");
                nonSpecialCtrlShortcuts.Add("{F4}", "{F4}");
            }
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            SendKeysAction action = actions.Peek() as SendKeysAction;
            return action != null && AggregatorUtilities.IsActionOnControlType(action, ControlType.Edit);
        }

        private static bool IsSpecialShortcutKey(SendKeysAction sendKeys)
        {
            if (sendKeys.IsGlobalHotkey)
            {
                return true;
            }
            if (sendKeys.ModifierKeys == ModifierKeys.None || sendKeys.ModifierKeys == ModifierKeys.Shift)
            {
                return specialNoneShortcuts.ContainsKey(sendKeys.Text);
            }
            if (sendKeys.ModifierKeys == ModifierKeys.Control || sendKeys.ModifierKeys == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                return !nonSpecialCtrlShortcuts.ContainsKey(sendKeys.Text);
            }
            return sendKeys.ModifierKeys != ModifierKeys.Alt && sendKeys.ModifierKeys != (ModifierKeys.Shift | ModifierKeys.Alt) || !nonSpecialAltShortcuts.ContainsKey(sendKeys.Text);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SendKeysAction sendKeys = actions.Peek() as SendKeysAction;
            if (IsSpecialShortcutKey(sendKeys))
            {
                sendKeys.AdditionalInfo = "DoNotAggregate";
            }
            return false;
        }
    }
}

