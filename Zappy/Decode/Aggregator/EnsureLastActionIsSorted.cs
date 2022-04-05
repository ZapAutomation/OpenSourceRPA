using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.Decode.Aggregator
{
    internal class EnsureLastActionIsSorted : ActionFilter
    {
        public EnsureLastActionIsSorted() : base("EnsureLastActionIsSorted", ZappyTaskActionFilterType.Binary, false, "SystemAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction action = actions.Peek();
                ZappyTaskAction action2 = actions.Peek(1);
                return action.Id <= action2.Id;
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction element = actions.Pop();
            ZappyTaskAction action2 = actions.Peek();
            object[] args = { element.Id };
            
            element.Id = ActionIDRegister.GetUniqueId();
            actions.Push(element);
            return false;
        }
    }
}