using System;

namespace ZappyLogger.Classes.Highlight
{
    [Serializable]
    public class ActionEntry
    {
        #region Fields

        public string actionParam;
        public string pluginName;

        #endregion

        #region Public methods

        public ActionEntry Copy()
        {
            ActionEntry e = new ActionEntry();
            e.pluginName = this.pluginName;
            e.actionParam = this.actionParam;
            return e;
        }

        #endregion
    }
}