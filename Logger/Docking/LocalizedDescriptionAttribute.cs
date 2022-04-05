using System;
using System.ComponentModel;
using ZappyLogger.Docking.Helpers;

namespace ZappyLogger.Docking
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        #region Fields

        private bool m_initialized = false;

        #endregion

        #region cTor

        public LocalizedDescriptionAttribute(string key) : base(key)
        {
        }

        #endregion

        #region Properties

        public override string Description
        {
            get
            {
                if (!m_initialized)
                {
                    string key = base.Description;
                    DescriptionValue = ResourceHelper.GetString(key);
                    if (DescriptionValue == null)
                    {
                        DescriptionValue = string.Empty;
                    }

                    m_initialized = true;
                }

                return DescriptionValue;
            }
        }

        #endregion
    }
}