#if UseMangoDB
using MangoDBConnector;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zappy.ActionMap.ElementManager;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.LowLevelHookEvent;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Hooks.PropertyChanged;
using Zappy.Decode.Hooks.Window;
using Zappy.Decode.LogManager;
using Zappy.Decode.Screenshot;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.Trapy;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Miscellaneous;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.RecordPlayback;

namespace Zappy.Graph
{
    public class GraphBuilder
    {
        public const int _ActionBatchingTimer = 500;
        private readonly List<IEventCapture> eventCaptures;

        private readonly IElementManager elementManager;

        private readonly ZappyTaskActionStack rawActionList;

        private readonly MouseEventCapture mouseEventCapture;
        private readonly WindowLaunchEventCapture _WindowLaunchEventCapture;
        private readonly PropertyChangeEventCapture _PropertyChangeEventCapture;

        private readonly KeyboardEventCapture keyboardEventCapture;

        internal ZappyTaskActionStack filteredActionList;
        private readonly UIElementDictionary uiElementList;

        private readonly RecorderOptions recorderOptions;
        private readonly IAggregatorFilter aggregatorFilter;

        public List<IZappyAction> RecordedEvents;

        public List<IZappyAction> _BatchedActivities;

        public DateTime LastActivity { get; set; }

                                private Timer _tmr;
        public static Type[] _IgnoredActivities = new Type[] { typeof(DelayAction), typeof(ErrorAction) };

        
        private int _LastWrittenAction = -1, _LastMLBatchedActionIndex = -1, _LastCleaned = -1;

        public event Action<List<IZappyAction>> BatchedActivities, MLBatchedActivities;

        private DateTime _LastMLBatchEventRaisedTime;

        private Process _ZappyPlaybackHelperProcess;

                private Process _zappyJavaHookProcess;


        private static BlockingCollection<IZappyAction> _ZappyRecordedEvents;

#if UseMangoDB
        private DataBaseImageFuncitionality dataBaseImageFuncitionality;
#endif
        
        public GraphBuilder()
        {
                                    _LastMLBatchEventRaisedTime = DateTime.MinValue;
                        RecordedEvents = new List<IZappyAction>();
            _BatchedActivities = new List<IZappyAction>();
            elementManager = new ElementManager();
            rawActionList = new ZappyTaskActionStack();

                        uiElementList = new UIElementDictionary();
            elementManager = new ElementManager();
            elementManager.UIElementList = uiElementList;
            filteredActionList = new ZappyTaskActionStack();

                        recorderOptions = new RecorderOptions();
            ZappyTaskService.Instance.Initialize();
            ZappyTaskService.Instance.StartSession(true);

            _WindowLaunchEventCapture = new WindowLaunchEventCapture(null);
            _PropertyChangeEventCapture = new PropertyChangeEventCapture(_WindowLaunchEventCapture);
            mouseEventCapture = new MouseEventCapture(_PropertyChangeEventCapture);

            keyboardEventCapture = new KeyboardEventCapture(_PropertyChangeEventCapture);

            eventCaptures = new List<IEventCapture>();
            eventCaptures.Add(_WindowLaunchEventCapture);
            eventCaptures.Add(_PropertyChangeEventCapture);
            eventCaptures.Add(mouseEventCapture);
            eventCaptures.Add(keyboardEventCapture);

            foreach (var capture2 in eventCaptures)
            {
                capture2.ElementManager = elementManager;
                capture2.RawActionList = rawActionList;
                                capture2.RecorderOptions = recorderOptions;
            }

            aggregatorFilter = new Aggregator();
            aggregatorFilter.RawActionList = rawActionList;
            aggregatorFilter.FilteredActionList = filteredActionList;

            ProcessManager.SetRecorderOptions(recorderOptions);
            elementManager.RecorderOptions = recorderOptions;
            aggregatorFilter.RecorderOptions = recorderOptions;


            _WindowLaunchEventCapture.Start();
            _PropertyChangeEventCapture.Start();

            aggregatorFilter.Start();

            TrapyService.KeyBoardInfoEvent += KeyBoardInfoEvent_Handler;
            TrapyService.MouseHookInfoEvent += MouseHookInfoEvent_Handler;

            aggregatorFilter.OnFilteredAction += OnRecordAction;
                                    _ZappyRecordedEvents = new BlockingCollection<IZappyAction>();
            Task.Factory.StartNew(RecordScreenshot);
                                    _tmr = new Timer(_TimerCallback, null, _ActionBatchingTimer, _ActionBatchingTimer);

                                    if (ApplicationSettingProperties.Instance.EnableFullAuditLog)
                _ZappyPlaybackHelperProcess = CommonProgram.RunProcessAsTask(Path.Combine(ZappyMessagingConstants.StartupPath, "ZappyPlaybackHelper.exe"), "--FullTrace=True");
            else
                _ZappyPlaybackHelperProcess = CommonProgram.RunProcessAsTask(Path.Combine(ZappyMessagingConstants.StartupPath, "ZappyPlaybackHelper.exe"));

            if (ApplicationSettingProperties.Instance.EnableJava32Bit)
            {
                _zappyJavaHookProcess = CommonProgram.RunProcessAsTask(Path.Combine
                    (ZappyMessagingConstants.ExtensionsFolder, "Java", "JavaBridgeX86", "ZappyJavaBridge.exe"));
            }
            else if (ApplicationSettingProperties.Instance.EnableJava64Bit)
            {
                _zappyJavaHookProcess = CommonProgram.RunProcessAsTask(Path.Combine
                    (ZappyMessagingConstants.ExtensionsFolder, "Java", "JavaBridgeX64", "ZappyJavaBridge.exe"));
            }

                        
            LastActivity = DateTime.MaxValue;

#if UseMangoDB
            dataBaseImageFuncitionality = new DataBaseImageFuncitionality(MLTaskWriter.dataBaseConnector,
                ZappyMessagingConstants.ImageFolderCollectionName);
#endif
        }

