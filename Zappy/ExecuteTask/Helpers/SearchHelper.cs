using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{
    internal class SearchHelper
    {
        private Dictionary<string, ZappyTaskControl> cachedTopLevelControls = new Dictionary<string, ZappyTaskControl>(StringComparer.Ordinal);
        private static SearchHelper searchHelper = new SearchHelper();
        private readonly object syncLock = new object();

        private SearchHelper()
        {
        }

        private ZappyTaskControl GetBoundZappyTaskControl(ISearchArgument searchArg)
        {
            ZappyTaskControl uITaskControl = searchArg.ZappyTaskControl;
            if (uITaskControl.IsBoundZappyTaskControlValid() && !uITaskControl.IsRefetchRequired)
            {
                return searchArg.ZappyTaskControl;
            }
            return null;
        }

        private ZappyTaskControl GetElement(bool useCache, ISearchArgument searchArg)
        {
            ThrowIfPlaybackCancelled();
            int searchTimeout = ExecutionHandler.Settings.SearchTimeout;
            List<string> windowTitles = new List<string>();
            windowTitles.InsertRange(0, searchArg.ZappyTaskControl.WindowTitles);
            if (searchArg.TopLevelSearchArgument != null && searchArg.TopLevelSearchArgument.ZappyTaskControl != null && searchArg.TopLevelSearchArgument.ZappyTaskControl.WindowTitles != null && searchArg.TopLevelSearchArgument.ZappyTaskControl.WindowTitles.Count > 0)
            {
                foreach (string str in searchArg.TopLevelSearchArgument.ZappyTaskControl.WindowTitles)
                {
                    if (!windowTitles.Contains(str))
                    {
                        windowTitles.Add(str);
                    }
                }
            }
            bool alwaysSearch = ALUtility.IsAlwaysSearchFlagSet(searchArg.ZappyTaskControl);
            ZappyTaskControl uiControl = null;
            if (!searchArg.SkipIntermediateElements)
            {
                try
                {
                    int num2 = searchTimeout;
                    uiControl = GetZappyTaskControlRecursive(useCache, alwaysSearch, searchArg, windowTitles, ref num2);
                    goto Label_01DC;
                }
                finally
                {
                    ExecutionHandler.Settings.SearchTimeout = searchTimeout;
                }
            }
            string queryStringRelativeToTopLevel = searchArg.QueryStringRelativeToTopLevel;
            bool isExpansionRequired = searchArg.IsExpansionRequired;
            ZappyTaskControl control2 = null;
            ZappyTaskUtilities.CheckForNull(queryStringRelativeToTopLevel, "queryId");
            object[] args = { queryStringRelativeToTopLevel };
            
            int timeLeft = searchTimeout;
            try
            {
                try
                {
                    control2 = GetTopLevelElement(useCache, false, searchArg, windowTitles, ref timeLeft);
                }
                catch (ZappyTaskException)
                {
                    if (InRetryMode)
                    {
                        control2 = GetTopLevelElement(true, true, searchArg, windowTitles, ref timeLeft);
                    }
                    if (control2 == null)
                    {
                        throw;
                    }
                }
                ZappyTaskUtilities.SwitchFromImmersiveToWindow(control2.WindowHandle);
                if (searchArg.IsTopLevelWindow)
                {
                    return control2;
                }
                ExecutionHandler.PlaybackContext = searchArg.PlaybackContext;
                if (isExpansionRequired | alwaysSearch)
                {
                    uiControl = control2.FindFirstDescendant(queryStringRelativeToTopLevel, isExpansionRequired, timeLeft);
                }
                else
                {
                    if (useCache)
                    {
                        uiControl = GetBoundZappyTaskControl(searchArg);
                    }
                    if (uiControl == null)
                    {
                        uiControl = control2.FindFirstDescendant(queryStringRelativeToTopLevel, searchArg.MaxDepth, ref timeLeft);
                        UpdateBoundZappyTaskControl(searchArg, uiControl);
                    }
                }
            }
            finally
            {
                ExecutionHandler.PlaybackContext = null;
                ExecutionHandler.Settings.SearchTimeout = searchTimeout;
            }
        Label_01DC:
            if (uiControl != null)
            {
                ALUtility.UpdateSqmData(uiControl);
            }
            return uiControl;
        }

        private ZappyTaskControl GetFromTopLevelCachesUsingSessionId(ISearchArgument searchArg)
        {
            ThrowIfPlaybackCancelled();
            if (searchArg == null || searchArg.SessionId == null)
            {
                return null;
            }
            ZappyTaskControl control = null;
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                if (cachedTopLevelControls.ContainsKey(searchArg.SessionId))
                {
                    control = cachedTopLevelControls[searchArg.SessionId];
                }
            }
            try
            {
                if (control != null && control.TechnologyElement != null && NativeMethods.IsWindow(control.TechnologyElement.WindowHandle))
                {
                    return ZappyTaskControlFactory.FromWindowHandle(control.TechnologyElement.WindowHandle);
                }
                return null;
            }
            catch (ZappyTaskException exception)
            {
                object[] args = { exception };
                CrapyLogger.log.ErrorFormat("SearchHelper:GetFromTopLevelCache: Got an Exception {0}", args);
                return null;
            }
        }

        private static int GetSearchTimeoutPerTitle(int windowTitleCount)
        {
            int num = -1;
            if (ScreenElement.SearchTimeout > 0 && !ScreenElement.TopLevelWindowSinglePassSearch)
            {
                num = ScreenElement.SearchTimeout * 15 / (windowTitleCount * 3 * 100);
            }
            return num;
        }

        private ZappyTaskControl GetTitleUpdatedTopLevelWindow(ISearchArgument topLevelSearchArg, string queryId, IList<string> windowTitles)
        {
            string queryIdString = queryId;
            QueryId topLevelQueryId = new QueryId(queryIdString);
            bool flag = false;
            SingleQueryId id2 = null;
            if (topLevelQueryId.SingleQueryIds != null && topLevelQueryId.SingleQueryIds.Count > 0)
            {
                id2 = topLevelQueryId.SingleQueryIds[0];
                int queryPropertyIndex = id2.GetQueryPropertyIndex(ZappyTaskControl.PropertyNames.Name);
                if (queryPropertyIndex != -1 && windowTitles != null)
                {
                    PropertyCondition condition = id2.SearchProperties[queryPropertyIndex];
                    if (condition.Value != null && windowTitles.Contains(condition.Value.ToString()))
                    {
                        windowTitles.Remove(condition.Value.ToString());
                    }
                    windowTitles.Insert(0, condition.Value.ToString());
                    if (windowTitles.Count > 1)
                    {
                        
                        flag = true;
                    }
                    if (windowTitles.Count == 1)
                    {
                        id2.UpdatePropertyValue(ZappyTaskControl.PropertyNames.Name, windowTitles[0]);
                        queryIdString = topLevelQueryId.ToString();
                        
                    }
                }
            }
            if (flag)
            {
                return SearchUsingMultipleTitles(topLevelSearchArg, topLevelQueryId, windowTitles);
            }
            return SearchTopLevelWindow(topLevelSearchArg, queryIdString);
        }

        private ZappyTaskControl GetTopLevelElement(bool useCache, bool useCacheOnly, ISearchArgument searchArg, IList<string> windowTitles, ref int timeLeft)
        {
            try
            {
                ExecutionSettings.SetSearchTimeOutOrThrowException(timeLeft, searchArg.FullQueryString);
            }
            catch
            {
                timeLeft = 1000;
            }

            ISearchArgument topLevelSearchArgument = searchArg.TopLevelSearchArgument;
            ZappyTaskControl boundZappyTaskControl = null;
            useCache = useCache && !ALUtility.IsAlwaysSearchFlagSet(topLevelSearchArgument.ZappyTaskControl);
            if (useCache)
            {
                boundZappyTaskControl = GetBoundZappyTaskControl(topLevelSearchArgument);
                if (boundZappyTaskControl == null)
                {
                    object[] args = { topLevelSearchArgument.QueryStringRelativeToTopLevel };
                    
                    boundZappyTaskControl = GetFromTopLevelCachesUsingSessionId(topLevelSearchArgument);
                    if (boundZappyTaskControl != null)
                    {
                        object[] objArray2 = { boundZappyTaskControl };
                        
                    }
                }
                if (useCacheOnly)
                {
                    return boundZappyTaskControl;
                }
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (boundZappyTaskControl == null)
            {
                string queryStringRelativeToTopLevel = topLevelSearchArgument.QueryStringRelativeToTopLevel;
                object[] objArray3 = { queryStringRelativeToTopLevel };
                
                ZappyTaskUtilities.CheckForNull(queryStringRelativeToTopLevel, "QueryId");
                ThrowIfPlaybackCancelled();
                ExecutionHandler.PlaybackContext = topLevelSearchArgument.PlaybackContext;
                try
                {
                    object[] objArray4 = { ExecutionHandler.Settings.UpdateTitleInWindowSearch };
                    
                    if (ExecutionHandler.Settings.UpdateTitleInWindowSearch)
                    {
                        boundZappyTaskControl = GetTitleUpdatedTopLevelWindow(topLevelSearchArgument, queryStringRelativeToTopLevel, windowTitles);
                    }
                    else
                    {
                        boundZappyTaskControl = SearchTopLevelWindow(topLevelSearchArgument, queryStringRelativeToTopLevel);
                    }
                }
                finally
                {
                    ExecutionHandler.PlaybackContext = null;
                }
                UpdateTopLevelElementCache(topLevelSearchArgument, boundZappyTaskControl);
            }
            UpdateBoundZappyTaskControl(topLevelSearchArgument, boundZappyTaskControl);
            timeLeft -= (int)stopwatch.ElapsedMilliseconds;
            return boundZappyTaskControl;
        }

        private ZappyTaskControl GetZappyTaskControlRecursive(bool useCache, bool alwaysSearch, ISearchArgument searchArg, IList<string> windowTitles, ref int timeLeft)
        {
            ZappyTaskControl uiControl = null;
            if (searchArg == null)
            {
                return uiControl;
            }
            if (useCache && !alwaysSearch)
            {
                uiControl = GetBoundZappyTaskControl(searchArg);
            }
            if (uiControl == null && searchArg.IsTopLevelWindow)
            {
                try
                {
                    uiControl = GetTopLevelElement(useCache, false, searchArg, windowTitles, ref timeLeft);
                }
                catch (ZappyTaskException)
                {
                    if (InRetryMode)
                    {
                        uiControl = GetTopLevelElement(true, true, searchArg, windowTitles, ref timeLeft);
                    }
                    if (uiControl == null)
                    {
                        throw;
                    }
                }
            }
            if (uiControl == null)
            {
                string singleQueryString = searchArg.SingleQueryString;
                ISearchArgument parentSearchArgument = searchArg.ParentSearchArgument;
                object[] args = { singleQueryString };
                
                if (parentSearchArgument == null)
                {
                    ThrowIfPlaybackCancelled();
                    ExecutionHandler.PlaybackContext = searchArg.PlaybackContext;
                    try
                    {
                        ExecutionSettings.SetSearchTimeOutOrThrowException(timeLeft, searchArg.FullQueryString);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        uiControl = ZappyTaskControl.FromQueryId(singleQueryString);
                        timeLeft -= (int)stopwatch.ElapsedMilliseconds;
                        goto Label_016C;
                    }
                    finally
                    {
                        ExecutionHandler.PlaybackContext = null;
                    }
                }
                ZappyTaskControl currentDocumentWindow = GetZappyTaskControlRecursive(useCache, alwaysSearch, parentSearchArgument, windowTitles, ref timeLeft);
                                                                                                                                                if (currentDocumentWindow != null)
                {
                    ThrowIfPlaybackCancelled();
                    ExecutionHandler.PlaybackContext = searchArg.PlaybackContext;
                    try
                    {
                        if (uiControl == null)
                        {
                            uiControl = currentDocumentWindow.FindFirstDescendant(singleQueryString, searchArg.MaxDepth, ref timeLeft);
                        }
                    }
                    finally
                    {
                        ExecutionHandler.PlaybackContext = null;
                    }
                }
            }
        Label_016C:
            UpdateBoundZappyTaskControl(searchArg, uiControl);
            return uiControl;
        }

        public void ResetCache()
        {
            PlaybackCanceled = false;
            InRetryMode = false;
        }

        public void ResetTopLevelElementCache()
        {
            object syncLock = Instance.syncLock;
            lock (syncLock)
            {
                cachedTopLevelControls = new Dictionary<string, ZappyTaskControl>(StringComparer.Ordinal);
            }
        }

        public ZappyTaskControl Search(ISearchArgument searchArg)
        {
                        {
                ThrowIfPlaybackCancelled();
                bool useCache = !InRetryMode;
                if (useCache)
                {
                    ZappyTaskControl boundZappyTaskControl = GetBoundZappyTaskControl(searchArg);
                    if (boundZappyTaskControl != null)
                    {
                        return boundZappyTaskControl;
                    }
                }
                return GetElement(useCache, searchArg);
            }
        }

        public ZappyTaskControlCollection SearchAll(ISearchArgument searchArg)
        {
                                                                                                                                                ISearchArgument parentSearchArgument = searchArg.ParentSearchArgument;
            ZappyTaskControl currentDocumentWindow = Search(parentSearchArgument);
                                                            string singleQueryString = searchArg.SingleQueryString;
            QueryId id = new QueryId(singleQueryString);
            IList<SingleQueryId> singleQueryIds = id.SingleQueryIds;
            if (singleQueryIds.Count > 1)
            {
                currentDocumentWindow = currentDocumentWindow.FindFirstDescendant(id.GetQueryString(0, singleQueryIds.Count - 2));
                singleQueryString = id.GetQueryString(singleQueryIds.Count - 1, singleQueryIds.Count - 1);
            }
            return currentDocumentWindow.FindDescendants(singleQueryString, searchArg.MaxDepth);
        }

        private static ZappyTaskControl SearchTopLevelWindow(ISearchArgument topLevelSearchArg, string topLevelElementQueryId)
        {
                                                                                                                                                                                                                                                return ZappyTaskControl.FromQueryId(topLevelElementQueryId);
        }

        private ZappyTaskControl SearchUsingMultipleTitles(ISearchArgument topLevelSearchArg, QueryId topLevelQueryId, IList<string> windowTitles)
        {
            ZappyTaskControl control2;
            int count = windowTitles.Count;
            int searchTimeoutPerTitle = GetSearchTimeoutPerTitle(count);
#if COMENABLED
            object playbackProperty = ScreenElement.GetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT);
            object obj3 = ScreenElement.GetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS);
#endif
            bool topLevelWindowSinglePassSearch = ScreenElement.TopLevelWindowSinglePassSearch;
            ScreenElement.SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, 1);
            ScreenElement.SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, SmartMatchOptions.None);
            ScreenElement.TopLevelWindowSinglePassSearch = true;
            try
            {
                int num3 = count * searchTimeoutPerTitle * 3;
                Stopwatch stopwatch = Stopwatch.StartNew();
                ZappyTaskControl control = null;
                Exception innerException = null;
                int queryPropertyIndex = topLevelQueryId.SingleQueryIds[0].GetQueryPropertyIndex(ZappyTaskControl.PropertyNames.Name);
                PropertyConditionOperator propertyOperator = topLevelQueryId.SingleQueryIds[0].SearchProperties[queryPropertyIndex].PropertyOperator;
                do
                {
                    topLevelQueryId.SingleQueryIds[0].SearchProperties[queryPropertyIndex].PropertyOperator = propertyOperator;
                    foreach (string str in windowTitles)
                    {
                        ThrowIfPlaybackCancelled();
                        topLevelQueryId.SingleQueryIds[0].UpdatePropertyValue(ZappyTaskControl.PropertyNames.Name, str);
                        string topLevelElementQueryId = topLevelQueryId.ToString();
                        try
                        {
                            control = SearchTopLevelWindow(topLevelSearchArg, topLevelElementQueryId);
                            if (control != null)
                            {
                                break;
                            }
                        }
                        catch (Exception exception2)
                        {
                            innerException = exception2;
                            object[] objArray1 = { exception2.GetType(), exception2.Message };
                            
                            if (exception2 is ExecuteCanceledException)
                            {
                                throw;
                            }
                        }
                        topLevelQueryId.SingleQueryIds[0].SearchProperties[queryPropertyIndex].PropertyOperator = PropertyConditionOperator.EqualTo;
                    }
                    ScreenElement.SearchTimeout = searchTimeoutPerTitle;
#if COMENABLED                
                    ScreenElement.SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, playbackProperty);
                    ScreenElement.SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, obj3);
