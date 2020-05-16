namespace Uno.Compiler.API.Domain.IL.Statements
{
    public enum StatementType
    {
        Expression,
        VariableDeclaration,
        FixedArrayDeclaration,

        Scope,
        While,
        For,
        IfElse,
        Return,
        Break,
        Continue,
        TryCatchFinally,
        Switch,
        Throw,
        ExternScope,

        // Graphics
        Draw,
        DrawDispose
    }
}