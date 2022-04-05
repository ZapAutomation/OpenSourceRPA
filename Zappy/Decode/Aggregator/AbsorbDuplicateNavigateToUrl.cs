using Zappy.ActionMap.TaskAction;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbDuplicateNavigateToUrl : ActionFilter
    {
        public AbsorbDuplicateNavigateToUrl() : base("AbsorbDuplicateNavigateToUrl", ZappyTaskActionFilterType.Binary, false, "LaunchApplicationAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
                                                                                    return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
                                                            return false;
        }
    }
}

