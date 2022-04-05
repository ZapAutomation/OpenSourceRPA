using System.Collections.Generic;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal class PropertyProviderManager
    {
        private static readonly PropertyProviderManager instance = new PropertyProviderManager();
        private bool isInGetPropertyProviderContext;
                private static IList<ZappyTaskPropertyProvider> providerList;
        private readonly object syncLock = new object();

        private PropertyProviderManager()
        {
        }

        public ZappyTaskPropertyProvider GetCorePropertyProvider(ZappyTaskControl uiControl)
        {
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                ZappyTaskPropertyProvider provider = null;
                int num = 0;
                foreach (ZappyTaskPropertyProvider provider2 in providerList)
                {
                                        {
                        int controlSupportLevel = provider2.GetControlSupportLevel(uiControl);
                        if (controlSupportLevel > num)
                        {
                            provider = provider2;
                            num = controlSupportLevel;
                        }
                    }
                }
                return provider;
            }
        }

        public ZappyTaskPropertyProvider GetPropertyProvider(ZappyTaskControl uiControl)
        {
            ZappyTaskPropertyProvider instance;
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                if (this.isInGetPropertyProviderContext)
                {
                    
                    instance = TechnologyElementPropertyProvider.Instance;
                }
                else
                {
                    this.isInGetPropertyProviderContext = true;
                    try
                    {
                        ZappyTaskPropertyProvider provider2 = null;
                        int num = 0;
                        foreach (ZappyTaskPropertyProvider provider3 in providerList)
                        {
                            int controlSupportLevel = provider3.GetControlSupportLevel(uiControl);
                            if (controlSupportLevel > num)
                            {
                                provider2 = provider3;
                                num = controlSupportLevel;
                            }
                        }
                        uiControl.InvalidateProvider();
                        if (provider2 is TechnologyElementPropertyProvider)
                        {
                            object[] args = new object[] { uiControl.TechnologyName };
                            
                        }
                        instance = provider2;
                    }
                    finally
                    {
                        this.isInGetPropertyProviderContext = false;
                    }
                }
            }
            return instance;
        }

        public static PropertyProviderManager Instance
        {
            get
            {
                if (providerList == null)
                {
                    bool flag = false;
                    if (!Execute.ExecutionHandler.IsInitialized)
                    {
                        ZappyTaskService.Instance.Initialize();
                        flag = true;
                    }
                    providerList = ZappyTaskService.Instance.GetExtensions<ZappyTaskPropertyProvider>();
                    if (flag)
                    {
                        ZappyTaskService.Instance.Cleanup();
                    }
                }
                return instance;
            }
        }

        public IList<ZappyTaskPropertyProvider> ProviderList =>
            providerList;
    }


}