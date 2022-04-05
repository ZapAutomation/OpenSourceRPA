#include "stdafx.h"
#include <windows.h>
#include "SystemHookCore.h"
#include "MessageFilter.h"
#include "WinUser.h"

HOOKPROC UserMouseHookCallback = NULL;
HOOKPROC UserKeyboardHookCallback = NULL;
WINEVENTPROC UserActiveWindowHookCallback = NULL;

HHOOK hookMouse = NULL;
HHOOK hookKeyboard = NULL;
HWINEVENTHOOK hookActiveWindow = NULL;

HINSTANCE g_appInstance = NULL;

static LRESULT CALLBACK InternalKeyboardHookCallback(int code, WPARAM wparam, LPARAM lparam);
static LRESULT CALLBACK InternalMouseHookCallback(int code, WPARAM wparam, LPARAM lparam);
static void CALLBACK HandleWinEvent(HWINEVENTHOOK hook, DWORD event, HWND hwnd,
	LONG idObject, LONG idChild,
	DWORD dwEventThread, DWORD dwmsEventTime);

int SetUserHookCallback(HOOKPROC userProc, UINT hookID)
{
	if (userProc == NULL)
	{
		return HookCoreErrors::SetCallBack::ARGUMENT_ERROR;
	}

	if (hookID == WH_KEYBOARD_LL)
	{
		if (UserKeyboardHookCallback != NULL)
			return HookCoreErrors::SetCallBack::ALREADY_SET;
		UserKeyboardHookCallback = userProc;
		return HookCoreErrors::SetCallBack::SUCCESS;
	}
	else if (hookID == WH_MOUSE_LL)
	{
		if (UserMouseHookCallback != NULL)
			return HookCoreErrors::SetCallBack::ALREADY_SET;
		UserMouseHookCallback = userProc;
		return HookCoreErrors::SetCallBack::SUCCESS;
	}
	return HookCoreErrors::SetCallBack::NOT_IMPLEMENTED;
}

bool InitializeHook(UINT hookID, int threadID)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	if (hookID == WH_KEYBOARD_LL)
	{
		if (UserKeyboardHookCallback == NULL)
		{
			return false;
		}

		hookKeyboard = SetWindowsHookEx(hookID, (HOOKPROC)InternalKeyboardHookCallback, g_appInstance, threadID);
		return hookKeyboard != NULL;
	}
	else if (hookID == WH_MOUSE_LL)
	{
		if (UserMouseHookCallback == NULL)
		{
			return false;
		}

		hookMouse = SetWindowsHookEx(hookID, (HOOKPROC)InternalMouseHookCallback, g_appInstance, threadID);
		return hookMouse != NULL;
	}
	return false;
}

HWINEVENTHOOK InitializeWinEvent(UINT SysEventIDStart, UINT SysEventIDStop, int threadID)
{
	if (g_appInstance == NULL)
	{
		return false;
	}

	hookActiveWindow = SetWinEventHook(
		SysEventIDStart, SysEventIDStop,  // Range of events (4 to 5).
		g_appInstance,                                          // Handle to DLL.
		HandleWinEvent,                                // The callback.
		0, 0,              // Process and thread IDs of interest (0 = all)
		WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS); // Flags.

	return hookActiveWindow ;
}

int SetWinEventCallback(WINEVENTPROC UserProc, HWINEVENTHOOK SysEventID)
{
	if (UserActiveWindowHookCallback != NULL)
		return HookCoreErrors::SetCallBack::ALREADY_SET;

	if (SysEventID != hookActiveWindow)
		return HookCoreErrors::SetCallBack::NOT_IMPLEMENTED;

	UserActiveWindowHookCallback = UserProc;
	return HookCoreErrors::SetCallBack::SUCCESS;
}

void UninitializeHook(UINT hookID)
{
	if (hookID == WH_KEYBOARD_LL)
	{
		if (hookKeyboard != NULL)
		{
			UnhookWindowsHookEx(hookKeyboard);
		}
		hookKeyboard = NULL;
		UserKeyboardHookCallback = NULL;
	}
	else if (hookID == WH_MOUSE_LL)
	{
		if (hookMouse != NULL)
		{
			UnhookWindowsHookEx(hookMouse);
		}
		UserMouseHookCallback = NULL;
		hookMouse = NULL;
	}
	

}

void UninitializeWinEvent(HWINEVENTHOOK SysEventID)
{
	if (SysEventID == hookActiveWindow)
	{
		if (hookActiveWindow != NULL)
		{
			UnhookWinEvent(hookActiveWindow);
		}
		hookActiveWindow = NULL;
		UserActiveWindowHookCallback = NULL;
	}


}

void Dispose()
{
	UninitializeHook(WH_KEYBOARD_LL);
	UninitializeHook(WH_MOUSE_LL);
	UninitializeWinEvent(hookActiveWindow);
	UserKeyboardHookCallback = NULL;
	UserMouseHookCallback = NULL;
	UserActiveWindowHookCallback = NULL;
}

//int FilterMessage(UINT hookID, int message)
//{
//	if (hookID == WH_KEYBOARD_LL)
//	{
//		if (keyboardFilter.AddMessage(message))
//		{
//			return HookCoreErrors::FilterMessage::SUCCESS;
//		}
//		else
//		{
//			return HookCoreErrors::FilterMessage::FAILED;
//		}
//	}
//	else if (hookID == WH_MOUSE_LL)
//	{
//		if(mouseFilter.AddMessage(message))
//		{
//			return HookCoreErrors::FilterMessage::SUCCESS;
//		}
//		else
//		{
//			return HookCoreErrors::FilterMessage::FAILED;
//		}
//	}
//
//	return HookCoreErrors::FilterMessage::NOT_IMPLEMENTED;
//}

static LRESULT CALLBACK InternalMouseHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if ((int)wparam != 0x0200 && code >= 0 && UserMouseHookCallback != NULL)//&& !mouseFilter.IsFiltered((int)wparam)
	{
		/*MOUSEHOOKSTRUCT * pMouseStruct = (MOUSEHOOKSTRUCT *)lparam;
		HWND _Handle = NULL;
		if (pMouseStruct != NULL)
		{
			_Handle = pMouseStruct->hwnd;
			if (_Handle == NULL)
			{
				_Handle = WindowFromPoint(pMouseStruct->pt);
				if (_Handle == NULL)
					ChildWindowFromPoint(NULL, pMouseStruct->pt);
			}
		}*/

		UserMouseHookCallback((int)wparam, wparam, lparam);
	}
	return CallNextHookEx(hookMouse, code, wparam, lparam);
}

static LRESULT CALLBACK InternalKeyboardHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0 && UserKeyboardHookCallback != NULL)//&& !keyboardFilter.IsFiltered((int)wparam)
	{
		bool _Call = false;

		if (((wparam != 0x100) &&
			(wparam != 0x101)) && (wparam != 260))
		{
			_Call = (wparam == 0x105);
		}
		else
			_Call = true;
		if (_Call)
			UserKeyboardHookCallback(code, wparam, lparam);
	}
	return CallNextHookEx(hookKeyboard, code, wparam, lparam);
}

static void CALLBACK HandleWinEvent(HWINEVENTHOOK hook, DWORD event, HWND hwnd,
	LONG idObject, LONG idChild,
	DWORD dwEventThread, DWORD dwmsEventTime)
{
	if (UserActiveWindowHookCallback != NULL)//event == EVENT_SYSTEM_FOREGROUND && 
		UserActiveWindowHookCallback(hook, event, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
}
