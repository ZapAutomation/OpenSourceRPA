#define WINVER 0x0500
#include <windows.h>
#include "keyboard.h"
using namespace std;
//namespace logging = boost::log;
IUIAutomation *g_pAutomation = nullptr;
IUIAutomationElement *focusPnode = NULL;
HWND childHandle = NULL;
static int initlog = 0, uiAuto = 0;
void init_logging()
{
	if (initlog == 0) {
		DWORD bufferSize = 100;
		struct stat st;
		string rpath;
		wstring appdata;
		appdata.resize(bufferSize);
		bufferSize = GetEnvironmentVariableW(L"LOCALAPPDATA", &appdata[0], bufferSize);
#ifdef _DEBUG
		rpath = "\\ZappyAI_DEBUG";
#else
		rpath = "\\ZappyAI";
#endif
		string s(appdata.begin(), appdata.end());
		string path = s.c_str() + rpath;
		if (stat(path.c_str(), &st) != 0)
		{
			CreateDirectoryA(path.c_str(), NULL);
			path = path + "\\Logs";
			CreateDirectoryA(path.c_str(), NULL);
			rpath = "\\PlaybackHelperLog.log";
			path = path.c_str() + rpath;
		}
		else
		{
			rpath = "\\Logs\\PlaybackHelperLog.log";
			path = path.c_str() + rpath;
		}
	    //Commented Boost
		//logging::add_file_log(
		//	boost::log::keywords::file_name = path,
		//	boost::log::keywords::rotation_size = 1 * 1024 * 1024,
		//	boost::log::keywords::max_size = 20 * 1024 * 1024,
		//	boost::log::keywords::time_based_rotation = boost::log::sinks::file::rotation_at_time_point(0, 0, 0),
		//	boost::log::keywords::auto_flush = true);

		//logging::core::get()->set_filter
		//(
		//	logging::trivial::severity >= logging::trivial::info
		//);
	}
	initlog = initlog + 1;
}

BOOL InitializeUIAutomation()
{
	if (uiAuto == 0) {
		CoInitialize(NULL);
		HRESULT hr = CoCreateInstance(__uuidof(CUIAutomation), NULL, CLSCTX_INPROC_SERVER,
			__uuidof(IUIAutomation), (void**)&g_pAutomation);
		return(SUCCEEDED(hr));
	}
	uiAuto = uiAuto + 1;
	return true;
}

IUIAutomationElement* GetTopLevelWindowByName(LPWSTR windowName)
{
	if (windowName == NULL)
	{
		return NULL;
	}

	VARIANT varProp;
	varProp.vt = VT_BSTR;
	varProp.bstrVal = SysAllocString(windowName);
	if (varProp.bstrVal == NULL)
	{
		return NULL;
	}

	IUIAutomationElement* pRoot = NULL;
	IUIAutomationElement* pFound = NULL;
	IUIAutomationCondition* pCondition = NULL;

	// Get the desktop element. 
	HRESULT hr = g_pAutomation->GetRootElement(&pRoot);
	if (FAILED(hr) || pRoot == NULL)
		goto cleanup;

	// Get a top-level element by name, such as "Program Manager"
	hr = g_pAutomation->CreatePropertyCondition(UIA_NamePropertyId, varProp, &pCondition);
	if (FAILED(hr))
		goto cleanup;

	pRoot->FindFirst(TreeScope_Children, pCondition, &pFound);
cleanup:
	if (pRoot != NULL)
		pRoot->Release();

	if (pCondition != NULL)
		pCondition->Release();

	VariantClear(&varProp);
	return pFound;
}
IUIAutomationElement* ListDescendants(IUIAutomationElement* pParent, wstring child)
{
	UIA_HWND hwnd = 0;
	IUIAutomationElement* pNode = NULL;
	static int i = 0;
	if (pParent == NULL)
		return NULL;

	IUIAutomationTreeWalker* pControlWalker = NULL;

	g_pAutomation->get_ControlViewWalker(&pControlWalker);
	if (pControlWalker == NULL)
		return NULL;

	pControlWalker->GetFirstChildElement(pParent, &pNode);
	if (pNode == NULL)
		return pNode;

	while (pNode)
	{
		BSTR desc = nullptr;
		BSTR desc1 = nullptr;
		pNode->get_CurrentAutomationId(&desc);
		pNode->get_CurrentNativeWindowHandle(&hwnd);
		pNode->get_CurrentName(&desc1);
		//GetWindowText((HWND)hwnd, windowTitle, sizeof(windowTitle));
		//BOOST_LOG_TRIVIAL(info) << "childwindow: " <<  i++ <<  "\t" << &desc[0];
		if (!child.empty()) {
			if (((wstring)(&desc[0])) == child)
			{
				//BOOST_LOG_TRIVIAL(info) << "child" << child;
				focusPnode = pNode;
				childHandle = (HWND)hwnd;
				break;
			}
			if (desc1 != nullptr && SysStringLen(desc1) != 0 
								 && ((wstring)(&desc1[0])) == child)
			{
				focusPnode = pNode;
				childHandle = (HWND)hwnd;
				break;
			}
		}

		if (focusPnode == NULL)
		{
			if(((wstring)(&desc[0])) != L"frmZappy") //Ignore zappy window
				ListDescendants(pNode, child);
		}
		if (focusPnode == NULL) {
			IUIAutomationElement* pNext;
			pControlWalker->GetNextSiblingElement(pNode, &pNext);
			pNode = pNext;
		}
		else
			pNode = NULL;

	}
	return NULL;
}

