using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;

namespace Zappy.ExecuteTask.TaskExecutor
{
    [CLSCompliant(true)]
    public class ZappyTaskControl
    {
        private ScreenElement boundaryScreenElement;
        private ZappyTaskControl cachedParent;
        private string cachedQueryId;
        private ZappyTaskControl container;
        private ZappyTaskControl controlWithSearchProperties;
        private bool doNotUnbindOnNextSearchPropertiesChangedEvent;
        private PropertyExpressionCollection filterProperties;
        private int instance;
        private int maxDepth;
        private ZappyTaskPropertyProvider propertyProvider;
        private static bool restoreIfMinimized = true;
        private ScreenElement screenElement;
        private ObservableCollection<string> searchConfigurations;
        private PropertyExpressionCollection searchProperties;
        private TaskActivityElement technologyElement;
        private string technologyName;
        private TaskActivityObject uiObject;
        internal const int WaitForEnabledTimeout = 500;
        private const int WaitForReadyTimeoutForExistsProperty = 500;
        private ObservableCollection<string> windowTitles;

        public ZappyTaskControl()
        {
            this.maxDepth = -1;
            this.instance = -1;
            this.searchProperties = new PropertyExpressionCollection();
            this.searchProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SearchPropertiesCollectionChanged);
            this.filterProperties = new PropertyExpressionCollection();
            this.filterProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SearchPropertiesCollectionChanged);
            this.searchConfigurations = new ObservableCollection<string>();
            this.searchConfigurations.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SearchPropertiesCollectionChanged);
            this.windowTitles = new ObservableCollection<string>();
            this.windowTitles.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SearchPropertiesCollectionChanged);
        }

        internal ZappyTaskControl(TaskActivityObject uiObject) : this()
        {
            ZappyTaskUtilities.CheckForNull(uiObject, "uiObject");
            this.technologyName = uiObject.Framework;
            if (uiObject.SearchConfigurations != null)
            {
                foreach (string str in uiObject.SearchConfigurations)
                {
                    this.searchConfigurations.Add(str);
                }
            }
            PropertyExpressionCollection.GetProperties(uiObject.Condition, out this.searchProperties, out this.filterProperties);
            if (this.searchProperties.Find(PropertyNames.ControlType) == null)
            {
                this.searchProperties.Add(PropertyNames.ControlType, uiObject.ControlType);
            }
            if (uiObject.Ancestor != null)
            {
                                                                                                {
                    this.container = new ZappyTaskControl(uiObject.Ancestor);
                }
            }
            if (uiObject.WindowTitles != null)
            {
                foreach (string str2 in uiObject.WindowTitles)
                {
                    this.windowTitles.Add(str2);
                }
            }
        }

                        
        public ZappyTaskControl(ZappyTaskControl searchLimitContainer) : this()
        {
            this.container = searchLimitContainer;
        }

        private ZappyTaskControl(Point point) : this()
        {
            object[] args = new object[] { point };
                        try
            {
                TaskActivityElement elementFromPoint = ZappyTaskService.Instance.GetElementFromPoint(point.X, point.Y);
                this.ScreenElement = ScreenElement.FromTechnologyElement(elementFromPoint);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, false);
                throw;
            }
            this.CacheQueryId();
        }

        private ZappyTaskControl(IntPtr windowHandle) : this()
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException("windowHandle");
            }
            object[] args = new object[] { windowHandle };
                        try
            {
                this.ScreenElement = ScreenElement.FindFromWindowHandle(windowHandle);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, false);
                throw;
            }
            this.CacheQueryId();
        }

        internal ZappyTaskControl(string queryId) : this()
        {
            ZappyTaskUtilities.CheckForNull(queryId, "queryId");
            object[] args = new object[] { queryId };
                        try
            {
                this.ScreenElement = ScreenElement.FindFromPartialQueryId(queryId);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, queryId);
                throw;
            }
            this.CacheQueryId(queryId);
        }

        private ZappyTaskControl(ScreenElement element, string queryIdForRefetch) : this()
        {
            ZappyTaskUtilities.CheckForNull(element, "element");
            object[] args = new object[] { element, queryIdForRefetch };
                        this.ScreenElement = element;
            this.CacheQueryId(queryIdForRefetch);
        }

        private ZappyTaskControl(object nativeElement, string technologyName) : this()
        {
            ZappyTaskUtilities.CheckForNull(nativeElement, "nativeElement");
            try
            {
                this.TechnologyElement = ZappyTaskService.Instance.GetElementFromNativeElement(technologyName, nativeElement);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                object[] args = new object[] { nativeElement, technologyName };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidNativeElement, args));
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, false);
                throw;
            }
            this.CacheQueryId();
        }

        private ZappyTaskControl(ITaskActivityElement element, ZappyTaskControl searchContainer, string queryIdForRefetch) : this()
        {
            ZappyTaskUtilities.CheckForNull(element, "element");
                        try
            {
                this.ScreenElement = ScreenElement.FromTechnologyElement(element);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, false);
                throw;
            }
            if (searchContainer == null)
            {
                this.CacheQueryId();
            }
            else
            {
                this.container = this.cachedParent = searchContainer;
                if (string.IsNullOrEmpty(queryIdForRefetch))
                {
                    this.CacheQueryId(this.UIObject.ToString());
                }
                else
                {
                    this.CacheQueryId(queryIdForRefetch);
                }
            }
        }

        private void CacheQueryId()
        {
            this.CacheQueryId(null);
        }

        private void CacheQueryId(string queryId)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                object[] args = new object[] { queryId };
                                if (ExecutionHandler.Settings.AutoRefetchEnabled)
                {
                    if (string.IsNullOrEmpty(queryId))
                    {
                                                {
                            this.cachedQueryId = this.GetQueryIdForCaching();
                            goto Label_0073;
                        }
                    }
                    this.cachedQueryId = queryId;
                }
            Label_0073:
                return null;
            }, this, true, false);
        }

                public Image CaptureImage() =>
            TaskMethodInvoker.Instance.InvokeMethod<Image>(() => this.CaptureImagePrivate(), this, true, false);

        private Image CaptureImagePrivate()
        {
            this.FindControlIfNecessary();
            if (ZappyTaskUtilities.IsProtectedWindow())
            {
                throw new SecurityException(Resources.CaptureProtectedWindowMessage);
            }
            Rectangle boundingRectangle = this.BoundingRectangle;
            Bitmap image = new Bitmap(boundingRectangle.Width, boundingRectangle.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CopyFromScreen(boundingRectangle.X, boundingRectangle.Y, 0, 0, boundingRectangle.Size);
            }
            return image;
        }

        internal void CheckInvalidOSVersionForTouch()
        {
            OperatingSystem oSVersion = Environment.OSVersion;
            if ((oSVersion.Version.Major < 6) || ((oSVersion.Version.Major == 6) && (oSVersion.Version.Minor < 2)))
            {
                
                throw new InvalidOperationException();
            }
        }

        internal void Click(MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinates)
        {
            object[] args = new object[] { button, modifierKeys, relativeCoordinates };
                        this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        object[] objArray2 = new object[] { button, modifierKeys, relativeCoordinates };
                        
                    }
                    this.ScreenElement.MouseButtonClick(relativeCoordinates.X, relativeCoordinates.Y, button, modifierKeys);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "Click", this, relativeCoordinates.X, relativeCoordinates.Y);
                throw;
            }
        }

        internal void CopyControlInternal(ZappyTaskControl controlToCopy)
        {
            if (controlToCopy != null)
            {
                this.screenElement = controlToCopy.screenElement;
                this.technologyElement = controlToCopy.technologyElement;
                this.boundaryScreenElement = controlToCopy.boundaryScreenElement;
                this.cachedParent = controlToCopy.cachedParent;
                this.cachedQueryId = controlToCopy.cachedQueryId;
            }
        }

        public virtual void CopyFrom(ZappyTaskControl controlToCopy)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.CopyFromPrivate(controlToCopy);
                return null;
            }, this, true, false);
        }

        private void CopyFromPrivate(ZappyTaskControl controlToCopy)
        {
            this.Unbind();
            this.container = controlToCopy.Container;
            this.technologyName = controlToCopy.TechnologyName;
            this.searchProperties.Clear();
            foreach (PropertyExpression expression in controlToCopy.SearchProperties)
            {
                this.searchProperties.Add(expression);
            }
            this.filterProperties.Clear();
            foreach (PropertyExpression expression2 in controlToCopy.FilterProperties)
            {
                this.filterProperties.Add(expression2);
            }
            this.searchConfigurations.Clear();
            foreach (string str in controlToCopy.SearchConfigurations)
            {
                this.searchConfigurations.Add(str);
            }
            this.windowTitles.Clear();
            foreach (string str2 in controlToCopy.WindowTitles)
            {
                this.windowTitles.Add(str2);
            }
            this.CopyControlInternal(controlToCopy);
        }

        private ZappyTaskControl CreateChildElementWithCachedQueryId(ScreenElement element, string childPartialQid)
        {
            if (ExecutionHandler.Settings.AutoRefetchEnabled)
            {
                ZappyTaskControl control = FromScreenElement(element, childPartialQid);
                control.cachedParent = this;
                return control;
            }
            return FromScreenElement(element, null);
        }

        internal void DoubleClick(MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinates)
        {
            object[] args = new object[] { button, modifierKeys, relativeCoordinates };
                        this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        object[] objArray2 = new object[] { button, modifierKeys, relativeCoordinates };
                        
                    }
                    this.ScreenElement.DoubleClick(relativeCoordinates.X, relativeCoordinates.Y, button, modifierKeys);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "DoubleClick", this, relativeCoordinates.X, relativeCoordinates.Y);
                throw;
            }
        }

                                                        
                public void DrawHighlight()
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.DrawHighlightPrivate();
                return null;
            }, this, true, false);
        }

        private void DrawHighlightPrivate()
        {
            this.FindControlIfNecessary();
            Rectangle boundingRectangle = this.BoundingRectangle;
            ALUtility.DrawHighlight(boundingRectangle.X, boundingRectangle.Y, boundingRectangle.Width, boundingRectangle.Height, 0x1b58);
        }

        public void EnsureClickable()
        {
            this.EnsureClickable(new Point(-1, -1));
        }

        public void EnsureClickable(Point point)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.EnsureVisiblePrivate(point);
                return null;
            }, this, true, false);
        }

        internal void EnsureValid(bool waitForReady = false, bool refetch = true)
        {
            if (!this.InHtmlLoggerContext || !this.IsBound)
            {
                                if (this.InEnsureValidContext)
                {
                    CrapyLogger.log.Error("EnsureValid called via EnsureValid call for same element");
                    throw new ZappyTaskControlNotAvailableException();
                }
                if (!ExecutionHandler.IsInExceptionMappingContext)
                {
                    this.InEnsureValidContext = true;
                    bool isRefetchRequired = !this.IsBound;
                    try
                    {
                        if (waitForReady && !isRefetchRequired)
                        {
                            try
                            {
                                this.WaitForControlReadyPrivate();
                            }
                            catch (ZappyTaskException)
                            {
                                if (!(ExecutionHandler.Settings.AutoRefetchEnabled & refetch))
                                {
                                    throw;
                                }
                                isRefetchRequired = true;
                            }
                        }
                        if (ExecutionHandler.Settings.AutoRefetchEnabled)
                        {
                            if (!isRefetchRequired)
                            {
                                isRefetchRequired = this.IsRefetchRequired;
                            }
                            if (isRefetchRequired)
                            {
                                object[] args = new object[] { this.cachedQueryId };
                                
                                try
                                {
                                    TaskActivityElement technologyElement = this.technologyElement;
                                    if (this.cachedParent != null)
                                    {
                                        if (this.Instance <= 0)
                                        {
                                            this.ScreenElement = this.cachedParent.FindFirstDescendant(this.cachedQueryId).ScreenElement;
                                        }
                                        else
                                        {
                                            this.cachedParent.EnsureValid(false, true);
                                            ScreenElement[] elementArray = this.cachedParent.ScreenElement.FindAllScreenElement(this.cachedQueryId, this.MaxDepth, true, false);
                                            if (elementArray.Length < this.Instance)
                                            {
                                                throw new ZappyTaskControlNotAvailableException(ExecutionHandler.GetZappyTaskControlString(this), this);
                                            }
                                            this.ScreenElement = elementArray[this.Instance];
                                        }
                                    }
                                                                                                                                                                                    else
                                    {
                                        this.ScreenElement = ScreenElement.FindFromPartialQueryId(this.cachedQueryId);
                                    }
                                    if (technologyElement != null)
                                    {
                                        technologyElement.CopyOptionsTo(this.technologyElement);
                                    }
                                }
                                catch (COMException exception)
                                {
                                    throw new ZappyTaskControlNotAvailableException(exception, ExecutionHandler.GetZappyTaskControlString(this), this);
                                }
                                catch (ZappyTaskException exception2)
                                {
                                    throw new ZappyTaskControlNotAvailableException(exception2, ExecutionHandler.GetZappyTaskControlString(this), this);
                                }
                                if (waitForReady)
                                {
                                    this.WaitForControlReadyPrivate();
                                }
                            }
                        }
                    }
                    finally
                    {
                        this.InEnsureValidContext = false;
                    }
                }
            }
        }

        private void EnsureVisiblePrivate(Point point)
        {
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                this.ScreenElement.EnsureVisible(point.X, point.Y);
            }
            catch (Exception)
            {
                throw new Exception("BasicExceptionMessage.ZappyTaskControlNotVisibleException," + ExecutionHandler.GetZappyTaskControlString(this));
            }
        }

        public override bool Equals(object other)
        {
            ZappyTaskControl control = other as ZappyTaskControl;
            if (control == null)
            {
                return false;
            }
            return (this == control);
        }

        private void ExecuteAction(Action action, Action<Exception> remapExceptionAction, Func<string> messageOnDisabled, bool ensureVisible)
        {
            this.CheckInvalidOSVersionForTouch();
            if (ensureVisible)
            {
                this.FindControlIfNecessary();
                this.EnsureValid(false, true);
            }
            try
            {
                if (ensureVisible && !this.WaitForControlEnabledPrivate(500))
                {
                    
                }
                action();
            }
            catch (Exception exception)
            {
                remapExceptionAction(exception);
                throw;
            }
        }

        public virtual void Find()
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.FindPrivate();
                return null;
            }, this, true, false);
        }

        private void FindControlIfNecessary()
        {
            if (!this.IsBoundZappyTaskControlValid())
            {
                this.FindInternal();
            }
            if (this.technologyElement != null)
            {
                if (restoreIfMinimized)
                {
                    Utility.RestoreMinimizedWindow(this.TechnologyElement);
                }
                ALUtility.EnsureWindowForegroundForMenuItem(this.TechnologyElement);
            }
        }

        internal ZappyTaskControlCollection FindDescendants(string queryId, int maxDepth)
        {
            ZappyTaskUtilities.CheckForNull(queryId, "queryId");
            object[] args = new object[] { queryId };
                        this.EnsureValid(false, true);
            ScreenElement[] elementArray = this.ScreenElement.FindAllScreenElement(queryId, maxDepth, true, false);
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            int num = 0;
            foreach (ScreenElement element in elementArray)
            {
                ZappyTaskControl item = FromScreenElement(element, queryId);
                item.cachedParent = this;
                item.Instance = num;
                controls.Add(item);
                num++;
            }
            return controls;
        }

        internal ZappyTaskControl FindFirstDescendant(string queryId)
        {
            int searchTimeout = ExecutionHandler.Settings.SearchTimeout;
            return this.FindFirstDescendant(queryId, -1, ref searchTimeout);
        }

        internal ZappyTaskControl FindFirstDescendant(string queryId, bool expandUIElementWhileSearching, int searchTime)
        {
            ExecutionSettings.SetSearchTimeOutOrThrowException(searchTime, queryId);
            ZappyTaskUtilities.CheckForNull(queryId, "queryId");
            object[] args = new object[] { queryId };
                        this.EnsureValid(false, true);
            ScreenElement element = null;
            try
            {
                if (expandUIElementWhileSearching)
                {
                    element = ScreenElement.FindScreenElementByExpandingUI(this.ScreenElement, queryId);
                }
                else
                {
                    element = this.ScreenElement.FindScreenElement(queryId);
                }
            }
            catch (ArgumentException exception)
            {
                throw new ZappyTaskControlNotAvailableException("Invalid Search Properties", exception);
            }
            catch (Exception exception2)
            {
                ExecutionHandler.MapAndThrowException(exception2, queryId);
                throw;
            }
            return this.CreateChildElementWithCachedQueryId(element, queryId);
        }

        internal ZappyTaskControl FindFirstDescendant(string queryId, int maxDepth, ref int timeLeft)
        {
            ExecutionSettings.SetSearchTimeOutOrThrowException(timeLeft, queryId);
            ZappyTaskUtilities.CheckForNull(queryId, "queryId");
            object[] args = new object[] { queryId };
                        this.EnsureValid(false, true);
            ScreenElement element = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                element = this.ScreenElement.FindScreenElement(queryId, maxDepth);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, queryId);
                throw;
            }
            finally
            {
                timeLeft -= (int)stopwatch.ElapsedMilliseconds;
            }
            return this.CreateChildElementWithCachedQueryId(element, queryId);
        }

        private void FindInternal()
        {
            if (this.IsBoundZappyTaskControlValid())
            {
                this.EnsureValid(false, true);
            }
            else
            {
                this.ValidateSearchProperties();
                ZappyTaskControl controlToCopy = SearchHelper.Instance.Search(new ZappyTaskControlSearchArgument(this));
                this.CopyControlInternal(controlToCopy);
            }
        }

        public ZappyTaskControlCollection FindMatchingControls() =>
            TaskMethodInvoker.Instance.InvokeMethod<ZappyTaskControlCollection>(() => this.FindMatchingControlsPrivate(), this, true, false);

        private ZappyTaskControlCollection FindMatchingControlsPrivate()
        {
            string str;
            this.ValidateSearchProperties();
            if ((this.SearchConfigurations != null) && this.SearchConfigurations.Contains(SearchConfiguration.NextSibling))
            {
                throw new ArgumentException(Resources.NextSiblingNotSupportedWithFindMatchingControls);
            }
            ZappyTaskControlCollection controls = null;
            bool flag = false;
            bool flag2 = this.SearchProperties.Contains(PropertyNames.Instance) | (OrderOfInvoke.ParseQueryId(this.QueryId, out str) > 0);
            if (!this.SearchConfigurations.Contains(SearchConfiguration.DisambiguateChild))
            {
                this.SearchConfigurations.Add(SearchConfiguration.DisambiguateChild);
                flag = true;
            }
            try
            {
                if (!flag2)
                {
                    if (this.Container == null)
                    {
                        this.Container = Desktop;
                    }
                    try
                    {
                        controls = SearchHelper.Instance.SearchAll(new ZappyTaskControlSearchArgument(this));
                    }
                    catch (Exception exception)
                    {
                        object[] args = new object[] { exception.Message };
                        
                        object[] objArray2 = new object[] { exception.Message };
                                                flag2 = true;
                    }
                }
                if (flag2)
                {
                    this.FindInternal();
                    controls = new ZappyTaskControlCollection {
                        this
                    };
                }
            }
            finally
            {
                if (flag)
                {
                    this.doNotUnbindOnNextSearchPropertiesChangedEvent = true;
                    this.SearchConfigurations.Remove(SearchConfiguration.DisambiguateChild);
                }
            }
            return controls;
        }

        private void FindPrivate()
        {
            this.Unbind();
            this.FindInternal();
        }

                                                        
        internal static ZappyTaskControl FromNativeElement(object nativeElement, string technologyName)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(nativeElement, technologyName);
            return GetSpecializedControl(uiControl);
        }

        internal static ZappyTaskControl FromPoint(Point absoluteCoordinates)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(absoluteCoordinates);
            return GetSpecializedControl(uiControl);
        }

        internal static ZappyTaskControl FromQueryId(string queryId)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(queryId);
            return GetSpecializedControl(uiControl);
        }

        internal static ZappyTaskControl FromScreenElement(ScreenElement element, string queryId)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(element, queryId);
            return GetSpecializedControl(uiControl);
        }

        internal static ZappyTaskControl FromTechnologyElement(ITaskActivityElement element) =>
            FromTechnologyElement(element, null);

        internal static ZappyTaskControl FromTechnologyElement(ITaskActivityElement element, ZappyTaskControl containerElement) =>
            FromTechnologyElement(element, containerElement, null);

        internal static ZappyTaskControl FromTechnologyElement(ITaskActivityElement element, ZappyTaskControl containerElement, string queryIdForRefetch)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(element, containerElement, queryIdForRefetch);
            return GetSpecializedControl(uiControl);
        }

        internal static ZappyTaskControl FromWindowHandle(IntPtr windowHandle)
        {
            ZappyTaskControl uiControl = new ZappyTaskControl(windowHandle);
            return GetSpecializedControl(uiControl);
        }

        public virtual ZappyTaskControlCollection GetChildren() =>
            TaskMethodInvoker.Instance.InvokeMethod<ZappyTaskControlCollection>(() => this.GetChildrenPrivate(), this, true, false);

        private ZappyTaskControlCollection GetChildrenPrivate()
        {
            this.FindControlIfNecessary();
            this.EnsureValid(true, true);
            return ZappyTaskControlCollection.FromChildren(this);
        }

                
        private Point GetClickablePointPrivate()
        {
            int num;
            int num2;
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                this.ScreenElement.GetClickablePoint(out num, out num2);
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "GetClickablePoint", this);
                throw;
            }
            Rectangle boundingRectangle = this.BoundingRectangle;
            return new Point(num - boundingRectangle.X, num2 - boundingRectangle.Y);
        }

        private bool GetEnabledWithoutWFR()
        {
            bool enabled;
            ExecutionHandler.Settings.WaitForReadyEnabled = false;
            try
            {
                enabled = this.Enabled;
            }
            finally
            {
                ExecutionHandler.Settings.WaitForReadyEnabled = true;
            }
            return enabled;
        }

        public override int GetHashCode()
        {
            try
            {
                if (!this.IsBound)
                {
                    this.Find();
                }
                if (this.technologyElement != null)
                {
                    return this.technologyElement.GetHashCode();
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            catch (Exception)
            {
            }
            return base.GetHashCode();
        }

        private static ZappyTaskControl GetNavigationElement(TaskActivityElement currentElement, NavigationType navigationType)
        {
            TaskActivityElement firstChild = null;
            ZappyTaskControl control = null;
            try
            {
                switch (navigationType)
                {
                    case NavigationType.FirstChild:
                        firstChild = ZappyTaskService.Instance.GetFirstChild(currentElement);
                        goto Label_0067;

                    case NavigationType.NextSibling:
                        firstChild = ZappyTaskService.Instance.GetNextSibling(currentElement);
                        goto Label_0067;

                    case NavigationType.PreviousSibling:
                        firstChild = ZappyTaskService.Instance.GetPreviousSibling(currentElement);
                        goto Label_0067;

                    case NavigationType.Parent:
                        firstChild = ZappyTaskService.Instance.GetParent(currentElement);
                        goto Label_0067;
                }
                firstChild = currentElement;
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                CrapyLogger.log.Error("AL: UIElementNotFoundException while getting navigation element");
            }
        Label_0067:
            if (firstChild != null)
            {
                control = FromTechnologyElement(firstChild);
            }
            return control;
        }

        public ZappyTaskControl GetParent() =>
            TaskMethodInvoker.Instance.InvokeMethod<ZappyTaskControl>(() => this.GetParentPrivate(), this, true, false);

        private ZappyTaskControl GetParentPrivate()
        {
            this.FindControlIfNecessary();
            this.EnsureValid(true, true);
            return GetNavigationElement(this.TechnologyElement, NavigationType.Parent);
        }

        public object GetProperty(string propertyName) =>
            TaskMethodInvoker.Instance.InvokeMethod<object>(() => this.GetPropertyPrivate(propertyName), this, true, false);

        private T GetPropertyOfType<T>(string propertyName)
        {
            object propertyPrivate = this.GetPropertyPrivate(propertyName);
            if (propertyPrivate is T)
            {
                return (T)propertyPrivate;
            }
            return (T)Convert.ChangeType(propertyPrivate, typeof(T), CultureInfo.InvariantCulture);
        }

        private object GetPropertyPrivate(string propertyName)
        {
            object obj3;
            bool restoreIfMinimized = ZappyTaskControl.restoreIfMinimized;
            ZappyTaskControl.restoreIfMinimized = false;
            bool flag2 = string.Equals(propertyName, PropertyNames.Exists, StringComparison.OrdinalIgnoreCase);
            int waitForReadyTimeout = ExecutionHandler.Settings.WaitForReadyTimeout;
            if (flag2)
            {
                ExecutionHandler.Settings.WaitForReadyTimeout = 500;
            }
            try
            {
                if (!this.IsBound)
                {
                    
                    this.Find();
                }
                object propertyValuePrivate = this.GetPropertyValuePrivate(propertyName);
                IEnumerable<TaskActivityElement> enumerable = propertyValuePrivate as IEnumerable<TaskActivityElement>;
                if (enumerable != null)
                {
                    ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
                    foreach (TaskActivityElement element in enumerable)
                    {
                        if (element != null)
                        {
                            controls.Add(FromTechnologyElement(element));
                        }
                    }
                    return controls;
                }
                if (!string.Equals(PropertyNames.UITechnologyElement, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    TaskActivityElement element2 = propertyValuePrivate as TaskActivityElement;
                    if (element2 != null)
                    {
                        return FromTechnologyElement(element2);
                    }
                }
                obj3 = propertyValuePrivate;
            }
                                                                                                                                                                                                                                                                                                            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                                throw;
            }
            finally
            {
                ZappyTaskControl.restoreIfMinimized = restoreIfMinimized;
                if (flag2)
                {
                    ExecutionHandler.Settings.WaitForReadyTimeout = waitForReadyTimeout;
                }
            }
            return obj3;
        }

        private object GetPropertyValuePrivate(string propertyName)
        {
            ZappyTaskUtilities.CheckForNull(propertyName, "propertyName");
            object[] args = new object[] { propertyName };
                        bool flag = string.Equals(propertyName, PropertyNames.Exists, StringComparison.OrdinalIgnoreCase);
            try
            {
                if (!this.IsCachedProperty(propertyName) | flag)
                {
                    this.EnsureValid(true, !flag);
                }
                if (flag)
                {
                    return true;
                }
            }
            catch (ZappyTaskException)
            {
                if (!flag)
                {
                    throw;
                }
                return false;
            }
            object parameterValue = null;
            try
            {
                parameterValue = this.PropertyProvider.GetPropertyValueWrapper(this, propertyName);
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (NotImplementedException exception)
            {
                object[] objArray2 = new object[] { propertyName, this.ControlType };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.GetPropertyNotSupportedMessage, objArray2), exception);
            }
            catch (Exception exception2)
            {
                object[] objArray3 = new object[] { propertyName };
                string actionName = string.Format(CultureInfo.CurrentCulture, Resources.GetPropertyActionName, objArray3);
                ExecutionHandler.MapAndThrowException(exception2, actionName, parameterValue, this);
                throw;
            }
            return parameterValue;
        }

        private string GetQueryIdForCaching()
        {
            string completeQueryId = null;
            try
            {
                completeQueryId = ScreenMapUtil.GetCompleteQueryId(this.TechnologyElement);
            }
            catch (NotSupportedException)
            {
                
            }
            catch (NotImplementedException)
            {
                
            }
            return completeQueryId;
        }

                                                                                                                                                                                
        internal static ZappyTaskControl GetSpecializedControl(ZappyTaskControl uiControl)
        {
            System.Type specializedType = ALUtility.GetSpecializedType(uiControl);
            if ((specializedType != null) && (specializedType != typeof(ZappyTaskControl)))
            {
                try
                {
                    ZappyTaskControl control = (ZappyTaskControl)Activator.CreateInstance(specializedType);
                    control.CopyFrom(uiControl);
                    return control;
                }
                catch (Exception)
                {
                }
            }
            return uiControl;
        }

        private string GetTechnologyNamePrivate()
        {
            if (string.IsNullOrEmpty(this.technologyName))
            {
                if (!this.IsBound)
                {
                    if ((this.Container == null) || string.IsNullOrEmpty(this.Container.TechnologyName))
                    {
                        throw new ArgumentException(Resources.NoTechnologyNameSpecified);
                    }
                    this.technologyName = this.Container.TechnologyName;
                }
                else
                {
                    this.technologyName = this.TechnologyElement.Framework;
                }
            }
            return this.technologyName;
        }

        protected virtual ZappyTaskControl[] GetZappyTaskControlsForSearch() =>
            null;

        private void InitializeBoundaryScreenElement()
        {
            ITaskActivityElement element = ZappyTaskService.Instance.ConvertToMappedTechnologyElement(this.technologyElement);
            if ((element != null) && (element != this.technologyElement))
            {
                if (FrameworkUtilities.IsTopLevelElement(this.technologyElement))
                {
                    
                    this.technologyElement = element as TaskActivityElement;
                    try
                    {
                        this.screenElement = ScreenElement.FromTechnologyElement(element);
                    }
                    catch (COMException exception)
                    {
                        object[] args = new object[] { exception };
                        CrapyLogger.log.ErrorFormat("ZappyTaskControl : InitializeBoundaryScreenElement : {0}", args);
                    }
                }
                else
                {
                    try
                    {
                        this.boundaryScreenElement = ScreenElement.FromTechnologyElement(element);
                    }
                    catch (COMException exception2)
                    {
                        object[] objArray2 = new object[] { exception2 };
                        CrapyLogger.log.ErrorFormat("ZappyTaskControl : InitializeBoundaryScreenElement : {0}", objArray2);
                    }
                }
            }
        }

        internal void InvalidateProvider()
        {
            this.propertyProvider = null;
        }

        internal bool IsBoundZappyTaskControlValid() =>
            ((this.InHtmlLoggerContext && this.IsBound) || (this.IsBound && ((!SearchConfiguration.ConfigurationExists(this.SearchConfigurations, SearchConfiguration.ExpandWhileSearching) && !ALUtility.IsAlwaysSearchFlagSet(this)) || this.UseCachedControl)));

        private bool IsCachedProperty(string propertyName)
        {
            if (((!string.Equals(PropertyNames.ClassName, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(PropertyNames.TechnologyName, propertyName, StringComparison.OrdinalIgnoreCase)) && (!string.Equals(PropertyNames.ControlType, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(PropertyNames.WindowHandle, propertyName, StringComparison.OrdinalIgnoreCase))) && (!string.Equals(PropertyNames.IsTopParent, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(PropertyNames.TopParent, propertyName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }

        internal void MouseHover(Point relativeCoordinates, int millisecondsDuration, int speed)
        {
            object[] args = new object[] { relativeCoordinates, millisecondsDuration };
                        this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        
                    }
                    this.ScreenElement.MouseHover(relativeCoordinates.X, relativeCoordinates.Y, 1, speed, millisecondsDuration);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "Hover", this, relativeCoordinates.X, relativeCoordinates.Y);
                throw;
            }
        }

        public static bool operator ==(ZappyTaskControl left, ZappyTaskControl right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            object obj2 = left;
            object obj3 = right;
            if (!ReferenceEquals(obj2, null) && !ReferenceEquals(obj3, null))
            {
                try
                {
                    if (!left.IsBound)
                    {
                        left.Find();
                    }
                    if (!right.IsBound)
                    {
                        right.Find();
                    }
                    return Equals(left.technologyElement, right.technologyElement);
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                }
                catch (Exception)
                {
                }
            }
            return (ReferenceEquals(obj2, null) && ReferenceEquals(obj3, null));
        }

        public static bool operator !=(ZappyTaskControl left, ZappyTaskControl right) =>
            !ReferenceEquals(left, right);

                                                        
                                                        
        internal void PressModifierKeys(ModifierKeys keys)
        {
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        
                    }
                    if (this.ControlType == ControlType.TreeItem)
                    {
                        this.ScreenElement.Select();
                    }
                    this.ScreenElement.PressModifierKeys(keys);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "PressModifierKeys", this);
                throw;
            }
        }

        internal void ReleaseModifierKeys(ModifierKeys keys)
        {
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        
                    }
                    if (this.ControlType == ControlType.TreeItem)
                    {
                        this.ScreenElement.Select();
                    }
                    this.ScreenElement.ReleaseModifierKeys(keys);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "ReleaseModifierKeys", this);
                throw;
            }
        }

        internal void ScrollMouseWheel(int wheelMoveCount, ModifierKeys modifierKeys)
        {
            object[] args = new object[] { wheelMoveCount, modifierKeys };
                        this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    this.ScreenElement.MouseWheel(wheelMoveCount, modifierKeys);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "Wheel", this, Point.Empty.X, Point.Empty.Y);
                throw;
            }
        }

        private void SearchPropertiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Unbind();
        }

        internal void SendKeys(string text, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
            ZappyTaskUtilities.CheckForNull(text, "text");
            if (isEncoded)
            {
                object[] args = new object[] { modifierKeys };
                            }
            else
            {
                object[] objArray2 = new object[] { text, modifierKeys };
                            }
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        
                    }
                    if (this.ControlType == ControlType.TreeItem)
                    {
                        this.ScreenElement.Select();
                    }
                    this.ScreenElement.SendKeys(text, modifierKeys, isEncoded, isUnicode);
                }
            }
            catch (Exception exception)
            {
                if (!isEncoded)
                {
                    string parameterValue = Utility.ConvertModiferKeysToString(modifierKeys, text);
                    ExecutionHandler.MapAndThrowException(exception, "SendKeys", parameterValue, this);
                }
                else
                {
                    ExecutionHandler.MapAndThrowException(exception, "SendKeys", this);
                }
                throw;
            }
        }

        public virtual void SetFocus()
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.SetFocusPrivate();
                return null;
            }, this, true, false);
        }

        private void SetFocusPrivate()
        {
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                this.ScreenElement.SetFocus();
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "SetFocus", this);
                throw;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.SetPropertyPrivate(propertyName, value);
                return null;
            }, this, true, true);
        }

        private void SetPropertyPrivate(string propertyName, object value)
        {
            bool restoreIfMinimized = ZappyTaskControl.restoreIfMinimized;
            ZappyTaskControl.restoreIfMinimized = false;
            this.FindControlIfNecessary();
                        if (ControlType.Window != this.ControlType)
            {
                ZappyTaskControl.restoreIfMinimized = restoreIfMinimized;
            }
            this.UseCachedControl = true;
            try
            {
                this.SetPropertyValue(propertyName, value);
            }
            catch (Exception)
            {
                ExecutionHandler.CaptureScreenShot(this);
                throw;
            }
            finally
            {
                this.UseCachedControl = false;
                ZappyTaskControl.restoreIfMinimized = restoreIfMinimized;
            }
        }

        private void SetPropertyValue(string propertyName, object propertyValue)
        {
            ZappyTaskUtilities.CheckForNull(propertyName, "propertyName");
            object[] args = new object[] { propertyName };
                        this.EnsureValid(true, true);
            try
            {
                this.PropertyProvider.SetPropertyValueWrapper(this, propertyName, propertyValue);
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (NotImplementedException exception)
            {
                object[] objArray2 = new object[] { propertyName, this.ControlType };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyNotSupportedMessage, objArray2), exception);
            }
            catch (Exception exception2)
            {
                object[] objArray3 = new object[] { propertyName };
                string actionName = string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyActionName, objArray3);
                ExecutionHandler.MapAndThrowException(exception2, actionName, propertyValue, this);
                throw;
            }
        }

                                                        
        internal void StartDragging(Point relativeCoordinate, MouseButtons button, ModifierKeys modifierKeys)
        {
            object[] args = new object[] { relativeCoordinate, button, modifierKeys };
                        this.FindControlIfNecessary();
            this.EnsureValid(false, true);
            try
            {
                                {
                    if (!this.WaitForControlEnabledPrivate(500))
                    {
                        
                    }
                    this.ScreenElement.StartDragging(relativeCoordinate.X, relativeCoordinate.Y, button, modifierKeys, true);
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "Drag", this);
                throw;
            }
        }

        private void StopDragging(Point relativeCoordinate)
        {
            object[] args = new object[] { relativeCoordinate };
                        try
            {
                                {
                    Point point;
                    this.ScreenElement.EnsureVisible(relativeCoordinate.X, relativeCoordinate.Y, 6);
                    bool flag = this.TryGetClickablePoint(out point);
                    int fEnable = 0;
                    if (!flag)
                    {
                        fEnable = ScreenElement.Playback.EnableEnsureVisibleForPrimitive(0);
                    }
                    try
                    {
                        this.ScreenElement.StopDragging(relativeCoordinate.X, relativeCoordinate.Y, Mouse.MouseDragSpeed);
                    }
                    finally
                    {
                        if (!flag)
                        {
                            ScreenElement.Playback.EnableEnsureVisibleForPrimitive(fEnable);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "Drag", this);
                throw;
            }
        }

        internal void StopDragging(int offsetX, int offsetY, bool isDisplacement)
        {
            this.FindControlIfNecessary();
            this.EnsureValid(false, true);
                        {
                if (!isDisplacement)
                {
                    this.StopDragging(new Point(offsetX, offsetY));
                }
                else
                {
                    object[] args = new object[] { offsetX, offsetY };
                                        Rectangle boundingRectangle = this.BoundingRectangle;
                    int num = (offsetX + Mouse.Location.X) - boundingRectangle.X;
                    int num2 = (offsetY + Mouse.Location.Y) - boundingRectangle.Y;
                    try
                    {
                        int num3;
                        int num4;
                        int num5;
                        int num6;
                        Point point2;
                        this.ScreenElement.EnsureVisible(num, num2, 4);
                        boundingRectangle = this.BoundingRectangle;
                        ZappyTaskControl control = null;
                        try
                        {
                            control = new ZappyTaskControl(new Point(num + boundingRectangle.X, num2 + boundingRectangle.Y));
                        }
                        catch (ZappyTaskControlNotAvailableException exception)
                        {
                            object[] objArray2 = new object[] { exception.Message };
                            CrapyLogger.log.ErrorFormat("ZappyTaskControl : StopDragging : {0}", objArray2);
                            throw new Exception("this.TechnologyElement.FriendlyName, Drag, exception, null, this.ToString(), null");
                        }
                        ScreenElement screenElement = control.ScreenElement;
                        screenElement.GetBoundingRectangle(out num3, out num4, out num5, out num6);
                        num = (boundingRectangle.X + num) - num3;
                        num2 = (boundingRectangle.Y + num2) - num4;
                        bool flag = control.TryGetClickablePoint(out point2);
                        int fEnable = 0;
                        if (!flag)
                        {
                            fEnable = ScreenElement.Playback.EnableEnsureVisibleForPrimitive(0);
                        }
                        try
                        {
                            screenElement.StopDragging(num, num2, Mouse.MouseDragSpeed);
                        }
                        finally
                        {
                            if (!flag)
                            {
                                ScreenElement.Playback.EnableEnsureVisibleForPrimitive(fEnable);
                            }
                        }
                    }
                    catch (Exception exception2)
                    {
                        ExecutionHandler.MapAndThrowException(exception2, "Drag", this);
                        throw;
                    }
                }
            }
        }

                                                        
                                                        
                                                        
        public override string ToString()
        {
            try
            {
                if (this.technologyElement != null)
                {
                    return this.technologyElement.ToString();
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            return base.ToString();
        }

        public bool TryFind()
        {
            try
            {
                this.FindPrivate();
                return true;
            }
            catch (ZappyTaskException)
            {
                return false;
            }
        }

        public bool TryGetClickablePoint(out Point point)
        {
            point = Point.Empty;
            try
            {
                point = this.GetClickablePointPrivate();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

                                                        
        internal void Unbind()
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                this.uiObject = null;
                this.controlWithSearchProperties = null;
                if (this.doNotUnbindOnNextSearchPropertiesChangedEvent)
                {
                    this.doNotUnbindOnNextSearchPropertiesChangedEvent = false;
                }
                else
                {
                    this.screenElement = null;
                    this.technologyElement = null;
                    this.boundaryScreenElement = null;
                    this.propertyProvider = null;
                }
                return null;
            }, this, true, false);
        }

        private void ValidateSearchProperties()
        {
            if ((this.SearchProperties.Count == 0) && (this.FilterProperties.Count == 0))
            {
                throw new ArgumentException(Resources.NoSearchPropertiesSpecified, "SearchProperties");
            }
            UITechnologyManager techManager = ZappyTaskService.Instance.TechnologyManagerByName(this.TechnologyName);
            if (techManager == null)
            {
                object[] args = new object[] { this.TechnologyName };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidTechnologyNameSpecified, args), "TechnologyName");
            }
            if (((this.FilterProperties.Count > 0) && !UITechnologyManager.GetTechnologyManagerProperty<bool>(techManager, UITechnologyManagerProperty.FilterPropertiesForSearchSupported)) && ((this.FilterProperties.Count > 1) || ALUtility.IsOrderOfInvokeFilterPropertyNotPresent(this)))
            {
                object[] objArray2 = new object[] { this.TechnologyName };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.NoSupportForFilterProperties, objArray2), "FilterProperties");
            }
            if ((this.container != null) && !this.container.IsBoundZappyTaskControlValid())
            {
                this.container.ValidateSearchProperties();
            }
        }

                
        public static bool WaitForCondition<T>(T conditionContext, Predicate<T> conditionEvaluator, int millisecondsTimeout)
        {
            if (conditionContext == null)
            {
                throw new ArgumentNullException("conditionContext");
            }
            if (conditionEvaluator == null)
            {
                throw new ArgumentNullException("conditionEvaluator");
            }
            object[] args = new object[] { millisecondsTimeout };
                        if (millisecondsTimeout == -1)
            {
                millisecondsTimeout = 0x7fffffff;
            }
            ExecutionSettings.CheckForMinimumPermissibleValue(0, millisecondsTimeout, "millisecondsTimeout");
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                if (conditionEvaluator(conditionContext))
                {
                    return true;
                }
                ZappyTaskUtilities.Sleep(Math.Min(Math.Max(millisecondsTimeout - ((int)stopwatch.ElapsedMilliseconds), 0), 100));
            }
            while (stopwatch.ElapsedMilliseconds < millisecondsTimeout);
            object[] objArray2 = new object[] { stopwatch.ElapsedMilliseconds };
                        return false;
        }

                
        public bool WaitForControlCondition(Predicate<ZappyTaskControl> conditionEvaluator, int millisecondsTimeout) =>
            WaitForCondition<ZappyTaskControl>(this, conditionEvaluator, millisecondsTimeout);

                
        public bool WaitForControlEnabled(int millisecondsTimeout) =>
            TaskMethodInvoker.Instance.InvokeMethod<bool>(() => this.WaitForControlEnabledPrivate(millisecondsTimeout), this, true, false);

        private bool WaitForControlEnabledPrivate(int millisecondsTimeout)
        {
            if (!ExecutionHandler.Settings.WaitForReadyEnabled || ExecutionHandler.IsInExceptionMappingContext)
            {
                return true;
            }
            object[] args = new object[] { millisecondsTimeout };
                        if (millisecondsTimeout == -1)
            {
                millisecondsTimeout = 0x7fffffff;
            }
            ExecutionSettings.CheckForMinimumPermissibleValue(0, millisecondsTimeout, "millisecondsTimeout");
            bool enabledWithoutWFR = this.GetEnabledWithoutWFR();
            try
            {
                if (enabledWithoutWFR)
                {
                    return enabledWithoutWFR;
                }
#if COMENABLED
                bool playbackProperty = (bool)ScreenElement.GetPlaybackProperty(ExecuteParameter.ENSURE_ENABLED);
#endif
                ScreenElement.SetPlaybackProperty(ExecuteParameter.ENSURE_ENABLED, true);
                try
                {
                    try
                    {
                        if (this.WaitForControlReadyPrivate(millisecondsTimeout, true))
                        {
                            enabledWithoutWFR = this.GetEnabledWithoutWFR();
                        }
                    }
                    catch                     {
                        enabledWithoutWFR = false;
                    }
                    return enabledWithoutWFR;
                }
                finally
                {
#if COMENABLED
                    ScreenElement.SetPlaybackProperty(ExecuteParameter.ENSURE_ENABLED, playbackProperty);
#endif
                }
            }
            catch (Exception exception)
            {
                ExecutionHandler.MapAndThrowException(exception, "WaitForControlEnabled", this);
                throw;
            }
            return enabledWithoutWFR;
        }

                
                
                
                
                
                                                                                        
                
                                                                                        
                
                
        private bool WaitForControlReadyPrivate() =>
            TaskMethodInvoker.Instance.InvokeMethod<bool>(() => this.WaitForControlReadyPrivate(ExecutionHandler.Settings.WaitForReadyTimeout, false), this, true, false);

        private bool WaitForControlReadyPrivate(int millisecondsTimeout, bool doLogging = true)
        {
            if (ExecutionHandler.Settings.WaitForReadyEnabled && !ExecutionHandler.IsInExceptionMappingContext)
            {
                object[] args = new object[] { millisecondsTimeout, doLogging };
                                if (millisecondsTimeout == -1)
                {
                    millisecondsTimeout = 0x7fffffff;
                }
                ExecutionSettings.CheckForMinimumPermissibleValue(0, millisecondsTimeout, "millisecondsTimeout");
                bool restoreIfMinimized = ZappyTaskControl.restoreIfMinimized;
                ZappyTaskControl.restoreIfMinimized = false;
                try
                {
                    this.FindControlIfNecessary();
                }
                finally
                {
                    ZappyTaskControl.restoreIfMinimized = restoreIfMinimized;
                }
                try
                {
                    TimeSpan span;
                    DateTime utcNow = WallClock.UtcNow;
                    if (this.boundaryScreenElement != null)
                    {
                        this.boundaryScreenElement.WaitForReady(millisecondsTimeout);
                        span = (TimeSpan)(WallClock.Now - utcNow);
                        millisecondsTimeout = Math.Max(1, millisecondsTimeout - ((int)span.TotalMilliseconds));
                    }
                    this.ScreenElement.WaitForReady(millisecondsTimeout);
                    if (doLogging)
                    {
                        object[] objArray2 = new object[1];
                        span = (TimeSpan)(WallClock.UtcNow - utcNow);
                        objArray2[0] = span.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                                            }
                }
                catch (Exception exception)
                {
                    switch (((uint)Marshal.GetHRForException(exception)))
                    {
                        case 0x80070015:
                        case 0x800704c7:
                        case 0x80131505:
                            return false;
                    }
                    ExecutionHandler.MapAndThrowException(exception, "WaitForControlReady", this);
                    throw;
                }
            }
            return true;
        }

        public virtual Rectangle BoundingRectangle =>
            this.GetPropertyOfType<Rectangle>(PropertyNames.BoundingRectangle);

        internal bool CanTrustState
        {
            get
            {
                bool flag = false;
                object technologyElementOption = TaskActivityElement.GetTechnologyElementOption(this.TechnologyElement, UITechnologyElementOption.TrustGetState);
                if (technologyElementOption is bool)
                {
                    flag = (bool)technologyElementOption;
                }
                return flag;
            }
        }

                
        internal string ClassNameValue =>
            this.GetPropertyOfType<string>(PropertyNames.ClassName);

        public virtual ZappyTaskControl Container
        {
            get =>
                this.container;
            set
            {
                this.container = value;
                this.Unbind();
            }
        }

        public virtual ControlType ControlType
        {
            get
            {
                if ((!this.IsBound && (this.SearchProperties != null)) && (this.SearchProperties.Count > 0))
                {
                    PropertyExpression expression = this.SearchProperties.Find(PropertyNames.ControlType);
                    if (expression != null)
                    {
                        return ControlType.GetControlType(expression.PropertyValue);
                    }
                }
                return this.GetPropertyOfType<ControlType>(PropertyNames.ControlType);
            }
        }

        internal ZappyTaskControl ControlWithSearchProperties
        {
            get
            {
                if (this.controlWithSearchProperties == null)
                {
                    if (this.SearchPropertiesSetExplicitly)
                    {
                        this.controlWithSearchProperties = this;
                    }
                    else if (this.IsBound && !string.IsNullOrEmpty(this.cachedQueryId))
                    {
                        string technologyName = this.TechnologyName;
                        ZappyTaskControl cachedParent = this.cachedParent;
                        ZappyTaskControl control2 = null;
                        QueryId id = new QueryId(this.cachedQueryId);
                        foreach (SingleQueryId id2 in id.SingleQueryIds)
                        {
                            control2 = new ZappyTaskControl(cachedParent);
                            cachedParent = control2;
                            foreach (PropertyCondition condition in id2.SearchProperties)
                            {
                                control2.SearchProperties.Add(new PropertyExpression(condition));
                            }
                            foreach (PropertyCondition condition2 in id2.FilterProperties)
                            {
                                control2.FilterProperties.Add(new PropertyExpression(condition2));
                            }
                            ICollection<string> technologyManagerNames = ZappyTaskService.Instance.PluginManager.TechnologyManagerNames;
                            foreach (string str2 in id2.TechnologyAttributes)
                            {
                                if (technologyManagerNames.Contains(str2))
                                {
                                    control2.TechnologyName = str2;
                                }
                                else
                                {
                                    control2.SearchConfigurations.Add(str2);
                                }
                            }
                        }
                        this.controlWithSearchProperties = control2;
                        this.controlWithSearchProperties.TechnologyName = technologyName;
                        this.controlWithSearchProperties.CopyControlInternal(this);
                    }
                    else
                    {
                        this.controlWithSearchProperties = new ZappyTaskControl(this.UIObject);
                        this.controlWithSearchProperties.CopyControlInternal(this);
                    }
                }
                return this.controlWithSearchProperties;
            }
        }

        public static ZappyTaskControl Desktop
        {
            get
            {
                ZappyTaskControl control;
                try
                {
                    control = new ZappyTaskControl(ScreenElement.Desktop, null);
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, false);
                    throw;
                }
                return control;
            }
        }

        public virtual bool Enabled =>
            this.GetPropertyOfType<bool>(PropertyNames.Enabled);

        public virtual bool Exists =>
            this.GetPropertyOfType<bool>(PropertyNames.Exists);

        public PropertyExpressionCollection FilterProperties =>
            this.filterProperties;

        internal ZappyTaskControl FirstChild
        {
            get
            {
                this.FindControlIfNecessary();
                this.EnsureValid(true, true);
                return GetNavigationElement(this.TechnologyElement, NavigationType.FirstChild);
            }
        }

                
                
                
        internal bool InEnsureValidContext { get; private set; }

        internal bool InHtmlLoggerContext { get; set; }

        internal int Instance
        {
            get =>
                this.instance;
            set
            {
                this.instance = value;
            }
        }

        internal bool IsBound =>
            (this.screenElement != null);

        internal bool IsRefetchRequired
        {
            get
            {
                IntPtr windowHandle = this.TechnologyElement.WindowHandle;
                bool flag = !NativeMethods.IsWindow(this.TechnologyElement.WindowHandle);
                if ((!flag && this.CanTrustState) && TaskActivityElement.IsState(this.TechnologyElement, AccessibleStates.Unavailable))
                {
                    flag = true;
                }
                return flag;
            }
        }

                
                
        internal int MaxDepth
        {
            get =>
                this.maxDepth;
            set
            {
                this.maxDepth = value;
            }
        }

        public virtual string Name =>
            this.GetPropertyOfType<string>(PropertyNames.Name);

        public virtual object NativeElement =>
            this.GetProperty(PropertyNames.NativeElement);

        internal ZappyTaskControl NextSibling
        {
            get
            {
                this.FindControlIfNecessary();
                this.EnsureValid(true, true);
                return GetNavigationElement(this.TechnologyElement, NavigationType.NextSibling);
            }
        }

                                                                        
        private ZappyTaskPropertyProvider PropertyProvider
        {
            get
            {
                if (this.propertyProvider == null)
                {
                    this.propertyProvider = PropertyProviderManager.Instance.GetPropertyProvider(this.ControlWithSearchProperties);
                }
                return this.propertyProvider;
            }
        }

        internal virtual string QueryId
        {
            get
            {
                ZappyTaskControl[] ZappyTaskControlsForSearch = this.GetZappyTaskControlsForSearch();
                if ((ZappyTaskControlsForSearch == null) || (ZappyTaskControlsForSearch.Length == 0))
                {
                    return this.UIObject.ToString();
                }
                StringBuilder builder = new StringBuilder();
                foreach (ZappyTaskControl control in ZappyTaskControlsForSearch)
                {
                    builder.Append(control.UIObject.ToString());
                }
                return builder.ToString();
            }
        }

        internal bool RestoreIfMinimized
        {
            get =>
                restoreIfMinimized;
            set
            {
                restoreIfMinimized = value;
            }
        }

        internal ScreenElement ScreenElement
        {
            get
            {
                if (this.screenElement == null)
                {
                    throw new ZappyTaskException();
                }
                this.TechnologyElement.CopyOptionsTo(this.screenElement.TechnologyElement);
                return this.screenElement;
            }
            private set
            {
                this.screenElement = value;
                this.technologyElement = ZappyTaskService.Instance.ConvertTechnologyElement(this.screenElement.TechnologyElement);
                this.InitializeBoundaryScreenElement();
            }
        }

        public ICollection<string> SearchConfigurations =>
            this.searchConfigurations;

        public PropertyExpressionCollection SearchProperties =>
            this.searchProperties;

        protected virtual bool SearchPropertiesSetExplicitly
        {
            get
            {
                if (this.searchProperties.Count <= 0)
                {
                    return (this.filterProperties.Count > 0);
                }
                return true;
            }
        }

        internal string SessionId { get; set; }

                
        internal ControlStates StateValue
        {
            get =>
                this.GetPropertyOfType<ControlStates>(PropertyNames.State);
            set
            {
                this.SetProperty(PropertyNames.State, value);
            }
        }

        internal TaskActivityElement TechnologyElement
        {
            get
            {
                if (this.technologyElement == null)
                {
                    
                }
                return this.technologyElement;
            }
            private set
            {
                this.technologyElement = value;
                this.screenElement = ScreenElement.FromTechnologyElement(this.technologyElement);
                this.InitializeBoundaryScreenElement();
            }
        }

        public string TechnologyName
        {
            get =>
                this.GetTechnologyNamePrivate();
            set
            {
                this.technologyName = value;
                this.Unbind();
            }
        }

                
        public virtual ZappyTaskControl TopParent =>
            this.GetPropertyOfType<ZappyTaskControl>(PropertyNames.TopParent);

        internal TaskActivityObject UIObject
        {
            get
            {
                if (this.uiObject == null)
                {
                    if (this.SearchPropertiesSetExplicitly)
                    {
                        if (this.Container == null)
                        {
                            this.uiObject = new TopLevelElement();
                        }
                        else
                        {
                            this.uiObject = new TaskActivityObject();
                        }
                        AndConditionBuilder builder = new AndConditionBuilder();
                        foreach (PropertyCondition condition in this.searchProperties.ToPropertyConditionArray())
                        {
                            builder.Append(condition);
                        }
                        if (this.filterProperties.Count > 0)
                        {
                            FilterCondition condition2 = new FilterCondition(this.filterProperties.ToPropertyConditionArray());
                            builder.Append(condition2);
                        }
                        this.uiObject.Condition = builder.Build();
                        this.uiObject.Framework = this.TechnologyName;
                        if ((this.SearchConfigurations != null) && (this.SearchConfigurations.Count > 0))
                        {
                            string[] array = new string[this.SearchConfigurations.Count];
                            this.SearchConfigurations.CopyTo(array, 0);
                            this.uiObject.SearchConfigurations = array;
                        }
                    }
                    else if (this.IsBound && !string.IsNullOrEmpty(this.cachedQueryId))
                    {
                        this.uiObject = this.ControlWithSearchProperties.UIObject;
                    }
                    else if (this.TechnologyElement != null)
                    {
                        this.uiObject = ScreenMapUtil.FromUIElement(this.TechnologyElement);
                    }
                    else
                    {
                        this.ValidateSearchProperties();
                    }
                }
                return this.uiObject;
            }
        }

        internal bool UseCachedControl { get; set; }

                
        public virtual IntPtr WindowHandle =>
            this.GetPropertyOfType<IntPtr>(PropertyNames.WindowHandle);

        public Collection<string> WindowTitles =>
            this.windowTitles;

        private enum NavigationType
        {
            None,
            FirstChild,
            NextSibling,
            PreviousSibling,
            Parent
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames
        {
            public static readonly string BoundingRectangle = "BoundingRectangle";
            public static readonly string ClassName = "ClassName";
            public static readonly string ControlType = "ControlType";
            public static readonly string Enabled = "Enabled";
            public static readonly string Exists = "Exists";
            public static readonly string FriendlyName = "FriendlyName";
            public static readonly string HasFocus = "HasFocus";
            public static readonly string Height = "Height";
            public static readonly string Instance = "Instance";
            public static readonly string IsTopParent = "IsTopParent";
            public static readonly string Left = "Left";
            public static readonly string MaxDepth = "MaxDepth";
            public static readonly string Name = "Name";
            public static readonly string NativeElement = "NativeElement";
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public static readonly string QueryId = "QueryId";
            public static readonly string State = "State";
            public static readonly string TechnologyName = "Framework";
            public static readonly string Top = "Top";
            public static readonly string TopParent = "TopParent";
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public static readonly string UITechnologyElement = "TaskActivityElement";
            internal static readonly string Value = "Value";
            public static readonly string Width = "Width";
            public static readonly string WindowHandle = "WindowHandle";

            protected PropertyNames()
            {
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object obj) =>
                base.Equals(obj);

                                    
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetHashCode() =>
                base.GetHashCode();

                                    
                                    
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override string ToString() =>
                base.ToString();
        }
    }


}