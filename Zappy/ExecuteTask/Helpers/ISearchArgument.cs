using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal interface ISearchArgument
    {
        string FullQueryString { get; }

        
        bool IsExpansionRequired { get; }

        bool IsTopLevelWindow { get; }

        int MaxDepth { get; }

        ISearchArgument ParentSearchArgument { get; }

        IPlaybackContext PlaybackContext { get; }

        string QueryStringRelativeToTopLevel { get; }

        string SessionId { get; }

        string SingleQueryString { get; }

        bool SkipIntermediateElements { get; }

        string TechnologyName { get; }

        ISearchArgument TopLevelSearchArgument { get; }

        ZappyTaskControl ZappyTaskControl { get; }
    }
}