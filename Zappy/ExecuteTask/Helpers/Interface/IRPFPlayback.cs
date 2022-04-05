using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
    [Guid("C2F7C968-7C3F-4F22-AC67-FD96382548E9")]
    [ComImport]
#endif
    internal interface IRPFPlayback
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void InitPlayBackForTargetWindow([MarshalAs(UnmanagedType.BStr), In] string bstrMainWindowClass, [MarshalAs(UnmanagedType.BStr), In] string bstrMainWindowCaption, [In] int fShowInfoWindow, [In] PluginRegistration regTypeForDefaultTechManagers = PluginRegistration.Registered);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FinishPlayBack();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IScreenElement FindScreenElementEx([MarshalAs(UnmanagedType.Interface), In] IScreenElement pScreenElementStart, [MarshalAs(UnmanagedType.BStr), In] string bstrQueryId, [MarshalAs(UnmanagedType.Struct), In] ref object pvarResKeys, [In] int cResKeys, int nMaxDepth);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IScreenElement FindScreenElement([MarshalAs(UnmanagedType.BStr), In] string bstrQueryId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindAllScreenElements([MarshalAs(UnmanagedType.Interface), In] IScreenElement pScreenElementStart, [MarshalAs(UnmanagedType.BStr), In] string bstrQueryId, [MarshalAs(UnmanagedType.Struct), In] ref object pvarResKeys, [In] int cResKeys, [In] int nMaxDepth, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] out object[] pFoundDescendants);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void TypeString([MarshalAs(UnmanagedType.BStr), In] string bstrKeys, [In] int nSleepBetweenActivities = 50, [In] int fbLiteral = 0, [In] int nKeyboardAction = 3);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int EnableEnsureVisibleForPrimitive([In] int fEnable);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetRobustnessLevel([In] int nRobustnessLevel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetLoggingFlag([In] int nLoggingFlag);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetCustomLogger([MarshalAs(UnmanagedType.Interface), In] ILoggerCallback pLoggerCallback);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LogInformation([MarshalAs(UnmanagedType.BStr), In] string bstrMessage, [In] int fDeleteFile);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IScreenElement ScreenElementFromWindow([ComAliasName("abc.LONG_PTR"), In] int hWnd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IScreenElement ScreenElementFromNativeElement([MarshalAs(UnmanagedType.Struct), In] object varNativeElement, [MarshalAs(UnmanagedType.BStr), In] string bstrTechnologyManagerName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IScreenElement ScreenElementFromUITechnologyElement([MarshalAs(UnmanagedType.Interface), In] ITaskActivityElement pIUITechnologyElement);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetPlaybackProperty([In] int nParam);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPlaybackProperty([In] int nParam, [MarshalAs(UnmanagedType.Struct), In] object varParamValue);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ILastInvocationInfo GetLastActionInfo();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ResetSkipStep();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetThreadInfoInterface([MarshalAs(UnmanagedType.Interface), In] IThreadInfo pThreadInfo);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void StartSession();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void StopSession();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddTechnologyManager([MarshalAs(UnmanagedType.Interface), In] IUITechnologyManager pTechnologyManager);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveTechnologyManager([MarshalAs(UnmanagedType.Interface), In] IUITechnologyManager pTechnologyManager);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDebugMode([In] int nDebuggingLevel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetSkipStepEventName([MarshalAs(UnmanagedType.BStr), In] string bstrName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IUITechnologyManager InitializeTechnologyManager([MarshalAs(UnmanagedType.BStr), In] string bstrFilename, [In] Guid clsidTechnologyManager, [In] PluginRegistration registrationType, [In] PluginType PluginType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IUITechnologyManager GetCoreTechnologyManager([MarshalAs(UnmanagedType.BStr), In] string bstrFilename);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLogInfo([In] int nParam);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int LaunchImmersiveApplication([MarshalAs(UnmanagedType.BStr), In] string bstrAppUserModelId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void TerminateImmersiveApplication([MarshalAs(UnmanagedType.BStr), In] string bstrPackageFullName, [In] int pdwProcessId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void InjectHardwareButton([In] int nButtonType, [In] int nInputType);
    }
}