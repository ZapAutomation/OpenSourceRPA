using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Miscellaneous;

namespace Zappy.Decode.Aggregator
{
    internal sealed class Aggregator : IAggregatorFilter
    {
        private ManualResetEvent aggregationCompleteEvent;
        private ZappyTaskActionStack filteredActionList;
        private Dictionary<ZappyTaskAction, RecordedEventArgs> filterEvents = new Dictionary<ZappyTaskAction, RecordedEventArgs>();
        private object filterEventsLock = new object();
        private Thread filterThread;
                private volatile ZappyTaskAction lastAggregatedAction;
        private ZappyTaskActionStack rawActionList;
                private RecorderOptions recorderOptions;
        private long startTimeStamp;

        log4net.ILog
            _AggregatedLogs = log4net.LogManager.GetLogger("AggregatedActionLogger"),
            _RawInputActionLog = log4net.LogManager.GetLogger("RawActionLogger"),
            _AggregatorLogger = log4net.LogManager.GetLogger("AggregatorLogger");

        [field: CompilerGenerated]
        public event EventHandler<RecordedEventArgs> OnFilteredAction;

        [field: CompilerGenerated]
        public event EventHandler<EventArgs> Started;

        private void AddOrUpdateFilterEvent(RecordedEventArgs eventArgs)
        {
            DelayAction action = eventArgs.Action as DelayAction;
            if (action == null || action.Duration >= 0)
            {
                object[] objArray1 = { eventArgs.ZappyTaskEventType, eventArgs.Action };
                _RawInputActionLog.InfoFormat("AddOrUpdateFilterEvent - ZappyTaskEventType: {0}, Action: {1}", objArray1);
                object filterEventsLock = this.filterEventsLock;
                lock (filterEventsLock)
                {
                    if (filterEvents.ContainsKey(eventArgs.Action as ZappyTaskAction))
                    {
                        ZappyTaskAction key = eventArgs.Action as ZappyTaskAction;
                        RecordedEventArgs args = filterEvents[key];
                        if (args.ZappyTaskEventType == RecordedEventType.NewAction && eventArgs.ZappyTaskEventType == RecordedEventType.DeletedAction)
                        {
                            filterEvents.Remove(key);
                        }
                        else if (args.ZappyTaskEventType == RecordedEventType.NewAction && eventArgs.ZappyTaskEventType == RecordedEventType.ModifiedAction)
                        {
                            filterEvents.Remove(key);
                            filterEvents.Add(key, new RecordedEventArgs(key, RecordedEventType.NewAction, false));
                        }
                        else if (args.ZappyTaskEventType == RecordedEventType.DeletedAction && eventArgs.ZappyTaskEventType == RecordedEventType.NewAction)
                        {
                            filterEvents.Remove(key);
                            filterEvents.Add(key, new RecordedEventArgs(key, RecordedEventType.ModifiedAction, false));
                        }
                        else
                        {
                            filterEvents.Remove(key);
                            filterEvents.Add(key, eventArgs);
                        }
                    }
                    else
                    {
                        filterEvents.Add(eventArgs.Action as ZappyTaskAction, eventArgs);
                    }
                }
            }
        }

        private static void FixStartEndTime(List<RecordedEventArgs> actionEvents, long startTimestamp, long endTimestamp)
        {
            foreach (RecordedEventArgs args in actionEvents)
            {
                ZappyTaskAction _temp = args.Action as ZappyTaskAction;

                if (_temp != null)
                {
                    if (_temp.StartTimestamp > startTimestamp)
                    {
                        _temp.StartTimestamp = startTimestamp;
                    }
                    if (_temp.EndTimestamp < endTimestamp)
                    {
                        _temp.EndTimestamp = endTimestamp;
                    }
                }

            }
        }

        private static void GetMinMaxTime(List<RecordedEventArgs> actionEvents, out long min, out long max)
        {
            min = 0x7fffffffffffffffL;
            max = -9223372036854775808L;
            foreach (RecordedEventArgs args in actionEvents)
            {
                ZappyTaskAction _temp = args.Action as ZappyTaskAction;

                if (_temp != null)
                {
                    min = Math.Min(_temp.StartTimestamp, min);
                    max = Math.Max(_temp.EndTimestamp, max);
                }
            }
        }

