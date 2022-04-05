namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [ComImport, Guid("5E7C0315-2E01-4119-960C-3993700665C5"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    public interface ILastInvocationInfo
    {
        string Message { get; }
        string Source { get; }
        ILastInvocationInfo InnerInfo { get; }
        string[] GetInfoProperties();
        object GetInfoPropertyValue(string propertyName);
    }
}