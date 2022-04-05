namespace IntegerAI.ManagedHooks
{
    /// <include file='ManagedHooks.xml' path='Docs/KeyboardTracking/Class/*'/>
    //public class KeyboardTracking : KeyboardHookExt
    //{
    //	private bool _controlPressed = false;
    //	private bool _altPressed = false;
    //	private bool _shiftPressed = false;

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardTracking/ctor/*'/>
    //	public KeyboardTracking()
    //	{
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnKeyDown/*'/>
    //	protected override void OnKeyDown(System.Windows.Forms.Keys key)
    //	{
    //		bool isSystemKey = SetTrackingState(key, true);
    //		if (isSystemKey)
    //		{
    //			return;
    //		}

    //		base.OnKeyDown(key);
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardHookExt/OnKeyUp/*'/>
    //	protected override void OnKeyUp(System.Windows.Forms.Keys key)
    //	{
    //		bool isSystemKey = SetTrackingState(key, true);
    //		if (isSystemKey)
    //		{
    //			return;
    //		}

    //		base.OnKeyUp(key);
    //	}

    //	private bool SetTrackingState(System.Windows.Forms.Keys key, bool isDown)
    //	{
    //		switch (key)
    //		{
    //			case Keys.Alt:
    //				_altPressed = isDown;
    //				return true;
    //			case Keys.Control:
    //				_controlPressed = isDown;
    //				return true;
    //			case Keys.Shift:
    //				_shiftPressed = isDown;
    //				return true;
    //		}

    //		return false;
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardTracking/ControlPressed/*'/>
    //	public bool ControlPressed
    //	{
    //		get
    //		{
    //			return _controlPressed;
    //		}
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardTracking/AltPressed/*'/>
    //	public bool AltPressed
    //	{
    //		get
    //		{
    //			return _altPressed;
    //		}
    //	}

    //	/// <include file='ManagedHooks.xml' path='Docs/KeyboardTracking/ShiftPressed/*'/>
    //	public bool ShiftPressed
    //	{
    //		get
    //		{
    //			return _shiftPressed;
    //		}
    //	}
    //}
}