        void IAggregatorFilter.CompleteAggregation()
        {
                        {
                _AggregatorLogger.InfoFormat("CompleteAggregation started.");
                if (filterThread == null || (filterThread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped | ThreadState.Unstarted)) != ThreadState.Running)
                {
                    _AggregatorLogger.InfoFormat("CompleteAggregation : Error : filterThread is either null, aborted, stopped or unstarted");
                }
                else
                {
                    DelayAction element = new DelayAction(-1);
                    rawActionList.Enqueue(element);
                    try
                    {
                        filterThread.Priority = ThreadPriority.AboveNormal;
                        try
                        {
                            rawActionList.WaitForEmpty();
                            while (lastAggregatedAction == null || lastAggregatedAction.Id != element.Id)
                            {
                                aggregationCompleteEvent.WaitOne();
                            }
                        }
                        finally
                        {
                                                        filterEvents.Clear();
                        }
                    }
                    catch (ThreadStateException exception)
                    {
                        object[] args = { exception };
                        _AggregatorLogger.InfoFormat("CompleteAggregation : Error : {0}", args);
                        return;
                    }
                    _AggregatorLogger.InfoFormat("Removing dummy action.");
                    for (int i = 0; i < filteredActionList.Count; i++)
                    {
                        if (filteredActionList.Peek(i).Id == element.Id)
                        {
                            filteredActionList.Pop(i);
                            break;
                        }
                    }
                    _AggregatorLogger.InfoFormat("Removing trailing implicit hovers.");
                    while (filteredActionList.Count > 0 && MouseAction.IsImplicitHover(filteredActionList.Peek()))
                    {
                        ZappyTaskAction action2 = filteredActionList.Pop();
                    }
                    if (recorderOptions.RecordThinkTime && filteredActionList.Count > 0)
                    {
                        long startTimeStamp = this.startTimeStamp;
                        foreach (ZappyTaskAction action3 in filteredActionList)
                        {
                            if (action3.StartTimestamp == 0)
                            {
                                action3.StartTimestamp = action3.EndTimestamp = startTimeStamp;
                            }
                            else
                            {
                                action3.ThinkTime = (int)(action3.StartTimestamp - startTimeStamp);
                                if (action3.ThinkTime < recorderOptions.ThinkTimeThreshold)
                                {
                                    action3.ThinkTime = 0;
                                }
                                else
                                {
                                    action3.ThinkTime = Math.Min(action3.ThinkTime, recorderOptions.ThinkTimeUpperCutoff);
                                    action3.ThinkTime = (action3.ThinkTime + 0x3e8 - 1) / 0x3e8 * 0x3e8;
                                }
                                startTimeStamp = action3.EndTimestamp;
                            }
                        }
                    }
                    ProcessFilteredEvents();
                    _AggregatorLogger.InfoFormat("CompleteAggregation ended.");
                }
            }
        }


        void IAggregatorFilter.ReliableStop()
        {
            _AggregatorLogger.Info("IAggregatorFilter.ReliableStop() started.");
            if (filterThread == null)
            {
                _AggregatorLogger.InfoFormat("Aggregator thread already exited.");
            }
            else
            {
                rawActionList.EndOfQueue = true;
                ZappyTaskUtilities.SafeThreadJoin(filterThread, 0x1388, true);
                filterThread = null;
                ActionFilterManager.CleanupFilterList();
                _AggregatorLogger.Info("IAggregatorFilter.ReliableStop() ended.");
            }
        }

        void IAggregatorFilter.Start()
        {
            ActionFilterManager.InitializeActionFilters(false);
            ActionFilterManager.ConfigureActionFilters(recorderOptions);
            rawActionList.EndOfQueue = false;
            lastAggregatedAction = null;
            startTimeStamp = WallClock.Now.Ticks / 0x2710L;
            aggregationCompleteEvent = new ManualResetEvent(false);
            filterThread = new Thread(ProcessEvents);
            filterThread.Name = "AggregatorThread";
            filterThread.SetApartmentState(ApartmentState.STA);
                                    filterThread.Start();
        }

        void IAggregatorFilter.Stop()
        {
                        {
                if (filterThread != null)
                {
                    _AggregatorLogger.Info("IAggregatorFilter.Stop() started.");
                    rawActionList.EndOfQueue = true;
                    ZappyTaskUtilities.SafeThreadJoin(filterThread);
                    filterThread = null;
                    ActionFilterManager.CleanupFilterList();
                    aggregationCompleteEvent.Dispose();
                    aggregationCompleteEvent = null;
                    _AggregatorLogger.Info("IAggregatorFilter.Stop() ended.");
                }
            }
        }

        private void OnListQueueStackChange(object sender, RecordedEventArgs eventArgs)
        {
            AddOrUpdateFilterEvent(eventArgs);
        }

        private void OnStarted(object sender, EventArgs args)
        {
            Started?.Invoke(this, args);
        }


