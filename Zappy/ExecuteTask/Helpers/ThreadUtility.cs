using System.Runtime.InteropServices;
using System.Threading;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
    internal static class ThreadUtility
    {
        private static ApartmentState playbackThreadApartmentState;
        private static int playbackThreadId;
        private static ThreadInfo threadInfo = new ThreadInfo();

        internal static void CacheThreadInfo()
        {
            playbackThreadApartmentState = Thread.CurrentThread.GetApartmentState();
            playbackThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        internal static bool DangerousFixPlaybackThreadAccess(IRPFPlayback playback, out IRPFPlayback rpfPlayback)
        {
            if (playback != null && playbackThreadApartmentState == ApartmentState.STA && playbackThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                
                Marshal.ReleaseComObject(playback);
                
                rpfPlayback = new CRPFPlaybackClass();
                
                return true;
            }
            rpfPlayback = null;
            return false;
        }

        internal static void SetThreadInfo(IRPFPlayback playback)
        {
            playback.SetThreadInfoInterface(threadInfo);
        }

        internal static void ThrowExceptionIfCrossThreadAccess(IRPFPlayback playback, IScreenElement uiElement)
        {
#if COMENABLED
            if (playback != null && playbackThreadApartmentState == ApartmentState.STA && playbackThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                object[] args = { playbackThreadId, Thread.CurrentThread.ManagedThreadId };
                CrapyLogger.log.ErrorFormat("ScreenElement: COM STA playback object created in thread {0} is being accessed in thread {1}", args);
                            }
            try
            {
                if (uiElement != null)
                {
                    object technologyElement = uiElement.TechnologyElement;
                }
            }
            catch (InvalidComObjectException exception)
            {
                object[] objArray2 = { exception.Message };
                CrapyLogger.log.ErrorFormat("ScreenElement: ExceptionThrown:{0}", objArray2);
                            }
#endif
        }

        internal static object ThreadInfoObject =>
            threadInfo;
    }
}