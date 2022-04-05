using System.Resources;

namespace ZappyLogger.Docking.Helpers
{
    internal static class ResourceHelper
    {
        #region Fields

        private static ResourceManager _resourceManager = null;

        #endregion

        #region Properties

        private static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = new ResourceManager("WeifenLuo.WinFormsUI.Docking.Strings",
                        typeof(ResourceHelper).Assembly);
                }
                return _resourceManager;
            }
        }

        #endregion

        #region Public methods

        public static string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }

        #endregion
    }
}