using System.Drawing;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Execute
{
    internal class InterpreterPlaybackContext : IPlaybackContext
    {
        private ZappyTaskAction action;
        private string actionName;
        private bool isSearchContext;
        private bool isTopLevelSearch;
        private ScreenIdentifier map;
        private object uiControl;
        private string uiObjectName;

        public InterpreterPlaybackContext(ScreenIdentifier map, ZappyTaskAction action) : this(map, action, null)
        {
        }

        public InterpreterPlaybackContext(ScreenIdentifier map, string uiObjectName) : this(map, uiObjectName, false)
        {
        }

        public InterpreterPlaybackContext(ScreenIdentifier map, ZappyTaskAction action, ZappyTaskControl uiControl)
        {
            this.map = map;
            this.action = action;
            if (action != null)
            {
                actionName = action.ActionName;
            }
            isSearchContext = false;
            this.uiControl = !ReferenceEquals(uiControl, null) ? uiControl : null;
        }

        public InterpreterPlaybackContext(ScreenIdentifier map, string uiObjectName, ZappyTaskControl uiControl)
        {
            this.map = map;
            isSearchContext = false;
            this.uiControl = uiControl != null ? uiControl : null;
            this.uiObjectName = uiObjectName;
        }

        public InterpreterPlaybackContext(ScreenIdentifier map, string uiObjectName, bool isTopLevelSearch)
        {
            this.uiObjectName = uiObjectName;
            this.map = map;
            isSearchContext = true;
            this.isTopLevelSearch = isTopLevelSearch;
        }

        public Point ActionLocation
        {
            get
            {
                Point empty = Point.Empty;
                MouseAction action = this.action as MouseAction;
                if (action != null)
                {
                    empty = new Point(action.Location.X, action.Location.Y);
                }
                return empty;
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

        public IQueryCondition Condition =>
            map.GetUIObjectFromUIObjectId(TaskActivityIdentifier).Condition;

        public string FriendlyName
        {
            get
            {
                string friendlyTypeName = string.Empty;
                return UIObjectUtil.GetFriendlyNameAndTypeName(TaskActivityIdentifier, map, out friendlyTypeName);
            }
        }

        public string FriendlyTypeName
        {
            get
            {
                string friendlyTypeName = string.Empty;
                string str2 = UIObjectUtil.GetFriendlyNameAndTypeName(TaskActivityIdentifier, map, out friendlyTypeName);
                return friendlyTypeName;
            }
        }

        public bool IsSearchContext =>
            isSearchContext;

        public bool IsTopLevelSearch =>
            isTopLevelSearch;

        public string ParentFriendlyName
        {
            get
            {
                string topLevelElementFriendlyName = ScreenIdentifier.GetTopLevelElementFriendlyName(TaskActivityIdentifier);
                string friendlyTypeName = string.Empty;
                return UIObjectUtil.GetFriendlyNameAndTypeName(topLevelElementFriendlyName, map, out friendlyTypeName);
            }
        }

        public string ParentTypeName
        {
            get
            {
                string topLevelElementFriendlyName = ScreenIdentifier.GetTopLevelElementFriendlyName(TaskActivityIdentifier);
                string friendlyTypeName = string.Empty;
                string str3 = UIObjectUtil.GetFriendlyNameAndTypeName(topLevelElementFriendlyName, map, out friendlyTypeName);
                return friendlyTypeName;
            }
        }

        public string QueryId =>
            Condition.ToString();

        private string TaskActivityIdentifier
        {
            get
            {
                if (string.IsNullOrEmpty(uiObjectName) && action != null)
                {
                    uiObjectName = action.TaskActivityIdentifier;
                }
                return uiObjectName;
            }
        }

        public object ZappyTaskControl
        {
            get =>
                uiControl;
            set
            {
                object obj2 = value != null && value is ZappyTaskControl ? value as ZappyTaskControl : value;
                uiControl = value;
            }
        }
    }
}