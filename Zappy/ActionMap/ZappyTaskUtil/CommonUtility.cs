namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal static class CommonUtility
    {
        internal static void PopulateAdditionalEnvironmentSettings(ZappyTaskEnvironment environment)
        {
                        environment.Group.Add(OSSettings.CaptureAllSettings());
        }
    }
}