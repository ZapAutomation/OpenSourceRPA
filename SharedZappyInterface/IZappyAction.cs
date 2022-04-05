using System;
using Zappy.SharedInterface.Helper;
using DateTime = System.DateTime;

namespace Zappy.SharedInterface
{
        public interface IZappyAction
    {
        void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker);

        long Id { get; set; }

        string DisplayName { get; set; }

        DateTime Timestamp { get; set; }

        string ExeName { get; set; }

        int TimeOutInMilliseconds { get; set; }

        bool ContinueOnError { get; set; }

        int PauseTimeAfterAction { get; set; }

        int NumberOfRetries { get; set; }

        Guid SelfGuid { get; set; }

        Guid NextGuid { get; set; }

        Guid ErrorHandlerGuid { get; set; }

        int EditorLocationX { get; set; }

        int EditorLocationY { get; set; }

                string AuditInfo();
    }
}