using System;

namespace Zappy.Decode.Mssa
{
    [Flags]
    internal enum QueryElementProperty
    {
        ClassName = 0x10,
        ControlId = 2,
        ControlName = 1,
        ControlTypeName = 0x100,
        Name = 4,
        NativeControlType = 0x200,
        None = 0,
        OrderOfInvocation = 0x20,
        Value = 0x40
    }
}