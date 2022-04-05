using System.Runtime.InteropServices;

namespace Zappy.ExecuteTask.Helpers.Interface
{
    #if COMENABLED
    [ComImport, Guid("FB04D4BF-E450-4ED8-A2A9-C32F7876F6C2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    internal interface IScreenElement
    {
        [DispId(1)]
        string IdentificationString { get; }
        [DispId(2)]
        IScreenElement Parent { get; }
        [DispId(3)]
        void LeftButtonClick(int x, int y, int fEnsureVisible, string bstrKeyModifiers);
        [DispId(4)]
        void DoubleClick(int x, int y, int nButton, int fEnsureVisible, string bstrKeyModifiers);
        [DispId(5)]
        void RightButtonClick(int x, int y, int fEnsureVisible, string bstrKeyModifiers);
        [DispId(6)]
        void MouseButtonClick(int x, int y, int nButton, int fEnsureVisible, string bstrKeyModifiers);
        [DispId(7)]
        void Expand(int nExpandCollapseFlag);
        [DispId(8)]
        void Check(int nCheckUncheckFlag);
        [DispId(9)]
        void Collapse(int nExpandCollapseFlag);
        [DispId(10)]
        void EnsureVisible(int nEnsureVisibleFlag, int x, int y, IScrollerCallback pScrollerCallback, int nScrollFlag, int nMaximumContainers);
        [DispId(11)]
        void Select(int nSelectionFlag, bool fVerify);
        [DispId(12)]
        void SendKeys(string bstrKeys, int nKeyboardAction, int nSendKeysFlag);
        [DispId(13)]
        void SetValueAsComboBox(string bstrNewValue, int nSetValueAsComboBoxFlag);
        [DispId(14)]
        void SetValueAsEditBox(string bstrNewValue, int nSetValueAsEditBoxFlag);
        [DispId(15)]
        void StartDragging(int x, int y, int nDraggingButton, string bstrKeyModifiers, int fEnsureVisible);
        [DispId(0x10)]
        void StopDragging(int x, int y, int nSpeedPixelPerSecond);
        [DispId(0x11)]
        void MoveMouse(int x, int y, int fEnsureVisible, int nSpeedPixelPerSecond);
        [DispId(0x12)]
        void MouseWheel(int nDelta, string bstrKeyModifiers, int fSetMousePos);
        [DispId(0x13)]
        void Uncheck(int nCheckUncheckFlag);
        [DispId(20)]
        void WaitForReady();
        [DispId(0x15)]
        void BringUp();
        [DispId(0x16)]
        IScreenElement FindScreenElementEx(string bstrQueryId, ref object pvarResKeys, int cResKeys, int nMaxDepth);
        [DispId(0x17)]
        object[] FindAllDescendants(string bstrQueryId, ref object pvarResKeys, int cResKeys, int nMaxDepth);
        [DispId(0x18)]
        void GetClickablePoint(out int x, out int y);
        [DispId(0x19)]
        void SetValueAsSlider(string bstrNewValue, int nOrientation);
        [DispId(0x1a)]
        void GetBoundingRectangle(out int pnLeft, out int pnTop, out int pnWidth, out int pnHeight);
        [DispId(0x1b)]
        ITaskActivityElement TechnologyElement { get; }
        [DispId(0x1c)]
        void SetFocus();
        [DispId(0x1d)]
        void DoSelectByMouseClick(object[] pArray, string bstrModifierKey);
        [DispId(30)]
        int Equals(IScreenElement pScreenElement);
        [DispId(0x1f)]
        object GetOption(int nPlaybackOption);
        [DispId(0x20)]
        void SetOption(int nPlaybackOption, object varPlaybackOptionValue);
        [DispId(0x21)]
        bool MatchesQueryId(string bstrQueryId);
        [DispId(0x26)]
        void PressRelease(int x, int y, int nPressReleaseFlag, bool fEnsureVisible, int nPressure);
        [DispId(0x27)]
        void Tap(int x, int y, bool ensureVisible, int nPressure);
        [DispId(40)]
        void DoubleTap(int x, int y, bool ensureVisible, int nPressure);
        [DispId(0x29)]
        void PressAndHold(int x, int y, int duration, bool ensureVisible, int nPressure);
        [DispId(0x2a)]
        void Flick(int x, int y, double nDirection, uint nLength, uint durationInMilliseconds, bool ensureVisible, int nPressure);
        [DispId(0x2b)]
        void Slide(int x, int y, double nDirection, uint nLength, uint durationInMilliseconds, bool ensureVisible, int nPressure);
        [DispId(0x2c)]
        void Swipe(int x, int y, double nDirection, uint nLength, bool ensureVisible, int nPressure);
        [DispId(0x2d)]
        void Zoom(int xThumb, int yThumb, int xIndex, int yIndex, int nLength, bool ensureVisible, int nPressure, int nSpeedPixelPerSecond);
        [DispId(0x2e)]
        void Turn(int xThumb, int yThumb, int xIndex, int yIndex, double nRotationAmount, bool ensureVisible, int nPressure, int nSpeedDegreesPerSecond);
    }
}