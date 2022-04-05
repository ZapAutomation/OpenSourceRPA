﻿using System;
using System.Collections.Generic;
using ZappyLogger.Classes.Highlight;

namespace ZappyLogger.Entities
{
    [Serializable]
    public class HilightGroup
    {
        #region Fields

        private string groupName = "";
        private List<HilightEntry> hilightEntryList = new List<HilightEntry>();

        #endregion

        #region Properties

        public string GroupName
        {
            get { return this.groupName; }
            set { this.groupName = value; }
        }

        public List<HilightEntry> HilightEntryList
        {
            get { return this.hilightEntryList; }
            set { this.hilightEntryList = value; }
        }

        #endregion
    }
}