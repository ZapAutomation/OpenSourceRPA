using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    public abstract class ZappyTaskEnvironmentContainerBase<TChild> where TChild : class
    {
        private Collection<TChild> childObjectCollection;
        private string name;

        protected ZappyTaskEnvironmentContainerBase()
        {
            childObjectCollection = new Collection<TChild>();
        }

        internal abstract Collection<SettingMismatch> Compare(ZappyTaskEnvironmentContainerBase<TChild> objectToCompare);
        public override bool Equals(object obj)
        {
            string b = obj as string;
            if (b != null)
            {
                return string.Equals(Name, b, StringComparison.Ordinal);
            }
            return base.Equals(obj);
        }

        internal TChild Get(string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                foreach (TChild local2 in ChildObjectCollection)
                {
                    if (local2.Equals(searchKey))
                    {
                        return local2;
                    }
                }
            }
            return default(TChild);
        }

        internal abstract Collection<SettingMismatch> GetAllSettingsAsMismatchedSettings(MismatchType mismatchType);
        public override int GetHashCode() =>
            base.GetHashCode();

        internal Collection<TChild> ChildObjectCollection
        {
            get =>
                childObjectCollection;
            set
            {
                childObjectCollection = value;
            }
        }

        [XmlIgnore]
        public string Name
        {
            get =>
                name;
            set
            {
                name = value;
            }
        }
    }
}