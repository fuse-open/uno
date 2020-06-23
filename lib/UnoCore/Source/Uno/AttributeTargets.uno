using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [Flags]
    [extern(DOTNET) DotNetType("System.AttributeTargets")]
    public enum AttributeTargets
    {
        /*
            Note: Outcommented ones are taken from .NET, but are not used in Uno (yet).
        */

        Assembly = 1,
        //Module = 2,
        Class = 4,
        Struct = 8,
        Enum = 16,
        Constructor = 32,
        Method = 64,
        Property = 128,
        Field = 256,
        Event = 512,
        Interface = 1024,
        Parameter = 2048,
        Delegate = 4096,
        //ReturnValue = 8192,
        //GenericParameter = 16384
        All = 32767,
    }
}
