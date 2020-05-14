namespace Uno.Compiler.API.Domain
{
    public enum OperatorType : byte
    {
        UnaryPlus = 1,
        UnaryNegation,
        OnesComplement,
        LogicalNot,
        Addition,
        Subtraction,
        Multiply,
        Division,
        Modulus,
        BitwiseAnd,
        BitwiseOr,
        ExclusiveOr,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Equality,
        Inequality,
        LeftShift,
        RightShift,
        Increase,
        Decrease
    }
}