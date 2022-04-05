namespace Zappy.ManagedHooks
{
	/// <include file='ManagedHooks.xml' path='Docs/MouseHook/Class/*'/>
	public class MouseHook : SystemHook
	{

        //    [StructLayout(LayoutKind.Sequential)]
        //    struct MOUSEHOOKSTRUCT
        //{
        //    public Point pt;
        //    public IntPtr hwnd;
        //    public uint wHitTestCode;
        //    public IntPtr dwExtraInfo;
        //}
       

       
        /// <include file='ManagedHooks.xml' path='Docs/MouseHook/ctor/*'/>
        public MouseHook() : base(NativeHookWrapper.HookTypes.MouseLL)
        {

        }


        ///// <include file='ManagedHooks.xml' path='Docs/MouseHook/HookCallback/*'/>
        //protected override void HookCallback(int code, UIntPtr wparam, IntPtr lparam)
        //{
        //    if (MouseEvent == null)
        //    {
        //        return;
        //    }
        //    MouseHookInfo _MouseHookInfo = new MouseHookInfo();
        //    _MouseHookInfo.MessageType = MessageType.MouseHookEvent;
        //    _MouseHookInfo.Event = (TrapyLowLevelHookMessage)code;
        //    _MouseHookInfo.EventTime = Environment.TickCount;
        //    _MouseHookInfo.Hwnd = wparam;
        //    unsafe
        //    {
        //        MOUSEHOOKSTRUCT* _Ptr = (MOUSEHOOKSTRUCT*)lparam;
        //        _MouseHookInfo.dwExtraInfo = _Ptr->dwExtraInfo;
        //        _MouseHookInfo.flags = _Ptr->flags;
        //        _MouseHookInfo.mouseData = _Ptr->mouseData;
        //        _MouseHookInfo.Point = _Ptr->pt;

        //    }

        //    MouseEvent(_MouseHookInfo);
        //}
        	
	}

}
