namespace Uno.Compiler.Frontend.Analysis
{
    public enum Precedence
    {
        Invalid = -1,
        Assignment = 1,
        Conditional = 2,
        NullCoalescing = 3,
        ConditionalOr = 4,
        ConditionalAnd = 5,
        LogicalOr = 6,
        LogicalXor = 7,
        LogicalAnd = 8,
        Equality = 9,
        Relational = 10,
        Shift = 11,
        Additive = 12,
        Multiplicative = 13,
        Primary = 14,
        Unary = 15
    }
}