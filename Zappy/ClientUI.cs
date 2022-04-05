//#define FreeVersion

using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoUpdaterDotNET;
//using Microsoft.Identity.Client;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask;
using Zappy.Graph;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.Trapy;
using Zappy.ZappyActions.OCR;
using Zappy.ZappyActions.Triggers;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;
using ZappyLogger.Controls.LogTabWindow;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;
using ZappyMessages.Triggers;

namespace Zappy
{
    internal partial class ClientUI : Form, IPubSubSubscriber
    {

        private bool _SignedIn;
        public static bool TrapyStarted;
        public static string _LearnedActivitiesFilename = String.Empty;

        public static List<WindowLaunchTriggerHelper> WindowLaunchTriggers = new List<WindowLaunchTriggerHelper>();

        public bool StartApp
        {
            get { return _SignedIn; }
            set
            {
                _SignedIn = value;
                if (_SignedIn)
                {
                    cmdStart.BackColor = Color.LimeGreen;
                    cmdStart_Click(null, EventArgs.Empty);                   
                }
                else
                {
                    if (TrapyStarted)
                        cmdStart_Click(null, EventArgs.Empty);
                    cmdStart.BackColor = Color.LightGray;
                }
                cmdStart.Enabled = _SignedIn;
            }
        }

        private Process _ZappyAnalyticsProcess;

        public void PingClient()
        {
        }

        public ClientUI()
        {
            InitializeComponent();
            TrapyService.Init();


            InternalNodeGenerator.Initialize();
            notifyIcon1.Visible = true;
            StartApp = true;
            notifyIcon1.Text = "Zap Automation (Zappy) " + (TrapyStarted ? "Running" : "Stopped");

        }

        public static void StopZappy()
        {
            TrapyService.Stop();
            TrapyStarted = false;
            PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals,
                PubSubMessages.StopZappyExecutionMessage);
        }

        public static void StartZappy()
        {
            TrapyService.Start();
            TrapyStarted = true;
            PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals,
                PubSubMessages.StartZappyExecutionMessage);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            if (TrapyStarted)
            {
                StopZappy();
                InternalNodeGenerator.stop();
                cmdStart.Text = "Start";
                cmdStart.BackColor = Color.LimeGreen;
                ZappyInvoker.ShowNotificationZappy("Zappy Stopped");
            }
            else
            {
                if (ApplicationSettingProperties.Instance.EnableTrapy)
                {
                    StartZappy();
                    
                    cmdStart.Text = "Stop";
                    cmdStart.BackColor = Color.Red;
                }
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }

