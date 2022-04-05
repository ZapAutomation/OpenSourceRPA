using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class DoNotAbsorbEnterOnEditableCombobox : ActionFilter
    {
        public DoNotAbsorbEnterOnEditableCombobox() : base("DoNotAbsorbEnterOnEditableCombobox", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            SetValueAction action = actions.Peek(0) as SetValueAction;
            SendKeysAction action2 = actions.Peek(1) as SendKeysAction;
            return AggregatorUtilities.IsActionOnControlType(action, ControlType.ComboBox) && AggregatorUtilities.IsActionOnControlType(action2, ControlType.Edit) && AggregatorUtilities.DoesSendKeysEndWithEnter(action2) && !action2.Tags.ContainsKey(RecorderUtilities.ImeLanguageTag);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SendKeysAction action = actions.Peek(1) as SendKeysAction;
            action.AdditionalInfo = "DoNotAggregate";
            return false;
        }
    }
}

