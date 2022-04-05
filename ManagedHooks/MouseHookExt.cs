// /////////////////////////////////////////////////////////////
// File: MouseHookExt.cs	Class: IntegerAI.Trapy.ManagedHooks.MouseHookExt
// Date: 2/25/2004			Author: Michael Kennedy
// Language: C#				Framework: .NET
//
// Copyright: Copyright (c) Michael Kennedy, 2004-2005
// /////////////////////////////////////////////////////////////
// License: See License.txt file included with application.
// Description: See compiled documentation (Managed Hooks.chm)
// /////////////////////////////////////////////////////////////


namespace Zappy.ManagedHooks
{
    /// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/Class/*'/>
    //public class MouseHookExt : MouseHook
    //{
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/MouseEventHandlerExt/*'/>
    //	public delegate void MouseEventHandlerExt(Point pt);

    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/LeftButtonDown/*'/>
    //	public event MouseEventHandlerExt LeftButtonDown;
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/RightButtonDown/*'/>
    //	public event MouseEventHandlerExt RightButtonDown;
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/LeftButtonUp/*'/>
    //	public event MouseEventHandlerExt LeftButtonUp;
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/RightButtonUp/*'/>
    //	public event MouseEventHandlerExt RightButtonUp;
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/MouseWheel/*'/>
    //	public event MouseEventHandlerExt MouseWheel;
    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/Move/*'/>
    //	public event MouseEventHandlerExt Move;

    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHookExt/ctor/*'/>
    //	public MouseHookExt()
    //	{
    //		this.MouseEvent += new MouseEventHandler(MouseHookExt_MouseEvent);
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/MouseHook/HookCallback/*'/>
    //	protected override void HookCallback(int code, UIntPtr wparam, IntPtr lparam)
    //	{
    //		base.HookCallback(code, wparam, lparam);
    //	}

    //	private void MouseHookExt_MouseEvent(MouseEvents mEvent, System.Drawing.Point point)
    //	{
    //		switch (mEvent)
    //		{
    //			case MouseEvents.LeftButtonUp:
    //				Fire_LeftButtonUp(point);
    //				break;
    //			case MouseEvents.LeftButtonDown:
    //				Fire_LeftButtonDown(point);
    //				break;
    //			case MouseEvents.RightButtonUp:
    //				Fire_RightButtonUp(point);
    //				break;
    //			case MouseEvents.RightButtonDown:
    //				Fire_RightButtonDown(point);
    //				break;
    //			case MouseEvents.MouseWheel:
    //				Fire_MouseWheel(point);
    //				break;
    //			case MouseEvents.Move:
    //				Fire_Move(point);
    //				break;
    //		}
    //	}

    //	private void Fire_LeftButtonDown(Point point)
    //	{
    //		if (LeftButtonDown != null)
    //		{
    //			LeftButtonDown(point);
    //		}
    //	}

    //	private void Fire_LeftButtonUp(Point point)
    //	{
    //		if (LeftButtonUp != null)
    //		{
    //			LeftButtonUp(point);
    //		}
    //	}

    //	private void Fire_RightButtonDown(Point point)
    //	{
    //		if (RightButtonDown != null)
    //		{
    //			RightButtonDown(point);
    //		}
    //	}

    //	private void Fire_RightButtonUp(Point point)
    //	{
    //		if (RightButtonUp != null)
    //		{
    //			RightButtonUp(point);
    //		}
    //	}

    //	private void Fire_Move(Point point)
    //	{
    //		if (Move != null)
    //		{
    //			Move(point);
    //		}
    //	}

    //	private void Fire_MouseWheel(Point point)
    //	{
    //		if (MouseWheel != null)
    //		{
    //			MouseWheel(point);
    //		}
    //	}
    //}
}
