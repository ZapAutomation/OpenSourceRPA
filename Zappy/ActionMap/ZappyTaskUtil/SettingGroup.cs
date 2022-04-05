using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    [Serializable]
    public class SettingGroup : ZappyTaskEnvironmentContainerBase<Setting>
    {
        public SettingGroup()
        {
        }

        public SettingGroup(string name)
        {
            Name = name;
        }

        internal override Collection<SettingMismatch> Compare(ZappyTaskEnvironmentContainerBase<Setting> targetBaseSettingGroup)
        {
            SettingGroup group = targetBaseSettingGroup as SettingGroup;
            Collection<SettingMismatch> collection = new Collection<SettingMismatch>();
            foreach (Setting setting in Setting)
            {
                Setting target = group.Get(setting.Name);
                if (target == null)
                {
                    collection.Add(new SettingMismatch(GroupName, setting, MismatchType.SourceOnly));
                }
                else if (!setting.Equals(target))
                {
                    collection.Add(new SettingMismatch(GroupName, setting, target));
                }
            }
            foreach (Setting setting3 in group.Setting)
            {
                if (Get(setting3.Name) == null)
                {
                    collection.Add(new SettingMismatch(GroupName, setting3, MismatchType.TargetOnly));
                }
            }
            return collection;
        }

        internal override Collection<SettingMismatch> GetAllSettingsAsMismatchedSettings(MismatchType mismatchType)
        {
            Collection<SettingMismatch> collection = new Collection<SettingMismatch>();
            foreach (Setting setting in Setting)
            {
                collection.Add(new SettingMismatch(GroupName, setting, mismatchType));
            }
            return collection;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            object[] args = { Name };
            builder.AppendFormat(CultureInfo.InvariantCulture, "SettingName: {0}. ", args);
            foreach (Setting setting in Setting)
            {
                builder.Append(setting);
            }
            return builder.ToString();
        }

        [XmlAttribute(AttributeName = "Name")]
        public string GroupName
        {
            get =>
                Name;
            set
            {
                Name = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public Collection<Setting> Setting =>
            ChildObjectCollection;
    }
}