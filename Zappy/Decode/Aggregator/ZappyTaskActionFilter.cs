using Zappy.ActionMap.TaskAction;

namespace Zappy.Decode.Aggregator
{
    public abstract class ZappyTaskActionFilter
    {
        public abstract bool ProcessRule(ZappyTaskActionStack actionStack);

        public abstract bool ApplyTimeout { get; }

        public abstract ZappyTaskActionFilterCategory Category { get; }

        public abstract bool Enabled { get; }

        public abstract ZappyTaskActionFilterType FilterType { get; }

        public abstract string Group { get; }

        public abstract string Name { get; }
    }
}