        private int _ML_Count = 0, _TimerCounter = 0;

        private void _TimerCallback(object dummy)
        {
            lock (_tmr)
            {
                _tmr.Change(Timeout.Infinite, Timeout.Infinite);
                _TimerCounter++;
                try
                {
                    if (RecordedEvents.Count > 0)
                    {
                        var _Now = WallClock.UtcNow;
                        _BatchedActivities.Clear();
                        for (var i = _LastWrittenAction + 1; i < RecordedEvents.Count; i++)
                        {
                            IZappyAction _Action = RecordedEvents[i];
                            Type _ActionType = _Action.GetType();
                            if ((_Now - _Action.Timestamp).TotalSeconds > 1)                             {

                                _LastWrittenAction = i;

                                if (Array.IndexOf(_IgnoredActivities, _ActionType) < 0)
                                {
                                    _BatchedActivities.Add(_Action);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                                                BatchedActivities?.Invoke(_BatchedActivities);

                        if (Properties.Settings.Default.ShowRecordedStepNotification && ClientUI._TaskRecording)
                        {
                            for (int i = 0; i < _BatchedActivities.Count; i++)
                            {
                                                                
                                frmActionLearner.LearningStepInstance.UpdateAction(_BatchedActivities[i]);
                                                                                                                                                            }
                        }

                        if (_TimerCounter % 1000 == 0)
                        {
                            if (!ClientUI._TaskRecording)
                                RemoveZappyTechManager(_Now);
                        }

                                                if (!ClientUI._TaskRecording && ((_Now - _LastMLBatchEventRaisedTime).Ticks > CrapyConstants.MLBatchTicks))
                            MLBatchedActivitiesInvoker(_Now);
                    }
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                }
                _tmr.Change(_ActionBatchingTimer, _ActionBatchingTimer);
            }
        }

        private void RemoveZappyTechManager(DateTime _Now)
        {

            _BatchedActivities.Clear();
            for (var i = _LastCleaned + 1; i < RecordedEvents.Count; i++)
            {
                IZappyAction _Action = RecordedEvents[i];
                if ((_Now - _Action.Timestamp).TotalSeconds > CrapyConstants.RemoveTechnologyElementTimeSec)                 {
                    _LastCleaned = i;
                    _BatchedActivities.Add(_Action);
                }
                else
                {
                    break;
                }

            }

            if (_BatchedActivities.Count > 0)
            {
                                                                                                                                foreach (var action in _BatchedActivities)
                {
                    if (action is ZappyTaskAction)
                    {
                                                                                                lock (action)
                        {
                            ZappyTaskAction zaction = action as ZappyTaskAction;
                            zaction.CopyActivityElementWithoutNotifier = null;
                        }
                    }
                }
#if COMENABLED
                GC.Collect(2, GCCollectionMode.Optimized, false, true);
                GC.WaitForPendingFinalizers();
#endif
            }
        }

        private void MLBatchedActivitiesInvoker(DateTime _Now)
        {
            _ML_Count++;
            _BatchedActivities.Clear();
            for (int i = _LastMLBatchedActionIndex + 1; i < RecordedEvents.Count; i++)
            {
                IZappyAction _Action = RecordedEvents[i];
                                if ((_Now - _Action.Timestamp).TotalSeconds > 10)
                {
                    _LastMLBatchedActionIndex = i;

                    if (_Action.Timestamp == DateTime.MinValue)
                        continue;

                    Type _ActionType = _Action.GetType();
                    if (Array.IndexOf(_IgnoredActivities, _ActionType) < 0)
                        _BatchedActivities.Add(_Action);
                }
                else
                {
                    break;
                }
            }

            if (_BatchedActivities.Count > 0)
                MLBatchedActivities?.Invoke(_BatchedActivities);

            if (_ML_Count % 3 == 0)
            {
                                            }

            _LastMLBatchEventRaisedTime = _Now;
        }

        public void CompleteAggregation()
        {
            aggregatorFilter.CompleteAggregation();
        }

        public void Stop(bool completeAggerationCall = true)
        {
            if (completeAggerationCall)
            {
                CompleteAggregation();
            }

            _LastMLBatchEventRaisedTime = DateTime.MinValue;
            _ML_Count = 0;
            _TimerCallback(null);
        }

        public void Dispose()
        {
            Stop(false);
            if (Environment.OSVersion.Version < new Version(6, 2))
            {
                                                if (_ZappyPlaybackHelperProcess != null && !_ZappyPlaybackHelperProcess.HasExited)
                    _ZappyPlaybackHelperProcess.Kill();
                if (_zappyJavaHookProcess != null && !_zappyJavaHookProcess.HasExited)
                    _zappyJavaHookProcess.Kill();
            }
        }


        private void OnRecordAction(object sender, RecordedEventArgs args)
        {
                        if (args.ZappyTaskEventType == RecordedEventType.NewAction ||
                args.ZappyTaskEventType == RecordedEventType.ModifiedAction)
            {
                if (!RecordedEvents.ContainsFromEnd(args.Action))
                {
                    lock (RecordedEvents)
                    {
                        if (args.Action is ChromeAction caction)
                        {
                            var zAction = RecordedEvents[RecordedEvents.Count - 1] as ZappyTaskAction;
                            if (zAction != null)
                            {
                                caction.ShallowCopy(zAction, true);
                                if (!(zAction is ChromeAction))
                                {

                                    RecordedEvents.RemoveAt(RecordedEvents.Count - 1);
                                }
                            }
                        }
                        
                        RecordedEvents.Add(args.Action);
                                                                    }
                }

                if (ApplicationSettingProperties.Instance.EnableRecordScreenshots)
                {
                                        _ZappyRecordedEvents.Add(args.Action);
                }
            }
            else if (args.ZappyTaskEventType == RecordedEventType.DeletedAction)
            {
                int _Index = RecordedEvents.IndexFromEnd(args.Action);
                if (_Index >= 0)
                {
                    lock (RecordedEvents)
                    {
                        RecordedEvents.RemoveAt(_Index);
                    }
                }
            }
        }

                        public void RecordScreenshot()
        {      
                                                            foreach (IZappyAction action in _ZappyRecordedEvents.GetConsumingEnumerable())
            {
                try
                {
                    if (action is SendKeysAction || action is MouseAction)
                    {
                        bool resizeImage = true;
                        System.Drawing.Rectangle BoundingRectangle = Rectangle.Empty;
                        ZappyTaskAction zaction = action as ZappyTaskAction;
                        if (zaction.ActivityElement != null)
                        {
                           
                            ActionListSerializer.FixZappyActionIdentifier(zaction);
                            

                            if (ApplicationSettingProperties.Instance.screenShotRecordingProcesses != null
                                && ApplicationSettingProperties.Instance.screenShotRecordingProcesses.Count > 0)
                            {
                                                                                                if (!ApplicationSettingProperties.Instance.screenShotRecordingProcesses.Contains(zaction.ExeName))
                                    continue;
                                                            }
#if UseMangoDB
                            if(!dataBaseImageFuncitionality.ImageDataExists(zaction.TaskActivityIdentifier))
                            {
                                System.Drawing.Rectangle BoundingRectangle = 
                                        ElementExtension.GetBoundingRectangleForScreenShot(zaction.ActivityElement);
                                Image actionScreenShot = ImageCaptureUtilitys.CaptureScreenShotAndDrawBounds(BoundingRectangle, 4, true, compressImage);
                                string ImageString = ImageCaptureUtility.ConvertImageToBase64String(actionScreenShot);
                                dataBaseImageFuncitionality.InsertImageData(ImageString, zaction.TaskActivityIdentifier);
                            }
#else
                            string imagePath = Path.Combine(CrapyConstants.MachineLearningFolder, zaction.TaskActivityIdentifier + ".jpg");
                            if (!File.Exists(imagePath))
                            {
                                try
                                {
                                    if (BoundingRectangle == Rectangle.Empty)
                                    {
                                        BoundingRectangle =
                                            ElementExtension.GetBoundingRectangleForScreenShot(zaction.ActivityElement);
                                    }
                                                                        if (BoundingRectangle != Rectangle.Empty)
                                    {
                                        Image actionScreenShot = ImageCaptureUtilitys.CaptureScreenShotAndDrawBounds(BoundingRectangle, 
                                            4, true, resizeImage);
                                        actionScreenShot.Save(imagePath,
                                        ImageCaptureUtility.myImageCodecInfo, ImageCaptureUtility.myEncoderParameters);
                                    }
                                }
                                catch(Exception ex)
                                {
                                                                    }
                            }
#endif
                        }
                    }

                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                }

            }
        }

        public void EditAction(IZappyAction Action, RecordedEventType EventType)
        {
            OnRecordAction(null, new RecordedEventArgs(Action, EventType, false));
        }

        public void MouseHookInfoEvent_Handler(MouseHookInfo mouseHookInfo)
        {
            NativeMethods.MouseLLHookStruct _HookStruct = new NativeMethods.MouseLLHookStruct();

            _HookStruct.dwExtraInfo = mouseHookInfo.dwExtraInfo;
            _HookStruct.flags = mouseHookInfo.flags;
            _HookStruct.mouseData = mouseHookInfo.mouseData;
            _HookStruct.pt.x = mouseHookInfo.Point.X;
            _HookStruct.pt.y = mouseHookInfo.Point.Y;
            _HookStruct.time = mouseHookInfo.EventTime;

            mouseEventCapture.LowLevelHookHandlerInternal((LowLevelHookMessage)mouseHookInfo.Event, _HookStruct);

            LastActivity = DateTime.Now;
        }

        public void KeyBoardInfoEvent_Handler(KeyBoardInfo keyBoardInfo)
        {
            var _hookStruct = new NativeMethods.KeyboardHookStruct();
            _hookStruct.dwExtraInfo = keyBoardInfo.dwExtraInfo;
            _hookStruct.flags = keyBoardInfo.Flags;
            _hookStruct.scanCode = keyBoardInfo.ScanCode;
            _hookStruct.time = keyBoardInfo.EventTime;
            _hookStruct.vkCode = keyBoardInfo.VirtualKeyCode;

            keyboardEventCapture.LowLevelHookHandlerInternal((LowLevelHookMessage)keyBoardInfo.Event, _hookStruct);

            LastActivity = DateTime.Now;
        }
    }
}
