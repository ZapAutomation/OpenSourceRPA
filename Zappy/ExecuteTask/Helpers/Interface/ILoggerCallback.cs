namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [ComImport, Guid("36BF0AE0-BAD6-4607-B1BB-5B2D98DDDE5C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    internal interface ILoggerCallback
    {

        void LoggerCallback(ref string logString, ref int logDefaultFlag, ref int dumpScreenFlag);
    }
}