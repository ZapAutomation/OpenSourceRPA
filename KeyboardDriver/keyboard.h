#pragma once
#include <uiautomation.h>
//#include <UIAutomationClient.h>
//#include <boost/log/trivial.hpp>
//#include <boost/log/utility/setup/file.hpp>
//#include <boost/log/expressions.hpp>
#include <vector>
#include <string>

int sendKeys(const char *str, int value, char** windowTitles, int size);
int mouseSingleClick(int locationX, int locationY, int mouseButton, int modifierKeys, char **windowTitles, int size);
int mouseDoubleClick(int locationX, int locationY, int mouseButton, int modifierKeys, char **windowTitles, int size);