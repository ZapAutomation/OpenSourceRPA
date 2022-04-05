using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
    #if COMENABLED
    [ComImport, Guid("7E4ECC9A-F501-46DA-9C5C-99B0BB50477F"), ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    internal class CRPFPlaybackClass : IRPFPlayback, CRPFPlayback
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddTechnologyManager([In, MarshalAs(UnmanagedType.Interface)] IUITechnologyManager pTechnologyManager);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int EnableEnsureVisibleForPrimitive([In] int fEnable);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void FindAllScreenElements([In, MarshalAs(UnmanagedType.Interface)] IScreenElement pScreenElementStart, [In, MarshalAs(UnmanagedType.BStr)] string bstrQueryId, [In, MarshalAs(UnmanagedType.Struct)] ref object pvarResKeys, [In] int cResKeys, [In] int nMaxDepth, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] out object[] pFoundDescendants);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IScreenElement FindScreenElement([In, MarshalAs(UnmanagedType.BStr)] string bstrQueryId);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IScreenElement FindScreenElementEx([In, MarshalAs(UnmanagedType.Interface)] IScreenElement pScreenElementStart, [In, MarshalAs(UnmanagedType.BStr)] string bstrQueryId, [In, MarshalAs(UnmanagedType.Struct)] ref object pvarResKeys, [In] int cResKeys, int nMaxDepth);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void FinishPlayBack();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IUITechnologyManager GetCoreTechnologyManager([In, MarshalAs(UnmanagedType.BStr)] string bstrFilename);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern ILastInvocationInfo GetLastActionInfo();
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern string GetLogInfo([In] int nParam);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IScreenElement ScreenElementFromUITechnologyElement([MarshalAs(UnmanagedType.Interface), In] MsaaElement pIUITechnologyElement);


        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern object GetPlaybackProperty([In] int nParam);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IUITechnologyManager InitializeTechnologyManager([In, MarshalAs(UnmanagedType.BStr)] string bstrFilename, [In] Guid clsidTechnologyManager, [In] PluginRegistration registrationType, [In] PluginType PluginType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void InitPlayBackForTargetWindow([In, MarshalAs(UnmanagedType.BStr)] string bstrMainWindowClass, [In, MarshalAs(UnmanagedType.BStr)] string bstrMainWindowCaption, [In] int fShowInfoWindow, [In] PluginRegistration regTypeForDefaultTechManagers);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void InjectHardwareButton([In] int nButtonType, [In] int nInputType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int LaunchImmersiveApplication([In, MarshalAs(UnmanagedType.BStr)] string bstrAppUserModelId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void LogInformation([In, MarshalAs(UnmanagedType.BStr)] string bstrMessage, [In] int fDeleteFile);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IScreenElement ScreenElementFromWindow([ComAliasName("abc.LONG_PTR"), In] int hWnd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoveTechnologyManager([In, MarshalAs(UnmanagedType.Interface)] IUITechnologyManager pTechnologyManager);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ResetSkipStep();
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IScreenElement ScreenElementFromNativeElement([In, MarshalAs(UnmanagedType.Struct)] object varNativeElement, [In, MarshalAs(UnmanagedType.BStr)] string bstrTechnologyManagerName);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IScreenElement ScreenElementFromUITechnologyElement([In, MarshalAs(UnmanagedType.Interface)] ITaskActivityElement pIUITechnologyElement);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IScreenElement ScreenElementFromWindow([In, ComAliasName("abc.LONG_PTR")] IntPtr hWnd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetCustomLogger([In, MarshalAs(UnmanagedType.Interface)] ILoggerCallback pLoggerCallback);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetDebugMode([In] int nDebuggingLevel);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int SetLoggingFlag([In] int nLoggingFlag);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetPlaybackProperty([In] int nParam, [In, MarshalAs(UnmanagedType.Struct)] object varParamValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetRobustnessLevel([In] int nRobustnessLevel);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetSkipStepEventName([In, MarshalAs(UnmanagedType.BStr)] string bstrName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetThreadInfoInterface([In, MarshalAs(UnmanagedType.Interface)] IThreadInfo pThreadInfo);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void StartSession();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void StopSession();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void TerminateImmersiveApplication([In, MarshalAs(UnmanagedType.BStr)] string bstrPackageFullName, [In] int pdwProcessId);


                        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void TypeString([In, MarshalAs(UnmanagedType.BStr)] string bstrKeys, [In] int nSleepBetweenActivities = 50, [In] int fbLiteral = 0, [In] int nKeyboardAction = 3);
    }

}