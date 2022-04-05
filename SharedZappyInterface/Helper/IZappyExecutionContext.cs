using System;
using System.Collections.Generic;

namespace Zappy.SharedInterface.Helper
{
    public interface IZappyExecutionContext
    {
        Dictionary<string, object> ContextData { get; }
        Dictionary<string, Tuple<IDynamicProperty, Func<object>>> Variables { get; }
        object GetValueFromKey(string Key);
        void SetValue(string Key, object Value);
        void ThrowIfCancellationRequested();
        void SaveContext(IZappyAction Action);
        void LoadContext(IZappyAction Action);
        string ToString();
        void DestroyContext();
    }
}