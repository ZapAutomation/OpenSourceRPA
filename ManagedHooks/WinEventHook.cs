using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zappy.ManagedHooks
{
    public class WinEventHook : IDisposable
    {
        #region Member Variables and Delegates

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookProcessedHandler/*'/>

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/type/*'/>
        private IntPtr _hWinEvent;
        public readonly AccessibleEvents Start, Stop;
        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/processHandler/*'/>

        private bool _isHooked;

        #endregion
        //public event Action<TrapyWinEvent> WinEventArrived;

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/ctor/*'/>
        public WinEventHook(AccessibleEvents Start= AccessibleEvents.Destroy, AccessibleEvents Stop= AccessibleEvents.Show)
        //public WinEventHook(AccessibleEvents Start= AccessibleEvents.Destroy, AccessibleEvents Stop= AccessibleEvents.Focus)
        {
            this.Start = Start;
            this.Stop = Stop;
        }

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/dtor/*'/>
        ~WinEventHook()
        {
            Trace.WriteLine("SystemHook (" + Start + Stop  + ") WARNING: Finalizer called, " +
                "a reference has not been properly disposed.");

            Dispose(false);
        }

        
        GCHandle gCHandle;
        
        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/InstallHook/*'/>
        public void InstallHook(NativeHookWrapper.WinEventDelegate del)
        {
            _hWinEvent = NativeHookWrapper.InitializeWinEvent(Start, Stop, 0);
            if (_hWinEvent==IntPtr.Zero)
            {
                throw new ManagedHooksException("Hook initialization failed.");
            }

            //NativeHookWrapper.WinEventDelegate del = InternalHookCallback;
            gCHandle = GCHandle.Alloc(del);
            NativeHookWrapper.SetCallBackResults result = NativeHookWrapper.SetWinEventCallback(del, _hWinEvent);
            if (result != NativeHookWrapper.SetCallBackResults.Success)
            {
                Dispose();
                GenerateCallBackException( result);
            }
            _isHooked = true;
        }

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninstallHook/*'/>
        public void UninstallHook()
        {
            
            _isHooked = false;
            NativeHookWrapper.UninitializeWinEvent(_hWinEvent);
            if (gCHandle.IsAllocated)
                gCHandle.Free();
        }

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/IsHooked/*'/>
        public bool IsHooked
        {
            get
            {
                return _isHooked;
            }
        }

        #region Exception Generation Helper Methods

        private void GenerateCallBackException(NativeHookWrapper.SetCallBackResults result)
        {
            if (result == NativeHookWrapper.SetCallBackResults.Success)
            {
                return;
            }

            string msg;

            switch (result)
            {
                case NativeHookWrapper.SetCallBackResults.AlreadySet:
                    msg = "A hook of type " + Start + Stop + " is already registered. You can only register ";
                    msg += "a single instance of each type of hook class. This can also occur when you forget ";
                    msg += "to unregister or dispose a previous instance of the class.";

                    throw new ManagedHooksException(msg);

                case NativeHookWrapper.SetCallBackResults.ArgumentError:
                    msg = "Failed to set hook callback due to an error in the arguments.";

                    throw new ArgumentException(msg);

                case NativeHookWrapper.SetCallBackResults.NotImplemented:
                    msg = "The hook type of type " + Start + Stop + " is not implemented in the C++ layer. ";
                    msg += "You must implement this hook type before you can use it. See the C++ function ";
                    msg += "SetUserHookCallback.";

                    throw new HookTypeNotImplementedException(msg);
            }

            msg = "Unrecognized exception during hook callback setup. Error code " + result + ".";
            throw new ManagedHooksException(msg);
        }


        #endregion

        #region IDisposable Members

        /// <include file='ManagedHooks.xml' path='Docs/SystemHook/Dispose/*'/>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            UninstallHook();
            NativeHookWrapper.DisposeCppLayer();
        }

        #endregion
    }

}
