using System;
using System.Collections.Generic;

namespace Zappy.Decode.Helper
{
    internal interface IZappyTaskExtensionPackageManager : IDisposable
    {
                IList<T> GetExtensions<T>() where T : class;
    }
}