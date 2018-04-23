namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public enum AstSymbolType : byte
    {
        Null = AstExpressionType.Null,
        Void = AstExpressionType.Void,
        Global = AstExpressionType.Global,
        Var = AstExpressionType.Var,
        This = AstExpressionType.This,
        Base = AstExpressionType.Base,
        True = AstExpressionType.True,
        False = AstExpressionType.False,
        Zero = AstExpressionType.Zero,
    }
}