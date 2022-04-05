
//namespace IntegerAI.Trapy.ManagedHooks
//{
//	/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/Class/*'/>
//	public class KeyboardHookExt : KeyboardHook
//	{
//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/KeyboardEventHandlerExt/*'/>
//		public delegate void KeyboardEventHandlerExt(System.Windows.Forms.Keys key);

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/KeyDown/*'/>
//		public event KeyboardEventHandlerExt KeyDown;
//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/KeyUp/*'/>
//		public event KeyboardEventHandlerExt KeyUp;
//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/SystemKeyDown/*'/>
//		public event KeyboardEventHandlerExt SystemKeyDown;
//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/SystemKeyUp/*'/>
//		public event KeyboardEventHandlerExt SystemKeyUp;

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/ctor/*'/>
//		public KeyboardHookExt()
//		{
//			this.KeyboardEvent += new KeyboardEventHandler(KeyboardHookExt_KeyboardEvent);
//		}

//		private void KeyboardHookExt_KeyboardEvent(IntegerAI.Trapy.ManagedHooks.KeyboardEvents kEvent, System.Windows.Forms.Keys key)
//		{
//			switch (kEvent)
//			{
//				case KeyboardEvents.KeyDown:
//					OnKeyDown(key);
//					break;
//				case KeyboardEvents.KeyUp:
//					OnKeyUp(key);
//					break;
//				case KeyboardEvents.SystemKeyDown:
//					OnSysKeyDown(key);
//					break;
//				case KeyboardEvents.SystemKeyUp:
//					OnSysKeyUp(key);
//					break;
//			}
//		}

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnKeyDown/*'/>
//		protected virtual void OnKeyDown(System.Windows.Forms.Keys key)
//		{
//			Fire_KeyDown(key);
//		}

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnKeyUp/*'/>
//		protected virtual void OnKeyUp(System.Windows.Forms.Keys key)
//		{
//			Fire_KeyUp(key);
//		}

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnSysKeyDown/*'/>
//		protected virtual void OnSysKeyDown(System.Windows.Forms.Keys key)
//		{
//			Fire_SystemKeyDown(key);
//		}

//		/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnSysKeyUp/*'/>
//		protected virtual void OnSysKeyUp(System.Windows.Forms.Keys key)
//		{
//			Fire_SystemKeyUp(key);
//		}

//		private void Fire_KeyDown(System.Windows.Forms.Keys key)
//		{
//			if (KeyDown != null)
//			{
//				KeyDown(key);
//			}
//		}

//		private void Fire_KeyUp(System.Windows.Forms.Keys key)
//		{
//			if (KeyUp != null)
//			{
//				KeyUp(key);
//			}
//		}

//		private void Fire_SystemKeyDown(System.Windows.Forms.Keys key)
//		{
//			if (SystemKeyDown != null)
//			{
//				SystemKeyDown(key);
//			}
//		}

//		private void Fire_SystemKeyUp(System.Windows.Forms.Keys key)
//		{
//			if (SystemKeyUp != null)
//			{
//				SystemKeyUp(key);
//			}
//		}

//	}
//}
