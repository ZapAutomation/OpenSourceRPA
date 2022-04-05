using System.Text.RegularExpressions;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbActivitiesOnCandidateListOnIME : ActionFilter
    {
        private static readonly Regex imeControlClassName = new Regex("mscandui[0-9]{2}.(comment|candidate)");

        public AbsorbActivitiesOnCandidateListOnIME() : base("AbsorbActivitiesOnCandidateListOnIME", ZappyTaskActionFilterType.Unary, false, "MiscellaneousAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek();
            if (action.ActivityElement == null)
            {
                return false;
            }
            TaskActivityElement element = FrameworkUtilities.TopLevelElement(action.ActivityElement);
            return element != null && !string.IsNullOrEmpty(element.ClassName) && imeControlClassName.IsMatch(element.ClassName);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            actions.Pop();
            return false;
        }
    }
}

