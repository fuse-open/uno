namespace Uno.Compiler.API.Domain.AST.Statements
{
    public enum AstStatementType : byte
    {
        Undef = 0,

        // AstModifiedStatement
        AutoRelease,
        Checked,
        Unchecked,
        Unsafe,

        // AstEmptyStatement
        Break,
        BuildError,
        BuildWarning,
        Continue,
        DrawDispose,
        Return,
        Throw,
        YieldBreak,

        // AstValueStatement
        Assert,
        BuildErrorMessage,
        BuildWarningMessage,
        DebugLog,
        ReturnValue,
        ThrowValue,
        YieldReturnValue,

        // AstWhile
        While,
        DoWhile,

        // Others
        FixedArrayDeclaration,
        VariableDeclaration,
        Scope,
        ExternScope,
        IfElse,
        For,
        Foreach,
        Switch,
        TryCatchFinally,
        Lock,
        Using,
        Draw,

        // Max Value (see ExpressionType)
        MaxValue = 64
    }
}