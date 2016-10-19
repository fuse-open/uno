using System;

namespace Uno.Compiler.API.Domain.IL
{
    [Flags]
    public enum EntityStats
    {
        ImplicitReturn = 1 << 1,
        ParameterizedDefinition = 1 << 2,
        PopulatingMembers = 1 << 3,
        RefCount = 1 << 4,
        RefCountAsBase = 1 << 5,
        RefCountAsOverridden = 1 << 6,
        GenericMethodType = 1 << 7,
        ImplementsInterface = 1 << 8,
        ThrowsException = 1 << 9,
        CanLink = 1 << 10,
        IsCompiled = 1 << 11,
    }
}