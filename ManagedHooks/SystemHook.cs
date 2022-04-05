using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Zappy.ManagedHooks
{
    /// <include file='ManagedHooks.xml' path='Docs/SystemHook/Class/*'/>
    public abstract class SystemHook : IDisposable
	{
		#region Member Variables and Delegates

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookProcessedHandler/*'/>

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/type/*'/>
		private NativeHookWrapper.HookTypes _type = NativeHookWrapper.HookTypes.None;

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/processHandler/*'/>

		private bool _isHooked;

        #endregion
		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/ctor/*'/>
		public SystemHook(NativeHookWrapper.HookTypes type)
		{
			_type = type;
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/dtor/*'/>
		~SystemHook()
		{
			Trace.WriteLine("SystemHook (" + _type + ") WARNING: Finalizer called, " +
				"a reference has not been properly disposed.");

			Dispose(false);
		}

		///// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookCallback/*'/>
		//protected abstract void HookCallback(int code, UIntPtr wparam, IntPtr lparam);

		///// <include file='ManagedHooks.xml' path='Docs/SystemHook/InternalHookCallback/*'/>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//private void InternalHookCallback( int code, UIntPtr wparam, IntPtr lparam)
		//{
  //          try
  //          {
  //              HookCallback(code, wparam, lparam);
  //          }

  //          catch (Exception e)
  //          {
  //              //
  //              // While it is not generally a good idea to trap and discard all exceptions
  //              // in a base class, this is a special case. Remember, this is the entry point
  //              // for the C++ library to call into our .NET code. We don't want to return
  //              // .NET exceptions to C++. If it gets this far all we can do is drop them.
  //              //
  //              Debug.WriteLine("Exception during hook callback: " + e.Message + " " + e);
  //          }
		//}

        GCHandle gcHandle;
		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InstallHook/*'/>
		public void InstallHook(NativeHookWrapper.HookProcessedHandler del)
		{
            gcHandle = GCHandle.Alloc(del);
            NativeHookWrapper.SetCallBackResults result = NativeHookWrapper.SetUserHookCallback(del, _type);
            if (result != NativeHookWrapper.SetCallBackResults.Success)
            {
                Dispose();
                GenerateCallBackException(_type, result);
            }

            if (!NativeHookWrapper.InitializeHook(_type, 0))
			{
				throw new ManagedHooksException("Hook initialization failed.");
			}
			_isHooked = true;
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninstallHook/*'/>
		public void UninstallHook()
		{
			_isHooked = false;
            NativeHookWrapper.UninitializeHook(_type);
            if(gcHandle.IsAllocated)
            gcHandle.Free();
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookType/*'/>
		protected NativeHookWrapper.HookTypes HookType
		{
			get
			{
				return _type;
			}
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

		private void GenerateCallBackException(NativeHookWrapper.HookTypes type, NativeHookWrapper.SetCallBackResults result)
		{
			if (result == NativeHookWrapper.SetCallBackResults.Success)
			{
				return;
			}

			string msg;

			switch (result)
			{
				case NativeHookWrapper.SetCallBackResults.AlreadySet:
					msg = "A hook of type " + type + " is already registered. You can only register ";
					msg += "a single instance of each type of hook class. This can also occur when you forget ";
					msg += "to unregister or dispose a previous instance of the class.";

					throw new ManagedHooksException(msg);

				case NativeHookWrapper.SetCallBackResults.ArgumentError:
					msg = "Failed to set hook callback due to an error in the arguments.";

					throw new ArgumentException(msg);

				case NativeHookWrapper.SetCallBackResults.NotImplemented:
					msg = "The hook type of type " + type + " is not implemented in the C++ layer. ";
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
