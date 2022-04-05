using log4net;
using System.Reflection;

namespace Zappy.Decode.LogManager
{
    public static class CrapyLogger
    {
        public static ILog log;

        public static void Init()
        {
            log = log4net.LogManager.GetLogger(Assembly.GetEntryAssembly().GetName().Name);
        }

    }
}
