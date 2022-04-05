using System;

namespace Zappy.Decode.Helper
{
    internal class BitmapSaveEventArgs : EventArgs
    {
        internal BitmapSaveEventArgs(string fileName)
        {
            this.FileName = fileName;
        }

        internal string FileName { get; private set; }
    }
}