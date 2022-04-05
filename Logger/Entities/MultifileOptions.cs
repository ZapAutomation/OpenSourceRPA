﻿using System;

namespace ZappyLogger.Entities
{
    [Serializable]
    public class MultifileOptions
    {
        #region Fields

        private string formatPattern = "*$J(.)";
        private int maxDayTry = 3;

        #endregion

        #region Properties

        public int MaxDayTry
        {
            get { return maxDayTry; }
            set { maxDayTry = value; }
        }

        public string FormatPattern
        {
            get { return formatPattern; }
            set { formatPattern = value; }
        }

        #endregion
    }
}