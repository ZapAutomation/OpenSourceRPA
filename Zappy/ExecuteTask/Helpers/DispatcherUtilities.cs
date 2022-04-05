using System;
using System.Windows.Threading;

namespace Zappy.ExecuteTask.Helpers
{
    public static class DispatcherUtilities
    {
        private static DispatcherOperationCallback exitFrameCallback = ExitFrame;

        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            if (frame != null)
            {
                frame.Continue = false;
            }
            return null;
        }

        public static void ProcessEventsOnUIThread()
        {
            DispatcherFrame arg = new DispatcherFrame();
            DispatcherOperation operation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, exitFrameCallback, arg);
            try
            {
                Dispatcher.PushFrame(arg);
            }
            catch (InvalidOperationException)
            {
            }
            if (operation.Status != DispatcherOperationStatus.Completed)
            {
                operation.Abort();
            }
        }
    }
}