namespace Uno.Compiler.Core.IL.Validation
{
    public enum VisibilityLevel
    {
        Global = 1,
        Bundle,
        SameTypeOrSubclassOfOrBundle,
        SameTypeOrSubclass,
        SameType
    }
}