        internal void ProcessEvents()
        {
            OnStarted(this, EventArgs.Empty);
            _AggregatorLogger.InfoFormat("Aggregator.ProcessEvents() Thread Started");
            try
            {
                WaitResult success = WaitResult.Success;
                while (success != WaitResult.EndOfQueue)
                {
                    success = rawActionList.WaitForElement(500);
                    if (success == WaitResult.Success)
                    {
                        if (aggregationCompleteEvent != null)
                        {
                            aggregationCompleteEvent.Reset();
                        }
                        ZappyTaskAction element = rawActionList.Dequeue();
                        object syncRoot = filteredActionList.SyncRoot;
                        lock (syncRoot)
                        {
                            filterEvents.Clear();

                                                        if (element is LaunchApplicationAction)
                            {
                                filterEvents.Remove(element);
                                filterEvents.Add(element, new RecordedEventArgs(element, RecordedEventType.NewAction, false));
                            }
                            else
                            {
                                filteredActionList.Enqueue(element);
                                ActionFilterManager.FilterActivities(filteredActionList);
                            }

                                                                                                                                                                                                                                                                                        
                            ProcessFilteredEvents();

                            lastAggregatedAction = element;
                            if (aggregationCompleteEvent != null)
                            {
                                aggregationCompleteEvent.Set();
                            }
                        }
                    }
                }
            }
            catch (ZappyTaskException)
            {
                            }
        }

        SendKeysAction _LastSendKeysAction;

        private void ProcessFilteredEvents()
        {
            long num;
            long num2;
            List<RecordedEventArgs> actionEvents = new List<RecordedEventArgs>();
            List<RecordedEventArgs> list2 = new List<RecordedEventArgs>();
            List<RecordedEventArgs> list3 = new List<RecordedEventArgs>();
            List<ZappyTaskAction> list4 = new List<ZappyTaskAction>();


            lock (filterEventsLock)
            {
                foreach (KeyValuePair<ZappyTaskAction, RecordedEventArgs> pair in filterEvents)
                {
                    object[] objArray1 = { pair.Value.ZappyTaskEventType, pair.Value.Action };
                    _AggregatedLogs.InfoFormat("ProcessFilteredEvents - ZappyTaskEventType: {0}, Action: {1}", objArray1);


                    switch (pair.Value.ZappyTaskEventType)
                    {
                        case RecordedEventType.NewAction:
                            if (!MergeSendKeysActivitiesRemoveStandaloneModifierKeys(pair.Key))
                                actionEvents.Add(pair.Value);
                            else
                                list4.Add(pair.Key);
                            break;
                        case RecordedEventType.ModifiedAction:
                            list2.Add(pair.Value);
                            break;

                        case RecordedEventType.DeletedAction:
                            list3.Add(pair.Value);
                            break;
                    }
                }

                for (int i = 0; i < list4.Count; i++)
                {
                    filteredActionList.Remove(list4[i]);
                }

            }

            GetMinMaxTime(list3, out num, out num2);


            if (num != 0x7fffffffffffffffL && num2 != -9223372036854775808L)
            {
                FixStartEndTime(actionEvents, num, num2);
                FixStartEndTime(list2, num, num2);
            }



            if (OnFilteredAction != null)
            {
                list3.ForEach(args => OnFilteredAction(null, args));
                list2.ForEach(args => OnFilteredAction(null, args));
                actionEvents.ForEach(args => OnFilteredAction(null, args));
            }
        }

        bool MergeSendKeysActivitiesRemoveStandaloneModifierKeys(ZappyTaskAction Action)
        {
            
            bool _Merged = false;

            if (Action is SendKeysAction)
            {
                SendKeysAction _NewSendKeysAction = (Action as SendKeysAction);

                if (_NewSendKeysAction.Text.Equals("{LMenu}") ||
                    _NewSendKeysAction.Text.Equals("{RMenu}") ||
                    _NewSendKeysAction.Text.Equals("{LShiftKey}") ||
                    _NewSendKeysAction.Text.Equals("{RShiftKey}") ||
                    _NewSendKeysAction.Text.Equals("{LControlKey}") ||
                    _NewSendKeysAction.Text.Equals("{RControlKey}"))
                {
                    _Merged = true;
                }
                                                                                                                            }
                        
            return _Merged;
        }
        ZappyTaskActionStack IAggregatorFilter.FilteredActionList
        {
            get =>
                filteredActionList;
            set
            {
                filteredActionList = value;
                filteredActionList.ZappyTaskActionStackModifiedEvent += OnListQueueStackChange;
            }
        }

        ZappyTaskActionStack IAggregatorFilter.RawActionList
        {
            get =>
                rawActionList;
            set
            {
                rawActionList = value;
            }
        }

        RecorderOptions IAggregatorFilter.RecorderOptions
        {
            get =>
                recorderOptions;
            set
            {
                recorderOptions = value;
            }
        }
    }
}