#endif
                    ScreenElement.TopLevelWindowSinglePassSearch = topLevelWindowSinglePassSearch;
                }
                while (control == null && stopwatch.ElapsedMilliseconds < num3);
                if (control == null)
                {
                    throw new Exception(Resources.UnableToFindTopLevelWindow, innerException);
                }
                object[] args = { stopwatch.ElapsedMilliseconds };
                
                control2 = control;
            }
            finally
            {
#if COMENABLED
                ScreenElement.SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, playbackProperty);
                ScreenElement.SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, obj3);
#endif
                ScreenElement.TopLevelWindowSinglePassSearch = topLevelWindowSinglePassSearch;
            }
            return control2;
        }

        private void ThrowIfPlaybackCancelled()
        {
            if (PlaybackCanceled)
            {
                throw new ExecuteCanceledException();
            }
        }

        private void UpdateBoundZappyTaskControl(ISearchArgument searchArg, ZappyTaskControl uiControl)
        {
            if (uiControl != null)
            {
                searchArg.ZappyTaskControl.CopyControlInternal(uiControl);
            }
        }

        public void UpdateTopLevelElementCache(ISearchArgument searchArg, ZappyTaskControl control)
        {
            if (control != null && searchArg != null && !string.IsNullOrEmpty(searchArg.SessionId))
            {
                object syncLock = this.syncLock;
                lock (syncLock)
                {
                    if (cachedTopLevelControls.ContainsKey(searchArg.SessionId))
                    {
                        cachedTopLevelControls.Remove(searchArg.SessionId);
                    }
                    cachedTopLevelControls.Add(searchArg.SessionId, control);
                }
                ThrowIfPlaybackCancelled();
            }
        }

        public bool InRetryMode { get; set; }

        public static SearchHelper Instance =>
            searchHelper;

        public bool PlaybackCanceled { get; set; }
    }


                    
            


}