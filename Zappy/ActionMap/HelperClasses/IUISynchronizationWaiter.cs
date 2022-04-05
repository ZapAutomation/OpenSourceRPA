using System;

namespace Zappy.ActionMap.HelperClasses
{
    public interface IUISynchronizationWaiter : IDisposable
    {
        bool Wait(int timeout, object data);
        void Remove();
        bool Reset();
    }
}