namespace Zappy.Decode.Helper
{
    public enum CLSCTX
    {
                                        CLSCTX_INPROC_SERVER = 1,
                                CLSCTX_INPROC_HANDLER = 2,
                                        CLSCTX_LOCAL_SERVER = 4,
                                CLSCTX_INPROC_SERVER16 = 8,
                                CLSCTX_REMOTE_SERVER = 16,
                                CLSCTX_INPROC_HANDLER16 = 32,
                                CLSCTX_RESERVED1 = 64,
                                CLSCTX_RESERVED2 = 128,
                                CLSCTX_RESERVED3 = 256,
                                CLSCTX_RESERVED4 = 512,
                                        CLSCTX_NO_CODE_DOWNLOAD = 1024,
                                CLSCTX_RESERVED5 = 2048,
                CLSCTX_NO_CUSTOM_MARSHAL = 4096,
                                        CLSCTX_ENABLE_CODE_DOWNLOAD = 8192,
                                        CLSCTX_NO_FAILURE_LOG = 16384,
                                        CLSCTX_DISABLE_AAA = 32768,
                                        CLSCTX_ENABLE_AAA = 65536,
                                        CLSCTX_FROM_DEFAULT_CONTEXT = 131072
    }
}