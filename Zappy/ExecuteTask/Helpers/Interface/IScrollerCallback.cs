namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [ComImport, Guid("83BEB5E7-E831-4885-A261-01FC308FC245"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    internal interface IScrollerCallback
    {
        void VerticalScrollCallback(int nScrollAmount, out int pnPercentScrolled);
        void HorizontalScrollCallback(int nScrollAmount, out int pnPercentScrolled);
    }
}