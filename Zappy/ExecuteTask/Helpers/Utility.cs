using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
    internal static class Utility
    {
        private const char AttributeSeparator = ',';
        private static readonly Regex attributesRegEx = new Regex(@"\[(?<attributes>.*?)\]", ZappyTaskUtilities.Compiled | RegexOptions.CultureInvariant);

        internal static string ConvertModiferKeysToString(ModifierKeys modifierKeys)
        {
            StringBuilder builder = new StringBuilder();
            if ((modifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                builder.Append('%');
            }
            if ((modifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                builder.Append('^');
            }
            if ((modifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                builder.Append('+');
            }
            if ((modifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                builder.Append("#");
            }
            return builder.ToString();
        }

        internal static string ConvertModiferKeysToString(ModifierKeys modifierKeys, string text)
        {
            string str = text;
            if (modifierKeys != ModifierKeys.None)
            {
                object[] args = { ConvertModiferKeysToString(modifierKeys), text };
                str = string.Format(CultureInfo.InvariantCulture, "{0}({1})", args);
            }
            return str;
        }

        internal static MouseButtonsPlayback ConvertToPlaybackMouseButton(MouseButtons button)
        {
            if (button <= MouseButtons.Right)
            {
                if (button == MouseButtons.Left)
                {
                    return MouseButtonsPlayback.LEFT_BUTTON;
                }
                if (button == MouseButtons.Right)
                {
                    return MouseButtonsPlayback.RIGHT_BUTTON;
                }
            }
            else
            {
                if (button == MouseButtons.Middle)
                {
                    return MouseButtonsPlayback.MIDDLE_BUTTON;
                }
                if (button == MouseButtons.XButton1)
                {
                    return MouseButtonsPlayback.X_BUTTON_1;
                }
                if (button == MouseButtons.XButton2)
                {
                    return MouseButtonsPlayback.X_BUTTON_2;
                }
            }
            object[] args = { button };
            throw new Exception();
        }

        internal static int DisablePlaybackLoggingContent(IRPFPlayback playback) =>
            playback.SetLoggingFlag(-1);

        internal static bool IsExpandable(ScreenElement element, out string expandableControlType)
        {
            ControlType controlType = ControlType.GetControlType(ZappyTaskService.Instance.ConvertTechnologyElement(element.TechnologyElement).ControlTypeName);
            bool flag = true;
            if (controlType == ControlType.MenuItem || controlType == ControlType.TreeItem || controlType == ControlType.CheckBoxTreeItem)
            {
                flag = true;
                expandableControlType = controlType.Name;
                return flag;
            }
            flag = false;
            expandableControlType = string.Empty;
            return flag;
        }

        internal static bool IsMenuContainer(ScreenElement element)
        {
            ControlType controlType = null;
            ITaskActivityElement technologyElement = element.TechnologyElement;
            controlType = ControlType.GetControlType(ZappyTaskService.Instance.ConvertTechnologyElement(technologyElement).ControlTypeName);
            if (controlType == null)
            {
                return false;
            }
            if (!(controlType == ControlType.MenuBar) && !(controlType == ControlType.StatusBar))
            {
                return controlType == ControlType.ToolBar;
            }
            return true;
        }

        internal static int LaunchImmersiveApplication(IRPFPlayback playback, string strAppModelId)
        {
            if (!ZappyTaskUtilities.IsWin8OrGreaterOs())
            {
                            }
            return playback.LaunchImmersiveApplication(strAppModelId);
        }

        internal static void RestoreMinimizedWindow(ITaskActivityElement element)
        {
            #if COMENABLED
            if ((bool)ScreenElement.GetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS))
#endif
            {
                try
                {
                    element = ZappyTaskService.Instance.ConvertTechnologyElement(element);
                    ITaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                    if (element2 != null)
                    {
                        IntPtr windowHandle = element2.WindowHandle;
                        if (NativeMethods.IsIconic(windowHandle) && NativeMethods.IsWindowVisible(windowHandle))
                        {
                            NativeMethods.ShowWindow(windowHandle, NativeMethods.WindowShowStyle.Restore);
                        }
                    }
                }
                catch (ZappyTaskException)
                {
                    CrapyLogger.log.ErrorFormat("ManagedWrapper : Utility : RestoreMinimizedWindow failed");
                }
            }
        }

        internal static void TerminateImmersiveApplication(IRPFPlayback playback, string strPackageFullName, int processId)
        {
            if (!ZappyTaskUtilities.IsWin8OrGreaterOs())
            {
                            }
            playback.TerminateImmersiveApplication(strPackageFullName, processId);
        }
    }
}