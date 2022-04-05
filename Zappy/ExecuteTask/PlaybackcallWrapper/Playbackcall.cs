using System.Runtime.InteropServices;

namespace Zappy.ExecuteTask.PlaybackcallWrapper
{
    class PlaybackCall
    {
        [DllImport("PlaybackCall.dll", EntryPoint = "FindAllScreenElements", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern object[] FindAllScreenElements(int cResKeys, int nMaxDepth);
    }
}
