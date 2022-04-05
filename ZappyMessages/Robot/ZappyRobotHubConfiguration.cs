using System.Collections.Generic;
using System.IO;
using ZappyMessages.Helpers;

namespace ZappyMessages.Robot
{
    public class ZappyRobotHubConfiguration
    {
        public string HubId { get; set; }

        public string HubUri { get; set; }


        public static Dictionary<string, ZappyRobotHubConfiguration> GetConfiguredHubs()
        {
            Dictionary<string, ZappyRobotHubConfiguration> _HubConfigs = null;

            if (File.Exists(ZappyMessagingConstants.ZappyHubConfigurationFileName))
            {
                try
                {
                    //Code where it connects to the hub
                    _HubConfigs = ZappySerializer.DeserializeObject<Dictionary<string, ZappyRobotHubConfiguration>>(File.ReadAllText(ZappyMessagingConstants.ZappyHubConfigurationFileName));
                }
                catch
                {
                    throw;
                }
            }

            if (_HubConfigs == null)
                _HubConfigs = new Dictionary<string, ZappyRobotHubConfiguration>();
            return _HubConfigs;
        }

        public static void SaveHubConfig(Dictionary<string, ZappyRobotHubConfiguration> HubConfigs)
        {
            try
            {
                File.WriteAllText(ZappyMessagingConstants.ZappyHubConfigurationFileName, ZappySerializer.SerializeObject(HubConfigs));
            }
            catch
            {
                throw;
            }
        }

    }
}
