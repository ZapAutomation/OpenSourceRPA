using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace Zappy.Decode.Mssa
{
    internal static class StringKeys
    {
        public const string A = "a";
        public const string Add = "{Add}";
        public const string Apps = "{Apps}";
        public const string Attn = "{Attn}";
        public const string B = "b";
        public const string Back = "{Back}";
        public const string BrowserBack = "{BrowserBack}";
        public const string BrowserFavorites = "{BrowserFavorites}";
        public const string BrowserForward = "{BrowserForward}";
        public const string BrowserHome = "{BrowserHome}";
        public const string BrowserRefresh = "{BrowserRefresh}";
        public const string BrowserSearch = "{BrowserSearch}";
        public const string BrowserStop = "{BrowserStop}";
        public const string C = "c";
        public const string Cancel = "{Cancel}";
        public const string CapsLock = "{CapsLock}";
        public const string Clear = "{Clear}";
        public static readonly StringComparer Comparer = StringComparer.Ordinal;
        public static readonly StringComparison Comparison = StringComparison.Ordinal;
        public const string ControlKey = "{ControlKey}";
        public const string Crsel = "{Crsel}";
        public const string D = "d";
        public const string D0 = "0";
        public const string D1 = "1";
        public const string D2 = "2";
        public const string D3 = "3";
        public const string D4 = "4";
        public const string D5 = "5";
        public const string D6 = "6";
        public const string D7 = "7";
        public const string D8 = "8";
        public const string D9 = "9";
        public const string Decimal = "{Decimal}";
        public const string Delete = "{Delete}";
        public const string Divide = "{Divide}";
        public const string Down = "{Down}";
        public const string E = "e";
        public const string End = "{End}";
        public const string Enter = "{Enter}";
        public const string EraseEof = "{EraseEof}";
        public const string Escape = "{Escape}";
        public const string Execute = "{Execute}";
        public const string Exsel = "{Exsel}";
        public const string F = "f";
        public const string F1 = "{F1}";
        public const string F10 = "{F10}";
        public const string F11 = "{F11}";
        public const string F12 = "{F12}";
        public const string F13 = "{F13}";
        public const string F14 = "{F14}";
        public const string F15 = "{F15}";
        public const string F16 = "{F16}";
        public const string F17 = "{F17}";
        public const string F18 = "{F18}";
        public const string F19 = "{F19}";
        public const string F2 = "{F2}";
        public const string F20 = "{F20}";
        public const string F21 = "{F21}";
        public const string F22 = "{F22}";
        public const string F23 = "{F23}";
        public const string F24 = "{F24}";
        public const string F3 = "{F3}";
        public const string F4 = "{F4}";
        public const string F5 = "{F5}";
        public const string F6 = "{F6}";
        public const string F7 = "{F7}";
        public const string F8 = "{F8}";
        public const string F9 = "{F9}";
        public const string FinalMode = "{FinalMode}";
        public const string G = "g";
        public const string H = "h";
        public const string HangulMode = "{HangulMode}";
        public const string HanjaMode = "{HanjaMode}";
        public const string Help = "{Help}";
        public const string Home = "{Home}";
        public const string I = "i";
        public const string IMEAccept = "{IMEAccept}";
        public const string IMEConvert = "{IMEConvert}";
        public const string IMEModeChange = "{IMEModeChange}";
        public const string IMENonconvert = "{IMENonconvert}";
        public const string Insert = "{Insert}";
        public const string J = "j";
        public const string JunjaMode = "{JunjaMode}";
        public const string K = "k";
        public const string KanaMode = "{KanaMode}";
        public const string KanjiMode = "{KanjiMode}";
        public const string L = "l";
        public const string LaunchApplication1 = "{LaunchApplication1}";
        public const string LaunchApplication2 = "{LaunchApplication2}";
        public const string LaunchMail = "{LaunchMail}";
        public const string LButton = "{LButton}";
        public const string LControlKey = "{LControlKey}";
        public const string Left = "{Left}";
        public const string LineFeed = "{LineFeed}";
        public const string LMenu = "{LMenu}";
        public const string LShiftKey = "{LShiftKey}";
        public const string LWin = "{LWin}";
        public const string M = "m";
        private static readonly Dictionary<Keys, string> mapOfKeysToString = InitializeKeysToStringMap();
        public static Dictionary<string, Keys> mapOfStringToKeys;

        public const string MButton = "{MButton}";
        public const string MediaNextTrack = "{MediaNextTrack}";
        public const string MediaPlayPause = "{MediaPlayPause}";
        public const string MediaPreviousTrack = "{MediaPreviousTrack}";
        public const string MediaStop = "{MediaStop}";
        public const string Menu = "{Menu}";
        public const string Multiply = "{Multiply}";
        public const string N = "n";
        public const string NoName = "{NoName}";
        public const string NumLock = "{NumLock}";
        public const string NumPad0 = "{NumPad0}";
        public const string NumPad1 = "{NumPad1}";
        public const string NumPad2 = "{NumPad2}";
        public const string NumPad3 = "{NumPad3}";
        public const string NumPad4 = "{NumPad4}";
        public const string NumPad5 = "{NumPad5}";
        public const string NumPad6 = "{NumPad6}";
        public const string NumPad7 = "{NumPad7}";
        public const string NumPad8 = "{NumPad8}";
        public const string NumPad9 = "{NumPad9}";
        public const string O = "o";
        public const string OemBackslash = @"\";
        public const string OemClear = "{OemClear}";
        public const string OemCloseBrackets = "{]}";
        public const string Oemcomma = " =";
        public const string OemMinus = "-";
        public const string OemOpenBrackets = "{[}";
        public const string OemPeriod = ".";
        public const string OemPipe = "|";
        public const string Oemplus = "{+}";
        public const string OemQuestion = "?";
        public const string OemQuotes = "'";
        public const string OemSemicolon = ";";
        public const string Oemtilde = "{~}";
        public const string P = "p";
        public const string Pa1 = "{Pa1}";
        public const string Packet = "{Packet}";
        public const string PageDown = "{PageDown}";
        public const string PageUp = "{PageUp}";
        public const string Pause = "{Pause}";
        public const string Play = "{Play}";
        public const string Print = "{Print}";
        public const string PrintScreen = "{PrintScreen}";
        public const string ProcessKey = "{ProcessKey}";
        public const string Q = "q";
        public const string R = "r";
        public const string RButton = "{RButton}";
        public const string RControlKey = "{RControlKey}";
        public const string Right = "{Right}";
        public const string RMenu = "{RMenu}";
        public const string RShiftKey = "{RShiftKey}";
        public const string RWin = "{RWin}";
        public const string S = "s";
        public const string Scroll = "{Scroll}";
        public const string Select = "{Select}";
        public const string SelectMedia = "{SelectMedia}";
        public const string Separator = "{Separator}";
        public const string ShiftKey = "{ShiftKey}";
        public const string Sleep = "{Sleep}";
        public const string Space = " ";
        public const string SpaceVirtualKey = "{Space}";
        public const string Subtract = "{Subtract}";
        public const string T = "t";
        public const string Tab = "{Tab}";
        public const string U = "u";
        public const string Up = "{Up}";
        public const string V = "v";
        public const string VolumeDown = "{VolumeDown}";
        public const string VolumeMute = "{VolumeMute}";
        public const string VolumeUp = "{VolumeUp}";
        public const string W = "w";
        public const string X = "x";
        public const string XButton1 = "{XButton1}";
        public const string XButton2 = "{XButton2}";
        public const string Y = "y";
        public const string Z = "z";
        public const string Zoom = "{Zoom}";

        private static Dictionary<Keys, string> InitializeKeysToStringMap()
        {

            Dictionary<Keys, string> _dict = new Dictionary<Keys, string>
            {
                {
                    Keys.LButton,
                    "{LButton}"
                },
                {
                    Keys.RButton,
                    "{RButton}"
                },
                {
                    Keys.Cancel,
                    "{Cancel}"
                },
                {
                    Keys.MButton,
                    "{MButton}"
                },
                {
                    Keys.XButton1,
                    "{XButton1}"
                },
                {
                    Keys.XButton2,
                    "{XButton2}"
                },
                {
                    Keys.Back,
                    "{Back}"
                },
                {
                    Keys.Tab,
                    "{Tab}"
                },
                {
                    Keys.LineFeed,
                    "{LineFeed}"
                },
                {
                    Keys.Clear,
                    "{Clear}"
                },
                {
                    Keys.Enter,
                    "{Enter}"
                },
                {
                    Keys.ShiftKey,
                    "{ShiftKey}"
                },
                {
                    Keys.ControlKey,
                    "{ControlKey}"
                },
                {
                    Keys.Menu,
                    "{Menu}"
                },
                {
                    Keys.Pause,
                    "{Pause}"
                },
                {
                    Keys.Capital,
                    "{CapsLock}"
                },
                {
                    Keys.HanguelMode,
                    "{KanaMode}"
                },
                {
                    Keys.JunjaMode,
                    "{JunjaMode}"
                },
                {
                    Keys.FinalMode,
                    "{FinalMode}"
                },
                {
                    Keys.HanjaMode,
                    "{KanjiMode}"
                },
                {
                    Keys.Escape,
                    "{Escape}"
                },
                {
                    Keys.IMEConvert,
                    "{IMEConvert}"
                },
                {
                    Keys.IMENonconvert,
                    "{IMENonconvert}"
                },
                {
                    Keys.IMEAccept,
                    "{IMEAccept}"
                },
                {
                    Keys.IMEModeChange,
                    "{IMEModeChange}"
                },
                {
                    Keys.Space,
                    " "
                },
                {
                    Keys.PageUp,
                    "{PageUp}"
                },
                {
                    Keys.Next,
                    "{PageDown}"
                },
                {
                    Keys.End,
                    "{End}"
                },
                {
                    Keys.Home,
                    "{Home}"
                },
                {
                    Keys.Left,
                    "{Left}"
                },
                {
                    Keys.Up,
                    "{Up}"
                },
                {
                    Keys.Right,
                    "{Right}"
                },
                {
                    Keys.Down,
                    "{Down}"
                },
                {
                    Keys.Select,
                    "{Select}"
                },
                {
                    Keys.Print,
                    "{Print}"
                },
                {
                    Keys.Execute,
                    "{Execute}"
                },
                {
                    Keys.PrintScreen,
                    "{PrintScreen}"
                },
                {
                    Keys.Insert,
                    "{Insert}"
                },
                {
                    Keys.Delete,
                    "{Delete}"
                },
                {
                    Keys.Help,
                    "{Help}"
                },
                {
                    Keys.D0,
                    "0"
                },
                {
                    Keys.D1,
                    "1"
                },
                {
                    Keys.D2,
                    "2"
                },
                {
                    Keys.D3,
                    "3"
                },
                {
                    Keys.D4,
                    "4"
                },
                {
                    Keys.D5,
                    "5"
                },
                {
                    Keys.D6,
                    "6"
                },
                {
                    Keys.D7,
                    "7"
                },
                {
                    Keys.D8,
                    "8"
                },
                {
                    Keys.D9,
                    "9"
                },
                {
                    Keys.A,
                    "a"
                },
                {
                    Keys.B,
                    "b"
                },
                {
                    Keys.C,
                    "c"
                },
                {
                    Keys.D,
                    "d"
                },
                {
                    Keys.E,
                    "e"
                },
                {
                    Keys.F,
                    "f"
                },
                {
                    Keys.G,
                    "g"
                },
                {
                    Keys.H,
                    "h"
                },
                {
                    Keys.I,
                    "i"
                },
                {
                    Keys.J,
                    "j"
                },
                {
                    Keys.K,
                    "k"
                },
                {
                    Keys.L,
                    "l"
                },
                {
                    Keys.M,
                    "m"
                },
                {
                    Keys.N,
                    "n"
                },
                {
                    Keys.O,
                    "o"
                },
                {
                    Keys.P,
                    "p"
                },
                {
                    Keys.Q,
                    "q"
                },
                {
                    Keys.R,
                    "r"
                },
                {
                    Keys.S,
                    "s"
                },
                {
                    Keys.T,
                    "t"
                },
                {
                    Keys.U,
                    "u"
                },
                {
                    Keys.V,
                    "v"
                },
                {
                    Keys.W,
                    "w"
                },
                {
                    Keys.X,
                    "x"
                },
                {
                    Keys.Y,
                    "y"
                },
                {
                    Keys.Z,
                    "z"
                },
                {
                    Keys.LWin,
                    "{LWin}"
                },
                {
                    Keys.RWin,
                    "{RWin}"
                },
                {
                    Keys.Apps,
                    "{Apps}"
                },
                {
                    Keys.Sleep,
                    "{Sleep}"
                },
                {
                    Keys.NumPad0,
                    "{NumPad0}"
                },
                {
                    Keys.NumPad1,
                    "{NumPad1}"
                },
                {
                    Keys.NumPad2,
                    "{NumPad2}"
                },
                {
                    Keys.NumPad3,
                    "{NumPad3}"
                },
                {
                    Keys.NumPad4,
                    "{NumPad4}"
                },
                {
                    Keys.NumPad5,
                    "{NumPad5}"
                },
                {
                    Keys.NumPad6,
                    "{NumPad6}"
                },
                {
                    Keys.NumPad7,
                    "{NumPad7}"
                },
                {
                    Keys.NumPad8,
                    "{NumPad8}"
                },
                {
                    Keys.NumPad9,
                    "{NumPad9}"
                },
                {
                    Keys.Multiply,
                    "{Multiply}"
                },
                {
                    Keys.Add,
                    "{Add}"
                },
                {
                    Keys.Separator,
                    "{Separator}"
                },
                {
                    Keys.Subtract,
                    "{Subtract}"
                },
                {
                    Keys.Decimal,
                    "{Decimal}"
                },
                {
                    Keys.Divide,
                    "{Divide}"
                },
                {
                    Keys.F1,
                    "{F1}"
                },
                {
                    Keys.F2,
                    "{F2}"
                },
                {
                    Keys.F3,
                    "{F3}"
                },
                {
                    Keys.F4,
                    "{F4}"
                },
                {
                    Keys.F5,
                    "{F5}"
                },
                {
                    Keys.F6,
                    "{F6}"
                },
                {
                    Keys.F7,
                    "{F7}"
                },
                {
                    Keys.F8,
                    "{F8}"
                },
                {
                    Keys.F9,
                    "{F9}"
                },
                {
                    Keys.F10,
                    "{F10}"
                },
                {
                    Keys.F11,
                    "{F11}"
                },
                {
                    Keys.F12,
                    "{F12}"
                },
                {
                    Keys.F13,
                    "{F13}"
                },
                {
                    Keys.F14,
                    "{F14}"
                },
                {
                    Keys.F15,
                    "{F15}"
                },
                {
                    Keys.F16,
                    "{F16}"
                },
                {
                    Keys.F17,
                    "{F17}"
                },
                {
                    Keys.F18,
                    "{F18}"
                },
                {
                    Keys.F19,
                    "{F19}"
                },
                {
                    Keys.F20,
                    "{F20}"
                },
                {
                    Keys.F21,
                    "{F21}"
                },
                {
                    Keys.F22,
                    "{F22}"
                },
                {
                    Keys.F23,
                    "{F23}"
                },
                {
                    Keys.F24,
                    "{F24}"
                },
                {
                    Keys.NumLock,
                    "{NumLock}"
                },
                {
                    Keys.Scroll,
                    "{Scroll}"
                },
                {
                    Keys.LShiftKey,
                    "{LShiftKey}"
                },
                {
                    Keys.RShiftKey,
                    "{RShiftKey}"
                },
                {
                    Keys.LControlKey,
                    "{LControlKey}"
                },
                {
                    Keys.RControlKey,
                    "{RControlKey}"
                },
                {
                    Keys.LMenu,
                    "{LMenu}"
                },
                {
                    Keys.RMenu,
                    "{RMenu}"
                },
                {
                    Keys.BrowserBack,
                    "{BrowserBack}"
                },
                {
                    Keys.BrowserForward,
                    "{BrowserForward}"
                },
                {
                    Keys.BrowserRefresh,
                    "{BrowserRefresh}"
                },
                {
                    Keys.BrowserStop,
                    "{BrowserStop}"
                },
                {
                    Keys.BrowserSearch,
                    "{BrowserSearch}"
                },
                {
                    Keys.BrowserFavorites,
                    "{BrowserFavorites}"
                },
                {
                    Keys.BrowserHome,
                    "{BrowserHome}"
                },
                {
                    Keys.VolumeMute,
                    "{VolumeMute}"
                },
                {
                    Keys.VolumeDown,
                    "{VolumeDown}"
                },
                {
                    Keys.VolumeUp,
                    "{VolumeUp}"
                },
                {
                    Keys.MediaNextTrack,
                    "{MediaNextTrack}"
                },
                {
                    Keys.MediaPreviousTrack,
                    "{MediaPreviousTrack}"
                },
                {
                    Keys.MediaStop,
                    "{MediaStop}"
                },
                {
                    Keys.MediaPlayPause,
                    "{MediaPlayPause}"
                },
                {
                    Keys.LaunchMail,
                    "{LaunchMail}"
                },
                {
                    Keys.SelectMedia,
                    "{SelectMedia}"
                },
                {
                    Keys.LaunchApplication1,
                    "{LaunchApplication1}"
                },
                {
                    Keys.LaunchApplication2,
                    "{LaunchApplication2}"
                },
                {
                    Keys.Oem1,
                    ";"
                },
                {
                    Keys.Oemplus,
                    "{+}"
                },
                {
                    Keys.Oemcomma,
                    " ="
                },
                {
                    Keys.OemMinus,
                    "-"
                },
                {
                    Keys.OemPeriod,
                    "."
                },
                {
                    Keys.Oem2,
                    "?"
                },
                {
                    Keys.Oem3,
                    "{~}"
                },
                {
                    Keys.Oem4,
                    "{[}"
                },
                {
                    Keys.Oem5,
                    "|"
                },
                {
                    Keys.Oem6,
                    "{]}"
                },
                {
                    Keys.Oem7,
                    "'"
                },
                {
                    Keys.Oem102,
                    @"\"
                },
                {
                    Keys.ProcessKey,
                    "{ProcessKey}"
                },
                {
                    Keys.Packet,
                    "{Packet}"
                },
                {
                    Keys.Attn,
                    "{Attn}"
                },
                {
                    Keys.Crsel,
                    "{Crsel}"
                },
                {
                    Keys.Exsel,
                    "{Exsel}"
                },
                {
                    Keys.EraseEof,
                    "{EraseEof}"
                },
                {
                    Keys.Play,
                    "{Play}"
                },
                {
                    Keys.Zoom,
                    "{Zoom}"
                },
                {
                    Keys.NoName,
                    "{NoName}"
                },
                {
                    Keys.Pa1,
                    "{Pa1}"
                },
                {
                    Keys.OemClear,
                    "{OemClear}"
                }
            };

            mapOfStringToKeys = new Dictionary<string, Keys>();

            foreach (KeyValuePair<Keys, string> item in _dict)
            {
                mapOfStringToKeys[item.Value] = item.Key;
            }
            return _dict;
        }

        public static bool IsValidKey(Keys key) =>
            mapOfKeysToString.ContainsKey(key);

        public static string KeyToText(Keys key)
        {
            if (IsValidKey(key))
            {
                return mapOfKeysToString[key];
            }
            object[] args = { key };
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Messages.InvalidKeySpecified, args), "key");
        }
    }
}