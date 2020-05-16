namespace Uno.Compiler.API.Domain.IL.Types
{
    public enum TypeType
    {
        Invalid,
        Void,
        Null,
        Enum,
        Class,
        Struct,
        Interface,
        Delegate,
        RefArray,
        FixedArray,
        GenericParameter,
        MethodGroup,
    }
}