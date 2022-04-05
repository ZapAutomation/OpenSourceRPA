using System;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class ValidationResult : EventArgs
    {
        internal Exception Exception { get; set; }

        internal string Message { get; set; }

        internal int Severity { get; set; }
    }
}