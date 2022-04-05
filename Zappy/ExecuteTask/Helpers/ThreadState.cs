namespace Zappy.ExecuteTask.Helpers
{
    internal enum ThreadState
    {
        Initialized,
        Ready,
        Running,
        Standby,
        Terminated,
        Waiting,
        Transition,
        UnknownState
    }
}