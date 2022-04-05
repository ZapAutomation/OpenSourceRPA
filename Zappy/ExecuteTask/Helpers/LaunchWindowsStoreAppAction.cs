using Zappy.ActionMap.ZappyTaskUtil;

namespace Zappy.ExecuteTask.Helpers
{
    internal class LaunchWindowsStoreAppAction
    {
        private string m_arguments;
        private string m_packageFamilyName;

        public LaunchWindowsStoreAppAction()
        {
        }

        public LaunchWindowsStoreAppAction(string packageFamilyName)
        {
            PackageFamilyName = packageFamilyName;
        }

        public LaunchWindowsStoreAppAction(string packageFamilyName, string arguments) : this(packageFamilyName)
        {
            Arguments = arguments;
        }

        public string Arguments
        {
            get =>
                m_arguments;
            set
            {
                m_arguments = value;
            }
        }

        public string PackageFamilyName
        {
            get =>
                m_packageFamilyName;
            set
            {
                ZappyTaskUtilities.CheckForNull(value, "value");
                m_packageFamilyName = value;
            }
        }
    }
}