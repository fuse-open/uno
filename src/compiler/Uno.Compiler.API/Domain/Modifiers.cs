using System;

namespace Uno.Compiler.API.Domain
{
    [Flags]
    public enum Modifiers : ushort
    {
        Private = 1 << 0,
        Protected = 1 << 1,
        Public = 1 << 2,
        Internal = 1 << 3,

        Partial = 1 << 4,
        Static = 1 << 5,
        Intrinsic = 1 << 6,
        Generated = 1 << 7,

        Virtual = 1 << 8,
        Override = 1 << 9,
        Abstract = 1 << 10,
        Sealed = 1 << 11,

        Extern = 1 << 12,

        Explicit = 1 << 13,
        Implicit = 1 << 14,

        New = 1 << 15,

        CastModifiers = Implicit | Explicit,
        MethodModifiers = Abstract | Virtual | Override | Sealed,
        ProtectionModifiers = Private | Protected | Public | Internal,

        LiteralMember = ProtectionModifiers | New | Generated | Extern,
        FieldMember = LiteralMember | Static,
        FunctionMember = FieldMember,
        PropertyMember = FunctionMember | MethodModifiers,
        EventMember = PropertyMember,

        ClassModifiers = Sealed | Abstract | Partial | Static,
        InterfaceModifiers = Abstract | Partial,

        NestedType = ProtectionModifiers | Generated | Extern | New,
        ParentType = Public | Internal | Generated | Extern | Intrinsic,
    }
}
