using System;

namespace Zappy.SharedInterface
{

    public interface IDynamicProperty
    {
        string DymanicKey { get; set; }
        bool DymanicKeySpecified { get; set; }
        object ObjectValue { get; set; }

        bool ValueSpecified { get; set; }
        string RuntimeScript { get; set; }
        Type ElementType { get; }
        bool RuntimeScriptSpecified { get; set; }
        bool EvaluateOnFirstUse { get; set; }
        void ResetFlags();
        
    }

                            
                                
        
                    
            }
