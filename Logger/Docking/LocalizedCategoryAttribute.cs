using System;
using System.ComponentModel;
using ZappyLogger.Docking.Helpers;

namespace ZappyLogger.Docking
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedCategoryAttribute : CategoryAttribute
    {
        #region cTor

        public LocalizedCategoryAttribute(string key) : base(key)
        {
        }

        #endregion

        #region Overrides

        protected override string GetLocalizedString(string key)
        {
            return ResourceHelper.GetString(key);
        }

        #endregion
    }
}