using System;
using System.Diagnostics;
using Zappy.ActionMap.TaskAction;
using Zappy.Properties;

namespace Zappy.Decode.Aggregator
{
    [Serializable, DebuggerDisplay("Name = {Name}")]
    internal abstract class ActionFilter : ZappyTaskActionFilter
    {
        protected bool applyTimeout;
        private ZappyTaskActionFilterCategory category;
        protected bool enabled = true;
        protected string group;
        protected string name;
        protected ZappyTaskActionFilterType type;

        internal ActionFilter(string filterName, ZappyTaskActionFilterType filterType, bool applyAggregatorTimeout, string groupName)
        {
            if (string.IsNullOrEmpty(filterName))
            {
                throw new Exception(Resources.FilterShouldHaveName);
            }
            name = filterName;
            type = filterType;
            applyTimeout = applyAggregatorTimeout;
            group = groupName;
        }

        protected abstract bool IsMatch(ZappyTaskActionStack actions);
        protected abstract bool ProcessOutputQuery(ZappyTaskActionStack actions);
        public override bool ProcessRule(ZappyTaskActionStack actions)
        {
            bool flag = false;
            if (IsMatch(actions))
            {
                object[] args = { Name };
                
                flag = ProcessOutputQuery(actions);
            }
            return flag;
        }

        public override bool ApplyTimeout =>
            applyTimeout;

        public override ZappyTaskActionFilterCategory Category =>
            CategoryInternal;

        internal ZappyTaskActionFilterCategory CategoryInternal
        {
            get =>
                category;
            set
            {
                category = value;
            }
        }

        public override bool Enabled =>
            EnabledInternal;

        internal bool EnabledInternal
        {
            get =>
                enabled;
            set
            {
                enabled = value;
            }
        }

        public override ZappyTaskActionFilterType FilterType =>
            type;

        public override string Group =>
            group;

        public override string Name =>
            name;
    }
}

