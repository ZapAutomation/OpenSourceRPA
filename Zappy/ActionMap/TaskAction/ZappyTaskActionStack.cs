using System;
using System.ComponentModel;
using System.Diagnostics;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.ActionMap.TaskAction
{
    [DebuggerDisplay("Count = {Count}")]
    public class ZappyTaskActionStack : ListQueueStack<ZappyTaskAction>, IZappyActionStack
    {
                public event EventHandler<RecordedEventArgs> ZappyTaskActionStackModifiedEvent;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseEventForZappyTaskAction(sender, RecordedEventType.ModifiedAction, e.PropertyName);
        }

        public override ZappyTaskAction Pop(int nth)
        {
            ZappyTaskAction element = base.Pop(nth);
            RaiseEventForZappyTaskAction(element, RecordedEventType.DeletedAction, string.Empty);
            element.PropertyChanged -= OnPropertyChanged;
            element.CleanupPropertyChangeHandler();
            return element;
        }

        public override void Push(ZappyTaskAction action)
        {
            ZappyTaskUtilities.CheckForNull(action, "action");
            action.PropertyChanged += OnPropertyChanged;
            base.Push(action);
            RaiseEventForZappyTaskAction(action, RecordedEventType.NewAction, string.Empty);
        }

        private void RaiseEventForZappyTaskAction(object element, RecordedEventType recordedEventType, string modifiedPropertyName)
        {
            ZappyTaskAction action = element as ZappyTaskAction;
            if (ZappyTaskActionStackModifiedEvent != null && action != null)
            {
                RecordedEventArgs e = new RecordedEventArgs(action, recordedEventType, false)
                {
                    ModifiedProperty = modifiedPropertyName
                };
                ZappyTaskActionStackModifiedEvent(this, e);
            }
        }
    }
}
