
using System;
using System.Collections.Generic;
using System.Threading;

namespace Zappy.Graph
{
    public static class UserTextDataManager
    {
        private static ReaderWriterLockSlim rwls;
        private static Dictionary<string, int> _StringDictionary;
        public static string[] removeEndSpecialChar = { "{LMenu}", "{RMenu}", "{LShiftKey}", "{RShiftKey}", "{LControlKey}", "{RControlKey}" };
        private static int _WrittenCount = 0;
        static UserTextDataManager()
        {
            rwls = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            try
            {
                LoadTextData();
            }
            catch
            {
            }
            if (_StringDictionary == null)
                _StringDictionary = new Dictionary<string, int>(StringComparer.Ordinal);

            _WrittenCount = _StringDictionary.Count;
        }

        public static int GetStringIndex(string Data, bool Register = true)
        {
            int _ReturnIndex;

            if (string.IsNullOrEmpty(Data))
                Data = string.Empty;
            rwls.EnterUpgradeableReadLock();
            try
            {
                if (!_StringDictionary.TryGetValue(Data, out _ReturnIndex) && Register)
                {

                    rwls.EnterWriteLock();
                    try
                    {
                        if (!_StringDictionary.TryGetValue(Data, out _ReturnIndex))
                            _StringDictionary[Data] = _ReturnIndex = _StringDictionary.Count + 1;
                    }
                    finally
                    {
                        rwls.ExitWriteLock();
                    }

                }
            }
            finally
            {
                rwls.ExitUpgradeableReadLock();
            }


            return _ReturnIndex;
        }

                        
                                                                                                                                                                                                                
                
        public static void SaveTextData()
        {
                        
                                                                                                                                                            
                                }

        public static void LoadTextData()
        {
                                                                                                                                                
                        
                                                                                    
        }
    }
}

