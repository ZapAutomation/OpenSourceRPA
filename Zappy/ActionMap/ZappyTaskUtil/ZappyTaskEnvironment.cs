using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    [Serializable]
    public class ZappyTaskEnvironment : ZappyTaskEnvironmentContainerBase<SettingGroup>
    {
        private static bool environmentInfoLogged;
        private static ZappyTaskEnvironment systemEnvironment;

        private static ZappyTaskEnvironment CaptureAllSettings()
        {
            ZappyTaskEnvironment environment = new ZappyTaskEnvironment();
            CommonUtility.PopulateAdditionalEnvironmentSettings(environment);
            environment.Group.Add(TechnologyManagerSettings.CaptureAllSettings());
            return environment;
        }

        internal override Collection<SettingMismatch> Compare(ZappyTaskEnvironmentContainerBase<SettingGroup> targetBaseEnvironment)
        {
            if (targetBaseEnvironment == null)
            {
                return GetAllSettingsAsMismatchedSettings(MismatchType.SourceOnly);
            }
            ZappyTaskEnvironment environment = targetBaseEnvironment as ZappyTaskEnvironment;
            Collection<SettingMismatch> collection = new Collection<SettingMismatch>();
            foreach (SettingGroup group in Group)
            {
                SettingGroup objectToCompare = environment.Get(group.Name);
                if (objectToCompare == null)
                {
                    foreach (SettingMismatch mismatch in group.GetAllSettingsAsMismatchedSettings(MismatchType.SourceOnly))
                    {
                        collection.Add(mismatch);
                    }
                }
                else
                {
                    foreach (SettingMismatch mismatch2 in group.Compare(objectToCompare))
                    {
                        collection.Add(mismatch2);
                    }
                }
            }
            foreach (SettingGroup group3 in environment.Group)
            {
                if (Get(group3.Name) == null)
                {
                    foreach (SettingMismatch mismatch3 in group3.GetAllSettingsAsMismatchedSettings(MismatchType.TargetOnly))
                    {
                        collection.Add(mismatch3);
                    }
                }
            }
            return collection;
        }

        internal override Collection<SettingMismatch> GetAllSettingsAsMismatchedSettings(MismatchType mismatchType)
        {
            Collection<SettingMismatch> collection = new Collection<SettingMismatch>();
            foreach (SettingGroup group in Group)
            {
                foreach (SettingMismatch mismatch in group.GetAllSettingsAsMismatchedSettings(mismatchType))
                {
                    collection.Add(mismatch);
                }
            }
            return collection;
        }

        internal static void LogEnvironmentInfo()
        {
            
                        environmentInfoLogged = true;
                        
                                    
                                }

        [XmlElement(IsNullable = false, ElementName = "SettingGroup")]
        public Collection<SettingGroup> Group =>
            ChildObjectCollection;

        internal static ZappyTaskEnvironment SystemEnvironment
        {
            get
            {
                if (systemEnvironment == null)
                {
                    systemEnvironment = CaptureAllSettings();
                }
                return systemEnvironment;
            }
        }
    }
}