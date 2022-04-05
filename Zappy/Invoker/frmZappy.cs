using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Window;
using Zappy.Decode.LogManager;
using Zappy.Graph;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Excel;
using Zappy.ZappyActions.Loops;
using Zappy.ZappyActions.OCR;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.EditorPage;
using Timer = System.Threading.Timer;

namespace Zappy.Invoker
{
    public partial class frmZappy : Form
    {
        #region Native

        [Flags]
        internal enum SetWinEventHookParameter
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }



        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(
            AccessibleEvents eventMin, //Specifies the event constant for the lowest event value in the range of c 
            AccessibleEvents eventMax, // Specifies the event constant for the highest event value in the range of 
            IntPtr eventHookAssemb1yHand1e, //Hand1e to the DLL that contains the hook function at IpfnWinEventPr 
            WinEventProc eventHookHand1e, // Pointer to the event hook function. For more information about this fl 
            uint processld, // Specifies the ID of the process from which the hook function receives events. Speci 
            uint threadld, //Specifies the ID of the thread from which the hook function receives events. If this 
            SetWinEventHookParameter parameterFlags);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr eventHookHand1e);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern bool SetActiveWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hwnd);

        internal delegate void WinEventProc(IntPtr winEventHookHand1e, AccessibleEvents accEvent,
            IntPtr windowHand1e, int objectld, int childld, uint eventThreadId, uint eventTimeInmi11iseconds);

        [StructLayout(LayoutKind.Sequential)]
        internal struct TITLEBARINFO
        {
            public uint cbSize; //Specifies the size, in bytes, of the structure. 
            public RECT rcTit1eBar; //Pointer to a RECT structure that receives the 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public AccessibleStates[] rgstate;
        }

        [StructLayout(LayoutKind.Sequential)]

        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        public enum GWLParameter
        {
            GWL_EXSTYLE = -20, //Sets a neu extended window style 
            GWL_HINSTANCE = -6, //Sets a new application instance handle. 
            GWL_HWNDPARENT = -8, //Set window handle as parent 
            GWL_ID = -12, //Sets a new identifier of the window. 
            GWL_STYLE = -16, // Set new window style 
            GWL_USERDATA = -21, //Sets the user data associated with the window. 
            GWL_WNDPROC = -4 //Sets a new address for the window procedure.         
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowLong(IntPtr windowHandle, GWLParameter nlndex, IntPtr NewLong);

        [DllImport("user32.dll")]
        internal static extern bool GetTitleBarInfo(IntPtr hwnd, ref TITLEBARINFO pti);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        #endregion

        bool _Expanded;
        IntPtr _TrackedHandle, _InvokingHandle;

        uint _Pid;
        Dictionary<AccessibleEvents, KeyValuePair<GCHandle, WinEventProc>> events;
        IntPtr hk1, hk2;
        Timer _tmr;
        private ZappyTask PredictedTask;

        public Tuple<string, IZappyAction>[] _SingletonActivities;

        public string TaskDirPath { get; set; }
        //string _AutoExec;
        public frmZappy()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                this.Close();
            }
        }

        //, string AutoExec = null
        public frmZappy(IntPtr TrackedHandle, string DirPath, IntPtr InvokingHandle, ZappyTask predictedTask) :
            this() // string Task1, string Task2,
        {
            PredictedTask = predictedTask;
            //_ExpandedSize = new Size(485, 637);
            //_CollapsedSize = new Size(16, 16);
            _TrackedHandle = TrackedHandle;
            _InvokingHandle = InvokingHandle;
            //_AutoExec = AutoExec;
            //if (!string.IsNullOrEmpty(_AutoExec))
            //    _AutoExec = _AutoExec.ToUpper();
            NativeMethods.GetWindowThreadProcessId(_InvokingHandle, out _Pid);

            if (!string.IsNullOrEmpty(DirPath) && Directory.Exists(DirPath))
            {
                TaskDirPath = DirPath;
            }
            if (PredictedTask != null)
                ChangeColor();
            // _PredictedTaskNode = AddNode(PredictedTask, "Predicted", _PredictedTaskNode);

            events = InitializeWinEventToHand1erMap();

            HandleCreated += FrmZappy_HandleCreated;

            hk1 = SetWinEventHook(AccessibleEvents.LocationChange,
                AccessibleEvents.LocationChange,
                IntPtr.Zero,
                events[AccessibleEvents.LocationChange].Value, _Pid, 0,
                SetWinEventHookParameter.WINEVENT_OUTOFCONTEXT);

            hk2 = SetWinEventHook(AccessibleEvents.Destroy,
                AccessibleEvents.Destroy,
                IntPtr.Zero,
                events[AccessibleEvents.Destroy].Value, _Pid, 0,
                SetWinEventHookParameter.WINEVENT_OUTOFCONTEXT);
            try
            {
                cmbRepeatCount.SelectedIndex = 0;
            }
            catch
            {

            }

            //treeView1.PinMode = ZappyControls.PinMode.RootNodes;
        }

        private IntPtr _ThisControlHandle = IntPtr.Zero;
        private void FrmZappy_HandleCreated(object sender, EventArgs e)
        {
            if (_ThisControlHandle == IntPtr.Zero)
            {
                if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
                {
                    ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
                }
                _ThisControlHandle = Handle;

                treeView1.BeginUpdate();

                //TODO - discovered tasks on Zappy??
                //if (Properties.Settings.Default.EnableDiscoveredProcess)
                //{
                //    string DiscoveredProcessPath =
                //        Path.Combine(CrapyConstants.DiscoveredProcessFolder,
                //            WindowLaunchEventCapture.GetProcess(_InvokingHandle).ProcessName);
                //    bool _LoadedDiscoveredProcesses = LoadTasks(DiscoveredProcessPath);
                //    if (_LoadedDiscoveredProcesses)
                //        ChangeColor();
                //}

                if (!string.IsNullOrEmpty(TaskDirPath) && Directory.Exists(TaskDirPath))
                    LoadTasks(TaskDirPath);


                CreateSingletonActivities();
                treeView1.EndUpdate();



                treeView1.BeforeExpand += TreeView1_BeforeExpand;


            }
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Expand)
                ExpandTreeNode(e.Node);
        }

        public bool LoadTasks(string TaskFolderPath)
        {
            bool _Loaded = false;
            if (Directory.Exists(TaskFolderPath))
            {
                string[] _Files = Directory.GetFiles(TaskFolderPath, "*.zappy");
                for (int i = 0; i < _Files.Length; i++)
                {
                    try
                    {

                        TreeNode _Node = CreateNode(_Files[i]);

                        treeView1.Nodes.Add(_Node);
                        _Loaded = true;

                        //if (_AutoExec != null && _Files[i].ToUpper() == _AutoExec)
                        //{

                        ///////////////////TODO HERE
                        //    StartPlayback(_AutoExec);
                        //    _Node.ImageIndex = 10;
                        //    treeView1.PinnedNode.Add(_Node);
                        //}
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.ErrorFormat("Error loading :{0} {1}", _Files[i], ex);
                    }
                }
            }
            return _Loaded;
        }

        //public void LoadSavedTasks()
        //{
        //    string[] _Files = Directory.GetFiles(TaskDirPath, "*.zappy");
        //    Array.Sort(_Files);
        //    bool _AutoExecuteCalled = false;

        //    for (int i = 0; i < _Files.Length; i++)
        //    {

        //        TreeNode _Node = CreateNode(_Files[i]);
        //        treeView1.Nodes.Add(_Node);
        //        //TODO: 16-09 add logic to determine autoexecute from filename / filepath
        //        //Tuple<string, ZappyTask> _Tuple = _Node.Tag as Tuple<string, ZappyTask>;

        //        //if (!_AutoExecuteCalled && _Tuple != null && _Tuple.Item2.AutoExecute)
        //        //{
        //        //    //CrapyLogger.log.ErrorFormat("Autoexecuting :{0}", _Files[i]);
        //        //    _AutoExecuteCalled = true;
        //        //    StartPlayback(_Tuple);
        //        //    _Node.ImageIndex = 10;
        //        //    treeView1.PinnedNode.Add(_Node);
        //        //}

        //    }

        //}

        public void CreateSingletonActivities()
        {
            _SingletonActivities = new[]
            {
                new Tuple<string, IZappyAction>("Copy Screen Text",new OcrToClipboardAction() {Hwnd_Int = _TrackedHandle.ToInt32()})
            };

            for (int i = 0; i < _SingletonActivities.Length; i++)
            {
                TreeNode _ActionNode = treeView1.Nodes.Add(_SingletonActivities[i].Item1);
                _ActionNode.Tag = _SingletonActivities[i];
            }
        }

        public TreeNode CreateNode(string FilePath)
        {
            TreeNode TaskNode = new TreeNode(Path.GetFileNameWithoutExtension(FilePath));
            TaskNode.Tag = FilePath;
            TaskNode.Nodes.Add("Loading...");
            return TaskNode;
        }


        public void RemoveInsertNewNode(string FilePath)
        {
            TreeNode TaskNode = CreateNode(FilePath);
            FilePath = FilePath.ToUpper();
            try
            {
                for (int i = 0; i < treeView1.Nodes.Count; i++)
                {
                    string _FilePath = string.Empty;
                    if (treeView1.Nodes[i].Tag is string)
                        _FilePath = treeView1.Nodes[i].Tag.ToString();
                    else if (treeView1.Nodes[i].Tag is Tuple<string, ZappyTask>)
                        _FilePath = (treeView1.Nodes[i].Tag as Tuple<string, ZappyTask>).Item1;

                    if (_FilePath.ToUpper() == FilePath)
                    {
                        treeView1.Nodes.RemoveAt(i);
                        break;
                    }
                }
                treeView1.Nodes.Insert(0, TaskNode);
                treeView1.Refresh();
            }
            catch (Exception exception)
            {
                CrapyLogger.log.Error(exception);
            }

            //return TaskNode;
        }


        public void ExpandTreeNode(TreeNode TaskNode)
        {
            if (TaskNode.Tag is string)
            {
                //CrapyLogger.log.Error("Expanding treenode.... " + TaskNode.Tag.ToString());
                try
                {
                    ZappyTask _Task = ZappyTask.Create(TaskNode.Tag.ToString());
                    DisplayHelper.buildfrmZappyDisplay(_Task, TaskNode);
                    TaskNode.Nodes.RemoveAt(0);
                    TaskNode.Tag = new Tuple<string, ZappyTask>(TaskNode.Tag.ToString(), _Task);
                }
                catch
                {

                }
                finally
                {

                }
                //treeView1.Refresh();
            }
        }

        void Cleanup()
        {
            try
            {
                ZappyInvoker.RemoveZappy(_TrackedHandle);
                IntPtr _Hk1 = hk1, _Hk2 = hk2;
                hk1 = hk2 = IntPtr.Zero;
                if (_Hk1 != IntPtr.Zero)
                    UnhookWinEvent(_Hk1);
                if (_Hk2 != IntPtr.Zero)
                    UnhookWinEvent(_Hk2);
                _tmr.Dispose();

                if (NativeMethods.IsWindow(_ThisControlHandle))
                    BeginInvoke(new Action(DestroyHelper));
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);

            }
        }

        void TimerCallback_tmr(object o)
        {
            try
            {
                if (_ThisControlHandle != IntPtr.Zero && _TrackedHandle != IntPtr.Zero && !NativeMethods.IsWindowVisible(_TrackedHandle))
                {
                    Cleanup();
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);

            }
        }

        //public void PredictedAction(ZappyTask PredictedTask)
        //{
        //    this.PredictedTask = PredictedTask;
        //    //CrapyLogger.log.Error("Changing existing instance of Zappy with PredictedTask");
        //    if (InvokeRequired)
        //        this.BeginInvoke(new Action(ChangeColor));
        //    else
        //        ChangeColor();
        //}

        public void ChangeColor()
        {
            //_PredictedTaskNode = AddNode(PredictedTask, "Predicted", _PredictedTaskNode, true); TODO:If use
            this.cmdZappy.BackColor = Color.Orange;
        }

        Dictionary<AccessibleEvents, KeyValuePair<GCHandle, WinEventProc>> InitializeWinEventToHand1erMap()
        {
            Dictionary<AccessibleEvents, KeyValuePair<GCHandle, WinEventProc>> dictionary = new Dictionary<
                AccessibleEvents, KeyValuePair<GCHandle, WinEventProc>>();

            WinEventProc eventHandler = LocationChangedCallback;

            GCHandle gch = GCHandle.Alloc(eventHandler);

            dictionary.Add(AccessibleEvents.LocationChange,
                new KeyValuePair<GCHandle, WinEventProc>(gch, eventHandler));

            eventHandler = DestroyCallback;

            gch = GCHandle.Alloc(eventHandler);

            dictionary.Add(AccessibleEvents.Destroy,
                new KeyValuePair<GCHandle, WinEventProc>(gch, eventHandler));

            return dictionary;
        }

        int right, top;
        private void TrackPosition()
        {
            TITLEBARINFO pti = new TITLEBARINFO();
            pti.cbSize = (uint)Marshal.SizeOf(pti); //Specifies the size, 
            bool res = GetTitleBarInfo(_TrackedHandle, ref pti);
            right = pti.rcTit1eBar.right;
            top = pti.rcTit1eBar.top;
            //SetLocationAndSizeForForm();
            DesktopBounds = new Rectangle(right - 160, top - 4, 16, 16);
        }

        private void cmdZappy_Click(object sender, EventArgs e)
        {
            _Expanded = !_Expanded;
            if (!_Expanded)
            {
                cmdZappy.Visible = true;
                cmdZappy2.Visible = false;
                DesktopBounds = new Rectangle(right - 160, top - 4, 16, 16);
            }
            else
            {
                cmdZappy.Visible = false;
                cmdZappy2.Visible = true;
                DesktopBounds = new Rectangle(right - 160 - 454 / 2, top - 4, 485, 637);
            }
        }

        void DestroyCallback(IntPtr winEventHookHand1e, AccessibleEvents accEvent,
            IntPtr windowHand1e, int objectld, int childld, uint eventThreadId, uint eventTimeInmi11iseconds)
        {
            if (_TrackedHandle != IntPtr.Zero && !NativeMethods.IsWindowVisible(_TrackedHandle))
            {
                Cleanup();

            }
        }

        public void ShowInactiveTopmost()
        {
            try
            {
                if (Handle != IntPtr.Zero && NativeMethods.IsWindowVisible(_TrackedHandle))
                {
                    _ThisControlHandle = Handle;
                    SetWindowLong(_ThisControlHandle, GWLParameter.GWL_HWNDPARENT, _TrackedHandle);
                    //IntPtr _returnFocus = SetFocus(Handle);
                    BackColor = Color.LimeGreen;
                    TransparencyKey = Color.LimeGreen;
                    TrackPosition();
                    //Size = _CollapsedSize;
                    _tmr = new Timer(TimerCallback_tmr, null, 2000, 1000);
                    NativeMethods.SetWindowPos(Handle, IntPtr.Zero, Left, Top, Width, Height,
                        NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOAOWNERZORDER);
                    NativeMethods.ShowWindow(Handle, NativeMethods.WindowShowStyle.ShowNormalNoActivate);
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }



        bool _HandleCheckChange = true;

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_HandleCheckChange)
            {
                treeView1.BeginUpdate();
                _HandleCheckChange = false;

                bool _Checked = e.Node.Checked;

                if (e.Node.Parent == null)
                {
                    if (e.Node.Nodes != null)
                        foreach (TreeNode item in e.Node.Nodes)
                            item.Checked = _Checked;
                }
                else
                {
                    if (_Checked)//check for all sibling nodes to be checked
                    {
                        bool _AllChecked = true;
                        foreach (TreeNode item in e.Node.Parent.Nodes)
                        {
                            _AllChecked &= item.Checked;
                            if (!_AllChecked)
                                break;
                        }
                        e.Node.Parent.Checked = _AllChecked;
                    }
                    else
                        e.Node.Parent.Checked = false;
                }

                _HandleCheckChange = true;
                treeView1.EndUpdate();

            }

        }


        //private void TreeView1_PinnedNodeChanged(TreeNode arg1, bool Pinned)
        //{
        //    try
        //    {
        //        string _AutorunFileName = null;
        //        if (Pinned)
        //        {
        //            Tuple<string, ZappyTask> _Tuple = arg1.Tag as Tuple<string, ZappyTask>;
        //            if (_Tuple == null)
        //            {
        //                if (arg1.Tag is string)
        //                    _AutoExec = arg1.Tag.ToString().ToUpper();
        //            }
        //            else
        //            {
        //                _AutoExec = _Tuple.Item1.ToUpper();

        //            }

        //            treeView1.Nodes.Remove(arg1);
        //            treeView1.Nodes.Insert(0, arg1);
        //            arg1.ImageIndex = 9;
        //            _AutorunFileName = _AutoExec;

        //        }
        //        else
        //        {
        //            //treeView1.Nodes.Remove(arg1);
        //            //treeView1.Nodes.Insert(0, arg1);
        //            arg1.ImageIndex = 0;
        //        }
        //        ZappyInvoker.HandleAutoExecChange(TaskDirPath, _AutorunFileName);

        //        //if (_Tuple != null)
        //        //{
        //        //    ZappyTask _Task = _Tuple.Item2;
        //        //    _Task.AutoExecute = Pinned;
        //        //    _Task.Save(_Tuple.Item1);
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        CrapyLogger.log.Error(ex);
        //    }
        //}

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.SelectedImageIndex = treeView1.SelectedNode.ImageIndex;
        }

        private void frmZappy_Deactivate(object sender, EventArgs e)
        {
            if (_Expanded)
                cmdZappy_Click(sender, e);
        }

        void StartPlayback(string FilePath)
        {
            this.cmdZappy.BackColor = Color.GreenYellow;
            CommonProgram.StartPlaybackFromFile(FilePath);
        }

        void StartPlayback(Tuple<string, IZappyAction> ActionDetails)
        {
            this.cmdZappy.BackColor = Color.GreenYellow;
            try
            {
                CommonProgram.StartPlaybackFromIZappyAction(ActionDetails.Item2);

                //string data = string.Empty;

                //using (StringWriter s = new StringWriter())
                //{
                //    ActionTypeInfo
                //        .SupportedActionTypesSerializers[
                //            Array.IndexOf(ActionTypeInfo.SupportedActionTypes, ActionDetails.Item2.GetType())]
                //        .Serialize(s, ActionDetails.Item2);
                //    data = s.ToString();
                //}

                //if (!string.IsNullOrEmpty(data))
                //{
                //    CommonProgram.StartPlaybackFromAction(data);
                //}
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show("ERR:1500 - Error in task execution!");
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Parent == null)
            {
                frmZappy_Deactivate(sender, e);

                if (treeView1.SelectedNode.Tag is Tuple<string, ZappyTask>)
                    StartPlayback((treeView1.SelectedNode.Tag as Tuple<string, ZappyTask>).Item1);
                else if (treeView1.SelectedNode.Tag is string)
                    StartPlayback(treeView1.SelectedNode.Tag.ToString());
                else if (treeView1.SelectedNode.Tag != null &&
                         _SingletonActivities.Contains(treeView1.SelectedNode.Tag as Tuple<string, IZappyAction>))
                {

                    //ZappyTask uit = new ZappyTask(new IZappyAction[] { (treeView1.SelectedNode.Tag as Tuple<string, IZappyAction>).Item2 });
                    StartPlayback(treeView1.SelectedNode.Tag as Tuple<string, IZappyAction>);
                }
            }
        }


        private void treeView1_RequestDisplayText(object sender, NodeRequestTextEventArgs e)
        {
            try
            {
                if (e.Node.Parent != null)
                {
                    if (e.Node.Tag != null)
                    {
                        if (e.Node.Tag is SendKeysAction _SendKeysAction)
                        {
                            string _NewSendKeysValue = e.Label;
                            _NewSendKeysValue = _NewSendKeysValue.Replace("\n", "{Enter}").Replace("\t", "{Tab}");
                            _SendKeysAction.Text = _NewSendKeysValue;
                            e.Label = e.Node.Text = DisplayHelper.NodeTextHelper(_SendKeysAction) + " (" + DisplayHelper.GetActionElementName(_SendKeysAction) + ")"; // = (e.Node.Tag as SendKeysAction).ActionName;
                            string actionValue = "\nValue " + _SendKeysAction.ValueAsString;
                            e.Node.ToolTipText = DisplayHelper.GetActionElementName(_SendKeysAction) + actionValue; //
                            //TODO: can save the task in same task file
                        }
                        else if (e.Node.Tag is ExcelSendKeysAction _ExcelSendKeysAction)
                        {
                            string _NewSendKeysValue = e.Label;
                            _NewSendKeysValue = _NewSendKeysValue.Replace("\n", "{Enter}").Replace("\t", "{Tab}");
                            _ExcelSendKeysAction.Text = _NewSendKeysValue;
                            e.Label = e.Node.Text = DisplayHelper.NodeTextHelper(_ExcelSendKeysAction) + " (" + DisplayHelper.GetActionElementName(_ExcelSendKeysAction) + ")"; // = (e.Node.Tag as SendKeysAction).ActionName;
                            string actionValue = "\nValue " + _ExcelSendKeysAction.ValueAsString;
                            e.Node.ToolTipText = DisplayHelper.GetActionElementName(_ExcelSendKeysAction) + actionValue; //
                            //TODO: can save the task in same task file
                        }
                        //                    e.Cancel = false;
                        else if (e.Node.Tag is ChromeActionKeyboard curChromeActionKeyboard)
                        {
                            string _NewSendKeysValue = e.Label;
                            _NewSendKeysValue = _NewSendKeysValue.Replace("\n", "{Enter}").Replace("\t", "{Tab}");
                            curChromeActionKeyboard.CommandValue = curChromeActionKeyboard.CommandValue = _NewSendKeysValue;
                            e.Label = e.Node.Text = DisplayHelper.NodeTextHelper(curChromeActionKeyboard) + " (" + DisplayHelper.GetActionElementName(curChromeActionKeyboard) + ")"; // = (e.Node.Tag as SendKeysAction).ActionName;
                            string actionValue = DisplayHelper.ActionValueHelper(curChromeActionKeyboard);// "\nValue " + (curChromeActionKeyboard);
                            e.Node.ToolTipText = DisplayHelper.GetActionElementName((curChromeActionKeyboard)) + actionValue; //
                        }
                        else
                            e.Cancel = true;

                        if (!e.Cancel)
                        {
                            Tuple<string, ZappyTask> _TaskTag = e.Node.Parent.Tag as Tuple<string, ZappyTask>;
                            _TaskTag.Item2.Save(_TaskTag.Item1);
                        }

                    }
                    else
                        e.Cancel = true;
                }
                else
                {
                    string _OriginalFilePath = String.Empty;
                    if (Tag is Tuple<string, ZappyTask>)
                        _OriginalFilePath = (e.Node.Tag as Tuple<string, ZappyTask>).Item1;
                    else if (Tag is string)
                        _OriginalFilePath = e.Node.Tag.ToString();


                    if (!string.IsNullOrEmpty(TaskDirPath))
                    {
                        if (File.Exists(_OriginalFilePath))
                        {
                            string _NewFilePath =
                                Path.Combine(TaskDirPath, e.Label + ".zappy");
                            File.Copy(_OriginalFilePath, _NewFilePath);
                            File.Delete(_OriginalFilePath);
                            e.Node.Tag =
                                new Tuple<string, ZappyTask>(_NewFilePath,
                                    (e.Node.Tag as Tuple<string, ZappyTask>).Item2);
                        }
                        else
                        {

                            string _NewFilePath = Path.Combine(TaskDirPath, e.Label + ".zappy");
                            if (!File.Exists(_NewFilePath))
                            {
                                (e.Node.Tag as Tuple<string, ZappyTask>).Item2.Save(_NewFilePath);
                                e.Node.Tag =
                                    new Tuple<string, ZappyTask>(_NewFilePath,
                                        (e.Node.Tag as Tuple<string, ZappyTask>).Item2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void treeView1_RequestEditText(object sender, NodeRequestTextEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(SendKeysAction))
                {
                    e.Label = (e.Node.Tag as SendKeysAction).ValueAsString;
                }
                else if (e.Node.Tag != null && e.Node.Tag is ChromeActionKeyboard)
                {
                    e.Label = (e.Node.Tag as ChromeActionKeyboard).CommandValue;
                }
                else
                    e.Cancel = true;
            }
            else
            {
                e.Label = e.Node.Text;
            }
        }

        public void PlayFirstTask()
        {
            if (treeView1.Nodes.Count > 0)
            {
                object Tag = treeView1.Nodes[0].Tag;
                if (Tag is Tuple<string, ZappyTask>)
                    StartPlayback((Tag as Tuple<string, ZappyTask>).Item1);
                else if (Tag is string)
                    StartPlayback(Tag.ToString());
                else if (Tag != null &&
                         _SingletonActivities.Contains(Tag as Tuple<string, IZappyAction>))
                {
                    StartPlayback(Tag as Tuple<string, IZappyAction>);
                }
            }
        }

        private void lblExport_Click(object sender, EventArgs e)
        {
            try
            {
                //ZappyTask _UitaskSelectedTasks = GetSelectedZappyTask(); //new ZappyTask();
                ZappyTask _UitaskSelectedTasks = new ZappyTask();
                bool foundSelectedTopNode = false;
                foreach (TreeNode _TopNode in treeView1.Nodes)
                {
                    if (_TopNode.Checked)
                    {
                        _TopNode.Checked = false;
                        if (foundSelectedTopNode)
                        {
                            MessageBox.Show("Please select only one task to Export");
                            return;
                        }
                        if (_TopNode.Tag is Tuple<string, ZappyTask>)
                        {
                            _UitaskSelectedTasks = (_TopNode.Tag as Tuple<string, ZappyTask>).Item2;
                        }
                        else if (_TopNode.Tag is string)
                        {
                            _UitaskSelectedTasks = ZappyTask.Create(_TopNode.Tag.ToString());
                        }
                        else if (_TopNode.Tag is Tuple<string, IZappyAction>)
                        {
                            MessageBox.Show("Please select correct ZappyTask");
                            return;
                        }
                        foundSelectedTopNode = true;
                    }
                }

                if (!foundSelectedTopNode)
                {
                    MessageBox.Show("Please select task to Export");
                    return;
                }

                if (_UitaskSelectedTasks != null && _UitaskSelectedTasks.ExecuteActivities.Count > 0)
                {
                    using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                    {
                        saveFileDialog1.Filter = "Zappy Files|*.zappy";
                        saveFileDialog1.FilterIndex = 2;
                        saveFileDialog1.RestoreDirectory = true;
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            using (Stream myStream = saveFileDialog1.OpenFile())
                                _UitaskSelectedTasks.Save(myStream, saveFileDialog1.FileName, false);
                        }
                    }
                }
                else
                    MessageBox.Show("No Action to Export!!");
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        ZappyTask GetSelectedZappyTask()
        {
            try
            {
                //TODO fix allow multiple task or not
                int _RepeatCount = 1;
                bool isExcelLastRowRepeat = false;
                //Check ExcelLastRow
                if (!string.IsNullOrEmpty(cmbRepeatCount.Text))
                {
                    if (cmbRepeatCount.Text == "LastRowExcel")
                        isExcelLastRowRepeat = true;
                    else if (!int.TryParse(cmbRepeatCount.Text.Trim(), out _RepeatCount))
                    {
                        _RepeatCount = 1;
                        //Can add exception message
                    }
                }

                cmbRepeatCount.SelectedIndex = 0;

                ZappyTask _UitaskSelectedTasks = new ZappyTask();

                List<IZappyAction> _SelectedActivities = new List<IZappyAction>();

                foreach (TreeNode _TopNode in treeView1.Nodes)
                {
                    if (_TopNode.Checked)
                    {
                        //if (j == (_RepeatCount - 1))
                        _TopNode.Checked = false;
                        if (_TopNode.Tag is Tuple<string, ZappyTask>)
                        {
                            ZappyTask _SelectedTask = (_TopNode.Tag as Tuple<string, ZappyTask>).Item2;
                            MergeZappyTasks(_UitaskSelectedTasks, _SelectedTask);
                        }
                        else if (_TopNode.Tag is string)
                        {
                            ZappyTask _SelectedTask = ZappyTask.Create(_TopNode.Tag.ToString());
                            MergeZappyTasks(_UitaskSelectedTasks, _SelectedTask);
                        }
                        else if (_TopNode.Tag is Tuple<string, IZappyAction>)
                        {
                            _SelectedActivities.Clear();
                            _SelectedActivities.Add((_TopNode.Tag as Tuple<string, IZappyAction>).Item2);
                            _UitaskSelectedTasks.Append(_SelectedActivities);
                        }
                    }
                    else
                    {
                        _SelectedActivities.Clear();
                        foreach (TreeNode _ActionNode in _TopNode.Nodes)
                            if (_ActionNode.Checked)
                            {
                                //if (j == (_RepeatCount - 1))
                                _ActionNode.Checked = false;
                                _SelectedActivities.Add(_ActionNode.Tag as IZappyAction);
                            }

                        if (_SelectedActivities.Count > 0)
                        {
                            //Collection<ScreenIdentifier> CurrentMaps = (_TopNode.Tag as Tuple<string, ZappyTask>).Item2.ScreenIdentifiers;
                            //if (!ReferenceEquals(CurrentMaps, null) && CurrentMaps.Count > 0)
                            //{
                            //    _UitaskSelectedTasks.Append(_SelectedActivities, CurrentMaps[0]);
                            //}
                            //else
                            {
                                _UitaskSelectedTasks.Append(_SelectedActivities);

                            }
                        }
                    }
                }

                if (_UitaskSelectedTasks.ExecuteActivities.Activities.Count > 0)
                {

                    List<IZappyAction> _OriginalActivities = new List<IZappyAction>(_UitaskSelectedTasks.ExecuteActivities.Activities.Count);

                    for (int i = 0; i < _UitaskSelectedTasks.ExecuteActivities.Activities.Count; i++)
                    {
                        if (_UitaskSelectedTasks.ExecuteActivities.Activities[i] is StartNodeAction || _UitaskSelectedTasks.ExecuteActivities.Activities[i] is EndNodeAction)
                            continue;
                        _OriginalActivities.Add(_UitaskSelectedTasks.ExecuteActivities.Activities[i]);
                    }

                    for (int i = 0; i < _OriginalActivities.Count; i++)
                    {
                        _OriginalActivities[i].NextGuid = Guid.Empty;
                        //_OriginalActivities[i].SelfGuid = Guid.Empty;
                    }
                    _UitaskSelectedTasks.ExecuteActivities.Activities.Clear();
                    _UitaskSelectedTasks.ExecuteActivities.Activities.AddRange(_OriginalActivities);
                    List<IZappyAction> _UitaskSelectedTasksActions = _UitaskSelectedTasks.ExecuteActivities.Activities;
                    if (_RepeatCount > 1 || isExcelLastRowRepeat)
                    {
                        //By default
                        //foreach (IZappyAction action in _UitaskSelectedTasksActions)
                        //{
                        //    if (action is ExcelSendKeysAction eaction)
                        //        eaction.UseFocusedRow = true;
                        //}

                        _UitaskSelectedTasks.BeforeSerialize(false);
                        if (_UitaskSelectedTasksActions[0] is StartNodeAction
                            startNodeAction &&
                            _UitaskSelectedTasksActions[_UitaskSelectedTasksActions.Count - 1]
                                is EndNodeAction endNodeAction &&
                            _UitaskSelectedTasksActions[_UitaskSelectedTasksActions.Count - 2].NextGuid == endNodeAction.SelfGuid)
                        {
                            ForLoopStartAction forLoopStartAction = new ForLoopStartAction();
                            forLoopStartAction.StepValue = 1;
                            forLoopStartAction.NextGuid = startNodeAction.NextGuid;
                            ForLoopEndAction forLoopEndAction = new ForLoopEndAction();
                            _UitaskSelectedTasksActions[_UitaskSelectedTasksActions.Count - 2].NextGuid =
                                forLoopEndAction.SelfGuid;
                            forLoopEndAction.NextGuid = endNodeAction.SelfGuid;
                            //Linking loop start and end
                            forLoopStartAction.LoopEndGuid = forLoopEndAction.SelfGuid;
                            forLoopEndAction.LoopStartGuid = forLoopStartAction.SelfGuid;

                            //Below values changes for excel
                            if (isExcelLastRowRepeat)
                            {
                                //Call a function to add find last row and get fouced row - insert it into top
                                //loopsfrom current row till last row
                                _UitaskSelectedTasks =
                                    LearnedActionFilters.excelLoopFromFocusedRowToLastRow(_UitaskSelectedTasks,
                                        startNodeAction, forLoopStartAction);
                                if (_UitaskSelectedTasks == null)
                                {
                                    MessageBox.Show("No Excel Actions Found");
                                    return null;
                                }
                            }
                            else
                            {
                                startNodeAction.NextGuid = forLoopStartAction.SelfGuid;
                                forLoopStartAction.InitialValue = 1;
                                forLoopStartAction.FinalValue = _RepeatCount;
                                _UitaskSelectedTasks.ExecuteActivities.Activities.Insert(_UitaskSelectedTasksActions.IndexOf(startNodeAction) + 1, forLoopStartAction);
                            }
                            _UitaskSelectedTasks.ExecuteActivities.Activities.Insert(_UitaskSelectedTasksActions.IndexOf(endNodeAction), forLoopEndAction);
                        }
                        else
                        {
                            MessageBox.Show("Error 3454 - Start Node, End Node Unavailable");
                            return null;
                        }

                        //for (int j = 1; j < _RepeatCount; j++)
                        //{
                        //    //Clone and add
                        //    List<IZappyAction> ClonedActivites =
                        //        frmZappyHelper.CloneSelectedActivities(_OriginalActivities);
                        //    _UitaskSelectedTasks.ExecuteActivities.Activities.AddRange(ClonedActivites);
                        //}
                    }
                }

                return _UitaskSelectedTasks;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show("Error exporting selected actions!!");
                return null;
            }
        }


        private void MergeZappyTasks(ZappyTask Destination_AppendActionsHere, ZappyTask Source)
        {
            //Collection<ScreenIdentifier> CurrentMaps = Source.ScreenIdentifiers;

            //if (!ReferenceEquals(CurrentMaps, null) && CurrentMaps.Count > 0)
            //{
            //    Destination_AppendActionsHere.Append(
            //        Source.ExecuteActivities.Activities,
            //        CurrentMaps[0]);
            //}
            //else
            {
                Destination_AppendActionsHere.Append(
                    Source.ExecuteActivities.Activities);
            }
        }


        private void lblImport_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Zappy Files|*.zappy";
            openFileDialog1.Title = "Select a Task File";
            openFileDialog1.Multiselect = true;
            openFileDialog1.ValidateNames = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog1.FileNames)
                {
                    try
                    {
                        ZappyTask taskToPlay = ZappyTask.Create(File.OpenRead(file));
                        string _NewFileName = Path.Combine(TaskDirPath, Path.GetFileName(file));
                        taskToPlay.Save(_NewFileName);
                        RemoveInsertNewNode(_NewFileName);
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lblExecute_Click(sender, e);
        }

        private void lblDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<TreeNode> nodesToDelete = new List<TreeNode>();
                foreach (TreeNode _TopNode in treeView1.Nodes)
                {
                    if (_TopNode.Checked)
                    {
                        if (_TopNode.Tag is Tuple<string, ZappyTask>)
                        {

                            File.Delete((_TopNode.Tag as Tuple<string, ZappyTask>).Item1);
                            nodesToDelete.Add(_TopNode);
                        }
                        else if (_TopNode.Tag is string)
                        {

                            File.Delete(_TopNode.Tag.ToString());
                            nodesToDelete.Add(_TopNode);
                        }

                    }
                }

                if (nodesToDelete.Count > 0)
                {
                    foreach (TreeNode _TopNode in nodesToDelete)
                    {
                        treeView1.Nodes.Remove(_TopNode);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to delete file");
            }
        }

        private void toolStripTaskEditor_Click(object sender, EventArgs e)
        {
            string _filename = null;
            bool foundSelectedTopNode = false;
            foreach (TreeNode _TopNode in treeView1.Nodes)
            {
                if (_TopNode.Checked)
                {
                    _TopNode.Checked = false;
                    if (foundSelectedTopNode)
                    {
                        MessageBox.Show("Please select only one task to open in editor");
                        return;
                    }
                    else if (_TopNode.Tag is Tuple<string, ZappyTask>)
                    {
                        _filename = ((_TopNode.Tag as Tuple<string, ZappyTask>).Item1);
                    }
                    else if (_TopNode.Tag is string)
                    {
                        _filename = _TopNode.Tag.ToString();
                    }
                    foundSelectedTopNode = true;
                }
            }

            if (_filename == null)
            {
                MessageBox.Show("Please select task to open in Editor");
                return;
            }
            PageFormTabbed frm = new PageFormTabbed(_filename);
            frm.Show();
            //TODO - send multiple files here
            //frm.CreateNewTab();
        }

        void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = "en";
                if (languageZappy == LanguageZappy.jp)
                {
                    lang = "ja-JP";
                }
                ComponentResourceManager resources = null;

                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeof(frmZappy));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                }

                foreach (ToolStripItem item in toolStrip1.Items)
                {
                    if (item is ToolStripDropDownItem)
                    {
                        resources = new ComponentResourceManager(typeof(frmZappy));
                        foreach (ToolStripItem dropDownItem in ((ToolStripDropDownItem)item).DropDownItems)
                        {
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


        private void lblExecute_Click(object sender, EventArgs e)
        {
            string _CustomExecutionTaskFileName = "CustomActivities";
            ZappyTask _UitaskSelectedTasks = GetSelectedZappyTask();

            if (_UitaskSelectedTasks != null && _UitaskSelectedTasks.ExecuteActivities.Count > 0)
            {
                string _FileName = CrapyWriter.Save(_UitaskSelectedTasks, TaskDirPath, _CustomExecutionTaskFileName + ".zappy");
                RemoveInsertNewNode(_FileName);
                StartPlayback(_FileName);
            }
            else
            {
                CrapyLogger.log.Error("Error Executing task!!");
            }
        }

        void LocationChangedCallback(IntPtr winEventHookHand1e, AccessibleEvents accEvent,
            IntPtr windowHand1e, int objectld, int childld, uint eventThreadId, uint eventTimeInmi11iseconds)
        {
            try
            {
                if (_TrackedHandle != IntPtr.Zero && windowHand1e == _TrackedHandle &&
                    _ThisControlHandle != IntPtr.Zero)
                    BeginInvoke(new Action(TrackPosition));
            }
            catch
            {
                Cleanup();
                Close();
            }

        }

        void DestroyHelper()
        {
            Close();
        }

    }
}


