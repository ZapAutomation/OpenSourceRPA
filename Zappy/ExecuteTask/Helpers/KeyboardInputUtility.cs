namespace Zappy.ExecuteTask.Helpers
{
    internal static class KeyboardInputUtility
    {
        private static bool restoreKeyState;
        private const int VK_CAPITAL = 20;

        internal static void Reset()
        {
            restoreKeyState = false;
        }

        internal static void RestoreKeyState()
        {
            if (restoreKeyState)
            {
                restoreKeyState = false;
                ScreenElement.Playback.TypeString("{CapsLock}", 0, 0, 3);
            }
        }

        internal static void SetKeyState()
        {
            restoreKeyState = false;
            if (((ushort)ScreenElementUtility.GetKeyState(20) & 0xffff) != 0)
            {
                ScreenElement.Playback.TypeString("{CapsLock}", 0, 0, 3);
                restoreKeyState = true;
            }
        }
    }
}