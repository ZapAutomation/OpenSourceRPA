using System.Collections.Generic;
using System.Text;
using Zappy.SharedInterface.Helper;

namespace Zappy.Helpers
{
    public static class ExtensionMethods
    {

        public static int IndexFromEnd<T>(this List<T> List, T ObjectToFind, int bound = 64)
            where T : class
        {
            int _Index = -1, _Count = List.Count - 1, _LowerBound = 0;
            if (_Count > bound && bound > 0)
                _LowerBound = _Count - bound;

            for (int i = _Count; i >= _LowerBound; i--)
            {
                if (object.ReferenceEquals(List[i], ObjectToFind))
                    return i;
            }

            return _Index;
        }

        public static int IndexFromEnd<T>(this List<T> List, T ObjectToFind)
            where T : class
        {
            int _Index = -1, _Count = List.Count - 1, _LowerBound = 0;
            if (_Count > 64)
                _LowerBound = _Count - 64;

            for (int i = _Count; i >= _LowerBound; i--)
            {
                if (object.ReferenceEquals(List[i], ObjectToFind))
                    return i;
            }

            return _Index;
        }

        public static bool ContainsFromEnd<T>(this List<T> List, T ObjectToFind)
            where T : class
        {
            return List.IndexFromEnd(ObjectToFind) >= 0;
        }

                                        
                                                                        
                                
                
                                                        
                                
                
                                                                                                                                        
        public static void ExpandVariable(this StringBuilder sb, string VariableStartReplacement, string VariableEndReplacement)
        {
            int _ScanLength = sb.Length;
            string value = SharedConstants.VariableNameBegin;
            for (int i = 0; i < _ScanLength; ++i)
            {
                if (sb[i] == value[0] && i + 1 < sb.Length && sb[i + 1] == value[1])
                {
                    sb.Remove(i, 2);
                    sb.Insert(i, VariableStartReplacement);
                    i += VariableStartReplacement.Length;
                    _ScanLength = sb.Length;
                    while (sb[i++] != CrapyConstants.VariableNameEnd[0]) ;
                    i--;
                    if (sb[i] == CrapyConstants.VariableNameEnd[0])
                    {
                        sb.Remove(i, 1);
                        sb.Insert(i, VariableEndReplacement);
                    }
                    i += VariableEndReplacement.Length;
                }
            }
        }

    }
}
