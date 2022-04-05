using System;

namespace Zappy.Decode.Mssa
{
    public sealed class StaThreadWorker : IDisposable
    {
                                        private static StaThreadWorker instance;
                                        
        private StaThreadWorker()
        {
                    }

        public void Dispose()
        {
                                                                                }

        private void FunctionInvoker()
        {
                                                                                                                                                                                            }

        internal object InvokeDelegate(Delegate methodToInvoke, params object[] args)
        {
                                    return methodToInvoke.DynamicInvoke(args);
                                                                                                                                                                                                                    }

                                                                                                        
        private void StopSTADelegateInvoker()
        {
                                    
                                                            
                                                                                                                        
                    }

        public static StaThreadWorker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaThreadWorker();
                }
                return instance;
            }
        }
    }
}