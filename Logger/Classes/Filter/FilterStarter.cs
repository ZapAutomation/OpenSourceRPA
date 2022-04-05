﻿using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using ZappyLogger.Controls.LogWindow;

namespace ZappyLogger.Classes.Filter
{
    internal class FilterStarter
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly LogWindow.ColumnizerCallback callback;
        private readonly SortedDictionary<int, int> filterHitDict;
        private readonly List<Filter> filterReadyList;
        private readonly SortedDictionary<int, int> filterResultDict;

        private readonly List<Filter> filterWorkerList;
        private readonly SortedDictionary<int, int> lastFilterLinesDict;

        private ProgressCallback progressCallback;
        private int progressLineCount;
        private bool shouldStop;

        #endregion

        #region cTor

        public FilterStarter(LogWindow.ColumnizerCallback callback, int minThreads)
        {
            this.callback = callback;
            this.FilterResultLines = new List<int>();
            this.LastFilterLinesList = new List<int>();
            this.FilterHitList = new List<int>();
            this.filterReadyList = new List<Filter>();
            this.filterWorkerList = new List<Filter>();
            this.filterHitDict = new SortedDictionary<int, int>();
            this.filterResultDict = new SortedDictionary<int, int>();
            this.lastFilterLinesDict = new SortedDictionary<int, int>();
            this.ThreadCount = Environment.ProcessorCount * 4;
            this.ThreadCount = minThreads;
            int worker, completion;
            ThreadPool.GetMinThreads(out worker, out completion);
            ThreadPool.SetMinThreads(minThreads, completion);
            ThreadPool.GetMaxThreads(out worker, out completion);
        }

        #endregion

        #region Properties

        public List<int> FilterResultLines { get; set; }

        public List<int> LastFilterLinesList { get; set; }

        public List<int> FilterHitList { get; set; }

        public int ThreadCount { get; set; }

        #endregion

        #region Public methods

        public void DoFilter(FilterParams filterParams, int startLine, int maxCount, ProgressCallback progressCallback)
        {
            this.FilterResultLines.Clear();
            this.LastFilterLinesList.Clear();
            this.FilterHitList.Clear();
            this.filterHitDict.Clear();
            this.filterReadyList.Clear();
            this.filterResultDict.Clear();
            this.lastFilterLinesDict.Clear();
            this.filterReadyList.Clear();
            this.filterWorkerList.Clear();
            this.shouldStop = false;

            int interval = maxCount / this.ThreadCount;
            if (interval < 1)
            {
                interval = 1;
            }
            int workStartLine = startLine;
            List<WaitHandle> handleList = new List<WaitHandle>();
            progressLineCount = 0;
            this.progressCallback = progressCallback;
            while (workStartLine < startLine + maxCount)
            {
                if (workStartLine + interval > maxCount)
                {
                    interval = maxCount - workStartLine;
                    if (interval == 0)
                    {
                        break;
                    }
                }
                _logger.Info("FilterStarter starts worker for line {0}, lineCount {1}", workStartLine, interval);
                WorkerFx workerFx = new WorkerFx(this.DoWork);
                IAsyncResult ar = workerFx.BeginInvoke(filterParams, workStartLine, interval, ThreadProgressCallback,
                    FilterDoneCallback, workerFx);
                workStartLine += interval;
                handleList.Add(ar.AsyncWaitHandle);
            }
            WaitHandle[] handles = handleList.ToArray();
            // wait for worker threads completion
            if (handles.Length > 0)
            {
                WaitHandle.WaitAll(handles);
            }

            MergeResults();
        }


        /// <summary>
        /// Requests the FilterStarter to stop all filter threads. Call this from another thread (e.g. GUI). The function returns
        /// immediately without waiting for filter end.
        /// </summary>
        public void CancelFilter()
        {
            this.shouldStop = true;
            lock (this.filterWorkerList)
            {
                _logger.Info("Filter cancel requested. Stopping all {0} threads.", this.filterWorkerList.Count);
                foreach (Filter filter in this.filterWorkerList)
                {
                    filter.ShouldCancel = true;
                }
            }
        }

        #endregion

        #region Private Methods

        private void ThreadProgressCallback(int lineCount)
        {
            int count = Interlocked.Add(ref this.progressLineCount, lineCount);
            this.progressCallback(count);
        }

        private Filter DoWork(FilterParams filterParams, int startLine, int maxCount, ProgressCallback progressCallback)
        {
            _logger.Info("Started Filter worker [{0}] for line {1}", Thread.CurrentThread.ManagedThreadId, startLine);

            // Give every thread own copies of ColumnizerCallback and FilterParams, because the state of the objects changes while filtering
            FilterParams threadFilterParams = filterParams.CreateCopy2();
            LogWindow.ColumnizerCallback threadColumnizerCallback = this.callback.createCopy();

            Filter filter = new Filter(threadColumnizerCallback);
            lock (this.filterWorkerList)
            {
                this.filterWorkerList.Add(filter);
            }
            if (this.shouldStop)
            {
                return filter;
            }
            int realCount = filter.DoFilter(threadFilterParams, startLine, maxCount, progressCallback);
            _logger.Info("Filter worker [{0}] for line {1} has completed.", Thread.CurrentThread.ManagedThreadId, startLine);
            lock (this.filterReadyList)
            {
                this.filterReadyList.Add(filter);
            }
            return filter;
        }

        private void FilterDoneCallback(IAsyncResult ar)
        {
            //if (ar.IsCompleted)
            //{
            //  Filter filter = ((WorkerFx)ar.AsyncState).EndInvoke(ar);
            //  lock (this.filterReadyList)
            //  {
            //    this.filterReadyList.Add(filter);
            //  }
            //}
            Filter filter = ((WorkerFx)ar.AsyncState).EndInvoke(ar); // EndInvoke() has to be called mandatory.
        }

        private void MergeResults()
        {
            _logger.Info("Merging filter results.");
            foreach (Filter filter in this.filterReadyList)
            {
                foreach (int lineNum in filter.FilterHitList)
                {
                    if (!this.filterHitDict.ContainsKey(lineNum))
                    {
                        this.filterHitDict.Add(lineNum, lineNum);
                    }
                }
                foreach (int lineNum in filter.FilterResultLines)
                {
                    if (!this.filterResultDict.ContainsKey(lineNum))
                    {
                        this.filterResultDict.Add(lineNum, lineNum);
                    }
                }
                foreach (int lineNum in filter.LastFilterLinesList)
                {
                    if (!this.lastFilterLinesDict.ContainsKey(lineNum))
                    {
                        this.lastFilterLinesDict.Add(lineNum, lineNum);
                    }
                }
            }
            this.FilterHitList.AddRange(this.filterHitDict.Keys);
            this.FilterResultLines.AddRange(this.filterResultDict.Keys);
            this.LastFilterLinesList.AddRange(this.lastFilterLinesDict.Keys);
            _logger.Info("Merging done.");
        }

        #endregion

        private delegate Filter WorkerFx(FilterParams filterParams, int startLine, int maxCount,
            ProgressCallback callback);
    }
}