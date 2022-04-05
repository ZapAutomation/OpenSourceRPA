using System.Globalization;
using Zappy.Decode.Helper;
using Zappy.Properties;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    public class SettingMismatch
    {
        private string helperText;
        private string message;
        private string settingGroup;
        private string settingName;
        private string sourceValue;
        private string targetValue;
        private MismatchType typeOfMismatch;
        private int warningLevel;

        public SettingMismatch(string name)
        {
            settingName = name;
        }

        internal SettingMismatch(string groupName, Setting setting, MismatchType mismatchType) : this(setting.Name, setting.Value, mismatchType, setting.WarningLevel)
        {
            settingGroup = groupName;
        }

        internal SettingMismatch(string groupName, Setting sourceSetting, Setting targetSetting)
        {
            settingGroup = groupName;
            typeOfMismatch = MismatchType.DifferentFromSource;
            settingName = sourceSetting.Name;
            sourceValue = sourceSetting.Value;
            targetValue = targetSetting.Value;
            warningLevel = sourceSetting.WarningLevel;
        }

        public SettingMismatch(string name, string value, MismatchType mismatchType, int warningLevel)
        {
            settingName = name;
            typeOfMismatch = mismatchType;
            this.warningLevel = warningLevel;
            MismatchType type = MismatchType;
            if (type != MismatchType.SourceOnly)
            {
                if (type != MismatchType.TargetOnly)
                {
                    throw new ZappyTaskException("Incorrect MismatchType");
                }
            }
            else
            {
                sourceValue = value;
                return;
            }
            targetValue = value;
        }

        public SettingMismatch(string name, string sourceValue, string targetValue, int warningLevel)
        {
            settingName = name;
            this.sourceValue = sourceValue;
            this.targetValue = targetValue;
            typeOfMismatch = MismatchType.DifferentFromSource;
            this.warningLevel = warningLevel;
        }

        public override string ToString()
        {
            object[] args = { MismatchType, SettingGroup, WarningLevel, SettingName, SourceValue, TargetValue };
            return string.Format(CultureInfo.CurrentCulture, Resources.SettingMismatchMessage, args);
        }

        public string HelperText
        {
            get =>
                helperText;
            set
            {
                helperText = value;
            }
        }

        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(message))
                {
                    return ToString();
                }
                return message;
            }
            set
            {
                message = value;
            }
        }

        public MismatchType MismatchType
        {
            get =>
                typeOfMismatch;
            set
            {
                typeOfMismatch = value;
            }
        }

        public string SettingGroup
        {
            get =>
                settingGroup;
            set
            {
                settingGroup = value;
            }
        }

        public string SettingName
        {
            get =>
                settingName;
            set
            {
                settingName = value;
            }
        }

        public string SourceValue
        {
            get =>
                sourceValue;
            set
            {
                sourceValue = value;
            }
        }

        public string TargetValue
        {
            get =>
                targetValue;
            set
            {
                targetValue = value;
            }
        }

        public int WarningLevel
        {
            get =>
                warningLevel;
            set
            {
                warningLevel = value;
            }
        }
    }
}