using System;

namespace Zappy.ActionMap.TaskAction
{
    internal class ActionLogEntry
    {
        private const string ActionIdAttribute = "ActionID";
        private const string ImagePathProperty = "ImagePath";
        private const string InteractionPointProperty = "InteractionPoint";
        private const string IsErrorActionAttribute = "ErrorAction";
        private const string IsParentWindowChangeAttribute = "ParentWindowChange";
        private const string LogEntry = "LogEntry";
        private const string PartialLogEntry = "PartialLogEntry";
        private const string RelativeTimeStampAttribute = "RelativeTimeStamp";
        private const string SnapShotTimeStamp = "SnapShotTimeStamp";
        private const string TimeStampAttribute = "TimeStamp";

                                                                                                                                                                
                                                
                                                
                
                                                                                
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        internal string GetPartialLogEntry()
        {
            if (!string.IsNullOrEmpty(PartialLogString))
            {
                return PartialLogString;
            }
            return LogString;
        }

        internal long ActionId { get; set; }

        internal string ImageFileName { get; set; }

        internal string InteractionPoint { get; set; }

        internal bool IsErrorAction { get; set; }

        internal bool IsParentWindowChange { get; set; }

        internal string LogString { get; set; }

        internal string PartialLogString { get; set; }

        internal TimeSpan RelativeTimeStamp { get; set; }

        internal string SnapShotTime { get; set; }

        internal TimeSpan TimeStamp { get; set; }
    }
}