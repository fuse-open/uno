namespace Uno.Compiler.API.Domain.AST.Statements
{
    public enum AstValueStatementType
    {
        Assert = AstStatementType.Assert,
        BuildError = AstStatementType.BuildErrorMessage,
        BuildWarning = AstStatementType.BuildWarningMessage,
        DebugLog = AstStatementType.DebugLog,
        Return = AstStatementType.ReturnValue,
        Throw = AstStatementType.ThrowValue,
        YieldReturn = AstStatementType.YieldReturnValue
    }
}