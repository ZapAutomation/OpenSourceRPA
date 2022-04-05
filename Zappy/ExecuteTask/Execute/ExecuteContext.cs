using System.Drawing;
using Zappy.ActionMap.Query;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Execute
{
    internal class ExecuteContext : IPlaybackContext
    {
        private string actionName;
        private bool isSearchContext;
        private Point location;
        private IQueryCondition queryCondition;
        private string queryId;
        private ZappyTaskControl uiControl;

        public ExecuteContext()
        {
        }

        public ExecuteContext(bool isSearchContext)
        {
            IsSearchContext = isSearchContext;
        }

        public ExecuteContext(string queryId)
        {
            this.queryId = queryId;
            isSearchContext = true;
        }

        public ExecuteContext(IQueryCondition queryCondition, ZappyTaskControl uiTaskControl)
        {
            this.queryCondition = queryCondition;
            uiControl = uiTaskControl;
            isSearchContext = true;
        }

        public ExecuteContext(string actionName, ZappyTaskControl uiControl)
        {
            this.actionName = actionName;
            this.uiControl = uiControl;
        }

        public ExecuteContext(string actionName, ZappyTaskControl uiControl, Point location)
        {
            this.actionName = actionName;
            this.uiControl = uiControl;
            this.location = location;
        }


        private static string GetFriendlyName(ZappyTaskControl uiControl)
        {
            string property = string.Empty;
            if (uiControl != null && uiControl.IsBound && !uiControl.InEnsureValidContext)
            {
                try
                {
                    property = (string)uiControl.GetProperty(TaskExecutor.ZappyTaskControl.PropertyNames.FriendlyName);
                    if (string.IsNullOrEmpty(property))
                    {
                        property = uiControl.Name;
                    }
                }
                catch
                {
                }
            }
            return property;
        }


        private static string GetFriendlyTypeName(ZappyTaskControl uiControl)
        {
            string friendlyName = null;            if (!ReferenceEquals(uiControl, null))
            {
                if (!uiControl.InEnsureValidContext)
                {
                    try
                    {
                        friendlyName = uiControl.ControlType.FriendlyName;
                    }
                    catch
                    {
                    }
                }

            }
            return friendlyName;
        }


        private static ZappyTaskControl GetParent(ZappyTaskControl uiControl)
        {
            ZappyTaskControl parent = null;
            if (uiControl != null && uiControl.IsBound && !uiControl.InEnsureValidContext)
            {
                try
                {
                    parent = uiControl.GetParent();
                }
                catch
                {
                }
            }
            return parent;
        }

        public Point ActionLocation
        {
            get =>
                location;
            set
            {
                location = value;
            }
        }

        public string ActionName
        {
            get =>
                actionName;
            set
            {
                actionName = value;
            }
        }

        public IQueryCondition Condition
        {
            get =>
                queryCondition;
            set
            {
                queryCondition = value;
            }
        }

        public string FriendlyName =>
            GetFriendlyName(uiControl);

        public string FriendlyTypeName =>
            GetFriendlyTypeName(uiControl);

        public bool IsSearchContext
        {
            get =>
                isSearchContext;
            set
            {
                isSearchContext = value;
            }
        }

        public bool IsTopLevelSearch =>
            false;

        public string ParentFriendlyName =>
            GetFriendlyName(GetParent(uiControl));

        public string ParentTypeName =>
            GetFriendlyTypeName(GetParent(uiControl));

        public string QueryId
        {
            get =>
                queryId;
            set
            {
                queryId = value;
            }
        }

        public object ZappyTaskControl
        {
            get =>
                uiControl;
            set
            {
                uiControl = value as ZappyTaskControl;
            }
        }
    }
}