            notifyIcon1.Text = "Zap Automation (Zappy) " + (TrapyStarted ? "Running" : "Stopped");
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer,
            int lpdwBufferLength);
        int UniqueCancelHotkeyId = 1;     
        private void ClientUI_Load(object sender, EventArgs e)
        {
            PubSubService.Instance.Subscribe("ZappySubscriber", this, new int[] {
                PubSubTopicRegister.AuditLogsChannel,
                PubSubTopicRegister.DebugStepProgress,
                PubSubTopicRegister.Notification,
                PubSubTopicRegister.ControlSignals,
                PubSubTopicRegister.TaskEditorFileOpenRequest,
                PubSubTopicRegister.ZappyPlayback2Zappy});         

            zappyVersionToolStripMenuItem1.Text = "Version " + Application.ProductVersion;

            //Registers cancel hotkey here
            Boolean cancelKeyRegistered = NativeMethods.RegisterHotKey(
                this.Handle, UniqueCancelHotkeyId, HotKeyHandler.Instance.ZappyDefaultModifierKeys, HotKeyHandler.Instance.cancelExecutionKey
            );

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, typeof(ClientUI));
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            taskEditorOpenFile(sender, e);
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdStart_Click(sender, e);
        }

        void PrepareToggleMenuItems()
        {
            if (TrapyStarted)
            {
                StartStopZappyToolStripMenuItem.Image = Properties.Resources.StopZappyMenuImage;
                StartStopZappyToolStripMenuItem.Text = "Stop Zappy";
            }
            else
            {
                StartStopZappyToolStripMenuItem.Image = Properties.Resources.StartZappyMenuImage;
                StartStopZappyToolStripMenuItem.Text = "Start Zappy";
            }
            StartStopRecordingTaskToolStripMenuItem.Text = _TaskRecording ? "Stop Recording" : "Start Recording";
        }

        private void ClientUI_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
            else
            {

                ShowInTaskbar = true;
                NativeMethods.RefreshTrayArea();
            }
        }

        private void ClientUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                exitToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                ClientUI_Resize(sender, e);
                e.Cancel = true;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            StartStopZappyToolStripMenuItem.Enabled = StartApp;
            PrepareToggleMenuItems();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                notifyIcon1.Visible = false;
                Task.Factory.StartNew(ForceKillApplication);

                if (Environment.OSVersion.Version < new Version(6, 2))
                {
                    if (_ZappyAnalyticsProcess != null && !_ZappyAnalyticsProcess.HasExited)
                        _ZappyAnalyticsProcess.Kill();
                }
                NativeMethods.UnregisterHotKey(this.Handle, UniqueCancelHotkeyId);
                InternalNodeGenerator.NodeGraphBuilder.Dispose();
                TrapyService.Stop();
                PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals,
                    PubSubMessages.StopZappyExecutionMessage);
                //PubSubService.Instance.Stop();
                CrapyLogger.log.Error(" App Exiting");

            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        public void ForceKillApplication()
        {
            //Kill after 40 seconds
            Thread.Sleep(40000);
            CrapyLogger.log.Error("ForceKillApplication App");

            Process.GetCurrentProcess().Kill();

        }

        public static bool _TaskRecording;

        private void taskEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LearnedActions.CreateLearnedActions();
        }

        public event Action<Guid, string, Guid> ActionExecutionUpdate;
        public event Action<IZappyAction, string, Guid> ActionExecutionTrace;

        public void OnActionExecutionUpdate(Guid Current, string Result, Guid Next)
        {
            ActionExecutionUpdate?.Invoke(Current, Result, Next);
        }

        private string IconStatusText = string.Empty;
        public void OnPublished(int channel, string publish)
        {
            try
            {
                //publish = StringCipher.Decrypt(publish, ZappyMessagingConstants.MessageKey);

                if (channel == PubSubTopicRegister.Notification)
                {
                    if (publish == ZappyMessagingConstants.ZappyExecutionStarted)
                    {
                        this.notifyIcon1.Icon = Properties.Resources.runningZappy;
                        IconStatusText = this.notifyIcon1.BalloonTipText;
                        this.notifyIcon1.BalloonTipText = "Running Zap Automation";
                    }
                    else if (publish == ZappyMessagingConstants.ZappyExecutionStopped)
                    {
                        this.notifyIcon1.Icon = Properties.Resources.favicon;
                        this.notifyIcon1.BalloonTipText = IconStatusText;
                    }
                    else
                    {
                        this.BeginInvoke(new Action<string>(ShowBaloonMessage), publish);
                    }
                }
                else if (channel == PubSubTopicRegister.DebugStepProgress)
                {
                    Tuple<Guid, string, Guid> _Update =
                        ZappySerializer.DeserializeObject<Tuple<Guid, string, Guid>>(publish);

                    this.BeginInvoke(new Action(() => OnActionExecutionUpdate(_Update.Item1, _Update.Item2, _Update.Item3)));
                }
                else if (channel == PubSubTopicRegister.ControlSignals)
                {
                    if (publish == PubSubMessages.RequestStateFromZappy)
                        ThreadPool.QueueUserWorkItem(RepublishState, null);
                    else if (publish == PubSubMessages.ExitZappyMessage)
                        exitToolStripMenuItem_Click(null, EventArgs.Empty);
                }
                else if (channel == PubSubTopicRegister.ZappyPlayback2Zappy)
                {
                    Tuple<PlayBackHelperRequestEnum, string> _Request =
                        ZappySerializer.DeserializeObject<Tuple<PlayBackHelperRequestEnum, string>>(publish);
                    if (_Request.Item1 == PlayBackHelperRequestEnum.GetLastActivityTime)
                    {
                        ZappyPlaybackCommunicator.Instance.PublishResponseToZappyPlayback(InternalNodeGenerator.LastActivity.ToString(), PubSubTopicRegister.Zappy2ZappyPlayback);
                    }
                    else if(_Request.Item1 == PlayBackHelperRequestEnum.RegisterWindowLaunchTriiger)
                    {
                        lock(WindowLaunchTriggers)
                             WindowLaunchTriggers.Add(ZappySerializer.DeserializeObject<WindowLaunchTriggerHelper>(_Request.Item2));
                        ZappyPlaybackCommunicator.Instance.
                            PublishResponseToZappyPlayback("Window Launch Trigger Added", PubSubTopicRegister.Zappy2ZappyPlayback);

                    }
                    else if (_Request.Item1 == PlayBackHelperRequestEnum.UnRegisterWindowLaunchTriiger)
                    {
                        lock (WindowLaunchTriggers)
                        {
                            var windowLaunchHelper = ZappySerializer.DeserializeObject<WindowLaunchTriggerHelper>(_Request.Item2);
                            //if (WindowLaunchTriggers.Contains(windowLaunchHelper))
                            WindowLaunchTriggerHelper itemtoRemove = null;
                            for (int i = 0; i < WindowLaunchTriggers.Count; i++)
                            {
                                if (WindowLaunchTriggers[i].SelfGuid == windowLaunchHelper.SelfGuid)
                                    itemtoRemove = WindowLaunchTriggers[i];
                            }

                            if (itemtoRemove != null)
                                WindowLaunchTriggers.Remove(itemtoRemove);
                        }
                        ZappyPlaybackCommunicator.Instance.
                            PublishResponseToZappyPlayback("Window Launch Trigger Removed", PubSubTopicRegister.Zappy2ZappyPlayback);
                    }
                }

                else if (channel == PubSubTopicRegister.TaskEditorFileOpenRequest)
                    this.BeginInvoke(new Action(() => taskEditorOpenFile(publish, EventArgs.Empty)));
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }

        }

        private void RepublishState(object dummy)
        {
            PubSubService.Instance.Publish(PubSubTopicRegister.ControlSignals, TrapyStarted ? PubSubMessages.StartZappyExecutionMessage : PubSubMessages.StopZappyExecutionMessage);
        }

        public void OnPublishedBinary(int channel, byte[] publish)
        {
        }

        private void taskEditorOpenFile(object sender, EventArgs e)
        {
            string _filename = string.Empty;
            if (sender is string)
                _filename = sender.ToString();
            PageFormTabbed frm = new PageFormTabbed(_filename);
            frm.Show();
        }

        private void ocrSnippingToolToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ZappyOcrForm frm = new ZappyOcrForm();
            frm.Show();
        }

        private void ShowBaloonMessage(string text)
        {
            if (text.StartsWith(ZappyMessagingConstants.ZappyExecutionFailed))
            {
                var input = text.Replace(ZappyMessagingConstants.ZappyExecutionFailed, "");
                SimpleNotificationForm frm = new SimpleNotificationForm("Failed Execution of Task: " + input);
                frm.Show();
            }
            else
            {
                frmActionLearner.NotificationInstance.ShowInactiveTopmost(text);
            }
        }

        private void zappyVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Zappy Version is " + Application.ProductVersion);
        }

        private void exportLearnedTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LearnedActions.ExportZappyLearnedActivities();
        }


        private void updateZappyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void logViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logWindowShow();
        }

        public static void logWindowShow()
        {
            string[] fileNames = new string[4];
            fileNames[0] = Path.Combine(CrapyConstants.LogFolder, "Zappy.log");
            fileNames[1] = Path.Combine(CrapyConstants.LogFolder, "ZappyHelper.log");
            if (ApplicationSettingProperties.Instance.EnableTraceLog)
                fileNames[2] = Path.Combine(CrapyConstants.LogFolder, "Trace.log");
            fileNames[3] = Path.Combine(CrapyConstants.AuditDirectory, CrapyConstants.Logfilename);
            LogTabWindow logWin = new LogTabWindow(fileNames, 1, false);
            //removes groupings
            TaskbarManager.Instance.SetApplicationIdForSpecificWindow(logWin.Handle, Guid.NewGuid().ToString());
            logWin.Show();
        }

        private void triggerManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTriggerManager frm = new frmTriggerManager();
            frm.Show();
        }
        
        private void applicationSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmApplicationSettings frm = new frmApplicationSettings();
            frm.Show();
        }

        private void configureHotkeysToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmHotKeyManager frm = new frmHotKeyManager();
            frm.Show();
        }

        private void viewAnalyticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonProgram.FreeVersionMessageBox();
        }

        public void ChangeLanguage(LanguageZappy languageZappy, Type typeOfForm)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);

                ComponentResourceManager resources = null;
                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeOfForm);
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                }

                foreach (ToolStripItem item in contextMenuStrip1.Items)
                {
                    if (item is ToolStripDropDownItem)
                    {
                        resources = new ComponentResourceManager(typeOfForm);
                        foreach (ToolStripItem dropDownItem in ((ToolStripDropDownItem)item).DropDownItems)
                        {
                            if (dropDownItem is ToolStripDropDownItem)
                            {
                                foreach (ToolStripItem dropDownItem1 in ((ToolStripDropDownItem)dropDownItem).DropDownItems)
                                {
                                    if (dropDownItem1 is ToolStripDropDownItem)
                                    {
                                        resources = new ComponentResourceManager(typeOfForm);
                                    }

                                    resources = new ComponentResourceManager(typeOfForm);
                                    resources.ApplyResources(dropDownItem1, dropDownItem1.Name, new CultureInfo(lang));
                                }
                            }
                            resources.ApplyResources(dropDownItem, dropDownItem.Name, new CultureInfo(lang));
                        }
                    }
                    resources.ApplyResources(item, item.Name, new CultureInfo(lang));
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void EnglishToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ApplicationSettingProperties.Instance.ZappyUILanguage = LanguageZappy.en;
            ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, typeof(ClientUI));
            File.WriteAllText(CrapyConstants.ZappySettings, ZappySerializer.SerializeObject(ApplicationSettingProperties.Instance));
        }

        private void JapaneaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationSettingProperties.Instance.ZappyUILanguage = LanguageZappy.jp;
            SaveChangeLanguage();
        }

        private void SaveChangeLanguage()
        {
            ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, typeof(ClientUI));
            File.WriteAllText(CrapyConstants.ZappySettings, ZappySerializer.SerializeObject(ApplicationSettingProperties.Instance));
        }

        private void addFirefoxAddinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("firefox.exe", "- install - extension " + "\"" + CrapyConstants.FirefoxExtensionFile + "\"");
        }

        private void NugetManagerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmNugetManager frm = new frmNugetManager();
            frm.Show();
        }

    }
}
