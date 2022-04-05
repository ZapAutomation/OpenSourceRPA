using System;
using Zappy.ActionMap.Enums;
using Zappy.SharedInterface;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    [Serializable]
    public class RecordedEventArgs : EventArgs
    {
        private IZappyAction action;
        private RecordedEventType eventType;
        private bool isExplicit;

        public RecordedEventArgs(IZappyAction action, RecordedEventType eventType, bool isExplicit)
        {
            this.action = action;
            this.eventType = eventType;
            this.isExplicit = isExplicit;
        }

                
        public IZappyAction Action =>
            action;

        public bool IsExplicit =>
            isExplicit;

        internal string ModifiedProperty { get; set; }

        public RecordedEventType ZappyTaskEventType =>
            eventType;
    }
}