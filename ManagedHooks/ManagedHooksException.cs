
using System;

namespace Zappy.ManagedHooks
{
	/// <include file='ManagedHooks.xml' path='Docs/ManagedHooksException/Class/*'/>
	public class ManagedHooksException : ApplicationException
	{
		/// <include file='ManagedHooks.xml' path='Docs/ManagedHooksException/ctor/*'/>
		public ManagedHooksException()
		{
		}

		/// <include file='ManagedHooks.xml' path='Docs/ManagedHooksException/ctor_string/*'/>
		public ManagedHooksException(string message) : base(message) 
		{
		}

		/// <include file='ManagedHooks.xml' path='Docs/ManagedHooksException/ctor_string_exception/*'/>
		public ManagedHooksException(string message, Exception innerException) 
			: base(message, innerException) 
		{
		}
	}
}
