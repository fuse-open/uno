namespace Uno.UX.Markup.UXIL.Expressions
{
    public enum Precedence
    {
        Invalid = -1,
        Assignment = 1,
        Conditional = 2,
        NullCoalescing = 3,
        LogicalOr = 4,
        LogicalAnd = 5,
        Relational = 6,
        Additive = 7,
        Multiplicative = 8
    }
}
