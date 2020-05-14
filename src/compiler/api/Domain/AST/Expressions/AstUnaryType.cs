namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public enum AstUnaryType : byte
    {
        Default = AstExpressionType.Default,
        Unsafe = AstExpressionType.Unsafe,
        Unchecked = AstExpressionType.Unchecked,
        Checked = AstExpressionType.Checked,
        NameOf = AstExpressionType.NameOf,
        SizeOf = AstExpressionType.SizeOf,
        TypeOf = AstExpressionType.TypeOf,
        Nullable = AstExpressionType.Nullable,
        Array = AstExpressionType.Array,
        ReadOnly = AstExpressionType.ReadOnly,
        Volatile = AstExpressionType.Volatile,
        Vertex = AstExpressionType.Vertex,
        Pixel = AstExpressionType.Pixel,
        DecreasePrefix = AstExpressionType.DecreasePrefix,
        DecreasePostfix = AstExpressionType.DecreasePostfix,
        IncreasePrefix = AstExpressionType.IncreasePrefix,
        IncreasePostfix = AstExpressionType.IncreasePostfix,
        Promote = AstExpressionType.Promote,
        Negate = AstExpressionType.Negate,
        LogNot = AstExpressionType.LogNot,
        BitwiseNot = AstExpressionType.BitwiseNot
    }
}
