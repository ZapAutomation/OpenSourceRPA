using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Execute
{
    public static class ExecutionHandler
    {
        private static readonly object initLockObject = new object();
        internal static int presentTestNumber = 1;
        private const int ScreenShotBorderPadding = 4;
        private const int ScreenShotBorderWidth = 4;
        
                
                public static event EventHandler<ExecuteErrorEventArgs> PlaybackError;

        internal static void AddZappyTaskControlDescriptionToException(Exception exception, IPlaybackContext context)
        {
            if (!ReferenceEquals(context, null))
            {
                ZappyTaskControlNotAvailableException innerException = exception as ZappyTaskControlNotAvailableException;
                string uITaskControlString = GetZappyTaskControlString(context.ZappyTaskControl as ZappyTaskControl);
                if (innerException != null && !string.IsNullOrEmpty(uITaskControlString) && string.IsNullOrEmpty(innerException.ZappyTaskControlDescription))
                {
                    throw new ZappyTaskControlNotAvailableException(innerException.Message, innerException, uITaskControlString, context.ZappyTaskControl);
                }
            }
        }

        public static void Cancel()
        {
            
            ScreenElement.SkipStep();
        }


        internal static void CaptureScreenShot(ZappyTaskControl control)
        {
                        {
                try
                {
                    TakeNextFailureScreenShot = false;
                                                                                                                                                                                                    }
                catch (ZappyTaskException)
                {
                }
                catch (Win32Exception)
                {
                }
                catch (SecurityException)
                {
                }
            }
        }

        private static void CheckTechnologyNotSupportedAndThrow(COMException ex)
        {
            if (Marshal.GetHRForException(ex) == -268111862)
            {
                string message = string.Empty;
                if (LastSearchInfo != null)
                {
                    ILastInvocationInfo innerInfo = LastSearchInfo.InnerInfo;
                    if (innerInfo != null)
                    {
                        message = innerInfo.Message;
                    }
                }
                throw new Exception(message, ex);
            }
        }

        public static void Cleanup()
        {
            
            object initLockObject = ExecutionHandler.initLockObject;
            lock (initLockObject)
            {
                if (!IsInitialized)
                {
                    throw new ZappyTaskException(Resources.PlaybackIsNotInitialized);
                }
                try
                {
                    Mouse.Cleanup();
                    Keyboard.Cleanup();
                    ALUtility.CleanupImmersiveSwitcher();
                    ApplicationBase.CloseProcessesOnPlaybackCleanup();
                }
                finally
                {
                    try
                    {
                        if (IsSessionStarted)
                        {
                            StopSession();
                        }
                                                                        ScreenElement.FinishPlayback();
                    }
                    catch (Exception)
                    {
                    }
                    ZappyTaskService.Instance.Cleanup();
                    PlaybackError -= PlaybackErrorHandler;
                                        IsInitialized = false;
                }
            }
        }

        internal static void Delay(int duration)
        {
            if (duration >= 0)
            {
                int num = duration / 100;
                for (long i = 0L; (i < num) && !ScreenElement.IsSkipStepOn; i += 1L)
                {
                    ZappyTaskUtilities.Sleep(100);
                }
                if (!ScreenElement.IsSkipStepOn)
                {
                    ZappyTaskUtilities.Sleep(duration % 100);
                }
            }
        }

        public static string EncryptText(string textToEncrypt)
        {
            if (textToEncrypt == null)
            {
                throw new ArgumentNullException("textToEncrypt");
            }
            return EncodeDecode.EncodeString(textToEncrypt);
        }

        public static bool ExceptionFromPropertyProvider(Exception ex) =>
            (ex is ArgumentException || ex is NotSupportedException) && ALUtility.IsTargetSitePropertyProvider(ex);

        internal static int GetActualThinkTime(int duration)
        {
            double num = duration * Settings.ThinkTimeMultiplier;
            int num2 = (int)Math.Min(num, 2147483647.0);
            return Math.Max(num2, Settings.DelayBetweenActivities);
        }

        private static Rectangle GetBoundsToHighlightInScreenShot(ZappyTaskControl control, out bool isActualControl)
        {
            Rectangle empty = Rectangle.Empty;
            isActualControl = true;
            try
            {
                if (control.IsBound && control.UIObject != null)
                {
                    empty = control.BoundingRectangle;
                }
            }
            catch
            {
            }
            if (empty.IsEmpty)
            {
                for (ZappyTaskControl control2 = control.Container; control2 != null; control2 = control2.Container)
                {
                    try
                    {
                        if (control2.IsBound && control2.UIObject != null)
                        {
                            empty = control2.BoundingRectangle;
                            isActualControl = false;
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            if (!empty.IsEmpty)
            {
                empty.Inflate(8, 8);
            }
            return empty;
        }

        public static ZappyTaskPropertyProvider GetCorePropertyProvider(ZappyTaskControl uiControl)
        {
                        if (!IsSessionStarted)
            {
                throw new ZappyTaskException(Resources.SessionIsNotStarted);
            }
            ZappyTaskPropertyProvider corePropertyProvider = PropertyProviderManager.Instance.GetCorePropertyProvider(uiControl);
            if (corePropertyProvider == null)
            {
                object[] args = { uiControl };
                throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.NoCorePropertyProviderExists, args));
            }
            return corePropertyProvider;
        }

        public static UITechnologyManager GetCoreTechnologyManager(string technologyName)
        {
                        if (!ZappyTaskService.Instance.IsSessionStarted)
            {
                throw new ZappyTaskException(Resources.SessionIsNotStarted);
            }
            UITechnologyManager technologyManagerByName = ZappyTaskService.Instance.PluginManager.GetTechnologyManagerByName(technologyName);
            if (technologyManagerByName != null && technologyManagerByName.GetType().FullName.StartsWith("StringComparison.OrdinalIgnoreCase"))
            {
                return technologyManagerByName;
            }
            object[] args = { technologyName };
            throw new ZappyTaskException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidCoreTechManager, args));
        }

        public static IUITechnologyManager GetNativeCoreTechnologyManager(string technologyName)
        {
            if (ScreenElement.Playback != null)
            {
                return ScreenElement.Playback.GetCoreTechnologyManager(technologyName);
            }
            return null;
        }

        private static string GetZappyTaskControlString(PropertyExpressionCollection properties)
        {
            if (properties == null || properties.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            foreach (PropertyExpression expression in properties)
            {
                object[] args = { expression.PropertyName, expression.PropertyValue, Environment.NewLine };
                builder.AppendFormat(CultureInfo.InvariantCulture, Resources.ZappyTaskControlStringFormat, args);
            }
            return builder.ToString();
        }

        internal static string GetZappyTaskControlString(ZappyTaskControl control)
        {
            CaptureScreenShot(control);
            if (ReferenceEquals(control, null))
            {
                return null;
                            }
            PropertyExpressionCollection properties = new PropertyExpressionCollection();
            TaskActivityObject uIObject = null;
            string technologyName = string.Empty;
            if (control.IsBound && control.TechnologyElement != null)
            {
                technologyName = control.TechnologyElement.Framework;
                uIObject = ScreenMapUtil.FromUIElement(control.TechnologyElement);
            }
            else
            {
                uIObject = control.UIObject;
                if (uIObject != null)
                {
                    technologyName = uIObject.Framework;
                }
            }
            if (!string.IsNullOrEmpty(technologyName))
            {
                properties.Add("TechnologyName", technologyName);
            }
            if (uIObject != null)
            {
                PropertyExpressionCollection expressions2;
                PropertyExpressionCollection expressions3;
                PropertyExpressionCollection.GetProperties(uIObject.Condition, out expressions2, out expressions3);
                if (expressions2 != null)
                {
                    foreach (PropertyExpression expression in expressions2)
                    {
                        properties.Add(expression.PropertyName, expression.PropertyValue);
                    }
                }
            }
            if (!properties.Contains(ZappyTaskControl.PropertyNames.MaxDepth) && control.MaxDepth > 0)
            {
                properties.Add(ZappyTaskControl.PropertyNames.MaxDepth, control.MaxDepth.ToString(CultureInfo.InvariantCulture));
            }
            if (!properties.Contains(ZappyTaskControl.PropertyNames.Instance) && control.Instance > 0)
            {
                properties.Add(ZappyTaskControl.PropertyNames.Instance, control.Instance.ToString(CultureInfo.InvariantCulture));
            }
            return GetZappyTaskControlString(properties);
        }

        internal static ExecuteErrorOptions HandleExceptionAndRetry(Exception ex, ZappyTaskControl control, bool allowUserToHandleException)
        {
                        ExecuteErrorOptions errorOption = ExecuteErrorOptions.Default;
            if (allowUserToHandleException && ALUtility.ShouldErrorEventBeRaisedForException(ex) && PlaybackError != null)
            {
                ExecuteErrorEventArgs e = new ExecuteErrorEventArgs(ex);
                try
                {
                    PlaybackError(null, e);
                    object[] args = { e.Result };
                    
                    errorOption = e.Result;
                }
                catch (Exception exception)
                {
                    object[] objArray2 = { exception.Message };
                    CrapyLogger.log.ErrorFormat("Error caught in retry handler: {0}. Ignoring it.", objArray2);
                }
            }
                                                                        return errorOption;
        }

        public static void Initialize()
        {
            

            {
                object initLockObject = ExecutionHandler.initLockObject;
                lock (initLockObject)
                {

                    if (!ZappyTaskUtilities.UserInteractive)
                    {
                        throw new ZappyTaskException("not user interactive!!!");
                    }

                    
                                        ZappyTaskService.Instance.Initialize();
                    
                    try
                    {
                        ScreenElement.InitPlayback();
                    }
                    catch (Exception exception)
                    {
                        MapAndThrowException(exception, new ExecuteContext());
                        throw;
                    }
                                                                                                    PlaybackError += PlaybackErrorHandler;
                                        IsInitialized = true;
                                        StartSession();
                }
                Settings.ResetToDefault();
                ZappyTaskEnvironment.LogEnvironmentInfo();
            }
        }

        private static bool IsDifferentWindowAtPoint(IntPtr currentProcess, NativeMethods.POINT pt, out IntPtr blockingWindow)
        {
            IntPtr hWnd = NativeMethods.WindowFromPoint(pt);
            blockingWindow = hWnd;
            return !(currentProcess == hWnd) && !NativeMethods.IsChild(currentProcess, hWnd) && !NativeMethods.IsChild(hWnd, currentProcess);
        }

        private static void MapAndThrowComException(COMException innerException, IPlaybackContext context)
        {
            CheckTechnologyNotSupportedAndThrow(innerException);
            uint hRForException = 0;
            hRForException = (uint)Marshal.GetHRForException(innerException);
            if (hRForException == 0x800704c7)
            {
                throw new ExecuteCanceledException();
            }
            MapControlNotFoundException(innerException, context);
            ThrowIfAnotherWindowIsBlockingControl(innerException, context);
            ZappyTaskException exception = null;
                                                                                                                                    
                                    
                                    
                                                
                                                                                                                                                if (exception != null)
            {
                throw exception;
            }
        }

        internal static void MapAndThrowException(Exception exception, IPlaybackContext context)
        {
            if (!IsInExceptionMappingContext)
            {
                try
                {
                    IsInExceptionMappingContext = true;
                    ThrowIfScreenLockedOrRemoteSessionMinimized();
                    COMException innerException = exception as COMException;
                    if (innerException != null)
                    {
                        MapAndThrowComException(innerException, context);
                    }
                    else if (exception is ArgumentException)
                    {
                        throw new Exception(exception.Message);
                    }
                    AddZappyTaskControlDescriptionToException(exception, context);
                    if (ZappyTaskUtilities.IsInvalidComObjectException(exception))
                    {
                        throw new ZappyTaskControlNotAvailableException(exception, GetZappyTaskControlString(context.ZappyTaskControl as ZappyTaskControl), context.ZappyTaskControl);
                    }
                }
                finally
                {
                    IsInExceptionMappingContext = false;
                }
            }
        }

        internal static void MapAndThrowException(Exception exception, bool isSearchContext)
        {
            IPlaybackContext playbackContext = PlaybackContext;
            if (playbackContext == null)
            {
                playbackContext = new ExecuteContext(isSearchContext);
            }
            MapAndThrowException(exception, playbackContext);
        }

        internal static void MapAndThrowException(Exception exception, string queryId)
        {
            IPlaybackContext playbackContext = PlaybackContext;
            if (playbackContext == null)
            {
                playbackContext = new ExecuteContext(queryId);
            }
            MapAndThrowException(exception, playbackContext);
        }

        internal static void MapAndThrowException(Exception exception, string actionName, ZappyTaskControl uiControl)
        {
            IPlaybackContext playbackContext = PlaybackContext;
            if (playbackContext == null)
            {
                playbackContext = new ExecuteContext(actionName, uiControl);
            }
            playbackContext.ActionName = actionName;
            MapAndThrowException(exception, playbackContext);
        }

        internal static void MapAndThrowException(Exception exception, string actionName, object parameterValue, ZappyTaskControl uiControl)
        {
            if (parameterValue is IEnumerable && !(parameterValue is string))
            {
                CommaListBuilder builder = new CommaListBuilder();
                builder.AddRange(parameterValue as IEnumerable);
                parameterValue = builder.ToString();
            }
            object[] args = { actionName, parameterValue };
            string str = string.Format(CultureInfo.CurrentCulture, Resources.ActionWithValue, args);
            MapAndThrowException(exception, str, uiControl);
        }

        internal static void MapAndThrowException(Exception exception, string actionName, ZappyTaskControl uiControl, int X, int Y)
        {
            IPlaybackContext playbackContext = PlaybackContext;
            if (playbackContext == null)
            {
                playbackContext = new ExecuteContext(actionName, uiControl, new Point(X, Y));
            }
            MapAndThrowException(exception, playbackContext);
        }

        private static void MapControlNotFoundException(COMException ex, IPlaybackContext context)
        {
            ZappyTaskException exception = null;
            PropertyExpressionCollection expressions;
            PropertyExpressionCollection expressions2;
            PropertyExpressionCollection.GetProperties(context.Condition, out expressions, out expressions2);
            if (context.IsSearchContext)
            {
                if (context.IsTopLevelSearch)
                {
                    object[] args = { context.FriendlyName };
                                    }
                else
                {
                    string controlNotFoundWithQidOrCondition;
                    if (string.IsNullOrEmpty(context.FriendlyName) && string.IsNullOrEmpty(context.ParentFriendlyName))
                    {
                        controlNotFoundWithQidOrCondition = Messages.ControlNotFoundWithQidOrCondition;
                    }
                    else
                    {
                        object[] objArray2 = { context.FriendlyTypeName, context.FriendlyName, context.ParentTypeName, context.ParentFriendlyName };
                        controlNotFoundWithQidOrCondition = string.Format(CultureInfo.CurrentCulture, Messages.ControlNotFoundException, objArray2);
                    }
                                    }
            }
            if (exception != null)
            {
                throw exception;
            }
        }

        private static void PlaybackErrorHandler(object sender, ExecuteErrorEventArgs eventArgs)
        {
            object[] args = { eventArgs.Error };
            CrapyLogger.log.ErrorFormat("AL: Playback Error: {0}", args);
            if (Settings.ContinueOnError)
            {
                
                                eventArgs.Result = ExecuteErrorOptions.Skip;
            }
        }

                                
        private static string SetStringIfNull(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return target;
            }
            return source;
        }


        public static void StartSession()
        {
            
            object initLockObject = ExecutionHandler.initLockObject;
            lock (initLockObject)
            {
                if (IsSessionStarted)
                {
                    throw new ZappyTaskException(Resources.SessionAlreadyStarted);
                }
                ZappyTaskService.Instance.StartSession(false);
                try
                {
                    ScreenElement.StartSession();
                }
                catch (Exception exception)
                {
                    ZappyTaskService.Instance.StopSession();
                    MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
                ALUtility.InitializeSTAHelperObject();
                ALUtility.InitializeBrowserWindow();
                SearchHelper.Instance.ResetCache();
                PlaybackContext = null;
                IsSessionStarted = true;
            }
            
        }

        public static void StopSession()
        {
            
            object initLockObject = ExecutionHandler.initLockObject;
            lock (initLockObject)
            {
                if (!IsSessionStarted)
                {
                    throw new ZappyTaskException(Resources.SessionIsNotStarted);
                }
                try
                {
                    ScreenElement.StopSession();
                }
                catch (Exception)
                {
#if COMENABLED
                    MapAndThrowException(exception, new ExecuteContext());
                    throw;
#endif
                }
                finally
                {
                    SearchHelper.Instance.ResetCache();
                    ALUtility.CleanupSTAHelperObject();
                    ZappyTaskService.Instance.StopSession();
                    ALUtility.CleanupBrowserWindow();
                    IsSessionStarted = false;
                }
            }
            
        }

        private static void ThrowIfAnotherWindowIsBlockingControl(COMException ex, IPlaybackContext context)
        {
            uint hRForException = 0;
            bool flag = false;
            hRForException = (uint)Marshal.GetHRForException(ex);
            IntPtr zero = IntPtr.Zero;
            IntPtr blockingWindow = IntPtr.Zero;
            if (hRForException == 0xf004f003 && context.ZappyTaskControl != null)
            {
                try
                {
                    zero = (IntPtr)(context.ZappyTaskControl as ZappyTaskControl).GetProperty(ZappyTaskControl.PropertyNames.WindowHandle);
                    Rectangle boundingRectangle = (context.ZappyTaskControl as ZappyTaskControl).BoundingRectangle;
                    NativeMethods.POINT pt = new NativeMethods.POINT(boundingRectangle.X + context.ActionLocation.X, boundingRectangle.Y + context.ActionLocation.Y);
                    flag = IsDifferentWindowAtPoint(zero, pt, out blockingWindow);
                    if (!flag)
                    {
                        pt.x = boundingRectangle.X + boundingRectangle.Width / 2;
                        pt.y = boundingRectangle.Y + boundingRectangle.Height / 2;
                        flag = IsDifferentWindowAtPoint(zero, pt, out blockingWindow);
                    }
                }
                catch (COMException)
                {
                }
            }
            if (flag)
            {
                IntPtr ancestor = NativeMethods.GetAncestor(blockingWindow, NativeMethods.GetAncestorFlag.GA_ROOT);
                if (ancestor == IntPtr.Zero)
                {
                    ancestor = blockingWindow;
                }
                string windowText = NativeMethods.GetWindowText(ancestor);
                string friendlyName = context.FriendlyName;
                object[] args = { friendlyName, windowText };
                            }
        }

        private static void ThrowIfScreenLockedOrRemoteSessionMinimized()
        {
                                                        }

        public static int Wait(int thinkTimeMilliseconds)
        {
            
            int actualThinkTime = GetActualThinkTime(thinkTimeMilliseconds);
            Delay(actualThinkTime);
            return actualThinkTime;
        }

        internal static void WaitForDelayBetweenActivities()
        {
            Delay(Settings.DelayBetweenActivities);
        }

        internal static bool IsInExceptionMappingContext { get; set; }

        public static bool IsInitialized
        { get; set; }

        public static bool IsSessionStarted
        { get; set; }

        internal static ILastInvocationInfo LastSearchInfo =>
            ScreenElement.LastSearchInfo;

                
        internal static IPlaybackContext PlaybackContext
        { get; set; }

        static ExecutionSettings _Settings;
        public static ExecutionSettings Settings
        {
            get
            {
                if (_Settings == null)
                    _Settings = ExecutionSettings.Default;
                return _Settings;
            }
            set { _Settings = value; }
        }

        internal static bool TakeNextFailureScreenShot { get; set; }
    }
}