using Zappy.Decode.Helper.Enums;

namespace Zappy.Decode.Helper
{
    public class ZappyTaskMediaEventInfo
    {
                                public ZappyTaskMediaEventInfo() { }
                                                                                        public ZappyTaskMediaEventInfo(MediaActionType actionType, object value) { }

                                                        public MediaActionType ActionType { get; set; }
                                                        public object Value { get; set; }
    }
}