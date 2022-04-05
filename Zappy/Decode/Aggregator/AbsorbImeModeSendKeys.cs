using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbImeModeSendKeys : ActionFilter
    {
        public AbsorbImeModeSendKeys() : base("AbsorbImeModeSendKeys", ZappyTaskActionFilterType.Unary, false, "MiscellaneousAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 0)
            {
                return false;
            }
            SendKeysAction action = actions.Peek() as SendKeysAction;
            if (action == null || string.IsNullOrEmpty(action.Text))
            {
                return false;
            }
            if (!StringKeys.Comparer.Equals("{KanaMode}", action.Text) && !StringKeys.Comparer.Equals("{KanjiMode}", action.Text) && !StringKeys.Comparer.Equals("{JunjaMode}", action.Text) && !StringKeys.Comparer.Equals("{FinalMode}", action.Text) && !StringKeys.Comparer.Equals("{HangulMode}", action.Text))
            {
                return StringKeys.Comparer.Equals("{HanjaMode}", action.Text);
            }
            return true;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            actions.Pop();
            return true;
        }
    }
}