void GetFocusPnode(vector <wstring> windows, int size)
{
	//BOOST_LOG_TRIVIAL(info) << "Size: " << size;
	IUIAutomationElement* pElement = NULL;
	IUIAutomationElement* pNode = NULL;
	focusPnode = NULL;
	childHandle = NULL;
	//InitializeUIAutomation();
	if (g_pAutomation)
		pElement = GetTopLevelWindowByName((LPWSTR)windows[0].c_str());

	if (pElement)
		ListDescendants(pElement, windows[1]);
		//BOOST_LOG_TRIVIAL(info) << "pNode after calling ListDescendants: " << focusPnode << endl;
}
void ShowIfMinimized(HWND windowHandle)
{
	if (windowHandle != NULL)
	{
		if (IsIconic(windowHandle) && IsWindowVisible(windowHandle))
			ShowWindow(windowHandle, SW_RESTORE);
	}
}
int sendKeys(char *text, int value, char **windowTitles, int size, int PauseTimeAfterAction)
{
	init_logging();
	//BOOST_LOG_TRIVIAL(info) << "This is an informational severity message";
	string temp;
	bool descendantIsTopLevelWindow = false;
	vector <wstring> windows;
	static IUIAutomationElement *focusedElement = NULL;
	if(g_pAutomation == nullptr)
		InitializeUIAutomation();
	for (int i = 0; i < size; i++)
	{
		temp = windowTitles[i];
		wstring temp1(temp.begin(), temp.end());
		windows.push_back(temp1);
	}
	if (size > 1)
	{
		if (windows[1] == windows[0])
			descendantIsTopLevelWindow = true;
		if (!descendantIsTopLevelWindow)
			GetFocusPnode(windows, size);
	}

	UINT keystrokes_sent;
	vector <INPUT> finalKeys;
	INPUT keyStroke;
	keyStroke.type = INPUT_KEYBOARD;
	keyStroke.ki.wScan = 0;
	keyStroke.ki.time = 0;
	keyStroke.ki.dwExtraInfo = 0;

	if (value != 0)
	{
		if (value == 1 || value == 2 || value == 4 || value == 8)
		{
			switch (value)
			{
			case 1:
				keyStroke.ki.wVk = VK_MENU;
				break;

			case 2:
				keyStroke.ki.wVk =  VK_CONTROL;
				break;
			case 4:
				keyStroke.ki.wVk = VK_SHIFT;
				break;

			case 8:
				keyStroke.ki.wVk = VK_LWIN;
				break;

			default:
				break;
			}

			keyStroke.ki.dwFlags = 0;

			finalKeys.push_back(keyStroke);
		}
		else if (value == 3 || value == 5 || value == 6 || value == 9 || value == 10 || value == 12)
		{

			switch (value)
			{
			case 3:
				keyStroke.ki.wVk = VK_MENU;
				keyStroke.ki.dwFlags = 0;


				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_CONTROL;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);
				break;


			default:
				break;
			}
		}
	}

	for (int start = 0; text[start]; start++)
	{
		if (text[start] == '{')
		{
			int startIndex = start + 1;
			while (text[start] != '}')
				start++;
			string virtualKey(text, startIndex, start - startIndex);
			if (virtualKey.compare("LControlKey") == 0 || 
				virtualKey.compare("RControlKey") == 0 || 
				virtualKey.compare("LWin") == 0 ||
				virtualKey.compare("RWin") == 0 || 
				virtualKey.compare("LMenu") == 0 || 
				virtualKey.compare("RMenu") == 0)
				continue;
			if (virtualKey.compare("Enter") == 0)
			{
				keyStroke.ki.wVk = VK_RETURN;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_RETURN;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Back") == 0)
			{
				keyStroke.ki.wVk = VK_BACK;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_BACK;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Tab") == 0)
			{
				keyStroke.ki.wVk = VK_TAB;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_TAB;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("CapsLock") == 0)
			{
				keyStroke.ki.wVk = VK_CAPITAL;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_CAPITAL;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("PageUp") == 0)
			{
				keyStroke.ki.wVk = VK_PRIOR;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_PRIOR;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("PageDown") == 0)
			{
				keyStroke.ki.wVk = VK_NEXT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NEXT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("End") == 0)
			{
				keyStroke.ki.wVk = VK_END;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_END;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Home") == 0)
			{
				keyStroke.ki.wVk = VK_HOME;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_HOME;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Left") == 0)
			{
				keyStroke.ki.wVk = VK_LEFT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_LEFT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Up") == 0)
			{
				keyStroke.ki.wVk = VK_UP;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_UP;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Right") == 0)
			{
				keyStroke.ki.wVk = VK_RIGHT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_RIGHT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Down") == 0)
			{
				keyStroke.ki.wVk = VK_DOWN;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_DOWN;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Select") == 0)
			{
				keyStroke.ki.wVk = VK_SELECT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_SELECT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Print") == 0)
			{
				keyStroke.ki.wVk = VK_PRINT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_PRINT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Insert") == 0)
			{
				keyStroke.ki.wVk = VK_INSERT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_INSERT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Delete") == 0)
			{
				keyStroke.ki.wVk = VK_DELETE;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_DELETE;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Help") == 0)
			{
				keyStroke.ki.wVk = VK_HELP;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_HELP;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Escape") == 0)
			{
				keyStroke.ki.wVk = VK_ESCAPE;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_ESCAPE;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad0") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD0;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD0;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad1") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD1;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD1;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad2") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD2;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD2;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad3") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD3;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD3;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad4") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD4;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD4;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad5") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD5;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD5;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad6") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD6;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD6;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad7") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD7;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD7;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad8") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD8;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD8;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumPad9") == 0)
			{
				keyStroke.ki.wVk = VK_NUMPAD9;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMPAD9;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Multiply") == 0)
			{
				keyStroke.ki.wVk = VK_MULTIPLY;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_MULTIPLY;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Add") == 0)
			{
				keyStroke.ki.wVk = VK_ADD;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_ADD;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Separator") == 0)
			{
				keyStroke.ki.wVk = VK_SEPARATOR;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_SEPARATOR;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Decimal") == 0)
			{
				keyStroke.ki.wVk = VK_DECIMAL;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_DECIMAL;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Divide") == 0)
			{
				keyStroke.ki.wVk = VK_DIVIDE;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_DIVIDE;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F1") == 0)
			{
				keyStroke.ki.wVk = VK_F1;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F1;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F2") == 0)
			{
				keyStroke.ki.wVk = VK_F2;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F2;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F3") == 0)
			{
				keyStroke.ki.wVk = VK_F3;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F3;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F4") == 0)
			{
				keyStroke.ki.wVk = VK_F4;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F4;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F5") == 0)
			{
				keyStroke.ki.wVk = VK_F5;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F5;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F6") == 0)
			{
				keyStroke.ki.wVk = VK_F6;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F6;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F7") == 0)
			{
				keyStroke.ki.wVk = VK_F7;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F7;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F8") == 0)
			{
				keyStroke.ki.wVk = VK_F8;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F8;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F9") == 0)
			{
				keyStroke.ki.wVk = VK_F9;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F9;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F10") == 0)
			{
				keyStroke.ki.wVk = VK_F10;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F10;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F11") == 0)
			{
				keyStroke.ki.wVk = VK_F11;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F11;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("F12") == 0)
			{
				keyStroke.ki.wVk = VK_F12;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_F12;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("NumLock") == 0)
			{
				keyStroke.ki.wVk = VK_NUMLOCK;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_NUMLOCK;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("LShiftKey") == 0)
			{
				keyStroke.ki.wVk = VK_LSHIFT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_LSHIFT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("RShiftKey") == 0)
			{
				keyStroke.ki.wVk = VK_RSHIFT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_RSHIFT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Space") == 0)
			{
				keyStroke.ki.wVk = VK_SPACE;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_SPACE;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("Scroll") == 0)
			{
				keyStroke.ki.wVk = VK_SCROLL;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_SCROLL;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else if (virtualKey.compare("PrintScreen") == 0)
			{
				keyStroke.ki.wVk = VK_SNAPSHOT;
				keyStroke.ki.dwFlags = 0;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_SNAPSHOT;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
			}
			else
			{
				for (int i = startIndex; i <= start - 1; i++)
				{
					//BOOST_LOG_TRIVIAL(info) << "text[i]" << text[i];
					keyStroke.ki.wScan = text[i];
					keyStroke.ki.wVk = 0;
					keyStroke.ki.dwFlags = KEYEVENTF_UNICODE;
					finalKeys.push_back(keyStroke);
				}
			}

		}
		else
		{
			HKL kbl = GetKeyboardLayout(0);
		
			if ((int)text[start] > 96 && (int)text[start] < 123) //for lowercase
			{
				keyStroke.ki.dwFlags = 0;
				keyStroke.ki.wScan = 0;
				keyStroke.ki.wVk = VkKeyScanEx(text[start], kbl);
				finalKeys.push_back(keyStroke);
				keyStroke.ki.wVk = VkKeyScanEx(text[start], kbl);
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;
				finalKeys.push_back(keyStroke);
			}
			else //for uppercase
			{
				keyStroke.ki.dwFlags = KEYEVENTF_UNICODE;
				keyStroke.ki.wScan = text[start];
				keyStroke.ki.wVk = 0;
				finalKeys.push_back(keyStroke);
			}
			//keyStroke.ki.wVk = VkKeyScanA(text[start]);
			//keyStroke.ki.dwFlags = 0;

			//finalKeys.push_back(keyStroke);

			//keyStroke.ki.wVk = VkKeyScanA(text[start]);
			//keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;
			//finalKeys.push_back(keyStroke);

			//SendMessage(childHandle, WM_IME_COMPOSITION, CS_INSERTCHAR,(LPARAM)text);
			//LPSTR layoutName=NULL;
			//GetKeyboardLayoutNameA(layoutName);
			//BOOST_LOG_TRIVIAL(info) << "layoutname======" << layoutName;
			//LoadKeyboardLayoutA(layoutName, KLF_SETFORPROCESS);
			
			//keyStroke.ki.wScan = text[start];
			//keyStroke.ki.wVk = 0;
			//keyStroke.ki.dwFlags = KEYEVENTF_UNICODE;
			//finalKeys.push_back(keyStroke);
		}
	}

	if (value != 0)
	{

		if (value == 1 || value == 2 || value == 4 || value == 8)
		{
			switch (value)
			{
			case 1:
				keyStroke.ki.wVk = VK_MENU;
				break;

			case 2:
				keyStroke.ki.wVk = VK_CONTROL;
				break;
            case 4:
				keyStroke.ki.wVk = VK_SHIFT;
				break;

			case 8:
				keyStroke.ki.wVk = VK_LWIN;
				break;

			default:
				break;
			}
			keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

			finalKeys.push_back(keyStroke);
		}
		
		else if (value == 3 || value == 5 || value == 6 || value == 9 || value == 10 || value == 12)
		{

			switch (value)
			{
			case 3:
				keyStroke.ki.wVk = VK_MENU;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);

				keyStroke.ki.wVk = VK_CONTROL;
				keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;

				finalKeys.push_back(keyStroke);
				break;


			default:
				break;
			}
		}

	}
	if (size > 1)
	{
		//BOOST_LOG_TRIVIAL(info) << "PNODE after calling list descendant" << focusPnode;
		HWND windowHandle = FindWindowW(NULL, (LPCWSTR)windows[0].c_str());
		if (windowHandle == NULL)
			return 1;

		ShowIfMinimized(windowHandle);
		//Set the window on top, if the focus is lost
		if(GetForegroundWindow() != windowHandle)
			BringWindowToTop(windowHandle);
		SetForegroundWindow(windowHandle);
		SetActiveWindow(windowHandle);
		Sleep(10);

		if (!descendantIsTopLevelWindow) {
			//BOOST_LOG_TRIVIAL(info) << "descendantIsTopLevelWindow: " << descendantIsTopLevelWindow;
		if (focusPnode == NULL)
			return 2;

			focusPnode->SetFocus();
			Sleep(10);
		}
	}
	
	if(g_pAutomation)
		g_pAutomation->GetFocusedElement(&focusedElement);
	for (int data = 0; data < finalKeys.size(); data++)
	{
		if(focusedElement != NULL)
			focusedElement->SetFocus();
		SendInput(1, &finalKeys[data], sizeof(INPUT));
		if(PauseTimeAfterAction > 0)
			Sleep(PauseTimeAfterAction);
	}
	//keystrokes_sent = SendInput(static_cast<::UINT>(finalKeys.size()), finalKeys.data()	, sizeof ::INPUT);
	return 0;
}

int mouseSingleClick(int locationX, int locationY, int mouseButton, int modifierKeys, char **windowTitles, int size)
{
	init_logging();
	//BOOST_LOG_TRIVIAL(info) << "This is an informational severity message";
	string temp;
	bool descendantIsTopLevelWindow = false;
	vector <wstring> windows;
	IUIAutomationElement *focusedElement = NULL;
	if (g_pAutomation == nullptr)
		InitializeUIAutomation();

	for (int i = 0; i < size; i++)
	{
		temp = windowTitles[i];
		wstring temp1(temp.begin(), temp.end());
		windows.push_back(temp1);
	}
	if (size > 1)
	{
		if (windows[1] == windows[0])
			descendantIsTopLevelWindow = true;
		HWND windowHandle = FindWindowW(NULL, (LPCWSTR)windows[0].c_str());
		if (windowHandle == NULL)
			return 1;
		if (!descendantIsTopLevelWindow) {
			RECT rect;
			GetFocusPnode(windows, size);
			//Throw the exception if descendant is not found
			if (focusPnode == NULL)
				return 2;
			//get rectangle to set the cursor position
			focusPnode->get_CurrentBoundingRectangle(&rect);

			locationX = rect.left + locationX;
			locationY = rect.top + locationY;
		}

		ShowIfMinimized(windowHandle);
		//Set the window on top, if the focus is lost
		if (GetForegroundWindow() != windowHandle)
			BringWindowToTop(windowHandle);
		SetForegroundWindow(windowHandle);
		Sleep(10);

		//Set the cursor position
		SetCursorPos(locationX, locationY);
		Sleep(10);
	}
	INPUT keyStroke = { 0 };
	INPUT mouseStroke = { 0 };

	if (modifierKeys != 0)
	{
		keyStroke.type = INPUT_KEYBOARD;
		switch (modifierKeys)
		{
		case 1:
			keyStroke.ki.wVk = VK_MENU;
			break;


		case 2:
			keyStroke.ki.wVk = VK_CONTROL;
			break;


		case 4:
			keyStroke.ki.wVk = VK_SHIFT;
			break;

		case 8:
			keyStroke.ki.wVk = VK_LWIN;
			break;

		default:
			break;
		}

		keyStroke.ki.wScan = 0;
		keyStroke.ki.dwFlags = 0;
		keyStroke.ki.time = 0;
		keyStroke.ki.dwExtraInfo = 0;
		SendInput(1, &keyStroke, sizeof(keyStroke));
	}

	mouseStroke.type = INPUT_MOUSE;
	switch (mouseButton)
	{
	case 1048576:
		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		break;

	case 2097152:
		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		break;

	case 4194304:
		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		break;

	case 8388608:
		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		break;

	case 16777216:
		break;

	default:
		break;
	}

	if (modifierKeys != 0)
	{
		keyStroke.type = INPUT_KEYBOARD;
		switch (modifierKeys)
		{
		case 1:
			keyStroke.ki.wVk = VK_MENU;
			break;
		case 2:
			keyStroke.ki.wVk = VK_CONTROL;
			break;
		case 4:
			keyStroke.ki.wVk = VK_SHIFT;
			break;
		case 8:
			keyStroke.ki.wVk = VK_LWIN;
			break;
		default:
			break;
		}

		keyStroke.ki.wScan = 0;
		keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;
		keyStroke.ki.time = 0;
		keyStroke.ki.dwExtraInfo = 0;

		SendInput(1, &keyStroke, sizeof(keyStroke));
	}
	return 0;
}

int mouseDoubleClick(int locationX, int locationY, int mouseButton, int modifierKeys, char **windowTitles, int size)
{
	init_logging();
	//BOOST_LOG_TRIVIAL(info) << "This is an informational severity message";
	string temp;
	bool descendantIsTopLevelWindow = false;
	vector <wstring> windows;
	IUIAutomationElement *focusedElement = NULL;
	if (g_pAutomation == nullptr)
		InitializeUIAutomation();

	for (int i = 0; i < size; i++)
	{
		temp = windowTitles[i];
		wstring temp1(temp.begin(), temp.end());
		windows.push_back(temp1);
	}
	if (size > 1)
	{
		if (windows[1] == windows[0])
			descendantIsTopLevelWindow = true;

		HWND windowHandle = FindWindowW(NULL, (LPCWSTR)windows[0].c_str());
		if (windowHandle == NULL)
			return 1;
		if (!descendantIsTopLevelWindow) {
			RECT rect;
			GetFocusPnode(windows, size);
			//throw the exception if descendant is not found
			if (focusPnode == NULL)
				return 2;
			//get rectangle to set the cursor position
			focusPnode->get_CurrentBoundingRectangle(&rect);

			locationX = rect.left + locationX;
			locationY = rect.top + locationY;
		}

		ShowIfMinimized(windowHandle);
		//Set the window on top, if the focus is lost
		if (GetForegroundWindow() != windowHandle)
			BringWindowToTop(windowHandle);
		SetForegroundWindow(windowHandle);
		Sleep(10);
		//Set the cursor position
		SetCursorPos(locationX, locationY);

	}

	INPUT keyStroke = { 0 };
	INPUT mouseStroke = { 0 };
	if (modifierKeys != 0)
	{
		keyStroke.type = INPUT_KEYBOARD;

		switch (modifierKeys)
		{
		case 1:
			keyStroke.ki.wVk = VK_MENU;
			break;
		case 2:
			keyStroke.ki.wVk = VK_CONTROL;
			break;
		case 4:
			keyStroke.ki.wVk = VK_SHIFT;
			break;
		case 8:
			keyStroke.ki.wVk = VK_LWIN;
			break;
		default:
			break;
		}

		keyStroke.ki.wScan = 0;
		keyStroke.ki.dwFlags = 0;
		keyStroke.ki.time = 0;
		keyStroke.ki.dwExtraInfo = 0;

		SendInput(1, &keyStroke, sizeof(keyStroke));
	}

	mouseStroke.type = INPUT_MOUSE;
	switch (mouseButton)
	{
	case 1048576:

		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_LEFTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		break;

	case 2097152:

		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));


		break;

	case 4194304:

		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));
		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_MIDDLEUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		break;

	case 8388608:
		mouseStroke.mi.dx = locationX;
		mouseStroke.mi.dy = locationY;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XDOWN;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));

		::ZeroMemory(&mouseStroke, sizeof(mouseStroke));
		mouseStroke.type = INPUT_MOUSE;
		mouseStroke.mi.dwFlags = MOUSEEVENTF_XUP;
		SendInput(1, &mouseStroke, sizeof(mouseStroke));


		break;

	case 16777216:
		break;

	default:
		break;
	}

	if (modifierKeys != 0)
	{
		keyStroke.type = INPUT_KEYBOARD;
		switch (modifierKeys)
		{
		case 1:
			keyStroke.ki.wVk = VK_MENU;
			break;
		case 2:
			keyStroke.ki.wVk = VK_CONTROL;
			break;
		case 4:
			keyStroke.ki.wVk = VK_SHIFT;
			break;
		case 8:
			keyStroke.ki.wVk = VK_LWIN;
			break;
		default:
			break;
		}

		keyStroke.ki.wScan = 0;
		keyStroke.ki.dwFlags = KEYEVENTF_KEYUP;
		keyStroke.ki.time = 0;
		keyStroke.ki.dwExtraInfo = 0;

		SendInput(1, &keyStroke, sizeof(keyStroke));
	}
	return 0;
}
