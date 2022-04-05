using System.Drawing;
using Zappy.ActionMap.Query;

namespace Zappy.ExecuteTask.Helpers
{
    internal interface IPlaybackContext
    {
        Point ActionLocation { get; }

        string ActionName { get; set; }

        IQueryCondition Condition { get; }

        string FriendlyName { get; }

        string FriendlyTypeName { get; }

        bool IsSearchContext { get; }

        bool IsTopLevelSearch { get; }

        string ParentFriendlyName { get; }

        string ParentTypeName { get; }

        string QueryId { get; }

        object ZappyTaskControl { get; set; }
    }
}