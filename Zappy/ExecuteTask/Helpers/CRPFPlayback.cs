using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
#if COMENABLED
    [ComImport, CoClass(typeof(CRPFPlaybackClass)), Guid("C2F7C968-7C3F-4F22-AC67-FD96382548E9"), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    internal interface CRPFPlayback : IRPFPlayback
    {
    }
}