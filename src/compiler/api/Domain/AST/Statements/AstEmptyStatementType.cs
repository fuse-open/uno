namespace Uno.Compiler.API.Domain.AST.Statements
{
    public enum AstEmptyStatementType
    {
        Break = AstStatementType.Break,
        BuildError = AstStatementType.BuildError,
        BuildWarning = AstStatementType.BuildWarning,
        Continue = AstStatementType.Continue,
        DrawDispose = AstStatementType.DrawDispose,
        Return = AstStatementType.Return,
        Throw = AstStatementType.Throw,
        YieldBreak = AstStatementType.YieldBreak,
    }
}