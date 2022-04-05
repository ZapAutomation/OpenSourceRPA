using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Extension.WinControls;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{
    [CLSCompliant(true)]
    public class ApplicationUnderTask : ApplicationBase, IDisposable
    {
        private string alternateFileName;
        private bool disableSystemRedirection;
        private bool explicitNullValueSetForProcess;
        private string fileName;
        private const int InvalidExecutableErrorCode = 0xc1;
        private bool isClosed;
        private static readonly Regex programFiles = new Regex("%ProgramFiles%", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Process underneathProcess;
        private bool usedAlternateFileName;

        public ApplicationUnderTask()
        {
            TechnologyName = "MSAA";
            SearchProperties.Add(PropertyNames.ControlType, ControlType.Window.Name);
        }

        internal ApplicationUnderTask(TaskActivityObject uiObject) : base(uiObject)
        {
        }

        private ApplicationUnderTask(ProcessStartInfo startInfo)
        {
            if (startInfo == null)
            {
                throw new ArgumentNullException("startInfo");
            }
            fileName = startInfo.FileName;
                        startInfo.FileName = GetActualFilename();
            Start(startInfo);
        }

        private ApplicationUnderTask(string fileName, string alternateFileName, string arguments)
        {
            object[] args = { fileName, alternateFileName, arguments };
            
            this.fileName = fileName;
            this.alternateFileName = alternateFileName;
                        string actualFilename = GetActualFilename();
            ProcessStartInfo startInfo = new ProcessStartInfo(actualFilename, arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(actualFilename),
                ErrorDialog = false
            };
            Start(startInfo);
        }

        private ApplicationUnderTask(string fileName, string alternateFileName, string arguments, string userName, SecureString password, string domain)
        {
            object[] args = { fileName, alternateFileName, arguments, userName, domain };
            
            this.fileName = fileName;
            this.alternateFileName = alternateFileName;
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                string actualFilename = GetActualFilename();
                ProcessStartInfo startInfo = new ProcessStartInfo(actualFilename, arguments)
                {
                    UserName = userName,
                    Password = password,
                    Domain = domain,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(actualFilename),
                    ErrorDialog = false
                };
                Start(startInfo);
                return null;
            }, this, true, true);
        }

        public override void Close()
        {
            CloseOnPlaybackCleanup = true;
            Dispose();
            isClosed = true;
        }

        public void Dispose()
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                Unbind();
                if (Execute.ExecutionHandler.TakeNextFailureScreenShot)
                {
                    Execute.ExecutionHandler.CaptureScreenShot(null);
                    Execute.ExecutionHandler.TakeNextFailureScreenShot = true;
                }
                Dispose(true);
                return null;
            }, this, true, false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (Process != null && CloseOnPlaybackCleanup && !Process.HasExited)
                    {
                        if (MainWindowHandle != IntPtr.Zero)
                        {
                            Process.CloseMainWindow();
                            Process.WaitForExit(0x7d0);
                        }
                        if (!Process.HasExited)
                        {
                            Shutdown();
                        }
                        Process.Close();
                        Process = null;
                    }
                }
                catch (InvalidOperationException exception)
                {
                    CrapyLogger.log.Error(exception);
                                    }
                finally
                {
                    RemoveFromApplicationCache(this);
                }
            }
        }

        public override void Find()
        {
            if (isClosed || underneathProcess != null && underneathProcess.HasExited)
            {
                throw new ZappyTaskControlNotAvailableException(Resources.AutClosedMessage);
            }
            base.Find();
        }

        public static ApplicationUnderTask FromProcess(Process processToWrap) =>
            new ApplicationUnderTask { Process = processToWrap };

        private string GetActualFilename()
        {
            string executableNotSpecified;
            bool flag = !string.IsNullOrEmpty(fileName);
            bool flag2 = !string.IsNullOrEmpty(this.alternateFileName);
            if (flag2)
            {
                if (!NativeMethods.Is64BitOperatingSystem)
                {
                    this.alternateFileName = ZappyTaskUtilities.ConvertTo32BitString(this.alternateFileName);
                }
                string path = Environment.ExpandEnvironmentVariables(this.alternateFileName);
                if (File.Exists(path))
                {
                    usedAlternateFileName = true;
                    return path;
                }
                string alternateFileName = this.alternateFileName;
                if (ShouldDisableSystemRedirection(ref alternateFileName))
                {
                    disableSystemRedirection = true;
                    usedAlternateFileName = true;
                    return alternateFileName;
                }
            }
            if (flag && File.Exists(fileName))
            {
                return fileName;
            }
            if (!flag && !flag2)
            {
                executableNotSpecified = Resources.ExecutableNotSpecified;
            }
            else
            {
                executableNotSpecified = GetErrorMessage(Resources.ExecutableNotFound, fileName, alternateFileName);
            }
            throw new Exception(executableNotSpecified);
        }

        private Process GetCurrentProcessPrivate()
        {
            if (underneathProcess == null && !explicitNullValueSetForProcess)
            {
                Process processById = Process.GetProcessById((int)NativeMethods.GetWindowProcessId(WindowHandle));
                if (processById.Id != 0)
                {
                    underneathProcess = processById;
                }
                else
                {
                    string format = "No process with Process Id:{0} found.";
                    if (processById.HasExited)
                    {
                        format = format + "The Process has exited.";
                    }
                    object[] args = { processById.Id };
                    
                }
            }
            return underneathProcess;
        }

        private static string GetErrorMessage(string baseMessage, string file, string alternateFile)
        {
            object[] args = { file };
            string str2 = string.Format(CultureInfo.CurrentCulture, Resources.FileName, args);
            if (!string.IsNullOrEmpty(alternateFile))
            {
                object[] objArray2 = { alternateFile };
                string str3 = string.Format(CultureInfo.CurrentCulture, Resources.AlternateFileName, objArray2);
                object[] objArray3 = { baseMessage, str2, str3 };
                return string.Format(CultureInfo.CurrentCulture, Resources.FormatForErrorWithFileNameAndAlternateFileName, objArray3);
            }
            object[] objArray4 = { baseMessage, str2 };
            return string.Format(CultureInfo.CurrentCulture, Resources.FormatForErrorWithFileName, objArray4);
        }

        public static ApplicationUnderTask Launch(ProcessStartInfo startInfo) =>
            TaskMethodInvoker.Instance.InvokeMethod<ApplicationUnderTask>(() => new ApplicationUnderTask(startInfo), null, true, true);

        public static ApplicationUnderTask Launch(string fileName) =>
            Launch(fileName, string.Empty, string.Empty);

        public static ApplicationUnderTask Launch(string fileName, string alternateFileName) =>
            Launch(fileName, alternateFileName, string.Empty);

        public static ApplicationUnderTask Launch(string fileName, string alternateFileName, string arguments) =>
            TaskMethodInvoker.Instance.InvokeMethod<ApplicationUnderTask>(() => new ApplicationUnderTask(fileName, alternateFileName, arguments), null, true, true);

        public static ApplicationUnderTask Launch(string fileName, string alternateFileName, string arguments, string userName, SecureString password, string domain) =>
            TaskMethodInvoker.Instance.InvokeMethod<ApplicationUnderTask>(() => new ApplicationUnderTask(fileName, alternateFileName, arguments, userName, password, domain), null, true, true);

        private void SetCurrentProcessPrivate(Process value)
        {
            UnderneathProcess = value;
            if (value == null)
            {
                return;
            }
            bool flag = false;
            try
            {
                for (int i = 0; i < 120 && HasValidProcess; i++)
                {
                    if (MainWindowHandle != IntPtr.Zero)
                    {
                        try
                        {
                            CopyFrom(FromWindowHandle(MainWindowHandle));
                            flag = true;
                            goto Label_0074;
                        }
                        catch (ZappyTaskException)
                        {
                        }
                    }
                    Thread.Sleep(500);
                    underneathProcess.Refresh();
                }
            }
            catch (SystemException)
            {
                ThrowIfProcessNotValid(underneathProcess);
                throw;
            }
        Label_0074:
            ThrowIfProcessNotValid(underneathProcess);
            if (!flag)
            {
                                                throw new Exception();
            }
        }

        private static bool ShouldDisableSystemRedirection(ref string fileName)
        {
            IntPtr zero = IntPtr.Zero;
            if (NativeMethods.Is64BitOperatingSystem && NativeMethods.Wow64DisableWow64FsRedirection(out zero))
            {
                try
                {
                    fileName = programFiles.Replace(fileName, "%ProgramW6432%");
                    fileName = Environment.ExpandEnvironmentVariables(fileName);
                    if (File.Exists(fileName))
                    {
                        return true;
                    }
                }
                finally
                {
                    NativeMethods.Wow64RevertWow64FsRedirection(zero);
                }
            }
            return false;
        }

        protected void Shutdown()
        {
            if (HasValidProcess)
            {
                Process.Kill();
                Process.WaitForExit();
            }
        }

        private void Start(ProcessStartInfo startInfo)
        {
            Process process;
            IntPtr zero = IntPtr.Zero;
            if (disableSystemRedirection && !NativeMethods.Wow64DisableWow64FsRedirection(out zero))
            {
                disableSystemRedirection = false;
            }
            try
            {
                process = Process.Start(startInfo);
            }
            catch (Win32Exception exception)
            {
                string message = null;
                if (exception.NativeErrorCode == 0xc1)
                {
                    string file = usedAlternateFileName ? alternateFileName : fileName;
                    message = GetErrorMessage(Resources.ExecutableNotValid, file, null);
                }
                else
                {
                    message = exception.Message;
                }
                throw new Exception(message, exception);
            }
            catch (InvalidOperationException exception2)
            {
                string str3 = usedAlternateFileName ? alternateFileName : fileName;
                object[] args = { exception2.Message };
                throw new Exception(GetErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.ExecutableUnknownError, args), str3, null), exception2);
            }
            finally
            {
                if (disableSystemRedirection)
                {
                    NativeMethods.Wow64RevertWow64FsRedirection(zero);
                }
            }
            ThrowIfProcessNotValid(process);
            Thread.Sleep(500);
            try
            {
                process.WaitForInputIdle();
            }
            catch (InvalidOperationException)
            {
            }
            ThrowIfProcessNotValid(process);
            try
            {
                Process = process;
            }
            catch (SystemException exception3)
            {
                string str5 = usedAlternateFileName ? alternateFileName : fileName;
                object[] objArray2 = { exception3.Message };
                throw new Exception(GetErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.ExecutableUnknownError, objArray2), str5, null), exception3);
            }
            ThrowIfProcessNotValid(process);
            CloseOnPlaybackCleanup = true;
            ZappyTaskUtilities.SwitchFromImmersiveToWindow(Process.MainWindowHandle);
        }

        private void ThrowIfProcessNotValid(Process process)
        {
            if (process == null || process.HasExited)
            {
                string file = usedAlternateFileName ? alternateFileName : fileName;
                Exception exception = new Exception(GetErrorMessage(Resources.ExecuteFailedAsProcessExited, file, null));
                                throw exception;
            }
        }

        public virtual bool AlwaysOnTop =>
            (bool)GetProperty(WinWindow.PropertyNames.AlwaysOnTop);

        public virtual bool HasTitleBar =>
            (bool)GetProperty(WinWindow.PropertyNames.HasTitleBar);

        private bool HasValidProcess =>
            Process != null && !Process.HasExited;

        private IntPtr MainWindowHandle
        {
            get
            {
                try
                {
                    if (HasValidProcess)
                    {
                        return underneathProcess.MainWindowHandle;
                    }
                }
                catch (InvalidOperationException)
                {
                }
                return IntPtr.Zero;
            }
        }

        public virtual bool Maximized
        {
            get =>
                (bool)GetProperty(WinWindow.PropertyNames.Maximized);
            set
            {
                SetProperty(WinWindow.PropertyNames.Maximized, value);
            }
        }

        public virtual bool Minimized
        {
            get =>
                (bool)GetProperty(WinWindow.PropertyNames.Minimized);
            set
            {
                SetProperty(WinWindow.PropertyNames.Minimized, value);
            }
        }

        public virtual bool Popup =>
            (bool)GetProperty(WinWindow.PropertyNames.Popup);

        public Process Process
        {
            get =>
                TaskMethodInvoker.Instance.InvokeMethod<Process>(() => GetCurrentProcessPrivate(), this, true, false);
            internal set
            {
                TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
                {
                    SetCurrentProcessPrivate(value);
                    return null;
                }, this, true, false);
            }
        }

        public virtual bool Resizable =>
            (bool)GetProperty(WinWindow.PropertyNames.Resizable);

        public virtual bool Restored
        {
            get =>
                (bool)GetProperty(WinWindow.PropertyNames.Restored);
            set
            {
                SetProperty(WinWindow.PropertyNames.Restored, value);
            }
        }

        public virtual bool ShowInTaskbar =>
            (bool)GetProperty(WinWindow.PropertyNames.ShowInTaskbar);

        public string Title
        {
            get
            {
                if (HasValidProcess)
                {
                    Process.Refresh();
                    return Process.MainWindowTitle;
                }
                return string.Empty;
            }
        }

        public virtual bool Transparent =>
            (bool)GetProperty(WinWindow.PropertyNames.Transparent);

        internal Process UnderneathProcess
        {
            set
            {
                underneathProcess = value;
                if (value == null)
                {
                    explicitNullValueSetForProcess = true;
                }
                else
                {
                    explicitNullValueSetForProcess = false;
                }
            }
        }
    }
}