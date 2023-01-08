namespace Uno.Compiler.Core.IL.Validation
{
    public enum VisibilityLevel
    {
        Global = 1,
        Bundle,
        SameTypeOrSubclassOfOrPackage,
        SameTypeOrSubclass,
        SameType
    }
}