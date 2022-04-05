using System.Runtime.InteropServices;


namespace Zappy.ExecuteTask.KeyboardWrapper
{
    public static class KeyboardWrapper
    {
        [DllImport("ZappyDriver.dll", EntryPoint = "sendKeys", SetLastError = true, ExactSpelling = true,
             CallingConvention = CallingConvention.Cdecl)]
        public static extern int sendKeys(string str, int value, string[] windowTitles, int size, int PauseTimeAfterAction);

        [DllImport("ZappyDriver.dll", EntryPoint = "mouseSingleClick", SetLastError = true, ExactSpelling = true,
           CallingConvention = CallingConvention.Cdecl)]
        public static extern int mouseSingleClick(int locationX, int locationY, int mouseButton, int modifierKeys, string[] windowTitles, int size);

        [DllImport("ZappyDriver.dll", EntryPoint = "mouseDoubleClick", SetLastError = true, ExactSpelling = true,
           CallingConvention = CallingConvention.Cdecl)]
        public static extern int mouseDoubleClick(int locationX, int locationY, int mouseButton, int modifierKeys, string[] windowTitles, int size);

        [DllImport("ZappyDriver.dll", EntryPoint = "sendCitrixKeys", SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int sendCitrixKeys(string Text, int modifierKeys);
    }
}
