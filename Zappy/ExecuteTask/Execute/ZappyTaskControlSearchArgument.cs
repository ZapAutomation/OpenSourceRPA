using System;
using System.Text;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Execute
{
    internal class ZappyTaskControlSearchArgument : ISearchArgument
    {
        private string fullQueryString;
        private string relativeQueryString;
        private string singleQueryString;
        private ZappyTaskControl uiControl;

        public ZappyTaskControlSearchArgument(ZappyTaskControl control)
        {
            uiControl = control;
        }

        public override bool Equals(object obj)
        {
            ZappyTaskControlSearchArgument argument = obj as ZappyTaskControlSearchArgument;
            if (argument == null)
            {
                return false;
            }
            return this == argument;
        }

        public override int GetHashCode() =>
            FullQueryString.GetHashCode();

        public static bool operator ==(ZappyTaskControlSearchArgument left, ZappyTaskControlSearchArgument right)
        {
            if (left == right)
            {
                return true;
            }
            object obj2 = left;
            object obj3 = right;
            if (obj2 != null && obj3 != null)
            {
                return left.uiControl != null && left.uiControl == right.uiControl && string.Equals(left.FullQueryString, right.FullQueryString, StringComparison.Ordinal);
            }
            return obj2 == null && obj3 == null;
        }

        public static bool operator !=(ZappyTaskControlSearchArgument left, ZappyTaskControlSearchArgument right) =>
            !(left == right);

        public override string ToString() =>
            FullQueryString;

        public string FullQueryString
        {
            get
            {
                if (fullQueryString == null)
                {
                    StringBuilder builder = new StringBuilder();
                    for (ZappyTaskControl control = uiControl; control != null; control = control.Container)
                    {
                        builder.Insert(0, control.QueryId);
                    }
                    fullQueryString = builder.ToString();
                }
                return fullQueryString;
            }
        }

                
        public bool IsExpansionRequired =>
            SearchConfiguration.ConfigurationExists(uiControl.SearchConfigurations, SearchConfiguration.ExpandWhileSearching);

        public bool IsTopLevelWindow =>
            uiControl.Container == null;

        public int MaxDepth
        {
            get
            {
                int result = -1;
                if (ZappyTaskControl.SearchProperties.Contains(ZappyTaskControl.PropertyNames.MaxDepth) && int.TryParse(ZappyTaskControl.SearchProperties[ZappyTaskControl.PropertyNames.MaxDepth], out result))
                {
                    ZappyTaskControl.MaxDepth = result;
                }
                return result;
            }
        }

        public ISearchArgument ParentSearchArgument
        {
            get
            {
                ZappyTaskControlSearchArgument argument = null;
                if (uiControl.Container != null)
                {
                    argument = new ZappyTaskControlSearchArgument(uiControl.Container);
                }
                return argument;
            }
        }

        public IPlaybackContext PlaybackContext
        {
            get
            {
                if (uiControl != null && uiControl.UIObject != null)
                {
                    return new ExecuteContext(uiControl.UIObject.Condition, uiControl);
                }
                return new ExecuteContext(true);
            }
        }

        public string QueryStringRelativeToTopLevel
        {
            get
            {
                if (relativeQueryString == null)
                {
                    if (IsTopLevelWindow)
                    {
                        relativeQueryString = SingleQueryString;
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        ZappyTaskControl uiControl = this.uiControl;
                        if (uiControl.Container != null && uiControl.Container.IsBound && (uiControl.Container.SearchProperties == null || uiControl.Container.SearchProperties.Count == 0))
                        {
                            string property = (string)uiControl.Container.GetProperty(ZappyTaskControl.PropertyNames.QueryId);
                            if (!string.IsNullOrEmpty(property))
                            {
                                builder.Append(uiControl.QueryId);
                                QueryId id = new QueryId(property);
                                builder.Insert(0, id.GetQueryString(1, id.SingleQueryIds.Count - 1));
                            }
                        }
                        if (builder.Length == 0)
                        {
                            while (uiControl.Container != null)
                            {
                                builder.Insert(0, uiControl.QueryId);
                                uiControl = uiControl.Container;
                            }
                        }
                        relativeQueryString = builder.ToString();
                    }
                }
                return relativeQueryString;
            }
        }

        public string SessionId =>
            uiControl.SessionId;

        public string SingleQueryString
        {
            get
            {
                if (singleQueryString == null)
                {
                    singleQueryString = uiControl.QueryId;
                }
                return singleQueryString;
            }
        }

        public bool SkipIntermediateElements
        {
            get
            {
                string controlTypeName = null;
                if (ZappyTaskControl.SearchProperties.Contains(ZappyTaskControl.PropertyNames.ControlType))
                {
                    controlTypeName = ZappyTaskControl.SearchProperties[ZappyTaskControl.PropertyNames.ControlType];
                }
                return FrameworkUtilities.TechnologySupportsSkippingIntermediateElements(uiControl.TechnologyName, controlTypeName);
            }
        }

        public string TechnologyName =>
            uiControl.TechnologyName;

        public ISearchArgument TopLevelSearchArgument
        {
            get
            {
                ZappyTaskControl uiControl = this.uiControl;
                if (uiControl.Container == null || !uiControl.Container.IsBound)
                {
                    goto Label_0055;
                }
                try
                {
                    return new ZappyTaskControlSearchArgument(uiControl.Container.TopParent);
                }
                catch (Exception exception)
                {
                    object[] args = { exception };
                    
                    goto Label_0055;
                }
            Label_004E:
                uiControl = uiControl.Container;
            Label_0055:
                if (uiControl.Container != null)
                {
                    goto Label_004E;
                }
                return new ZappyTaskControlSearchArgument(uiControl);
            }
        }

        public ZappyTaskControl ZappyTaskControl =>
            uiControl;
    }
}