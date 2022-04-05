namespace Zappy.ExecuteTask.Execute
{
    public enum ZappyTaskErrorActionResult
    {
        Default,
        StopPlaybackAndContinueManually,
        StopPlaybackAndRerecord,
        RetryAction,
        SkipAction
    }
}