#pragma once

namespace HookCoreErrors
{
	namespace SetCallBack
	{
		const int SUCCESS = 1;
		const int ALREADY_SET = -2;
		const int NOT_IMPLEMENTED = -3;
		const int ARGUMENT_ERROR = -4;
	}
	namespace FilterMessage
	{
		const int SUCCESS = 1;
		const int FAILED = -2;
		const int NOT_IMPLEMENTED = -3;
	}
}
bool InitializeHook(UINT hookID, int threadID);
int SetUserHookCallback(HOOKPROC userProc, UINT hookID);
HWINEVENTHOOK InitializeWinEvent(UINT SysEventIDStart, UINT SysEventIDStop, int threadID);
int SetWinEventCallback(WINEVENTPROC UserProc, HWINEVENTHOOK EventID);
void UninitializeHook(UINT hookID);
void UninitializeWinEvent(HWINEVENTHOOK SysEventID);
void Dispose();

//int FilterMessage(UINT hookID, int message);
//bool GetMousePosition(WPARAM wparam, LPARAM lparam, int & x, int & y);

//bool GetKeyboardReading(WPARAM wparam, LPARAM lparam, int & vkCode);

