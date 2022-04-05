using System.Runtime.InteropServices;

namespace Zappy.ExecuteTask.Helpers.Interface
{
        internal interface IThreadInfo
    {
        [DispId(1)]
        int GetThreadWaitReason(int nProcId, int nThreadId, out int pnThreadState);
    }
}